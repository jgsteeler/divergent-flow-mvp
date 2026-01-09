import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { fetchCaptures, createCapture } from '@/lib/api/capturesApi'

// Mock fetch globally
const mockFetch = vi.fn()
global.fetch = mockFetch

describe('capturesApi', () => {
  beforeEach(() => {
    mockFetch.mockClear()
  })

  afterEach(() => {
    vi.restoreAllMocks()
  })

  describe('fetchCaptures', () => {
    it('should fetch captures successfully', async () => {
      const mockCaptures = [
        { id: '1', text: 'Test capture 1', createdAt: Date.now() },
        { id: '2', text: 'Test capture 2', createdAt: Date.now() },
      ]

      mockFetch.mockResolvedValueOnce({
        ok: true,
        json: async () => mockCaptures,
      })

      const result = await fetchCaptures()

      expect(mockFetch).toHaveBeenCalledWith(
        expect.stringContaining('/api/captures'),
        expect.objectContaining({
          headers: expect.objectContaining({
            'Content-Type': 'application/json',
            'X-User-Id': 'local',
          }),
        })
      )
      expect(result).toEqual(mockCaptures)
    })

    it('should throw error on failed fetch', async () => {
      mockFetch.mockResolvedValueOnce({
        ok: false,
        statusText: 'Internal Server Error',
      })

      await expect(fetchCaptures()).rejects.toThrow(
        'Failed to fetch captures: Internal Server Error'
      )
    })
  })

  describe('createCapture', () => {
    it('should create capture successfully', async () => {
      const mockCapture = {
        id: '1',
        text: 'New capture',
        createdAt: Date.now(),
      }

      mockFetch.mockResolvedValueOnce({
        ok: true,
        json: async () => mockCapture,
      })

      const result = await createCapture({ text: 'New capture' })

      expect(mockFetch).toHaveBeenCalledWith(
        expect.stringContaining('/api/captures'),
        expect.objectContaining({
          method: 'POST',
          headers: expect.objectContaining({
            'Content-Type': 'application/json',
            'X-User-Id': 'local',
          }),
          body: JSON.stringify({ text: 'New capture' }),
        })
      )
      expect(result).toEqual(mockCapture)
    })

    it('should handle optional properties', async () => {
      const mockCapture = {
        id: '1',
        text: 'New capture',
        createdAt: Date.now(),
        inferredType: 'note',
        typeConfidence: 0.95,
      }

      mockFetch.mockResolvedValueOnce({
        ok: true,
        json: async () => mockCapture,
      })

      await createCapture({
        text: 'New capture',
        inferredType: 'note',
        typeConfidence: 0.95,
      })

      expect(mockFetch).toHaveBeenCalledWith(
        expect.anything(),
        expect.objectContaining({
          body: JSON.stringify({
            text: 'New capture',
            inferredType: 'note',
            typeConfidence: 0.95,
          }),
        })
      )
    })

    it('should throw error on failed create', async () => {
      mockFetch.mockResolvedValueOnce({
        ok: false,
        statusText: 'Bad Request',
      })

      await expect(createCapture({ text: 'Test' })).rejects.toThrow(
        'Failed to create capture: Bad Request'
      )
    })
  })
})
