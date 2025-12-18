import { Item, ReviewItem } from './types'
import { HIGH_CONFIDENCE_THRESHOLD } from './constants'

function isValidConfidence(confidence: number | undefined): boolean {
  return typeof confidence === 'number' && !isNaN(confidence) && confidence >= 0 && confidence <= 100
}

export function getTopReviewItems(items: Item[], limit: number = 3): ReviewItem[] {
  const reviewItems: ReviewItem[] = []

  for (const item of items) {
    // Skip items that have been reviewed with high confidence
    if (item.lastReviewedAt && 
        item.typeConfidence && item.typeConfidence >= HIGH_CONFIDENCE_THRESHOLD &&
        item.collectionConfidence && item.collectionConfidence >= HIGH_CONFIDENCE_THRESHOLD) {
      continue
    }

    let priority = 0
    let reason = ''

    // Highest priority: Missing type
    if (!item.inferredType) {
      priority = 1000
      reason = 'No type'
    }
    // High priority: Missing collection
    else if (!item.collection) {
      priority = 900
      reason = 'No collection'
    }
    // High priority: Invalid confidence data
    else if (!isValidConfidence(item.typeConfidence)) {
      priority = 900
      reason = 'Invalid data'
    }
    // Medium priority: Low type confidence
    else if (item.typeConfidence < HIGH_CONFIDENCE_THRESHOLD) {
      priority = 800
      reason = `Low confidence (${item.typeConfidence}%)`
    }
    // Medium priority: Low collection confidence
    else if (item.collectionConfidence !== undefined && item.collectionConfidence < HIGH_CONFIDENCE_THRESHOLD) {
      priority = 700
      reason = `Collection uncertain (${item.collectionConfidence}%)`
    }
    // Low priority: Never reviewed but has good confidence
    else if (!item.lastReviewedAt) {
      priority = 500
      reason = 'Needs review'
    }

    if (priority > 0) {
      // Add age factor to break ties
      const age = Date.now() - (item.lastReviewedAt || item.createdAt)
      const hoursOld = age / (1000 * 60 * 60)
      priority += hoursOld

      reviewItems.push({
        item,
        reviewPriority: priority,
        reason,
      })
    }
  }

  // Sort by priority (descending) and take top N
  return reviewItems
    .sort((a, b) => b.reviewPriority - a.reviewPriority)
    .slice(0, limit)
}
