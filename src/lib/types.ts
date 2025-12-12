export type ItemType = 'note' | 'action' | 'reminder'

export type Priority = 'low' | 'medium' | 'high'

export type ConfidenceLevel = 'high' | 'medium' | 'low'

export interface InferredAttributes {
  type?: ItemType
  typeConfidence?: ConfidenceLevel
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
  typeConfidence?: ConfidenceLevel
  needsTypeConfirmation?: boolean
  processedAt?: number
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
}

export interface TypeLearningData {
  pattern: string
  type: ItemType
  confidence: ConfidenceLevel
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
  captureId: string
  text: string
  inferredAttributes: InferredAttributes
  missingFields: string[]
  priority: number
}
