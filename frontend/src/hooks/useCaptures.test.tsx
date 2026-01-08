import { describe, it, expect, vi, beforeEach } from 'vitest'
import { renderHook, waitFor } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { useCaptures, useCreateCapture } from '@/hooks/useCaptures'
import * as capturesApi from '@/lib/api/capturesApi'

// Mock the API module
vi.mock('@/lib/api/capturesApi')

// Mock the useAuth hook
vi.mock('@/hooks/useAuth', () => ({
  useAuth: () => ({
    getUserId: () => 'test-user-id',
    getAccessToken: async () => 'test-access-token',
    isAuthenticated: true,
    isLoading: false,
    user: { sub: 'test-user-id', email: 'test@example.com' },
    login: vi.fn(),
    logout: vi.fn(),
  }),
}))

const createWrapper = () => {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false },
      mutations: { retry: false },
    },
  })
  return ({ children }: { children: React.ReactNode }) => (
    <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  )
}

describe('useCaptures', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should fetch captures successfully', async () => {
    const mockCaptures = [
      { id: '1', text: 'Test capture', createdAt: Date.now() },
    ]

    vi.mocked(capturesApi.fetchCaptures).mockResolvedValueOnce(mockCaptures)

    const { result } = renderHook(() => useCaptures(), {
      wrapper: createWrapper(),
    })

    await waitFor(() => expect(result.current.isSuccess).toBe(true))

    expect(result.current.data).toEqual(mockCaptures)
    expect(capturesApi.fetchCaptures).toHaveBeenCalledTimes(1)
  })

  it('should handle fetch error', async () => {
    const mockError = new Error('Network error')
    vi.mocked(capturesApi.fetchCaptures).mockRejectedValueOnce(mockError)

    const { result } = renderHook(() => useCaptures(), {
      wrapper: createWrapper(),
    })

    await waitFor(() => expect(result.current.isError).toBe(true))

    expect(result.current.error).toEqual(mockError)
  })
})

describe('useCreateCapture', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should create capture successfully', async () => {
    const mockCapture = {
      id: '1',
      text: 'New capture',
      createdAt: Date.now(),
    }

    vi.mocked(capturesApi.createCapture).mockResolvedValueOnce(mockCapture)

    const { result } = renderHook(() => useCreateCapture(), {
      wrapper: createWrapper(),
    })

    result.current.mutate({ text: 'New capture' })

    await waitFor(() => expect(result.current.isSuccess).toBe(true))

    expect(result.current.data).toEqual(mockCapture)
    expect(capturesApi.createCapture).toHaveBeenCalledWith(
      { text: 'New capture' },
      expect.anything()
    )
  })

  it('should handle create error', async () => {
    const mockError = new Error('Failed to create')
    vi.mocked(capturesApi.createCapture).mockRejectedValueOnce(mockError)

    const { result } = renderHook(() => useCreateCapture(), {
      wrapper: createWrapper(),
    })

    result.current.mutate({ text: 'Test' })

    await waitFor(() => expect(result.current.isError).toBe(true))

    expect(result.current.error).toEqual(mockError)
  })
})
