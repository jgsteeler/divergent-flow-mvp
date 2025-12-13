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
