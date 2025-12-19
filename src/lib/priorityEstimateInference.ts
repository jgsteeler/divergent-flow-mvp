import { Priority, Estimate, PriorityLearningData, EstimateLearningData, ItemType } from './types'

interface PriorityPattern {
  patterns: RegExp[]
  priority: Priority
  weight: number
}

interface EstimatePattern {
  patterns: RegExp[]
  estimate: Estimate
  weight: number
}

// Priority patterns - looking for urgency indicators
const HIGH_PRIORITY_PATTERNS: PriorityPattern = {
  patterns: [
    /\burgent\b/i,
    /\basap\b/i,
    /\bcritical\b/i,
    /\bemergency\b/i,
    /\bimportant\b/i,
    /\btoday\b/i,
    /\bnow\b/i,
    /\bimmediately\b/i,
    /\bdeadline\b/i,
    /\boverdue\b/i,
    /\bhigh priority\b/i,
  ],
  priority: 'high',
  weight: 1.0
}

const LOW_PRIORITY_PATTERNS: PriorityPattern = {
  patterns: [
    /\bsomeday\b/i,
    /\bmaybe\b/i,
    /\beventually\b/i,
    /\bwhenever\b/i,
    /\blow priority\b/i,
    /\bnice to have\b/i,
    /\bif (i|we) (have )?time\b/i,
    /\bno rush\b/i,
  ],
  priority: 'low',
  weight: 1.0
}

// Estimate patterns - looking for time/effort indicators
const QUICK_ESTIMATES: EstimatePattern = {
  patterns: [
    /\b(quick|fast|rapid)\b/i,
    /\b5 ?min(ute)?s?\b/i,
    /\btake(s)? (a )?second\b/i,
    /\btake(s)? (a )?moment\b/i,
    /\beasy\b/i,
    /\bsimple\b/i,
  ],
  estimate: '5min',
  weight: 1.0
}

const SHORT_ESTIMATES: EstimatePattern = {
  patterns: [
    /\b15 ?min(ute)?s?\b/i,
    /\b(quarter|fifteen) ?(-| )?(hour|min)/i,
    /\bshort\b/i,
  ],
  estimate: '15min',
  weight: 1.0
}

const HALF_HOUR_ESTIMATES: EstimatePattern = {
  patterns: [
    /\b30 ?min(ute)?s?\b/i,
    /\bhalf( |-)?hour\b/i,
  ],
  estimate: '30min',
  weight: 1.0
}

const ONE_HOUR_ESTIMATES: EstimatePattern = {
  patterns: [
    /\b1 ?hour?\b/i,
    /\bone( |-)?hour\b/i,
  ],
  estimate: '1hour',
  weight: 1.0
}

const TWO_HOUR_ESTIMATES: EstimatePattern = {
  patterns: [
    /\b2 ?hours?\b/i,
    /\btwo( |-)?hours?\b/i,
  ],
  estimate: '2hours',
  weight: 0.9
}

const LONG_ESTIMATES: EstimatePattern = {
  patterns: [
    /\b(half|1\/2) ?(-| )?day\b/i,
    /\b(full|whole|entire) ?day\b/i,
    /\bmulti(-| )?day\b/i,
    /\bseveral (days|hours)\b/i,
    /\bproject\b/i,
    /\bcomplex\b/i,
  ],
  estimate: 'day',
  weight: 1.0
}

function calculatePatternScore(text: string, pattern: PriorityPattern | EstimatePattern): number {
  let score = 0
  for (const regex of pattern.patterns) {
    if (regex.test(text)) {
      score += pattern.weight
      break
    }
  }
  return score
}

function applyPriorityLearning(
  text: string,
  learningData: PriorityLearningData[]
): { priority: Priority; confidence: number; reasoning: string } | null {
  const recentLearning = learningData
    .filter(ld => ld.wasCorrect !== false)
    .slice(-50)

  for (const learning of recentLearning.reverse()) {
    const patternWords = learning.pattern.toLowerCase().split(/\s+/)
    const textWords = text.toLowerCase().split(/\s+/)
    
    const matchingWords = patternWords.filter(word => 
      textWords.some(tw => tw.includes(word) || word.includes(tw))
    )
    
    if (matchingWords.length >= Math.min(2, patternWords.length)) {
      const confidenceBoost = learning.confidence >= 80 ? 25 : 15
      return {
        priority: learning.priority,
        confidence: 70 + confidenceBoost,
        reasoning: `Similar to previous ${learning.priority} priority patterns (matched ${matchingWords.length} keywords)`
      }
    }
  }
  
  return null
}

