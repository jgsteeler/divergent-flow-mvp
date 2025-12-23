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
  
  // Boost note confidence when action/reminder have low confidence
  if (maxScore < 1.0 && scores.note.score < maxScore) {
    scores.note.score = maxScore * 0.9
  }
  
  // Determine winning type
  let inferredType: ItemType
  let typeScore: number
  let matchCount: number
  
  if (scores.reminder.score > scores.action.score && scores.reminder.score > scores.note.score) {
    inferredType = 'reminder'
    typeScore = scores.reminder.score
    matchCount = scores.reminder.matches
  } else if (scores.action.score > scores.note.score) {
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
