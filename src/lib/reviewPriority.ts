import { Item, ReviewItem } from './types'
import { HIGH_CONFIDENCE_THRESHOLD } from './constants'

function isValidConfidence(confidence: number | undefined): boolean {
  return typeof confidence === 'number' && !isNaN(confidence) && confidence >= 0 && confidence <= 100
}

function needsReview(item: Item): boolean {
  // Type review
  if (!item.inferredType) return true
  if (!isValidConfidence(item.typeConfidence)) return true
  if (item.typeConfidence !== undefined && item.typeConfidence < HIGH_CONFIDENCE_THRESHOLD) return true

  // Collection review
  if (!item.collection) return true
  if (!isValidConfidence(item.collectionConfidence)) return true
  if (item.collectionConfidence !== undefined && item.collectionConfidence < HIGH_CONFIDENCE_THRESHOLD) {
    return true
  }

  // Priority review (for actions and reminders)
  if (item.inferredType === 'action' || item.inferredType === 'reminder') {
    if (!item.priority) return true
    if (!isValidConfidence(item.priorityConfidence)) return true
    if (item.priorityConfidence !== undefined && item.priorityConfidence < HIGH_CONFIDENCE_THRESHOLD) {
      return true
    }
  }

  // Estimate review (for actions)
  if (item.inferredType === 'action') {
    if (!item.estimate) return true
    if (!isValidConfidence(item.estimateConfidence)) return true
    if (item.estimateConfidence !== undefined && item.estimateConfidence < HIGH_CONFIDENCE_THRESHOLD) {
      return true
    }
  }

  // If all checks pass, it doesn't need review
  return false
}

export function getTopReviewItems(items: Item[], limit: number = 3): ReviewItem[] {
  const reviewItems: ReviewItem[] = items
    .filter(item => {
      // Skip items that have been reviewed with high confidence on all applicable fields
      if (item.lastReviewedAt) {
        const typeConfident = item.typeConfidence && item.typeConfidence >= HIGH_CONFIDENCE_THRESHOLD
        const collectionConfident =
          item.collectionConfidence && item.collectionConfidence >= HIGH_CONFIDENCE_THRESHOLD
        const priorityConfident =
          item.inferredType === 'note' ||
          (item.priorityConfidence && item.priorityConfidence >= HIGH_CONFIDENCE_THRESHOLD)
        const estimateConfident =
          item.inferredType !== 'action' ||
          (item.estimateConfidence && item.estimateConfidence >= HIGH_CONFIDENCE_THRESHOLD)

        if (typeConfident && collectionConfident && priorityConfident && estimateConfident) {
          return false
        }
      }
      return needsReview(item)
    })
    .map(item => {
      let priority = 0
      const reasons: string[] = []

      // Type issues (highest priority)
      if (!item.inferredType) {
        priority = 1000
        reasons.push('No type assigned')
      } else if (!isValidConfidence(item.typeConfidence)) {
        priority = 950
        reasons.push('Invalid type confidence')
      } else if (item.typeConfidence !== undefined && item.typeConfidence < HIGH_CONFIDENCE_THRESHOLD) {
        priority = 800
        reasons.push(`Low type confidence (${item.typeConfidence}%)`)
      }

      // Collection issues
      if (!item.collection) {
        priority = Math.max(priority, 900)
        reasons.push('Missing collection')
      } else if (!isValidConfidence(item.collectionConfidence)) {
        priority = Math.max(priority, 875)
        reasons.push('Invalid collection confidence')
      } else if (
        item.collectionConfidence !== undefined &&
        item.collectionConfidence < HIGH_CONFIDENCE_THRESHOLD
      ) {
        priority = Math.max(priority, 750)
        reasons.push(`Low collection confidence (${item.collectionConfidence}%)`)
      }

      // Priority issues (for actions and reminders)
      if (item.inferredType === 'action' || item.inferredType === 'reminder') {
        if (!item.priority) {
          priority = Math.max(priority, 700)
          reasons.push('Missing priority')
        } else if (!isValidConfidence(item.priorityConfidence)) {
          priority = Math.max(priority, 650)
          reasons.push('Invalid priority confidence')
        } else if (
          item.priorityConfidence !== undefined &&
          item.priorityConfidence < HIGH_CONFIDENCE_THRESHOLD
        ) {
          priority = Math.max(priority, 600)
          reasons.push(`Low priority confidence (${item.priorityConfidence}%)`)
        }
      }

      // Estimate issues (for actions)
      if (item.inferredType === 'action') {
        if (!item.estimate) {
          priority = Math.max(priority, 500)
          reasons.push('Missing estimate')
        } else if (!isValidConfidence(item.estimateConfidence)) {
          priority = Math.max(priority, 450)
          reasons.push('Invalid estimate confidence')
        } else if (
          item.estimateConfidence !== undefined &&
          item.estimateConfidence < HIGH_CONFIDENCE_THRESHOLD
        ) {
          priority = Math.max(priority, 400)
          reasons.push(`Low estimate confidence (${item.estimateConfidence}%)`)
        }
      }

      // Add age factor to break ties
      const age = Date.now() - (item.lastReviewedAt || item.createdAt)
      const hoursOld = age / (1000 * 60 * 60)
      priority += hoursOld

      return {
        item,
        reviewPriority: priority,
        reason: reasons.join(', '),
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

  return reviewItems.slice(0, limit)
}

