import { Item, ReviewItem } from './types'

function isValidConfidence(confidence: number | undefined): boolean {
  return typeof confidence === 'number' && !isNaN(confidence) && confidence >= 0 && confidence <= 100
}

function needsReview(item: Item): boolean {
  if (!item.inferredType) return true
  if (!isValidConfidence(item.typeConfidence)) return true
  if (item.typeConfidence !== undefined && item.typeConfidence < 85) return true
  return false
}

export function calculateReviewPriority(items: Item[]): ReviewItem[] {
  const reviewItems: ReviewItem[] = items
    .filter(item => needsReview(item))
    .map(item => {
      let priority = 0
      let reason = ''

      if (!item.inferredType) {
        priority = 1000
        reason = 'No type assigned'
      }
      else if (!isValidConfidence(item.typeConfidence)) {
        priority = 900
        reason = 'Invalid confidence'
      }
      else if (item.typeConfidence !== undefined && item.typeConfidence < 85) {
        priority = 800
        reason = `Low confidence (${item.typeConfidence}%)`
      }

      return {
        item,
        reviewPriority: priority,
        reason
      }
    })
    .sort((a, b) => {
      if (b.reviewPriority !== a.reviewPriority) {
        return b.reviewPriority - a.reviewPriority
      }
      const aReview = a.item.lastReviewedAt || a.item.createdAt
      const bReview = b.item.lastReviewedAt || b.item.createdAt
      return aReview - bReview
    })

  return reviewItems
}

export function getTopReviewItems(items: Item[], limit: number = 3): ReviewItem[] {
  const prioritized = calculateReviewPriority(items)
  return prioritized.slice(0, limit)
}
