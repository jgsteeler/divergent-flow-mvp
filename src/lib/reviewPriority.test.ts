import { describe, it, expect } from 'vitest'
import { getTopReviewItems } from './reviewPriority'
import { Item } from './types'

describe('reviewPriority', () => {
  describe('getTopReviewItems', () => {
    it('should prioritize items without type', () => {
      const items: Item[] = [
        {
          id: '1',
          text: 'test item 1',
          createdAt: Date.now(),
          migratedCapture: false,
          inferredType: 'action',
          typeConfidence: 50,
        },
        {
          id: '2',
          text: 'test item 2',
          createdAt: Date.now(),
          migratedCapture: false,
          // No inferredType
        },
      ]

      const reviewItems = getTopReviewItems(items, 2)
      expect(reviewItems[0].item.id).toBe('2')
      expect(reviewItems[0].reason).toContain('No type assigned')
    })

    it('should prioritize items with low type confidence', () => {
      const items: Item[] = [
        {
          id: '1',
          text: 'test item 1',
          createdAt: Date.now(),
          migratedCapture: false,
          inferredType: 'action',
          typeConfidence: 95,
          collection: 'work',
          collectionConfidence: 95,
        },
        {
          id: '2',
          text: 'test item 2',
          createdAt: Date.now(),
          migratedCapture: false,
          inferredType: 'note',
          typeConfidence: 40,
          collection: 'personal',
          collectionConfidence: 95,
        },
      ]

      const reviewItems = getTopReviewItems(items, 2)
      expect(reviewItems[0].item.id).toBe('2')
      expect(reviewItems[0].reason).toContain('Low type confidence')
    })

    it('should prioritize items without collection', () => {
      const items: Item[] = [
        {
          id: '1',
          text: 'test item 1',
          createdAt: Date.now(),
          migratedCapture: false,
          inferredType: 'action',
          typeConfidence: 95,
          collection: 'work',
          collectionConfidence: 95,
        },
        {
          id: '2',
          text: 'test item 2',
          createdAt: Date.now(),
          migratedCapture: false,
          inferredType: 'action',
          typeConfidence: 95,
          // No collection
        },
      ]

      const reviewItems = getTopReviewItems(items, 2)
      expect(reviewItems[0].item.id).toBe('2')
      expect(reviewItems[0].reason).toContain('Missing collection')
    })

    it('should prioritize actions without priority', () => {
      const items: Item[] = [
        {
          id: '1',
          text: 'test item 1',
          createdAt: Date.now(),
          migratedCapture: false,
          inferredType: 'action',
          typeConfidence: 95,
          collection: 'work',
          collectionConfidence: 95,
          priority: 'high',
          priorityConfidence: 95,
        },
        {
          id: '2',
          text: 'test item 2',
          createdAt: Date.now(),
          migratedCapture: false,
          inferredType: 'action',
          typeConfidence: 95,
          collection: 'work',
          collectionConfidence: 95,
          // No priority
        },
      ]

      const reviewItems = getTopReviewItems(items, 2)
      expect(reviewItems[0].item.id).toBe('2')
      expect(reviewItems[0].reason).toContain('Missing priority')
    })

    it('should prioritize actions without estimate', () => {
      const items: Item[] = [
        {
          id: '1',
          text: 'test item 1',
          createdAt: Date.now(),
          migratedCapture: false,
          inferredType: 'action',
          typeConfidence: 95,
          collection: 'work',
          collectionConfidence: 95,
          priority: 'high',
          priorityConfidence: 95,
          estimate: '1hour',
          estimateConfidence: 95,
        },
        {
          id: '2',
          text: 'test item 2',
          createdAt: Date.now(),
          migratedCapture: false,
          inferredType: 'action',
          typeConfidence: 95,
          collection: 'work',
          collectionConfidence: 95,
          priority: 'high',
          priorityConfidence: 95,
          // No estimate
        },
      ]

      const reviewItems = getTopReviewItems(items, 2)
      expect(reviewItems[0].item.id).toBe('2')
      expect(reviewItems[0].reason).toContain('Missing estimate')
    })

    it('should not require priority for notes', () => {
      const items: Item[] = [
        {
          id: '1',
          text: 'test note',
          createdAt: Date.now(),
          migratedCapture: false,
          inferredType: 'note',
          typeConfidence: 95,
          collection: 'personal',
          collectionConfidence: 95,
          // No priority (not needed for notes)
        },
      ]

      const reviewItems = getTopReviewItems(items, 1)
      expect(reviewItems.length).toBe(0) // Should not need review
    })

    it('should not require estimate for notes', () => {
      const items: Item[] = [
        {
          id: '1',
          text: 'test note',
          createdAt: Date.now(),
          migratedCapture: false,
          inferredType: 'note',
          typeConfidence: 95,
          collection: 'personal',
          collectionConfidence: 95,
          // No estimate (not needed for notes)
        },
      ]

      const reviewItems = getTopReviewItems(items, 1)
      expect(reviewItems.length).toBe(0) // Should not need review
    })

    it('should handle invalid confidence values', () => {
      const items: Item[] = [
        {
          id: '1',
          text: 'test item',
          createdAt: Date.now(),
          migratedCapture: false,
          inferredType: 'action',
          typeConfidence: NaN, // Invalid confidence
          collection: 'work',
          collectionConfidence: 95,
        },
      ]

      const reviewItems = getTopReviewItems(items, 1)
      expect(reviewItems.length).toBe(1)
      expect(reviewItems[0].reason).toContain('Invalid type confidence')
    })

    it('should respect the limit parameter', () => {
      const items: Item[] = Array.from({ length: 10 }, (_, i) => ({
        id: `${i}`,
        text: `test item ${i}`,
        createdAt: Date.now(),
        migratedCapture: false,
        // Missing all required fields
      }))

      const reviewItems = getTopReviewItems(items, 3)
      expect(reviewItems.length).toBe(3)
    })

    it('should sort by priority with highest first', () => {
      const items: Item[] = [
        {
          id: '1',
          text: 'low priority issue',
          createdAt: Date.now(),
          migratedCapture: false,
          inferredType: 'action',
          typeConfidence: 50, // Low confidence
          collection: 'work',
          collectionConfidence: 95,
        },
        {
          id: '2',
          text: 'high priority issue',
          createdAt: Date.now(),
          migratedCapture: false,
          // No type - highest priority
          collection: 'work',
          collectionConfidence: 95,
        },
      ]

      const reviewItems = getTopReviewItems(items, 2)
      expect(reviewItems[0].item.id).toBe('2') // No type should come first
      expect(reviewItems[1].item.id).toBe('1')
    })

    it('should consider age when breaking ties', () => {
      const now = Date.now()
      const items: Item[] = [
        {
          id: '1',
          text: 'newer item',
          createdAt: now,
          migratedCapture: false,
        },
        {
          id: '2',
          text: 'older item',
          createdAt: now - 1000 * 60 * 60 * 24, // 1 day ago
          migratedCapture: false,
        },
      ]

      const reviewItems = getTopReviewItems(items, 2)
      // Older items should have slightly higher priority
      expect(reviewItems[0].item.id).toBe('2')
    })

    it('should exclude fully reviewed high-confidence items', () => {
      const items: Item[] = [
        {
          id: '1',
          text: 'fully reviewed item',
          createdAt: Date.now(),
          migratedCapture: false,
          inferredType: 'note',
          typeConfidence: 95,
          collection: 'work',
          collectionConfidence: 95,
          lastReviewedAt: Date.now(),
        },
      ]

      const reviewItems = getTopReviewItems(items, 1)
      expect(reviewItems.length).toBe(0)
    })

    it('should include reviewed items with low confidence', () => {
      const items: Item[] = [
        {
          id: '1',
          text: 'reviewed but low confidence',
          createdAt: Date.now(),
          migratedCapture: false,
          inferredType: 'action',
          typeConfidence: 50, // Below threshold
          collection: 'work',
          collectionConfidence: 95,
          lastReviewedAt: Date.now(),
        },
      ]

      const reviewItems = getTopReviewItems(items, 1)
      expect(reviewItems.length).toBe(1)
      expect(reviewItems[0].reason).toContain('Low type confidence')
    })
  })
})
