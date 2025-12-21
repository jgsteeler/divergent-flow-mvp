import { LearningData, CollectionInference } from './types'
import { 
  MIN_WORD_LENGTH_FOR_LEARNING, 
  MAX_LEARNING_PATTERNS,
  MIN_MATCHING_WORDS,
  LEARNED_COLLECTION_MAX_CONFIDENCE,
  LEARNED_COLLECTION_BASE_CONFIDENCE,
  LEARNED_COLLECTION_CONFIDENCE_DIVISOR,
  MEDIUM_CONFIDENCE_THRESHOLD,
} from './constants'

function applyCollectionLearning(
  text: string,
  learningData: LearningData[]
): CollectionInference[] {
  // Include learning data where:
  // 1. A collection was provided in correctedAttributes
  // 2. wasCorrect is not explicitly false when there was an inference
  // (if wasCorrect is false but no collection was inferred, still learn from it)
  const recentLearning = learningData
    .filter(ld => {
      if (!ld.correctedAttributes.collection) return false
      // If no collection was inferred originally, always learn from it
      if (!ld.inferredAttributes.collection) return true
      // If collection was inferred, only learn if it wasn't marked as incorrect
      return ld.wasCorrect !== false
    })
    .slice(-MAX_LEARNING_PATTERNS)

  const inferences: CollectionInference[] = []
  const seen = new Set<string>()

  for (const learning of recentLearning.reverse()) {
    if (seen.has(learning.correctedAttributes.collection!)) {
      continue
    }

    const patternWords = learning.originalText.toLowerCase().split(/\s+/)
    const textWords = text.toLowerCase().split(/\s+/)
    
    const matchingWords = patternWords.filter(word => 
      word.length >= MIN_WORD_LENGTH_FOR_LEARNING && textWords.some(tw => tw.includes(word) || word.includes(tw))
    )
    
    if (matchingWords.length >= Math.min(MIN_MATCHING_WORDS, patternWords.length)) {
      const confidenceBoost = learning.correctedAttributes.collectionConfidence || 70
      const confidence = Math.min(
        LEARNED_COLLECTION_MAX_CONFIDENCE, 
        LEARNED_COLLECTION_BASE_CONFIDENCE + confidenceBoost / LEARNED_COLLECTION_CONFIDENCE_DIVISOR
      )
      
      inferences.push({
        collection: learning.correctedAttributes.collection!,
        confidence,
        reasoning: `Similar to previous "${learning.correctedAttributes.collection}" items (${matchingWords.length} matching keywords)`
      })
      
      seen.add(learning.correctedAttributes.collection!)
    }
  }
  
  return inferences.sort((a, b) => b.confidence - a.confidence)
}

export function inferCollections(
  text: string,
  learningData: LearningData[] = []
): CollectionInference[] {
  // Return multiple inferences sorted by confidence
  return applyCollectionLearning(text, learningData)
}

export function inferCollection(
  text: string,
  learningData: LearningData[] = []
): { collection: string | null; confidence: number; reasoning: string } {
  // Backward compatibility - return single highest confidence
  const inferences = inferCollections(text, learningData)
  
  if (inferences.length > 0) {
    return inferences[0]
  }

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

export function getRelevantInferences(
  inferences: CollectionInference[]
): CollectionInference[] {
  // Filter to show only medium+ confidence or highest if all are low
  const mediumPlus = inferences.filter(inf => inf.confidence >= MEDIUM_CONFIDENCE_THRESHOLD)
  
  if (mediumPlus.length > 0) {
    return mediumPlus
  }
  
  // Return highest confidence inference if all are below medium
  if (inferences.length > 0) {
    return [inferences[0]]
  }
  
  return []
}
