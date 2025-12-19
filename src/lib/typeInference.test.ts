import { describe, it, expect } from 'vitest'
import { inferType, getTypeLabel, getTypeDescription } from './typeInference'
import { ItemType } from './types'

describe('typeInference', () => {
  describe('inferType', () => {
    describe('reminder patterns', () => {
      it('should identify "remind me to" pattern', () => {
        const result = inferType('remind me to call mom')
        expect(result.type).toBe('reminder')
        expect(result.confidence).toBeGreaterThan(50)
      })

      it('should identify "follow up on" pattern', () => {
        const result = inferType('follow up on the project proposal')
        expect(result.type).toBe('reminder')
        expect(result.confidence).toBeGreaterThan(50)
      })

      it('should identify "don\'t forget" pattern', () => {
        const result = inferType("don't forget to submit the report")
        expect(result.type).toBe('reminder')
        expect(result.confidence).toBeGreaterThan(50)
      })
    })

    describe('action patterns', () => {
      it('should identify "need to" pattern', () => {
        const result = inferType('need to finish the presentation')
        expect(result.type).toBe('action')
        expect(result.confidence).toBeGreaterThan(50)
      })

      it('should identify "should" pattern', () => {
        const result = inferType('should update the documentation')
        expect(result.type).toBe('action')
        expect(result.confidence).toBeGreaterThan(50)
      })

      it('should identify action verbs', () => {
        const actionVerbs = ['create', 'make', 'write', 'send', 'call', 'email', 'buy', 'fix']
        
        actionVerbs.forEach(verb => {
          const result = inferType(`${verb} a new feature`)
          expect(result.type).toBe('action')
          expect(result.confidence).toBeGreaterThan(50)
        })
      })
    })

    describe('note patterns', () => {
      it('should identify "note:" pattern', () => {
        const result = inferType('note: interesting insight about React')
        expect(result.type).toBe('note')
        expect(result.confidence).toBeGreaterThan(50)
      })

      it('should identify "learned that" pattern', () => {
        const result = inferType('learned that TypeScript has better inference now')
        expect(result.type).toBe('note')
        expect(result.confidence).toBeGreaterThan(50)
      })

      it('should identify "discovered" pattern', () => {
        const result = inferType('discovered a new library for animations')
        expect(result.type).toBe('note')
        expect(result.confidence).toBeGreaterThan(50)
      })
    })

    describe('no pattern match', () => {
      it('should return null type when no patterns match', () => {
        const result = inferType('some random text without patterns')
        expect(result.type).toBeNull()
        expect(result.confidence).toBe(0)
        expect(result.reasoning).toBe('No patterns matched')
      })
    })

    describe('confidence levels', () => {
      it('should have high confidence for strong matches', () => {
        const result = inferType('remind me to call mom')
        expect(result.confidence).toBeGreaterThanOrEqual(75)
      })

      it('should return type, confidence, and reasoning', () => {
        const result = inferType('remind me to call mom')
        expect(result).toHaveProperty('type')
        expect(result).toHaveProperty('confidence')
        expect(result).toHaveProperty('reasoning')
        expect(typeof result.confidence).toBe('number')
        expect(typeof result.reasoning).toBe('string')
      })
    })

    describe('learning data integration', () => {
      it('should apply learning data when available', () => {
        const learningData = [
          {
            pattern: 'meeting with team',
            type: 'action' as ItemType,
            confidence: 85,
            timestamp: Date.now(),
            wasCorrect: true,
          },
        ]

        const result = inferType('meeting with team about project', learningData)
        expect(result.type).toBe('action')
        expect(result.confidence).toBeGreaterThanOrEqual(70)
      })

      it('should prioritize learning data with high confidence', () => {
        const learningData = [
          {
            pattern: 'check email',
            type: 'reminder' as ItemType,
            confidence: 90,
            timestamp: Date.now(),
            wasCorrect: true,
          },
        ]

        const result = inferType('check email inbox', learningData)
        expect(result.confidence).toBeGreaterThanOrEqual(80)
      })
    })
  })

  describe('getTypeLabel', () => {
    it('should return correct label for note', () => {
      expect(getTypeLabel('note')).toBe('Note')
    })

    it('should return correct label for action', () => {
      expect(getTypeLabel('action')).toBe('Action Item')
    })

    it('should return correct label for reminder', () => {
      expect(getTypeLabel('reminder')).toBe('Reminder')
    })
  })

  describe('getTypeDescription', () => {
    it('should return correct description for note', () => {
      expect(getTypeDescription('note')).toBe('Information to remember')
    })

    it('should return correct description for action', () => {
      expect(getTypeDescription('action')).toBe('Something to do')
    })

    it('should return correct description for reminder', () => {
      expect(getTypeDescription('reminder')).toBe('Time-sensitive prompt')
    })
  })
})
