import { LearningData } from './types'
import { 
  MIN_WORD_LENGTH_FOR_LEARNING, 
  MAX_LEARNING_PATTERNS,
  MIN_MATCHING_WORDS,
  LEARNED_COLLECTION_MAX_CONFIDENCE,
  LEARNED_COLLECTION_BASE_CONFIDENCE,
  LEARNED_COLLECTION_CONFIDENCE_DIVISOR,
  LEARNING_CONFIDENCE_THRESHOLD
} from './constants'

interface CollectionPattern {
  patterns: RegExp[]
  collection: string
  weight: number
}

const COLLECTION_PATTERNS: CollectionPattern[] = [
  {
    patterns: [
      /\b(meeting|standup|sync|call|presentation|deadline|project|client|boss|colleague|office)\b/i,
      /\b(sprint|deploy|release|code|bug|review|PR|pull request|merge)\b/i,
      /\b(email|slack|teams|zoom|calendar|schedule)\b/i,
    ],
    collection: 'Work',
    weight: 1.0,
  },
  {
    patterns: [
      /\b(grocery|groceries|shopping|store|market|buy|purchase|order)\b/i,
      /\b(pick up|drop off|return|exchange)\b/i,
      /\b(pharmacy|drugstore|prescription)\b/i,
    ],
    collection: 'Errands',
    weight: 1.0,
  },
  {
    patterns: [
      /\b(family|kids|children|spouse|partner|parents|friend|friends)\b/i,
      /\b(birthday|anniversary|celebration|party|dinner|lunch)\b/i,
      /\b(hobby|hobbies|read|book|watch|movie|show|game)\b/i,
      /\b(vacation|trip|travel|weekend|holiday)\b/i,
    ],
    collection: 'Personal',
    weight: 0.9,
  },
  {
    patterns: [
      /\b(doctor|appointment|checkup|dentist|therapy|counseling|medication)\b/i,
      /\b(gym|workout|exercise|run|walk|yoga|fitness)\b/i,
      /\b(sleep|rest|relax|meditate|mental health)\b/i,
    ],
    collection: 'Health',
    weight: 1.0,
  },
  {
    patterns: [
      /\b(bill|bills|payment|pay|invoice|budget|finance|money|bank)\b/i,
      /\b(tax|taxes|insurance|mortgage|rent)\b/i,
    ],
    collection: 'Finance',
    weight: 1.0,
  },
  {
    patterns: [
      /\b(fix|repair|clean|organize|maintenance|chore|chores)\b/i,
      /\b(laundry|dishes|vacuum|tidy|declutter)\b/i,
      /\b(home|house|apartment|kitchen|bathroom)\b/i,
    ],
    collection: 'Home',
    weight: 0.9,
  },
  {
    patterns: [
      /\b(idea|concept|thought|brainstorm|inspiration|creative)\b/i,
      /\b(learn|study|research|course|tutorial|practice)\b/i,
      /\b(note:|remember:|interesting)\b/i,
    ],
    collection: 'Ideas',
    weight: 0.8,
  },
]

function calculateCollectionScore(text: string, pattern: CollectionPattern): number {
  let score = 0
  for (const regex of pattern.patterns) {
    if (regex.test(text)) {
      score += pattern.weight
      break
    }
  }
  return score
}

function applyCollectionLearning(
  text: string,
  learningData: LearningData[]
): { collection: string; confidence: number; reasoning: string } | null {
  const recentLearning = learningData
    .filter(ld => ld.correctedAttributes.collection && ld.wasCorrect !== false)
    .slice(-MAX_LEARNING_PATTERNS)

  for (const learning of recentLearning.reverse()) {
    const patternWords = learning.originalText.toLowerCase().split(/\s+/)
    const textWords = text.toLowerCase().split(/\s+/)
    
    const matchingWords = patternWords.filter(word => 
      word.length > MIN_WORD_LENGTH_FOR_LEARNING && textWords.some(tw => tw.includes(word) || word.includes(tw))
    )
    
    if (matchingWords.length >= Math.min(MIN_MATCHING_WORDS, patternWords.length)) {
      const confidenceBoost = learning.correctedAttributes.collectionConfidence || 70
      return {
        collection: learning.correctedAttributes.collection!,
        confidence: Math.min(LEARNED_COLLECTION_MAX_CONFIDENCE, LEARNED_COLLECTION_BASE_CONFIDENCE + confidenceBoost / LEARNED_COLLECTION_CONFIDENCE_DIVISOR),
        reasoning: `Similar to previous "${learning.correctedAttributes.collection}" items (${matchingWords.length} matching keywords)`
      }
    }
  }
  
  return null
}

export function inferCollection(
  text: string,
  learningData: LearningData[] = []
): { collection: string | null; confidence: number; reasoning: string } {
  const learned = applyCollectionLearning(text, learningData)
  if (learned && learned.confidence >= LEARNING_CONFIDENCE_THRESHOLD) {
    return learned
  }

  const scores = COLLECTION_PATTERNS.map(pattern => ({
    collection: pattern.collection,
    score: calculateCollectionScore(text, pattern)
  }))

  const maxScore = Math.max(...scores.map(s => s.score))

  if (learned) {
    const learnedScore = scores.find(s => s.collection === learned.collection)
    if (learnedScore) {
      learnedScore.score += learned.confidence / 100
    }
  }

  if (maxScore === 0) {
    return { collection: null, confidence: 0, reasoning: 'No collection patterns matched' }
  }

  const bestMatch = scores.reduce((prev, current) => 
    current.score > prev.score ? current : prev
  )

  let confidence: number
  let strengthLabel: string
  if (maxScore >= 1.0) {
    confidence = 85
    strengthLabel = 'Strong match'
  } else if (maxScore >= 0.8) {
    confidence = 70
    strengthLabel = 'Good match'
  } else {
    confidence = 50
    strengthLabel = 'Weak match'
  }

  const reasoning = `${strengthLabel} with ${bestMatch.collection} patterns`

  return {
    collection: bestMatch.collection,
    confidence,
    reasoning
  }
}

export function getCommonCollections(): string[] {
  return COLLECTION_PATTERNS.map(p => p.collection)
}
