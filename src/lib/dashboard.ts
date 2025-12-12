import { Item, Capture, InferredAttributes } from './types'
import { getMissingFields, calculateReviewPriority } from './inference'

export interface ReviewQueueItem {
  captureId: string
  text: string
  inferredAttributes: InferredAttributes
  missingFields: string[]
  priority: number
}

export function getReviewQueue(
  captures: Capture[],
  items: Item[]
): ReviewQueueItem[] {
  const reviewItems: ReviewQueueItem[] = []
  
  for (const capture of captures) {
    if (!capture.needsTypeConfirmation) continue
    
    const attributes: InferredAttributes = {
      type: capture.inferredType,
      typeConfidence: capture.typeConfidence
    }
    const missingFields = getMissingFields(attributes)
    
    if (missingFields.length > 0) {
      reviewItems.push({
        captureId: capture.id,
        text: capture.text,
        inferredAttributes: attributes,
        missingFields: missingFields || [],
        priority: calculateReviewPriority(attributes, capture.createdAt)
      })
    }
  }
  
  return reviewItems
    .sort((a, b) => b.priority - a.priority)
    .slice(0, 5)
}

export function getNextAction(items: Item[]): Item | null {
  const now = Date.now()
  const activeActions = items.filter(
    item => item.type === 'action' && !item.completed
  )
  
  if (activeActions.length === 0) return null
  
  const scored = activeActions.map(item => {
    let score = 0
    
    if (item.priority === 'high') score += 100
    else if (item.priority === 'medium') score += 50
    else score += 10
    
    if (item.dueDate) {
      const daysUntilDue = (item.dueDate - now) / (1000 * 60 * 60 * 24)
      if (daysUntilDue < 0) score += 200
      else if (daysUntilDue < 1) score += 150
      else if (daysUntilDue < 3) score += 75
      else if (daysUntilDue < 7) score += 25
    }
    
    const age = now - item.createdAt
    const daysOld = age / (1000 * 60 * 60 * 24)
    if (daysOld > 7) score += 30
    else if (daysOld > 3) score += 15
    
    return { item, score }
  })
  
  scored.sort((a, b) => b.score - a.score)
  return scored[0]?.item || null
}

export function getQuickWins(items: Item[]): Item[] {
  const activeActions = items.filter(
    item => item.type === 'action' && !item.completed
  )
  
  const quickWins = activeActions.filter(item => {
    if (!item.text) return false
    
    const wordCount = item.text.split(/\s+/).length
    const hasLowPriority = !item.priority || item.priority === 'low'
    const isShort = wordCount < 10
    
    return hasLowPriority && isShort
  })
  
  return quickWins.slice(0, 3)
}

export function getRecentCaptures(
  captures: Capture[],
  limit: number = 5
): Capture[] {
  return [...captures]
    .filter(c => !c.needsTypeConfirmation)
    .sort((a, b) => b.createdAt - a.createdAt)
    .slice(0, limit)
}
