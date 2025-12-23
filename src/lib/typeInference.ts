import { ItemType, TypeLearningData } from './types'
import { extractDateTimeFromText } from './dateParser'
import { 
  DATE_TIME_REMINDER_BOOST, 
  ACTION_REMINDER_THRESHOLD,
  NOTE_PATTERN_THRESHOLD,
  CATCHALL_NOTE_CONFIDENCE
} from './constants'

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

const REMINDER_PATTERNS: TypePattern = {
  patterns: [
    // Core reminder phrases from Phase 2 requirements
    /remind\s+me(\s+to)?/i,
    /remember\s+to/i,
    /don't\s+forget(\s+to)?/i,
    /need\s+to\s+remember/i,
    /^reminder:/i,
    /^remember:/i,
    // Additional reminder indicators
    /follow\s+up(\s+on)?/i,
    /check\s+in(\s+on)?/i,
    /ping\s+me/i,
    /alert\s+me/i,
    /notify\s+me/i,
    /schedule\s+(a\s+)?reminder/i,
  ],
  type: 'reminder',
  weight: 1.0
}

const ACTION_PATTERNS: TypePattern = {
  patterns: [
    // Modal/imperative patterns
    /^(i\s+)?need\s+to/i,
    /^(i\s+)?(have\s+to|must)/i,
    /^(i\s+)?should/i,
    // Phase 2 preloaded action phrases - "Create a...", "Build...", etc.
    /^create\s+(a\s+)?/i,
    /^take\s+the/i,
    /^build\s+/i,
    /^fix\s+/i,
    /^update\s+/i,
    /^review\s+/i,
    /^send\s+(a\s+)?/i,
    /^call\s+/i,
    /^email\s+/i,
    /^schedule\s+/i,
    /^complete\s+/i,
    /^finish\s+/i,
    /^submit\s+/i,
    /^prepare\s+/i,
    /^order\s+/i,
    // Additional common action patterns
    /^make\s+(a\s+)?/i,
    /^write\s+(a\s+)?/i,
    /^buy\s+/i,
    /^book\s+(a\s+)?/i,
    /^research\s+/i,
    /^organize\s+/i,
    /^plan\s+/i,
    /^draft\s+/i,
  ],
  type: 'action',
  weight: 0.9
}

const NOTE_PATTERNS: TypePattern = {
  patterns: [
    /^note:/i,
    /^idea:/i,
    /^thought:/i,
    /interesting\s+that/i,
    /learned\s+(that|about)/i,
    /found\s+out/i,
    /discovered/i,
    /realized/i,
    /noticed/i,
    /observation:/i,
    /insight:/i,
  ],
  type: 'note',
  weight: 0.8
}

function calculatePatternScore(text: string, patterns: TypePattern): number {
  let score = 0
  for (const pattern of patterns.patterns) {
    if (pattern.test(text)) {
      score += patterns.weight
      break
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
    note: { score: 0, matches: 0 }
  }
  
  const validLearning = learningData.filter(ld => ld.wasCorrect !== false || ld.isDefault)
  
  for (const learning of validLearning) {
    for (const keyword of keywords) {
      for (const learnedKeyword of learning.keywords) {
        if (keyword.includes(learnedKeyword) || learnedKeyword.includes(keyword)) {
          const matchStrength = keyword === learnedKeyword ? 1.0 : 0.7
          const weight = (learning.confidence / 100) * matchStrength
          scores[learning.type].score += weight
          scores[learning.type].matches += 1
          break
        }
      }
    }
  }
  
  return scores
}

/**
 * Infer type using keyword-based matching against learning data
 */
export function inferType(
  text: string,
  learningData: TypeLearningData[] = []
): { type: ItemType | null; confidence: number; reasoning: string; keywords: string[] } {
  const keywords = parseKeywords(text)
  const { dateTime } = extractDateTimeFromText(text)
  const hasDateTime = dateTime !== null
  
  const scores = calculateTypeScores(keywords, learningData)
  
  // Boost reminder score if date/time is present with action indicators
  if (hasDateTime && scores.action.score > 0) {
    scores.reminder.score += scores.action.score * 0.5
    scores.reminder.matches += 1
  }
  
  const maxScore = Math.max(scores.action.score, scores.reminder.score, scores.note.score)
  
  // Catchall: if no matches, default to note
  if (maxScore === 0 || (scores.action.matches === 0 && scores.reminder.matches === 0)) {
    return {
      type: 'note',
      confidence: CATCHALL_NOTE_CONFIDENCE,
      reasoning: 'Default to note - no strong action or reminder indicators',
      keywords
    }
  }

  // Check for date/time presence (Phase 2 requirement: strong reminder indicator)
  const { dateTime } = extractDateTimeFromText(text)
  const hasDateTime = dateTime !== null

  let reminderScore = calculatePatternScore(text, REMINDER_PATTERNS)
  let actionScore = calculatePatternScore(text, ACTION_PATTERNS)
  let noteScore = calculatePatternScore(text, NOTE_PATTERNS)

  // Phase 2: Date/time presence is a strong reminder indicator
  // If text has date/time and action patterns but no reminder patterns, boost reminder
  if (hasDateTime && reminderScore === 0 && actionScore > 0) {
    reminderScore = actionScore + DATE_TIME_REMINDER_BOOST // Boost to prioritize reminder over action
  }

  const maxScore = Math.max(reminderScore, actionScore, noteScore)

  if (learned) {
    const learnedBoost = learned.confidence / 100
    if (learned.type === 'reminder') reminderScore += learnedBoost
    if (learned.type === 'action') actionScore += learnedBoost
    if (learned.type === 'note') noteScore += learnedBoost
  }

  // Phase 2 Catchall Logic: If no patterns matched or all scores are low,
  // default to note with high confidence
  if (maxScore === 0 || (reminderScore < ACTION_REMINDER_THRESHOLD && actionScore < ACTION_REMINDER_THRESHOLD && noteScore < NOTE_PATTERN_THRESHOLD)) {
    return { 
      type: 'note', 
      confidence: CATCHALL_NOTE_CONFIDENCE, 
      reasoning: 'Default to note - no strong action or reminder indicators' 
    }
  }

  let inferredType: ItemType
  let typeScore: number
  let matchCount: number
  
  if (scores.reminder.score > scores.action.score && scores.reminder.score > scores.note.score) {
    inferredType = 'reminder'
    patternName = hasDateTime ? 'reminder keywords + date/time' : 'reminder keywords'
  } else if (actionScore > noteScore) {
    inferredType = 'action'
    typeScore = scores.action.score
    matchCount = scores.action.matches
  } else {
    inferredType = 'note'
    typeScore = scores.note.score
    matchCount = scores.note.matches
  }
  
  // Calculate confidence (0-100)
  let confidence = Math.min(95, Math.floor((typeScore / Math.max(1, keywords.length)) * 100) + (matchCount * 10))
  if (confidence < 50) confidence = 50
  
  // Build reasoning
  const reasons: string[] = []
  if (matchCount > 0) {
    reasons.push(`${matchCount} keyword match${matchCount > 1 ? 'es' : ''}`)
  }
  if (hasDateTime && inferredType === 'reminder') {
    reasons.push('date/time detected')
  }
  
  const reasoning = reasons.length > 0 ? reasons.join(', ') : `Inferred as ${inferredType}`
  
  return { type: inferredType, confidence, reasoning, keywords }
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
