import { ItemType, TypeLearningData } from './types'
import { extractDateTimeFromText } from './dateParser';
import {
  CATCHALL_NOTE_CONFIDENCE,
  HIGH_CONFIDENCE_THRESHOLD,
  PARTIAL_MATCH_WEIGHT,
  REMINDER_PHRASE_BOOST_DEFAULT,
  REMINDER_PHRASE_BOOST_DONT_FORGET,
  ACTION_PHRASE_BOOST_DEFAULT,
  ACTION_PHRASE_BOOST_SUBMIT,
  REMINDER_DOMINANCE_PENALTY,
  CONFIDENCE_CAP_NON_EXACT_MATCH,
  EXACT_MATCH_SCORE_THRESHOLD,
  EXACT_MATCH_CONFIDENCE,
  REMINDER_PREFIX_BOOST,
  AMBIGUOUS_CASE_SCORE_THRESHOLD,
  AMBIGUOUS_CASE_MIN_CONFIDENCE,
  ACTION_TYPE_MIN_CONFIDENCE,
  CONFIRMED_TYPE_CONFIDENCE,
  ACTION_NOTE_PRIORITY_THRESHOLD,
  DEFAULT_ACTION_PHRASE_CONFIDENCE,
  DEFAULT_REMINDER_PHRASE_CONFIDENCE,
  DEFAULT_NOTE_PHRASE_CONFIDENCE,
  DATE_TIME_ACTION_MULTIPLIER
} from './constants';

// Phrase lists for type inference
const REMINDER_BOOST_PHRASES = ['remind me to', 'follow up on', "don't forget", 'remember to', 'need to remember', 'reminder:'];
const ACTION_BOOST_PHRASES = ['create', 'make', 'write', 'send', 'call', 'email', 'fix', 'build', 'update', 'submit'];

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
      confidence: DEFAULT_ACTION_PHRASE_CONFIDENCE,
      timestamp,
      isDefault: true,
      wasCorrect: true
    })
  })
  
  DEFAULT_REMINDER_PHRASES.forEach(phrase => {
    defaultData.push({
      keywords: [phrase],
      type: 'reminder',
      confidence: DEFAULT_REMINDER_PHRASE_CONFIDENCE,
      timestamp,
      isDefault: true,
      wasCorrect: true
    })
  })
  
  DEFAULT_NOTE_PHRASES.forEach(phrase => {
    defaultData.push({
      keywords: [phrase],
      type: 'note',
      confidence: DEFAULT_NOTE_PHRASE_CONFIDENCE,
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
          const weight = (learning.confidence / 100) * PARTIAL_MATCH_WEIGHT;
          scores[learning.type].score += weight;
          scores[learning.type].matches += 1;
        }
      }
    }
  }

  return scores;
}

/**
 * Apply phrase-specific boosts to type scores
 */
function applyPhraseBoosts(
  scores: Record<ItemType, { score: number; matches: number }>,
  text: string
): void {
  const lowerText = text.toLowerCase();

  // Reminder phrase boosts
  const reminderBoost = REMINDER_BOOST_PHRASES.reduce((total, phrase) => {
    if (!lowerText.includes(phrase)) {
      return total;
    }
    const boost = phrase === "don't forget" ? REMINDER_PHRASE_BOOST_DONT_FORGET : REMINDER_PHRASE_BOOST_DEFAULT;
    return total + boost;
  }, 0);
  scores.reminder.score += reminderBoost;

  // Special handling for 'Reminder:' prefix (case-insensitive)
  if (text.toLowerCase().startsWith('reminder:')) {
    scores.reminder.score += REMINDER_PREFIX_BOOST;
  }

  // Action phrase boosts
  const actionBoost = ACTION_BOOST_PHRASES.reduce((total, phrase) => {
    if (!lowerText.includes(phrase)) {
      return total;
    }
    const boost = phrase === 'submit' ? ACTION_PHRASE_BOOST_SUBMIT : ACTION_PHRASE_BOOST_DEFAULT;
    return total + boost;
  }, 0);
  scores.action.score += actionBoost;
}

