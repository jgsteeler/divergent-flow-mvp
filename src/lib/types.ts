export type ItemType = 'note' | 'action' | 'reminder'

export type Priority = 'low' | 'medium' | 'high'

export type Estimate =
  | '5min'
  | '15min'
  | '30min'
  | '1hour'
  | '2hours'
  | 'halfday'
  | 'day'
  | 'multiday'

export interface Item {
  id: string
  text: string
  createdAt: number
  inferredType?: ItemType
  typeConfidence?: number
  confidenceReasoning?: string
  lastReviewedAt?: number
  migratedCapture: boolean
  // Merged properties from both branches
  collection?: string
  collectionConfidence?: number
  dueDate?: number
  startDate?: number
  reviewDate?: number
  remindTime?: number
  context?: string
  tags?: string[]
  type?: ItemType
  completed?: boolean
  priority?: Priority
  priorityConfidence?: number
  priorityReasoning?: string
  estimate?: Estimate
  estimateConfidence?: number
  estimateReasoning?: string
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
  priorityConfidence?: number
  estimateConfidence?: number
  priorityReasoning?: string
  estimateReasoning?: string
  estimate?: Estimate | null
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

export interface PriorityLearningData {
  pattern: string
  priority: Priority
  confidence: number
  timestamp: number
  wasCorrect?: boolean
}

export interface EstimateLearningData {
  pattern: string
  estimate: Estimate
  confidence: number
  timestamp: number
  wasCorrect?: boolean
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
  priorityConfidence?: number
  estimateConfidence?: number
  priorityReasoning?: string
  estimateReasoning?: string
  estimate?: Estimate | null
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

export interface PriorityLearningData {
  pattern: string
  priority: Priority
  confidence: number
  timestamp: number
  wasCorrect?: boolean
}

export interface EstimateLearningData {
  pattern: string
  estimate: Estimate
  confidence: number
  timestamp: number
  wasCorrect?: boolean
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
  priorityConfidence?: number
  estimateConfidence?: number
  priorityReasoning?: string
  estimateReasoning?: string
  estimate?: Estimate | null
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

export interface PriorityLearningData {
  pattern: string
  priority: Priority
  confidence: number
  timestamp: number
  wasCorrect?: boolean
}

export interface EstimateLearningData {
  pattern: string
  estimate: Estimate
  confidence: number
  timestamp: number
  wasCorrect?: boolean
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
  priorityConfidence?: number
  estimateConfidence?: number
  priorityReasoning?: string
  estimateReasoning?: string
  estimate?: Estimate | null
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

export interface PriorityLearningData {
  pattern: string
  priority: Priority
  confidence: number
  timestamp: number
  wasCorrect?: boolean
}

export interface EstimateLearningData {
  pattern: string
  estimate: Estimate
  confidence: number
  timestamp: number
  wasCorrect?: boolean
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
  priorityConfidence?: number
  estimateConfidence?: number
  priorityReasoning?: string
  estimateReasoning?: string
  estimate?: Estimate | null
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

export interface PriorityLearningData {
  pattern: string
  priority: Priority
  confidence: number
  timestamp: number
  wasCorrect?: boolean
}

export interface EstimateLearningData {
  pattern: string
  estimate: Estimate
  confidence: number
  timestamp: number
  wasCorrect?: boolean
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
  priorityConfidence?: number
  estimateConfidence?: number
  priorityReasoning?: string
  estimateReasoning?: string
  estimate?: Estimate | null
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

export interface PriorityLearningData {
  pattern: string
  priority: Priority
  confidence: number
  timestamp: number
  wasCorrect?: boolean
}

export interface EstimateLearningData {
  pattern: string
  estimate: Estimate
  confidence: number
  timestamp: number
  wasCorrect?: boolean
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
  priorityConfidence?: number
  estimateConfidence?: number
  priorityReasoning?: string
  estimateReasoning?: string
  estimate?: Estimate | null
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

export interface PriorityLearningData {
  pattern: string
  priority: Priority
  confidence: number
  timestamp: number
  wasCorrect?: boolean
}

export interface EstimateLearningData {
  pattern: string
  estimate: Estimate
  confidence: number
  timestamp: number
  wasCorrect?: boolean
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
  priorityConfidence?: number
  estimateConfidence?: number
  priorityReasoning?: string
  estimateReasoning?: string
  estimate?: Estimate | null
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

export interface PriorityLearningData {
  pattern: string
  priority: Priority
  confidence: number
  timestamp: number
  wasCorrect?: boolean
}

export interface EstimateLearningData {
  pattern: string
  estimate: Estimate
  confidence: number
  timestamp: number
  wasCorrect?: boolean
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
  priorityConfidence?: number
  estimateConfidence?: number
  priorityReasoning?: string
  estimateReasoning?: string
  estimate?: Estimate | null
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

export interface PriorityLearningData {
  pattern: string
  priority: Priority
  confidence: number
  timestamp: number
  wasCorrect?: boolean
}

export interface EstimateLearningData {
  pattern: string
  estimate: Estimate
  confidence: number
  timestamp: number
  wasCorrect?: boolean
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
  priorityConfidence?: number
  estimateConfidence?: number
  priorityReasoning?: string
  estimateReasoning?: string
  estimate?: Estimate | null
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

export interface PriorityLearningData {
  pattern: string
  priority: Priority
  confidence: number
  timestamp: number
  wasCorrect?: boolean
}

export interface EstimateLearningData {
  pattern: string
  estimate: Estimate
  confidence: number
  timestamp: number
  wasCorrect?: boolean
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
  priorityConfidence?: number
  estimateConfidence?: number
  priorityReasoning?: string
  estimateReasoning?: string
  estimate?: Estimate | null
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

