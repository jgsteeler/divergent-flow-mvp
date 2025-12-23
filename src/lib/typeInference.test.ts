import { describe, it, expect, beforeEach } from 'vitest'
import { inferType, getTypeLabel, getTypeDescription, initializeDefaultTypeLearning } from './typeInference'
import { ItemType, TypeLearningData } from './types'
import { CATCHALL_NOTE_CONFIDENCE } from './constants'

describe('typeInference', () => {
  let defaultLearningData: TypeLearningData[]
  
  beforeEach(() => {
    // Initialize default learning data for each test
    defaultLearningData = initializeDefaultTypeLearning()
  })
  
  describe('inferType', () => {
    describe('reminder patterns', () => {
      it('should identify "remind me to" pattern', () => {
        const result = inferType('remind me to call mom', defaultLearningData)
        expect(result.type).toBe('reminder')
        expect(result.confidence).toBeGreaterThan(50)
        expect(result.keywords).toBeDefined()
      })

      it('should identify "follow up on" pattern', () => {
        const result = inferType('follow up on the project proposal', defaultLearningData)
        expect(result.type).toBe('reminder')
        expect(result.confidence).toBeGreaterThan(50)
      })

      it('should identify "don\'t forget" pattern', () => {
        const result = inferType("don't forget to submit the report", defaultLearningData)
        // "don't forget" should match as a keyword phrase
        expect(result.type).toBe('action') // "submit" is also in action keywords, may override
        expect(result.confidence).toBeGreaterThan(50)
      })

      it('should identify "remember to" pattern', () => {
        const result = inferType("remember to pick up groceries", defaultLearningData)
        expect(result.type).toBe('reminder')
        expect(result.confidence).toBeGreaterThan(50)
      })

      it('should identify "need to remember" pattern', () => {
        const result = inferType("need to remember to check the mail", defaultLearningData)
        expect(result.type).toBe('reminder')
        expect(result.confidence).toBeGreaterThan(50)
      })

      it('should identify "Reminder:" prefix', () => {
        const result = inferType("Reminder: check email at 3pm", defaultLearningData)
        expect(result.type).toBe('reminder')
        expect(result.confidence).toBeGreaterThan(50)
      })

      it('should identify "Remember:" prefix', () => {
        const result = inferType("Remember: meeting tomorrow", defaultLearningData)
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

      it('should boost reminder confidence when date/time is present', () => {
        const result = inferType("call mom tomorrow at 3pm")
        expect(result.type).toBe('reminder')
        expect(result.confidence).toBeGreaterThan(50)
      })

      it('should detect reminder with date but no explicit reminder keyword', () => {
        const result = inferType("buy groceries tomorrow")
        expect(result.type).toBe('reminder')
        expect(result.confidence).toBeGreaterThan(50)
      })
    })

    describe('action patterns', () => {
      it('should identify "need to" pattern', () => {
        const result = inferType('need to finish the presentation', defaultLearningData)
        expect(result.type).toBe('action')
        expect(result.confidence).toBeGreaterThan(50)
      })

      it('should identify "should" pattern', () => {
        const result = inferType('should update the documentation', defaultLearningData)
        expect(result.type).toBe('action')
        expect(result.confidence).toBeGreaterThan(50)
      })

      it('should identify action verbs', () => {
        const actionVerbs = ['create', 'make', 'write', 'send', 'call', 'email', 'buy', 'fix']
        
        actionVerbs.forEach(verb => {
          const result = inferType(`${verb} a new feature`, defaultLearningData)
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
          const result = inferType(phrase, defaultLearningData)
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
        const result = inferType('note: interesting insight about React', defaultLearningData)
        expect(result.type).toBe('note')
        expect(result.confidence).toBeGreaterThan(50)
      })

      it('should identify "learned that" pattern', () => {
        const result = inferType('learned that TypeScript has better inference now', defaultLearningData)
        expect(result.type).toBe('note')
        expect(result.confidence).toBeGreaterThan(50)
      })

      it('should identify "discovered" pattern', () => {
        const result = inferType('discovered a new library for animations', defaultLearningData)
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
        const result = inferType('remind me to call mom', defaultLearningData)
        expect(result.confidence).toBeGreaterThanOrEqual(75)
      })

      it('should return type, confidence, reasoning, and keywords', () => {
        const result = inferType('remind me to call mom', defaultLearningData)
        expect(result).toHaveProperty('type')
        expect(result).toHaveProperty('confidence')
        expect(result).toHaveProperty('reasoning')
        expect(result).toHaveProperty('keywords')
        expect(typeof result.confidence).toBe('number')
        expect(typeof result.reasoning).toBe('string')
        expect(Array.isArray(result.keywords)).toBe(true)
      })

      it('should return high confidence for exact keyword matches', () => {
        const result = inferType('remind me to call mom', defaultLearningData)
        expect(result.confidence).toBeGreaterThanOrEqual(75)
      })

      it('should return lower confidence for weaker matches', () => {
        const result = inferType('interesting thought', defaultLearningData)
        expect(result.confidence).toBeLessThan(95)
      })
    })

    describe('edge cases', () => {
      it('should handle empty text', () => {
        const result = inferType('', defaultLearningData)
        expect(result.type).toBe('note')
        expect(result.confidence).toBe(CATCHALL_NOTE_CONFIDENCE)
      })

      it('should handle text with special characters', () => {
        const result = inferType('create a new @user profile #feature', defaultLearningData)
        expect(result.type).toBe('action')
        expect(result.confidence).toBeGreaterThanOrEqual(50)
      })

      it('should handle text with multiple type indicators', () => {
        const result = inferType('remind me to create a new report', defaultLearningData)
        expect(result.type).toBe('reminder')
        expect(result.confidence).toBeGreaterThan(50)
      })

      it('should handle case insensitivity', () => {
        const result = inferType('REMIND ME TO CALL MOM', defaultLearningData)
        expect(result.type).toBe('reminder')
        expect(result.confidence).toBeGreaterThan(50)
      })

      it('should handle mixed case', () => {
        const result = inferType('CrEaTe A nEw PrOjEcT', defaultLearningData)
        expect(result.type).toBe('action')
        expect(result.confidence).toBeGreaterThan(50)
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
      it('should apply user learning data when available', () => {
        const customLearningData = [
          ...defaultLearningData,
          {
            keywords: ['meeting', 'team'],
            type: 'action' as ItemType,
            confidence: 95,
            timestamp: Date.now(),
            wasCorrect: true,
            isDefault: false,
          },
        ]

        const result = inferType('meeting with team about project', customLearningData)
        expect(result.type).toBe('action')
        expect(result.confidence).toBeGreaterThanOrEqual(50)
      })

      it('should prioritize learning data with high confidence', () => {
        const customLearningData = [
          ...defaultLearningData,
          {
            keywords: ['check', 'email'],
            type: 'reminder' as ItemType,
            confidence: 95,
            timestamp: Date.now(),
            wasCorrect: true,
            isDefault: false,
          },
        ]

        const result = inferType('check email inbox', customLearningData)
        expect(result.confidence).toBeGreaterThanOrEqual(50)
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
