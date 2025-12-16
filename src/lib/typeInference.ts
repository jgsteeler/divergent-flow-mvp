import { ItemType, TypeLearningData } from './types'

interface TypePattern {
  patterns: RegExp[]
  type: ItemType
  weight: number
}

const REMINDER_PATTERNS: TypePattern = {
  patterns: [
    /remind\s+me(\s+to)?/i,
    /follow\s+up(\s+on)?/i,
    /check\s+in(\s+on)?/i,
    /don't\s+forget/i,
    /remember\s+to/i,
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
    /^(i\s+)?need\s+to/i,
    /^(i\s+)?(have\s+to|must)/i,
    /^(i\s+)?should/i,
    /^create\s+(a\s+)?/i,
    /^make\s+(a\s+)?/i,
    /^write\s+(a\s+)?/i,
    /^send\s+(a\s+)?/i,
    /^call\s+/i,
    /^email\s+/i,
    /^buy\s+/i,
    /^fix\s+/i,
    /^update\s+/i,
    /^finish\s+/i,
    /^complete\s+/i,
    /^schedule\s+/i,
    /^book\s+(a\s+)?/i,
    /^prepare\s+/i,
    /^review\s+/i,
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
  return score
}

function applyLearningData(
  text: string,
  learningData: TypeLearningData[]
): { type: ItemType; confidence: number; reasoning: string } | null {
  const recentLearning = learningData
    .filter(ld => ld.wasCorrect !== false)
    .slice(-50)

  for (const learning of recentLearning.reverse()) {
    const patternWords = learning.pattern.toLowerCase().split(/\s+/)
    const textWords = text.toLowerCase().split(/\s+/)
    
    const matchingWords = patternWords.filter(word => 
      textWords.some(tw => tw.includes(word) || word.includes(tw))
    )
    
    if (matchingWords.length >= Math.min(2, patternWords.length)) {
      const confidenceBoost = learning.confidence >= 80 ? 30 : 20
      return {
        type: learning.type,
        confidence: 70 + confidenceBoost,
        reasoning: `Similar to previous ${learning.type} patterns (matched ${matchingWords.length} keywords)`
      }
    }
  }
  
  return null
}

export function inferType(
  text: string,
  learningData: TypeLearningData[] = []
): { type: ItemType | null; confidence: number; reasoning: string } {
  const learned = applyLearningData(text, learningData)
  if (learned && learned.confidence >= 80) {
    return {
      type: learned.type,
      confidence: learned.confidence,
      reasoning: learned.reasoning
    }
  }

  let reminderScore = calculatePatternScore(text, REMINDER_PATTERNS)
  let actionScore = calculatePatternScore(text, ACTION_PATTERNS)
  let noteScore = calculatePatternScore(text, NOTE_PATTERNS)

  const maxScore = Math.max(reminderScore, actionScore, noteScore)

  if (learned) {
    const learnedBoost = learned.confidence / 100
    if (learned.type === 'reminder') reminderScore += learnedBoost
    if (learned.type === 'action') actionScore += learnedBoost
    if (learned.type === 'note') noteScore += learnedBoost
  }

  if (maxScore === 0) {
    return { type: null, confidence: 0, reasoning: 'No patterns matched' }
  }

  let inferredType: ItemType
  let patternName: string
  if (reminderScore > actionScore && reminderScore > noteScore) {
    inferredType = 'reminder'
    patternName = 'reminder keywords'
  } else if (actionScore > noteScore) {
    inferredType = 'action'
    patternName = 'action verbs'
  } else {
    inferredType = 'note'
    patternName = 'note indicators'
  }

  let confidence: number
  let strengthLabel: string
  if (maxScore >= 1.0) {
    confidence = 95
    strengthLabel = 'Strong match'
  } else if (maxScore >= 0.8) {
    confidence = 75
    strengthLabel = 'Good match'
  } else {
    confidence = 50
    strengthLabel = 'Weak match'
  }

  const reasoning = `${strengthLabel} with ${patternName}`

  return { type: inferredType, confidence, reasoning }
}

export async function saveTypeLearning(
  text: string,
  inferredType: ItemType | null,
  actualType: ItemType,
  confidence: number
): Promise<TypeLearningData> {
  const learningData: TypeLearningData = {
    pattern: text.toLowerCase().substring(0, 100),
    type: actualType,
    confidence,
    timestamp: Date.now(),
    wasCorrect: inferredType === actualType,
  };

  return learningData;
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