function applyEstimateLearning(
  text: string,
  learningData: EstimateLearningData[]
): { estimate: Estimate; confidence: number; reasoning: string } | null {
  const recentLearning = learningData
    .filter(ld => ld.wasCorrect !== false)
    .slice(-50)

  for (const learning of recentLearning.reverse()) {
    const patternWords = learning.pattern.toLowerCase().split(/\s+/)
    const textWords = text.toLowerCase().split(/\s+/)
    
    const matchingWords = patternWords.filter(word => 
      textWords.some(tw => tw.includes(word) || word.includes(tw))
    )
    
    if (matchingWords.length >= Math.min(2, patternWords.length)) {
      const confidenceBoost = learning.confidence >= 80 ? 25 : 15
      return {
        estimate: learning.estimate,
        confidence: 70 + confidenceBoost,
        reasoning: `Similar to previous ${learning.estimate} estimate patterns (matched ${matchingWords.length} keywords)`
      }
    }
  }
  
  return null
}

export function inferPriority(
  text: string,
  itemType: ItemType | null | undefined,
  learningData: PriorityLearningData[] = []
): { priority: Priority | null; confidence: number; reasoning: string } {
  // Only infer priority for actions and reminders
  if (itemType !== 'action' && itemType !== 'reminder') {
    return { priority: null, confidence: 0, reasoning: 'Priority only applies to actions and reminders' }
  }

  const learned = applyPriorityLearning(text, learningData)
  if (learned && learned.confidence >= 80) {
    return learned
  }

  let highScore = calculatePatternScore(text, HIGH_PRIORITY_PATTERNS)
  let lowScore = calculatePatternScore(text, LOW_PRIORITY_PATTERNS)

  if (learned) {
    const learnedBoost = learned.confidence / 100
    if (learned.priority === 'high') highScore += learnedBoost
    if (learned.priority === 'low') lowScore += learnedBoost
  }

  const maxScore = Math.max(highScore, lowScore)

  if (maxScore === 0) {
    // Default to medium if no patterns found
    return { priority: 'medium', confidence: 40, reasoning: 'No priority indicators found, defaulting to medium' }
  }

  let inferredPriority: Priority
  let patternName: string
  let confidence: number

  if (highScore > lowScore) {
    inferredPriority = 'high'
    patternName = 'urgency indicators'
    confidence = 85
  } else if (lowScore > highScore) {
    inferredPriority = 'low'
    patternName = 'low urgency indicators'
    confidence = 85
  } else {
    inferredPriority = 'medium'
    patternName = 'balanced urgency'
    confidence = 60
  }

  const reasoning = `Detected ${patternName}`

  return { priority: inferredPriority, confidence, reasoning }
}

