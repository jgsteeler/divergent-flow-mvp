import { ItemType, TypeLearningData } from './types'
import { extractDateTimeFromText } from './dateParser'
import { CATCHALL_NOTE_CONFIDENCE, HIGH_CONFIDENCE_THRESHOLD } from './constants'

// Default preloaded phrases for each type (will be stored in learning data)
export const DEFAULT_ACTION_PHRASES = [
  'create', 'build', 'fix', 'update', 'review', 'send', 'call', 'email',
  'schedule', 'complete', 'finish', 'submit', 'prepare', 'order', 'take',
  'make', 'write', 'buy', 'book', 'research', 'organize', 'plan', 'draft',
  'need to', 'have to', 'must', 'should'
]

export const DEFAULT_REMINDER_PHRASES = [
  'remind me', 'remember to', 'don\'t forget', 'need to remember',
  'reminder', 'remember', 'follow up', 'check in', 'ping me', 'alert me',
  'notify me'
]

export const DEFAULT_NOTE_PHRASES = [
  'note', 'idea', 'thought', 'interesting', 'learned', 'found out',
  'discovered', 'realized', 'noticed', 'observation', 'insight'
]

/**
 * Initialize default type learning data if not present
 */
export function initializeDefaultTypeLearning(): TypeLearningData[] {
  const defaultData: TypeLearningData[] = []
  const timestamp = Date.now()
  
  DEFAULT_ACTION_PHRASES.forEach(phrase => {
    defaultData.push({
      keywords: [phrase],
      type: 'action',
      confidence: 95,
      timestamp,
      isDefault: true,
      wasCorrect: true
    })
  })
  
  DEFAULT_REMINDER_PHRASES.forEach(phrase => {
    defaultData.push({
      keywords: [phrase],
      type: 'reminder',
      confidence: 95,
      timestamp,
      isDefault: true,
      wasCorrect: true
    })
  })
  
  DEFAULT_NOTE_PHRASES.forEach(phrase => {
    defaultData.push({
      keywords: [phrase],
      type: 'note',
      confidence: 90,
      timestamp,
      isDefault: true,
      wasCorrect: true
    })
  })
  
  return defaultData
}

/**
 * Parse text into keywords (normalized, lowercased, filtered)
 */
