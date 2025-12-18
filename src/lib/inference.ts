import { InferredAttributes, ItemType, Priority, LearningData } from './types'
import { extractDateTimeFromText } from './dateParser'
import { inferType } from './typeInference'
import { inferCollection } from './collectionInference'
import { MAX_PATTERN_LENGTH, DEFAULT_LEARNING_CONFIDENCE, HIGH_CONFIDENCE_THRESHOLD } from './constants'

export async function inferAttributes(
  text: string,
  learningData: LearningData[] = []
): Promise<InferredAttributes> {
  // Extract date/time from text first
  const { dateTime: extractedDate, cleanText } = extractDateTimeFromText(text)
  
  // Extract type learning data from general learning data for backwards compatibility
  const typeLearningData = learningData
    .filter(ld => ld.correctedAttributes.type)
    .map(ld => ({
      pattern: ld.originalText.toLowerCase().substring(0, MAX_PATTERN_LENGTH),
      type: ld.correctedAttributes.type!,
      confidence: ld.correctedAttributes.typeConfidence || DEFAULT_LEARNING_CONFIDENCE,
      timestamp: ld.timestamp,
      wasCorrect: ld.wasCorrect
    }))
  
  // Use pattern-based inference for type with learning data
  const typeInference = inferType(text, typeLearningData)
  
  // Use pattern-based inference for collection
  const collectionInference = inferCollection(text, learningData)
  
  const examples = learningData.slice(-10).map(ld => ({
    input: ld.originalText,
    output: ld.correctedAttributes
  }))

  const examplesText = examples.length > 0
    ? `\n\nLearn from these past corrections:\n${examples.map(ex => 
        `Input: "${ex.input}"\nOutput: ${JSON.stringify(ex.output)}`
      ).join('\n\n')}`
    : ''

  const now = Date.now()
  const dateExamples = `
Examples of natural language dates and times (current time: ${now}):
- "tomorrow" = ${now + 86400000}
- "tomorrow at 3pm" = (tomorrow's date + 15:00)
- "next Tuesday" = (calculate next occurrence of Tuesday)
- "next Tuesday at noon" = (next Tuesday + 12:00)
- "in 3 days" = ${now + (3 * 86400000)}
- "5:30pm" or "5:30 pm" = (today or tomorrow + 17:30)
- "at noon" = (today or tomorrow + 12:00)
- "midnight" = (today or tomorrow + 00:00)
- "next week" = ${now + (7 * 86400000)}
- "January 15th at 2pm" = (timestamp for next January 15th at 14:00)
- "3/20 at 9am" or "3/20/24 at 9:00am" = (timestamp for that date at 09:00)
`

  const promptText = `You are an intelligent assistant helping users with ADHD capture and organize their thoughts.

Analyze this capture and infer its attributes:
"${text}"

Determine:
1. type: Is it a "note" (information to remember), "action" (something to do), or "reminder" (time-sensitive prompt)?
2. collection: What category/project does this belong to? (e.g., "Work", "Personal", "Health", "Ideas"). Infer from context.
3. priority: Is it "low", "medium", or "high" priority?
4. dueDate: If a date/time is mentioned, convert to Unix timestamp (milliseconds). Parse natural language dates carefully.${extractedDate ? ` IMPORTANT: A date was already extracted: ${extractedDate}. Use this value.` : ''}
5. context: Where or when should this be done? (e.g., "home", "office", "phone", "computer")
6. tags: Extract 1-3 relevant keywords as tags

${dateExamples}
${examplesText}

Return ONLY a JSON object with this exact structure:
{
  "result": {
    "type": "note" | "action" | "reminder" | null,
    "collection": "string" | null,
    "priority": "low" | "medium" | "high" | null,
    "dueDate": ${extractedDate || 'number | null'},
    "context": "string" | null,
    "tags": ["string"] | null
  }
}

If you cannot confidently infer a field, set it to null. Be conservative - it's better to return null than guess incorrectly.`

  // Start with pattern-based inference
  const baseAttributes: InferredAttributes = {
    type: typeInference.type,
    typeConfidence: typeInference.confidence,
    collection: collectionInference.collection,
    collectionConfidence: collectionInference.confidence,
    dueDate: extractedDate,
  }

  // Only use LLM if window.spark is available (Phase 4+)
  if (typeof window !== 'undefined' && (window as any).spark?.llm) {
    try {
      const response = await (window as any).spark.llm(promptText, 'gpt-4o-mini', true)
      const parsed = JSON.parse(response)
      const result = parsed.result || {}
      
      // Merge LLM results with pattern-based inference, preferring higher confidence
      if (extractedDate && !result.dueDate) {
        result.dueDate = extractedDate
      }
      
      // Use pattern-based collection if LLM didn't provide one or has lower confidence
      if (!result.collection && baseAttributes.collection) {
        result.collection = baseAttributes.collection
        result.collectionConfidence = baseAttributes.collectionConfidence
      }
      
      // Use pattern-based type if LLM didn't provide one
      if (!result.type && baseAttributes.type) {
        result.type = baseAttributes.type
        result.typeConfidence = baseAttributes.typeConfidence
      }
      
      return result
    } catch (error) {
      console.error('LLM inference failed, using pattern-based inference:', error)
      return baseAttributes
    }
  }
  
  // Fallback to pattern-based inference only
  return baseAttributes
}

export function getMissingFields(attributes: InferredAttributes): string[] {
  const missing: string[] = []
  
  if (!attributes.type) missing.push('type')
  if (!attributes.collection) missing.push('collection')
  
  if (attributes.type === 'action') {
    if (!attributes.priority) missing.push('priority')
  }
  
  if (attributes.type === 'reminder') {
    if (!attributes.dueDate && !attributes.remindTime) missing.push('date/time')
  }
  
  return missing
}

export function calculateReviewPriority(
  attributes: InferredAttributes,
  createdAt: number
): number {
  const missing = getMissingFields(attributes)
  const age = Date.now() - createdAt
  const hoursOld = age / (1000 * 60 * 60)
  
  // Critical: missing type or collection
  if (!attributes.type || !attributes.collection) {
    return 1000 + hoursOld
  }
  
  // High priority: missing other important fields
  if (missing.length > 0) {
    return 500 + hoursOld
  }
  
  // Low confidence on type or collection
  const typeConf = attributes.typeConfidence || 0
  const collectionConf = attributes.collectionConfidence || 0
  if (typeConf < HIGH_CONFIDENCE_THRESHOLD || collectionConf < HIGH_CONFIDENCE_THRESHOLD) {
    return 300 + hoursOld
  }
  
  return hoursOld
}
