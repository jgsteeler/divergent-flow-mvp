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

export function calculateReviewPriority(captures: Capture[]): ReviewItem[] {
  const now = Date.now()
  
  const reviewItems: ReviewItem[] = captures
    .map(capture => {
      let priority = 0
      let reason = ''

      if (!capture.inferredType) {
        priority = 1000
        reason = 'No type assigned'
      }
      else if (capture.needsTypeConfirmation) {
        priority = 900
        reason = 'Type needs confirmation'
      }
      else if (!capture.typeConfirmed && capture.typeConfidence !== undefined && capture.typeConfidence < 90) {
        priority = 900
        reason = `Low confidence (${capture.typeConfidence}%) - needs confirmation`
      }
      else {
        const { invalid, missingProps } = hasInvalidProperties(capture)
        
        if (invalid) {
          priority = 900
          reason = `Missing: ${missingProps.join(', ')}`
        } else {
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
      }

      return {
        capture,
        reviewPriority: priority,
        reason
      }
    })
    .sort((a, b) => b.reviewPriority - a.reviewPriority)

  return reviewItems
}

export function getTopReviewItems(captures: Capture[], limit: number = 5): ReviewItem[] {
  const prioritized = calculateReviewPriority(captures)
  return prioritized.slice(0, limit)
}
