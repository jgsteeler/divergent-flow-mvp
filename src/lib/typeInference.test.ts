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

      it('should identify "remember to" pattern', () => {
        const result = inferType("remember to pick up groceries")
        expect(result.type).toBe('reminder')
        expect(result.confidence).toBeGreaterThan(50)
      })

      it('should identify "need to remember" pattern', () => {
        const result = inferType("need to remember to check the mail")
        expect(result.type).toBe('reminder')
        expect(result.confidence).toBeGreaterThan(50)
      })

      it('should identify "Reminder:" prefix', () => {
        const result = inferType("Reminder: check email at 3pm")
        expect(result.type).toBe('reminder')
        expect(result.confidence).toBeGreaterThan(50)
      })

      it('should identify "Remember:" prefix', () => {
        const result = inferType("Remember: meeting tomorrow")
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

      it('should identify Phase 2 preloaded action phrases', () => {
        const phrases = [
          'Create a new project',
          'Take the trash out',
          'Build a mobile app',
          'Fix the bug in production',
          'Update the README',
          'Review the pull request',
          'Send an email to client',
          'Call the office',
          'Email the team',
          'Schedule a meeting',
          'Complete the onboarding',
          'Finish the report',
          'Submit the proposal',
          'Prepare the presentation',
          'Order new supplies'
        ]
        
        phrases.forEach(phrase => {
          const result = inferType(phrase)
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

      it('should use catchall logic for ambiguous text', () => {
        const result = inferType('interesting observation about user behavior')
        expect(result.type).toBe('note')
        expect(result.confidence).toBe(85)
      })

      it('should use catchall logic for text without clear indicators', () => {
        const result = inferType('some thoughts about the new design')
        expect(result.type).toBe('note')
        expect(result.confidence).toBe(85)
      })
    })

    describe('no pattern match', () => {
      it('should default to note when no patterns match (catchall logic)', () => {
        const result = inferType('some random text without patterns')
        expect(result.type).toBe('note')
        expect(result.confidence).toBe(85)
        expect(result.reasoning).toBe('Default to note - no strong action or reminder indicators')
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

      it('should return 95% confidence for exact pattern matches', () => {
        const result = inferType('remind me to call mom')
        expect(result.confidence).toBe(95)
      })

      it('should return lower confidence for weaker matches', () => {
        const result = inferType('interesting thought')
        expect(result.confidence).toBeLessThan(95)
      })
    })

    describe('edge cases', () => {
      it('should handle empty text', () => {
        const result = inferType('')
        expect(result.type).toBe('note')
        expect(result.confidence).toBe(85)
      })

      it('should handle text with special characters', () => {
        const result = inferType('create a new @user profile #feature')
        expect(result.type).toBe('action')
        expect(result.confidence).toBeGreaterThan(50)
      })

      it('should handle text with multiple type indicators', () => {
        const result = inferType('remind me to create a new report')
        expect(result.type).toBe('reminder')
        expect(result.confidence).toBeGreaterThan(50)
      })

      it('should handle case insensitivity', () => {
        const result = inferType('REMIND ME TO CALL MOM')
        expect(result.type).toBe('reminder')
        expect(result.confidence).toBeGreaterThan(50)
      })

      it('should handle mixed case', () => {
        const result = inferType('CrEaTe A nEw PrOjEcT')
        expect(result.type).toBe('action')
        expect(result.confidence).toBeGreaterThan(50)
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
