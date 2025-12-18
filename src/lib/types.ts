export type ItemType = 'note' | 'action' | 'reminder'

export type Priority = 'low' | 'medium' | 'high'

export interface Item {
  id: string
  text: string
  createdAt: number
  inferredType?: ItemType
  typeConfidence?: number
  confidenceReasoning?: string
  lastReviewedAt?: number
  migratedCapture: boolean
  // New properties for collection and date/time inference
  collection?: string
  collectionConfidence?: number
  priority?: Priority
  dueDate?: number
  startDate?: number
  reviewDate?: number
  remindTime?: number
  context?: string
  tags?: string[]
  type?: ItemType
  completed?: boolean
}

export interface TypeLearningData {
  pattern: string
  type: ItemType
  confidence: number
  timestamp: number
  wasCorrect?: boolean
}

export interface ReviewItem {
  item: Item
  reviewPriority: number
  reason: string
}

export interface InferredAttributes {
  type?: ItemType | null
  collection?: string | null
  priority?: Priority | null
  dueDate?: number | null
  startDate?: number | null
  reviewDate?: number | null
  remindTime?: number | null
  context?: string | null
  tags?: string[] | null
  typeConfidence?: number
  collectionConfidence?: number
}

export interface LearningData {
  originalText: string
  inferredAttributes: InferredAttributes
  correctedAttributes: InferredAttributes
  timestamp: number
  wasCorrect?: boolean
}

export interface Capture {
  id: string
  text: string
  createdAt: number
  inferredType?: ItemType
  typeConfidence?: number
  typeConfirmed?: boolean
}

export interface CollectionInference {
  collection: string
  confidence: number
  reasoning: string
}
