import { describe, it, expect } from 'vitest'
import { inferCollection, inferCollections, getRelevantInferences } from './collectionInference'
import { LearningData } from './types'

describe('collectionInference', () => {
  const learningData: LearningData[] = [
    {
      originalText: 'drain oil from mgb',
      inferredAttributes: {
        typeConfidence: 0,
        collectionConfidence: 0,
      },
      correctedAttributes: {
        type: 'action',
        collection: 'MGB',
        typeConfidence: 100,
        collectionConfidence: 100,
      },
      timestamp: Date.now(),
      wasCorrect: false,
    },
  ]

  it('should infer collection "MGB" from "drain coolant from the radiator in the mgb"', () => {
    const text = 'drain coolant from the radiator in the mgb'
    const result = inferCollection(text, learningData)

    expect(result.collection).toBe('MGB')
    expect(result.confidence).toBeGreaterThan(0)
    expect(result.confidence).toBeLessThanOrEqual(90)
  })

  it('should return multiple inferences sorted by confidence', () => {
    const text = 'drain coolant from the radiator in the mgb'
    const results = inferCollections(text, learningData)

    expect(results).toHaveLength(1)
    expect(results[0].collection).toBe('MGB')
    expect(results[0].confidence).toBe(90)
  })

  it('should match 3-character words like "mgb" when combined with other matches', () => {
    const text = 'drain from mgb' // Has "drain", "from", "mgb" - 3 matches
    const result = inferCollection(text, learningData)

    expect(result.collection).toBe('MGB')
    expect(result.confidence).toBeGreaterThan(0)
  })

  it('should not match with only 1 matching word (minimum is 2)', () => {
    const text = 'check the mgb' // Only "mgb" matches - not enough
    const result = inferCollection(text, learningData)

    expect(result.collection).toBeNull()
    expect(result.confidence).toBe(0)
  })

  it('should filter relevant inferences by medium threshold', () => {
    const inferences = [
      { collection: 'MGB', confidence: 90, reasoning: 'test' },
      { collection: 'Work', confidence: 50, reasoning: 'test' },
    ]

    const relevant = getRelevantInferences(inferences)

    expect(relevant).toHaveLength(1)
    expect(relevant[0].collection).toBe('MGB')
  })

  it('should return highest confidence if all are below medium', () => {
    const inferences = [
      { collection: 'Work', confidence: 50, reasoning: 'test' },
      { collection: 'Personal', confidence: 40, reasoning: 'test' },
    ]

    const relevant = getRelevantInferences(inferences)

    expect(relevant).toHaveLength(1)
    expect(relevant[0].collection).toBe('Work')
    expect(relevant[0].confidence).toBe(50)
  })
})
