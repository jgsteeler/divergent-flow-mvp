import { InferredAttributes, ItemType, Priority, LearningData } from './types'

export async function inferAttributes(
  text: string,
  learningData: LearningData[] = []
): Promise<InferredAttributes> {
  const examples = learningData.slice(-10).map(ld => ({
    input: ld.originalText,
    output: ld.correctedAttributes
  }))

  const examplesText = examples.length > 0
    ? `\n\nLearn from these past corrections:\n${examples.map(ex => 
        `Input: "${ex.input}"\nOutput: ${JSON.stringify(ex.output)}`
      ).join('\n\n')}`
    : ''

  const promptText = `You are an intelligent assistant helping users with ADHD capture and organize their thoughts.

Analyze this capture and infer its attributes:
"${text}"

Determine:
1. type: Is it a "note" (information to remember), "action" (something to do), or "reminder" (time-sensitive prompt)?
2. collection: What category/project does this belong to? (e.g., "Work", "Personal", "Health", "Ideas"). Infer from context.
3. priority: Is it "low", "medium", or "high" priority?
4. dueDate: If a date/time is mentioned, convert to Unix timestamp (milliseconds). Understand natural language like "tomorrow", "next week", "in 3 days".
5. context: Where or when should this be done? (e.g., "home", "office", "phone", "computer")
6. tags: Extract 1-3 relevant keywords as tags

${examplesText}

Return ONLY a JSON object with this exact structure:
{
  "result": {
    "type": "note" | "action" | "reminder" | null,
    "collection": "string" | null,
    "priority": "low" | "medium" | "high" | null,
    "dueDate": number | null,
    "context": "string" | null,
    "tags": ["string"] | null
  }
}

If you cannot confidently infer a field, set it to null. Be conservative - it's better to return null than guess incorrectly.`

  try {
    const response = await window.spark.llm(promptText, 'gpt-4o-mini', true)
    const parsed = JSON.parse(response)
    return parsed.result || {}
  } catch (error) {
    console.error('Inference failed:', error)
    return {}
  }
}

export function getMissingFields(attributes: InferredAttributes): string[] {
  const missing: string[] = []
  
  if (!attributes.type) missing.push('type')
  if (!attributes.collection) missing.push('collection')
  
  if (attributes.type === 'action' || attributes.type === 'reminder') {
    if (!attributes.priority) missing.push('priority')
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
  
  if (!attributes.type || !attributes.collection) {
    return 1000 + hoursOld
  }
  
  if (missing.length > 0) {
    return 500 + hoursOld
  }
  
  return hoursOld
}
