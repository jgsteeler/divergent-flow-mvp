import { Capture, ReviewItem } from './types'

const ONE_DAY = 24 * 60 * 60 * 1000
const ONE_WEEK = 7 * ONE_DAY

function hasInvalidProperties(capture: Capture): { invalid: boolean; missingProps: string[] } {
  const missingProps: string[] = []
  
  if (capture.inferredType === 'action' && !capture.priority) {
    missingProps.push('priority')
  }
  
  if (capture.inferredType === 'reminder' && !capture.dueDate) {
    missingProps.push('due date')
  }
  
  if (capture.inferredType === 'action' && !capture.dueDate && !capture.context) {
    missingProps.push('context or due date')
  }
  
  return {
    invalid: missingProps.length > 0,
    missingProps
  }
}

function isValidConfidence(confidence: number | undefined): boolean {
  return typeof confidence === 'number' && !isNaN(confidence) && confidence >= 0 && confidence <= 100
}

export function calculateReviewPriority(captures: Capture[]): ReviewItem[] {
  const now = Date.now()
  
  const reviewItems: ReviewItem[] = captures
    .filter(capture => !capture.typeConfirmed)
    .map(capture => {
      let priority = 0
      let reason = ''

      if (!capture.inferredType) {
        priority = 1000
        reason = 'No type assigned'
      }
      else if (!isValidConfidence(capture.typeConfidence) || (capture.typeConfidence !== undefined && capture.typeConfidence < 90)) {
        priority = 900
        const confDisplay = isValidConfidence(capture.typeConfidence) ? `${capture.typeConfidence}%` : 'invalid'
        reason = `Low confidence (${confDisplay})`
      }
      else {
        const daysSinceCreated = (now - capture.createdAt) / ONE_DAY
        const daysSinceReview = capture.lastReviewedAt 
          ? (now - capture.lastReviewedAt) / ONE_DAY 
          : daysSinceCreated

        if (daysSinceReview > 30) {
          priority = 700
          reason = 'Not reviewed in 30+ days'
        } else if (daysSinceReview > 14) {
          priority = 600
          reason = 'Not reviewed in 14+ days'
        } else if (daysSinceReview > 7) {
          priority = 500
          reason = 'Not reviewed in 7+ days'
        } else {
          priority = 400 - Math.floor(daysSinceReview * 10)
          reason = 'Routine review'
        }
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
