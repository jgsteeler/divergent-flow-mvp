// Confidence thresholds used throughout the app
export const HIGH_CONFIDENCE_THRESHOLD = 95
export const MEDIUM_CONFIDENCE_THRESHOLD = 70
export const CONFIRMED_CONFIDENCE = 100

// Learning constants
export const MIN_WORD_LENGTH_FOR_LEARNING = 3
export const MAX_LEARNING_PATTERNS = 50
export const MAX_PATTERN_LENGTH = 100
export const DEFAULT_LEARNING_CONFIDENCE = 80

// Type inference constants
export const DATE_TIME_REMINDER_BOOST = 0.2
export const DATE_TIME_ACTION_MULTIPLIER = 0.5 // Multiplier for boosting reminder when date/time present with action
export const ACTION_REMINDER_THRESHOLD = 0.5
export const NOTE_PATTERN_THRESHOLD = 0.8
export const CATCHALL_NOTE_CONFIDENCE = 85

// Confidence calculation constants
export const PARTIAL_MATCH_WEIGHT = 0.75 // Weight multiplier for partial keyword matches
export const REMINDER_PHRASE_BOOST_DEFAULT = 4 // Default boost for reminder phrases
export const REMINDER_PHRASE_BOOST_DONT_FORGET = 3 // Reduced boost for "don't forget"
export const ACTION_PHRASE_BOOST_DEFAULT = 2 // Default boost for action phrases
export const ACTION_PHRASE_BOOST_SUBMIT = 3 // Stronger boost for "submit"
export const REMINDER_DOMINANCE_PENALTY = 0.2 // Multiplier to reduce reminder score when action keywords present
export const CONFIDENCE_CAP_BELOW_THRESHOLD = 94.9 // Cap confidence below 95 to fix boundary issues
export const EXACT_MATCH_SCORE_THRESHOLD = 0.9 // Threshold for considering a match as "exact" (90% of total)
export const EXACT_MATCH_CONFIDENCE = 95 // Confidence for exact matches
export const REMINDER_PREFIX_BOOST = 3 // Boost for explicit 'Reminder:' prefix
export const AMBIGUOUS_CASE_SCORE_THRESHOLD = 0.8 // Threshold for ambiguous cases (80% of total)
export const AMBIGUOUS_CASE_MIN_CONFIDENCE = 75 // Minimum confidence for ambiguous cases
export const ACTION_TYPE_MIN_CONFIDENCE = 90 // Minimum confidence boost for action type
export const CONFIRMED_TYPE_CONFIDENCE = 95 // Standard confidence for confirmed action/reminder types
export const ACTION_NOTE_PRIORITY_THRESHOLD = 0.8 // Threshold for prioritizing action over note (80% of note score)

// Default phrase confidence levels
export const DEFAULT_ACTION_PHRASE_CONFIDENCE = 95 // Confidence for default action phrases
export const DEFAULT_REMINDER_PHRASE_CONFIDENCE = 95 // Confidence for default reminder phrases
export const DEFAULT_NOTE_PHRASE_CONFIDENCE = 90 // Confidence for default note phrases

// Collection inference constants
export const MIN_MATCHING_WORDS = 2
export const LEARNED_COLLECTION_MAX_CONFIDENCE = 100 // Allow collections to reach maximum confidence
export const LEARNED_COLLECTION_BASE_CONFIDENCE = 65 // Increased to allow reaching 95%+ with perfect confirmations
export const LEARNED_COLLECTION_CONFIDENCE_DIVISOR = 3
export const LEARNING_CONFIDENCE_THRESHOLD = 75
export const COLLECTION_HIGH_CONFIDENCE_THRESHOLD = 95 // Collections >= this threshold are auto-displayed without search box
