import { LearningData } from './types'
import { 
  MIN_WORD_LENGTH_FOR_LEARNING, 
  MAX_LEARNING_PATTERNS,
  MIN_MATCHING_WORDS,
  LEARNED_COLLECTION_MAX_CONFIDENCE,
  LEARNED_COLLECTION_BASE_CONFIDENCE,
  LEARNED_COLLECTION_CONFIDENCE_DIVISOR,
} from './constants'

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
  // Only use learning data - no pre-defined patterns
  const learned = applyCollectionLearning(text, learningData)
  
  if (learned) {
    return learned
  }

  // No learning data available - return null to prompt user
  return { 
    collection: null, 
    confidence: 0, 
    reasoning: 'No previous collections to learn from' 
  }
}

export function getLearnedCollections(learningData: LearningData[]): string[] {
  // Extract unique collections from learning data
  const collections = new Set<string>()
  
  learningData.forEach(ld => {
    if (ld.correctedAttributes.collection) {
      collections.add(ld.correctedAttributes.collection)
    }
  })
  
  return Array.from(collections).sort()
}