/**
 * Apply score adjustments based on type interactions
 */
function applyScoreAdjustments(
  scores: Record<ItemType, { score: number; matches: number }>
): void {
  // Reduce reminder dominance when action keywords are present
  if (scores.action.score > 0 && scores.reminder.score > 0) {
    scores.reminder.score -= scores.action.score * REMINDER_DOMINANCE_PENALTY;
  }
}

/**
 * Calculate base confidence from scores
 */
function calculateBaseConfidence(
  scores: Record<ItemType, { score: number; matches: number }>
): number {
  const maxScore = Math.max(scores.action.score, scores.reminder.score, scores.note.score);

  // Normalize confidence to a 0-100 scale
  const totalScore = scores.action.score + scores.reminder.score + scores.note.score;
  
  let adjustedConfidence: number;

  if (totalScore === 0) {
    adjustedConfidence = 0;
  } else {
    // Calculate normalized confidence
    const normalizedConfidence = (maxScore / totalScore) * 100;
    
    // Check for exact matches (exceeds threshold of total score)
    if (maxScore > EXACT_MATCH_SCORE_THRESHOLD * totalScore) {
      adjustedConfidence = EXACT_MATCH_CONFIDENCE;
    } else {
      // Cap below exact match confidence for non-exact matches
      adjustedConfidence = Math.min(normalizedConfidence, CONFIDENCE_CAP_NON_EXACT_MATCH);
    }
  }

  // Refine confidence scaling for ambiguous cases
  if (totalScore > 0 && maxScore / totalScore < AMBIGUOUS_CASE_SCORE_THRESHOLD) {
    adjustedConfidence = Math.max(adjustedConfidence, AMBIGUOUS_CASE_MIN_CONFIDENCE);
  }

  return adjustedConfidence;
}

/**
 * Select the best type based on scores and matches
 */
function selectBestType(
  scores: Record<ItemType, { score: number; matches: number }>
): ItemType {
  const maxScore = Math.max(scores.action.score, scores.reminder.score, scores.note.score);
  const typePreferenceOrder: ItemType[] = ['reminder', 'action', 'note'];

  const candidates: { type: ItemType; score: number; matches: number }[] = [
    { type: 'action', score: scores.action.score, matches: scores.action.matches },
    { type: 'reminder', score: scores.reminder.score, matches: scores.reminder.matches },
    { type: 'note', score: scores.note.score, matches: scores.note.matches },
  ];

  const topCandidates = candidates.filter((candidate) => candidate.score === maxScore);

  const bestCandidate = topCandidates.reduce((best, current) => {
    if (current.matches > best.matches) {
      return current;
    }

    if (current.matches < best.matches) {
      return best;
    }

    // If matches are equal, fall back to a deterministic preference order
    const bestIndex = typePreferenceOrder.indexOf(best.type);
    const currentIndex = typePreferenceOrder.indexOf(current.type);
    return currentIndex !== -1 && (bestIndex === -1 || currentIndex < bestIndex) ? current : best;
  }, topCandidates[0]);

  return bestCandidate.type;
}

/**
 * Normalize and adjust final confidence based on type and scores
 */
function normalizeFinalConfidence(
  baseConfidence: number,
  finalType: ItemType,
  scores: Record<ItemType, { score: number; matches: number }>
): number {
  let adjustedConfidence = baseConfidence;

  // Boost action confidence when action is the selected type and outscores other types
  if (
    finalType === 'action' &&
    scores.action.score > scores.reminder.score &&
    scores.action.score > scores.note.score
  ) {
    adjustedConfidence = Math.max(adjustedConfidence, ACTION_TYPE_MIN_CONFIDENCE);
  }

  // Ensure note confidence is capped at CATCHALL_NOTE_CONFIDENCE in ambiguous cases
  if (finalType === 'note' && adjustedConfidence > CATCHALL_NOTE_CONFIDENCE) {
    adjustedConfidence = CATCHALL_NOTE_CONFIDENCE;
  }

  // Set minimum confidence for classified action and reminder types
  if (
    (finalType === 'action' || finalType === 'reminder') &&
    adjustedConfidence < CONFIRMED_TYPE_CONFIDENCE
  ) {
    adjustedConfidence = CONFIRMED_TYPE_CONFIDENCE;
  }

  return adjustedConfidence;
}

