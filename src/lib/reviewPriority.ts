import { Capture, ReviewItem } from './types'

function isValidConfidence(confidence: number | undefined): boolean {
  return typeof confidence === 'number' && !isNaN(confidence) && confidence >= 0 && confidence <= 100
}

function needsReview(capture: Capture): boolean {
  if (capture.typeConfirmed) return false
  if (!capture.inferredType) return true
  if (!isValidConfidence(capture.typeConfidence)) return true
  if (capture.typeConfidence !== undefined && capture.typeConfidence < 90) return true
  return false
}

export function calculateReviewPriority(captures: Capture[]): ReviewItem[] {
  const reviewItems: ReviewItem[] = captures
    .filter(capture => needsReview(capture))
    .map(capture => {
      let priority = 0
      let reason = ''

      if (!capture.inferredType) {
        priority = 1000
        reason = 'No type assigned'
      }
      else if (!isValidConfidence(capture.typeConfidence)) {
        priority = 900
        reason = 'Invalid confidence'
      }
      else if (capture.typeConfidence !== undefined && capture.typeConfidence < 90) {
        priority = 900
        reason = `Low confidence (${capture.typeConfidence}%)`
      }

      return {
        capture,
        reviewPriority: priority,
        reason
      }
    })
    .sort((a, b) => {
      if (b.reviewPriority !== a.reviewPriority) {
        return b.reviewPriority - a.reviewPriority
      }
      const aReview = a.capture.lastReviewedAt || a.capture.createdAt
      const bReview = b.capture.lastReviewedAt || b.capture.createdAt
      return aReview - bReview
    })

  return reviewItems
}

export function getTopReviewItems(captures: Capture[], limit: number = 3): ReviewItem[] {
  const prioritized = calculateReviewPriority(captures)
  return prioritized.slice(0, limit)
}
