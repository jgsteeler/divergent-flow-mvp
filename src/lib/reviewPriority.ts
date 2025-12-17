import { Item, ReviewItem } from './types'

export function getTopReviewItems(items: Item[], limit: number = 3): ReviewItem[] {
  const reviewItems: ReviewItem[] = []

  for (const item of items) {
    // Skip items that have been reviewed with high confidence
    if (item.lastReviewedAt && 
        item.typeConfidence && item.typeConfidence >= 85 &&
        item.collectionConfidence && item.collectionConfidence >= 85) {
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
    else if (
      item.typeConfidence === undefined ||
      isNaN(item.typeConfidence) ||
      item.typeConfidence < 0 ||
      item.typeConfidence > 100
    ) {
      priority = 900
      reason = 'Invalid data'
    }
    // Medium priority: Low type confidence
    else if (item.typeConfidence < 85) {
      priority = 800
      reason = `Low confidence (${item.typeConfidence}%)`
    }
    // Medium priority: Low collection confidence
    else if (item.collectionConfidence !== undefined && item.collectionConfidence < 85) {
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