export function parseKeywords(text: string): string[] {
  const normalized = text.toLowerCase().trim()
  const cleaned = normalized.replace(/[^\w\s']/g, ' ')
  const words = cleaned.split(/\s+/).filter(word => word.length > 0)
  
  // Remove common stop words
  const stopWords = ['the', 'a', 'an', 'and', 'or', 'but', 'in', 'on', 'at', 'to', 'for', 'of', 'with', 'by']
  const filtered = words.filter(word => !stopWords.includes(word))
  
  // Include multi-word phrases (2-3 word combinations)
  const phrases: string[] = []
  for (let i = 0; i < words.length - 1; i++) {
    phrases.push(`${words[i]} ${words[i + 1]}`)
    if (i < words.length - 2) {
      phrases.push(`${words[i]} ${words[i + 1]} ${words[i + 2]}`)
    }
  }
  
  return [...filtered, ...phrases]
}

/**
 * Calculate type scores based on keyword matches
 */
function calculateTypeScores(
  keywords: string[],
  learningData: TypeLearningData[]
): Record<ItemType, { score: number; matches: number }> {
  const scores: Record<ItemType, { score: number; matches: number }> = {
    action: { score: 0, matches: 0 },
    reminder: { score: 0, matches: 0 },
    note: { score: 0, matches: 0 },
  };

  const validLearning = learningData.filter((ld) => ld.wasCorrect !== false || ld.isDefault);

  for (const learning of validLearning) {
    for (const keyword of keywords) {
      for (const learnedKeyword of learning.keywords) {
        if (keyword === learnedKeyword) {
          // Exact match
          const weight = learning.confidence / 100;
          scores[learning.type].score += weight;
          scores[learning.type].matches += 1;
        } else if (keyword.includes(learnedKeyword) || learnedKeyword.includes(keyword)) {
          // Partial match
          const weight = (learning.confidence / 100) * 0.75; // Adjusted partial match weight
          scores[learning.type].score += weight;
          scores[learning.type].matches += 1;
        }
      }
    }
  }

  return scores;
}

/**
 * Calculate confidence for a given type based on scores and context
 */
function calculateConfidence(
  selectedType: ItemType,
  scores: Record<ItemType, { score: number; matches: number }>,
  hasStrongIndicators: boolean
): number {
  const maxScore = Math.max(scores.action.score, scores.reminder.score, scores.note.score);
  const totalScore = scores.action.score + scores.reminder.score + scores.note.score;

  // Default case: no matches
  if (maxScore === 0) {
    return 85; // Default confidence for note type
  }

  // Strong indicators (explicit keywords): high confidence
  if (hasStrongIndicators && (selectedType === 'action' || selectedType === 'reminder')) {
    return 95;
  }

  // Calculate base confidence from score distribution
  const normalizedConfidence = (maxScore / totalScore) * 100;

  // Determine confidence based on type and score dominance
  const scoreDominance = maxScore / totalScore;

  if (scoreDominance > 0.9) {
    // Very strong match (>90% of total score)
    if (selectedType === 'action' || selectedType === 'reminder') {
      return 95;
    }
    return Math.min(normalizedConfidence, 85); // Cap note confidence at 85
  } else if (scoreDominance > 0.8) {
    // Strong match (>80% of total score)
    if (selectedType === 'action' || selectedType === 'reminder') {
      return 95;
    }
    return Math.min(normalizedConfidence, 85); // Cap note confidence
  } else if (scoreDominance > 0.6) {
    // Moderate match (>60% of total score)
    if (selectedType === 'action' || selectedType === 'reminder') {
      return 90;
    }
    return Math.min(normalizedConfidence, 85); // Cap note confidence
  } else {
    // Ambiguous case (<60% of total score)
    return Math.max(75, Math.min(normalizedConfidence, 85));
  }
}

/**
 * Determine the best type from scores with tie-breaking rules
 */
function selectBestType(
  scores: Record<ItemType, { score: number; matches: number }>,
  text: string
): ItemType {
  const maxScore = Math.max(scores.action.score, scores.reminder.score, scores.note.score);

  // Default to note if no scores
  if (maxScore === 0) {
    return 'note';
  }

  const typePreferenceOrder: ItemType[] = ['reminder', 'action', 'note'];

  const candidates: { type: ItemType; score: number; matches: number }[] = [
    { type: 'action', score: scores.action.score, matches: scores.action.matches },
    { type: 'reminder', score: scores.reminder.score, matches: scores.reminder.matches },
    { type: 'note', score: scores.note.score, matches: scores.note.matches },
  ];

  const topCandidates = candidates.filter((candidate) => candidate.score === maxScore);

  if (topCandidates.length === 0) {
    return 'note';
  }

  // Apply tie-breaking: prefer higher match count, then preference order
  const bestCandidate = topCandidates.reduce((best, current) => {
    if (current.matches > best.matches) {
      return current;
    }
    if (current.matches < best.matches) {
      return best;
    }
    // Equal matches: use preference order
    const bestIndex = typePreferenceOrder.indexOf(best.type);
    const currentIndex = typePreferenceOrder.indexOf(current.type);
    return currentIndex !== -1 && (bestIndex === -1 || currentIndex < bestIndex) ? current : best;
  }, topCandidates[0]);

  let selectedType = bestCandidate.type;

  // Edge case: override note with action if action score is strong
  if (selectedType === 'note' && scores.action.score > 0.8 * scores.note.score) {
    selectedType = 'action';
  }

  return selectedType;
}

/**
 * Infer type using keyword-based matching against learning data
 */
export function inferType(
  text: string,
  learningData: TypeLearningData[] = []
): { type: ItemType | null; confidence: number; reasoning: string; keywords: string[] } {
  const keywords = parseKeywords(text);
  const { dateTime } = extractDateTimeFromText(text);
  const hasDateTime = dateTime !== null;

  const scores = calculateTypeScores(keywords, learningData);

  const lowerText = text.toLowerCase();

  // Apply phrase-based score adjustments
  const reminderBoostPhrases = ['remind me to', 'follow up on', "don't forget", 'remember to', 'need to remember', 'Reminder:'];
  const reminderBoost = reminderBoostPhrases.reduce((total, phrase) => {
    if (!lowerText.includes(phrase)) {
      return total;
    }
    const boost = phrase === "don't forget" ? 3 : 4;
    return total + boost;
  }, 0);
  scores.reminder.score += reminderBoost;

  const actionBoostPhrases = ['create', 'make', 'write', 'send', 'call', 'email', 'fix', 'build', 'update', 'submit'];
  const actionBoost = actionBoostPhrases.reduce((total, phrase) => {
    if (!lowerText.includes(phrase)) {
      return total;
    }
    const boost = phrase === 'submit' ? 3 : 2;
    return total + boost;
  }, 0);
  scores.action.score += actionBoost;

  // Special handling for 'Reminder:' prefix
  if (text.startsWith('Reminder:')) {
    scores.reminder.score += 3;
  }

  // Reduce reminder dominance when action keywords are present
  if (scores.action.score > 0 && scores.reminder.score > 0) {
    scores.reminder.score -= scores.action.score * 0.2;
  }

  // Determine if there are strong explicit indicators
  const hasStrongIndicators = reminderBoostPhrases.some(phrase => lowerText.includes(phrase));

  // Select the best type based on scores
  const selectedType = selectBestType(scores, text);

  // Calculate confidence once based on selected type and scores
  const confidence = calculateConfidence(selectedType, scores, hasStrongIndicators);

  // Handle default case (no matches) with specific reasoning
  const maxScore = Math.max(scores.action.score, scores.reminder.score, scores.note.score);
  if (maxScore === 0) {
    return {
      type: 'note',
      confidence: 85,
      reasoning: 'Default to note - no strong action or reminder indicators',
      keywords,
    };
  }

  // Build reasoning
  const scoreSummary = `Scores - note: ${scores.note.score.toFixed(2)}, action: ${scores.action.score.toFixed(2)}, reminder: ${scores.reminder.score.toFixed(2)}`;
  const keywordSummary =
    keywords.length > 0
      ? `Matched keywords (${keywords.length}): ${keywords.join(', ')}`
      : 'No specific keywords matched';

  let reasoningDetail: string;
  if (selectedType === 'action') {
    reasoningDetail =
      'Classified as action because action-related patterns had the strongest score after refined prioritization.';
  } else if (selectedType === 'reminder') {
    reasoningDetail =
      'Classified as reminder because reminder-related patterns had the strongest score after refined prioritization.';
  } else if (selectedType === 'note') {
    reasoningDetail =
      'Classified as note because informational patterns outweighed action and reminder indicators after refined prioritization.';
  } else {
    reasoningDetail =
      'Type could not be confidently determined; using highest available score with refined prioritization.';
  }

  const detailedReasoning = `${reasoningDetail} ${scoreSummary}. ${keywordSummary}.`;

  return {
    type: selectedType,
    confidence,
    reasoning: detailedReasoning,
    keywords,
  };
}

/**
 * Save type learning after confirmation
 */
export async function saveTypeLearning(
  keywords: string[],
  inferredType: ItemType | null,
  actualType: ItemType,
  confidence: number
): Promise<TypeLearningData> {
  return {
    keywords,
    type: actualType,
    confidence: HIGH_CONFIDENCE_THRESHOLD,
    timestamp: Date.now(),
    wasCorrect: inferredType === actualType,
    isDefault: false
  }
}

export function getTypeLabel(type: ItemType): string {
  const labels: Record<ItemType, string> = {
    note: 'Note',
    action: 'Action Item',
    reminder: 'Reminder'
  }
  return labels[type]
}

export function getTypeDescription(type: ItemType): string {
  const descriptions: Record<ItemType, string> = {
    note: 'Information to remember',
    action: 'Something to do',
    reminder: 'Time-sensitive prompt'
  }
  return descriptions[type]
}
