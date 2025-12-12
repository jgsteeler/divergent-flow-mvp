export type ItemType = 'note' | 'action' | 'reminder'

export type Priority = 'low' | 'medium' | 'high'

export interface InferredAttributes {
  type?: ItemType
  typeConfidence?: number
  collection?: string
  priority?: Priority
  dueDate?: number
  context?: string
  tags?: string[]
}

export interface Capture {
  id: string
  text: string
  createdAt: number
  inferredType?: ItemType
  typeConfidence?: number
  needsTypeConfirmation?: boolean
  processedAt?: number
  lastReviewedAt?: number
}

export interface Item {
  id: string
  type: ItemType
  text: string
  collection: string
  priority?: Priority
  dueDate?: number
  context?: string
  tags?: string[]
  completed: boolean
  createdAt: number
  completedAt?: number
  lastReviewedAt?: number
}

export interface TypeLearningData {
  pattern: string
  type: ItemType
  confidence: number
  timestamp: number
  wasCorrect?: boolean
}

export interface LearningData {
  originalText: string
  inferredAttributes: InferredAttributes
  correctedAttributes: InferredAttributes
  timestamp: number
}

export interface ReviewItem {
  capture: Capture
  reviewPriority: number
  reason: string
}
