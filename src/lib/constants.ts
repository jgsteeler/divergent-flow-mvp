// Confidence thresholds used throughout the app
export const HIGH_CONFIDENCE_THRESHOLD = 95
export const MEDIUM_CONFIDENCE_THRESHOLD = 70
export const CONFIRMED_CONFIDENCE = 100

// Learning constants
export const MIN_WORD_LENGTH_FOR_LEARNING = 3
export const MAX_LEARNING_PATTERNS = 50
export const MAX_PATTERN_LENGTH = 100
export const DEFAULT_LEARNING_CONFIDENCE = 80

// Collection inference constants
export const MIN_MATCHING_WORDS = 2
export const LEARNED_COLLECTION_MAX_CONFIDENCE = 100 // Allow collections to reach maximum confidence
export const LEARNED_COLLECTION_BASE_CONFIDENCE = 65 // Increased to allow reaching 95%+ with perfect confirmations
export const LEARNED_COLLECTION_CONFIDENCE_DIVISOR = 3
export const LEARNING_CONFIDENCE_THRESHOLD = 75
export const COLLECTION_HIGH_CONFIDENCE_THRESHOLD = 95 // Collections >= this threshold are auto-displayed without search box