export function inferEstimate(
  text: string,
  itemType: ItemType | null | undefined,
  learningData: EstimateLearningData[] = []
): { estimate: Estimate | null; confidence: number; reasoning: string } {
  // Only infer estimate for actions
  if (itemType !== 'action') {
    return { estimate: null, confidence: 0, reasoning: 'Estimate only applies to action items' }
  }

  const learned = applyEstimateLearning(text, learningData)
  if (learned && learned.confidence >= 80) {
    return learned
  }

  let quickScore = calculatePatternScore(text, QUICK_ESTIMATES)
  let shortScore = calculatePatternScore(text, SHORT_ESTIMATES)
  let halfHourScore = calculatePatternScore(text, HALF_HOUR_ESTIMATES)
  let oneHourScore = calculatePatternScore(text, ONE_HOUR_ESTIMATES)
  let twoHourScore = calculatePatternScore(text, TWO_HOUR_ESTIMATES)
  let longScore = calculatePatternScore(text, LONG_ESTIMATES)

  if (learned) {
    const learnedBoost = learned.confidence / 100
    if (learned.estimate === '5min') quickScore += learnedBoost
    if (learned.estimate === '15min') shortScore += learnedBoost
    if (learned.estimate === '30min') halfHourScore += learnedBoost
    if (learned.estimate === '1hour') oneHourScore += learnedBoost
    if (learned.estimate === '2hours') twoHourScore += learnedBoost
    if (learned.estimate === 'halfday' || learned.estimate === 'day' || learned.estimate === 'multiday') longScore += learnedBoost
  }

  const maxScore = Math.max(quickScore, shortScore, halfHourScore, oneHourScore, twoHourScore, longScore)

  if (maxScore === 0) {
    // Infer based on text length as fallback
    const wordCount = text.split(/\s+/).length
    if (wordCount < 5) {
      return { estimate: '15min', confidence: 30, reasoning: 'Brief description suggests quick task' }
    } else if (wordCount < 15) {
      return { estimate: '30min', confidence: 30, reasoning: 'Moderate description suggests standard task' }
    } else {
      return { estimate: '1hour', confidence: 30, reasoning: 'Detailed description suggests longer task' }
    }
  }

  let inferredEstimate: Estimate
  let patternName: string
  let confidence: number

  if (quickScore > 0 && quickScore >= Math.max(shortScore, halfHourScore, oneHourScore, twoHourScore, longScore)) {
    inferredEstimate = '5min'
    patternName = 'quick task indicators'
    confidence = 85
  } else if (shortScore > 0 && shortScore >= Math.max(halfHourScore, oneHourScore, twoHourScore, longScore)) {
    inferredEstimate = '15min'
    patternName = 'short task indicators'
    confidence = 80
  } else if (halfHourScore > 0 && halfHourScore >= Math.max(oneHourScore, twoHourScore, longScore)) {
    inferredEstimate = '30min'
    patternName = 'half-hour task indicators'
    confidence = 80
  } else if (oneHourScore > 0 && oneHourScore >= Math.max(twoHourScore, longScore)) {
    inferredEstimate = '1hour'
    patternName = 'one-hour task indicators'
    confidence = 80
  } else if (twoHourScore > 0 && twoHourScore >= longScore) {
    inferredEstimate = '2hours'
    patternName = 'two-hour task indicators'
    confidence = 75
  } else if (longScore > 0) {
    inferredEstimate = 'day'
    patternName = 'complex task indicators'
    confidence = 80
  } else {
    inferredEstimate = '30min'
    patternName = 'default estimate'
    confidence = 40
  }

  const reasoning = `Detected ${patternName}`

  return { estimate: inferredEstimate, confidence, reasoning }
}

export async function savePriorityLearning(
  text: string,
  inferredPriority: Priority | null,
  actualPriority: Priority,
  confidence: number
): Promise<PriorityLearningData> {
  const learningData: PriorityLearningData = {
    pattern: text.toLowerCase().substring(0, 100),
    priority: actualPriority,
    confidence,
    timestamp: Date.now(),
    wasCorrect: inferredPriority === actualPriority,
  }

  return learningData
}

export async function saveEstimateLearning(
  text: string,
  inferredEstimate: Estimate | null,
  actualEstimate: Estimate,
  confidence: number
): Promise<EstimateLearningData> {
  const learningData: EstimateLearningData = {
    pattern: text.toLowerCase().substring(0, 100),
    estimate: actualEstimate,
    confidence,
    timestamp: Date.now(),
    wasCorrect: inferredEstimate === actualEstimate,
  }

  return learningData
}

export function getPriorityLabel(priority: Priority): string {
  const labels: Record<Priority, string> = {
    low: 'Low Priority',
    medium: 'Medium Priority',
    high: 'High Priority'
  }
  return labels[priority]
}

export function getEstimateLabel(estimate: Estimate): string {
  const labels: Record<Estimate, string> = {
    '5min': '5 minutes',
    '15min': '15 minutes',
    '30min': '30 minutes',
    '1hour': '1 hour',
    '2hours': '2 hours',
    'halfday': 'Half day',
    'day': 'Full day',
    'multiday': 'Multi-day'
  }
  return labels[estimate]
}

export function getPriorityDescription(priority: Priority): string {
  const descriptions: Record<Priority, string> = {
    low: 'Can be done later',
    medium: 'Should be done soon',
    high: 'Needs immediate attention'
  }
  return descriptions[priority]
}

export function getEstimateDescription(estimate: Estimate): string {
  const descriptions: Record<Estimate, string> = {
    '5min': 'Quick task',
    '15min': 'Short task',
    '30min': 'Half-hour task',
    '1hour': 'One-hour task',
    '2hours': 'Two-hour task',
    'halfday': 'Half-day project',
    'day': 'Full-day project',
    'multiday': 'Multi-day project'
  }
  return descriptions[estimate]
}