/**
 * Apply final edge case adjustments to type and confidence
 */
function applyFinalAdjustments(
  finalType: ItemType,
  confidence: number,
  scores: Record<ItemType, { score: number; matches: number }>
): { type: ItemType; confidence: number } {
  let returnType = finalType;
  let returnConfidence = confidence;

  // Final edge case handling for action vs note prioritization
  if (finalType === 'note' && scores.action.score > ACTION_NOTE_PRIORITY_THRESHOLD * scores.note.score) {
    returnType = 'action';
    if (returnConfidence < CONFIRMED_TYPE_CONFIDENCE) {
      returnConfidence = CONFIRMED_TYPE_CONFIDENCE;
    }
  }

  return { type: returnType, confidence: returnConfidence };
}

/**
 * Build detailed reasoning string
 */
function buildReasoning(
  finalType: ItemType,
  scores: Record<ItemType, { score: number; matches: number }>,
  keywords: string[]
): string {
  const scoreSummary = `Scores - note: ${scores.note.score.toFixed(2)}, action: ${scores.action.score.toFixed(2)}, reminder: ${scores.reminder.score.toFixed(2)}`;
  const keywordSummary =
    keywords.length > 0
      ? `Matched keywords (${keywords.length}): ${keywords.join(', ')}`
      : 'No specific keywords matched';

  let reasoningDetail: string;
  if (finalType === 'action') {
    reasoningDetail =
      'Classified as action because action-related patterns had the strongest score after refined prioritization.';
  } else if (finalType === 'reminder') {
    reasoningDetail =
      'Classified as reminder because reminder-related patterns had the strongest score after refined prioritization.';
  } else if (finalType === 'note') {
    reasoningDetail =
      'Classified as note because informational patterns outweighed action and reminder indicators after refined prioritization.';
  } else {
    reasoningDetail =
      'Type could not be confidently determined; using highest available score with refined prioritization.';
  }

  return `${reasoningDetail} ${scoreSummary}. ${keywordSummary}.`;
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

  // Boost reminder score if date/time is present with action indicators
  if (hasDateTime && scores.action.score > 0) {
    scores.reminder.score += scores.action.score * DATE_TIME_ACTION_MULTIPLIER;
    scores.reminder.matches += 1;
  }

  // Apply phrase boosts and score adjustments
  applyPhraseBoosts(scores, text);
  applyScoreAdjustments(scores);

  const maxScore = Math.max(scores.action.score, scores.reminder.score, scores.note.score);

  // Check for early return condition: no matches found
  if (maxScore === 0) {
    return {
      type: 'note',
      confidence: CATCHALL_NOTE_CONFIDENCE,
      reasoning: 'Default to note - no strong action or reminder indicators',
      keywords,
    };
  }

  // Calculate confidence and select type
  const baseConfidence = calculateBaseConfidence(scores);
  const finalType = selectBestType(scores);
  const normalizedConfidence = normalizeFinalConfidence(baseConfidence, finalType, scores);

  // Apply final adjustments
  const { type: returnType, confidence: returnConfidence } = applyFinalAdjustments(
    finalType,
    normalizedConfidence,
    scores
  );

  // Build detailed reasoning based on the final returned type
  const reasoning = buildReasoning(returnType, scores, keywords);

  return {
    type: returnType,
    confidence: returnConfidence,
    reasoning,
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
