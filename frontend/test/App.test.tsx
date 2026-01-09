import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import App from '../src/App'
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

// Mock the Auth0 config
vi.mock('@/lib/auth/authConfig', () => ({
  isAuth0Configured: () => false, // Disable Auth0 in tests
  getAuth0Config: () => {
    throw new Error('Auth0 not configured')
  },
}))

// Helper to wrap component with QueryClient
const renderWithQueryClient = (component: React.ReactElement) => {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false },
      mutations: { retry: false },
    },
  })
  return render(
    <QueryClientProvider client={queryClient}>
      {component}
    </QueryClientProvider>
  )
}

describe('App - Basic Capture and View', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    // Default mock: return empty array for captures
    vi.mocked(capturesApi.fetchCaptures).mockResolvedValue([])
  })

  it('should render the capture input', async () => {
    renderWithQueryClient(<App />)
    
    await waitFor(() => {
      expect(screen.getByText('Divergent Flow')).toBeInTheDocument()
    })
    
    expect(screen.getByPlaceholderText(/Capture anything/i)).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /Capture/i })).toBeInTheDocument()
  })

  it('should disable capture button when input is empty', async () => {
    renderWithQueryClient(<App />)
    
    await waitFor(() => {
      expect(screen.getByRole('button', { name: /^Capture$/i })).toBeInTheDocument()
    })
    
    const captureButton = screen.getByRole('button', { name: /^Capture$/i })
    expect(captureButton).toBeDisabled()
  })

  it('should enable capture button when input has text', async () => {
    renderWithQueryClient(<App />)
    
    await waitFor(() => {
      expect(screen.getByPlaceholderText(/Capture anything/i)).toBeInTheDocument()
    })
    
    const textarea = screen.getByPlaceholderText(/Capture anything/i)
    const captureButton = screen.getByRole('button', { name: /^Capture$/i })
    
    fireEvent.change(textarea, { target: { value: 'Test capture' } })
    
    expect(captureButton).not.toBeDisabled()
  })

  it('should capture text and show view captures button', async () => {
    const mockCapture = {
      id: '1',
      text: 'My first capture',
      createdAt: Date.now(),
    }
    
    // Mock API responses
    vi.mocked(capturesApi.createCapture).mockResolvedValue(mockCapture)
    vi.mocked(capturesApi.fetchCaptures).mockResolvedValue([mockCapture])
    
    renderWithQueryClient(<App />)
    
    await waitFor(() => {
      expect(screen.getByPlaceholderText(/Capture anything/i)).toBeInTheDocument()
    })
    
    const textarea = screen.getByPlaceholderText(/Capture anything/i)
    const captureButton = screen.getByRole('button', { name: /^Capture$/i })
    
    // Type and capture
    fireEvent.change(textarea, { target: { value: 'My first capture' } })
    fireEvent.click(captureButton)
    
    // Should show view captures button after refetch
    await waitFor(() => {
      expect(screen.getByRole('button', { name: /View Captures \(1\)/i })).toBeInTheDocument()
    })
    
    // Textarea should be cleared
    expect(textarea).toHaveValue('')
  })

  it('should navigate to list view and back', async () => {
    const mockCapture = {
      id: '1',
      text: 'Test item',
      createdAt: Date.now(),
    }
    
    vi.mocked(capturesApi.createCapture).mockResolvedValue(mockCapture)
    vi.mocked(capturesApi.fetchCaptures).mockResolvedValue([mockCapture])
    
    renderWithQueryClient(<App />)
    
    await waitFor(() => {
      expect(screen.getByPlaceholderText(/Capture anything/i)).toBeInTheDocument()
    })
    
    // Capture an item
    const textarea = screen.getByPlaceholderText(/Capture anything/i)
    fireEvent.change(textarea, { target: { value: 'Test item' } })
    fireEvent.click(screen.getByRole('button', { name: /^Capture$/i }))
    
    // Click view captures
    await waitFor(() => {
      const viewButton = screen.getByRole('button', { name: /View Captures/i })
      fireEvent.click(viewButton)
    })
    
    // Should show list view
    await waitFor(() => {
      expect(screen.getByText('Your Captures')).toBeInTheDocument()
    })
    expect(screen.getByText('Test item')).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /Back to Capture/i })).toBeInTheDocument()
    
    // Click back
    fireEvent.click(screen.getByRole('button', { name: /Back to Capture/i }))
    
    // Should be back to capture view
    await waitFor(() => {
      expect(screen.getByPlaceholderText(/Capture anything/i)).toBeInTheDocument()
    })
  })

  it('should display multiple captures in list view', async () => {
    const captures = [
      { id: '1', text: 'First capture', createdAt: Date.now() },
      { id: '2', text: 'Second capture', createdAt: Date.now() + 1000 },
    ]
    
    let captureIndex = 0
    vi.mocked(capturesApi.createCapture).mockImplementation(async (req) => {
      const capture = { ...captures[captureIndex], text: req.text }
      captureIndex++
      return capture
    })
    vi.mocked(capturesApi.fetchCaptures).mockResolvedValue(captures)
    
    renderWithQueryClient(<App />)
    
    await waitFor(() => {
      expect(screen.getByPlaceholderText(/Capture anything/i)).toBeInTheDocument()
    })
    
    const textarea = screen.getByPlaceholderText(/Capture anything/i)
    
    // Capture first item
    fireEvent.change(textarea, { target: { value: 'First capture' } })
    fireEvent.click(screen.getByRole('button', { name: /^Capture$/i }))
    
    // Wait for textarea to clear and capture second item
    await waitFor(() => expect(textarea).toHaveValue(''))
    fireEvent.change(textarea, { target: { value: 'Second capture' } })
    fireEvent.click(screen.getByRole('button', { name: /^Capture$/i }))
    
    // View captures
    await waitFor(() => {
      const viewButton = screen.getByRole('button', { name: /View Captures \(2\)/i })
      fireEvent.click(viewButton)
    })
    
    // Should show both captures
    await waitFor(() => {
      expect(screen.getByText('First capture')).toBeInTheDocument()
      expect(screen.getByText('Second capture')).toBeInTheDocument()
    })
  })

  it('should show empty state when no captures exist', async () => {
    vi.mocked(capturesApi.fetchCaptures).mockResolvedValue([])
    
    renderWithQueryClient(<App />)
    
    await waitFor(() => {
      expect(screen.getByPlaceholderText(/Capture anything/i)).toBeInTheDocument()
    })
    
    // No view captures button should be shown initially
    expect(screen.queryByRole('button', { name: /View Captures/i })).not.toBeInTheDocument()
  })

  it('should persist captures via API', async () => {
    const mockCapture = {
      id: '1',
      text: 'Persisted capture',
      createdAt: Date.now(),
    }
    
    vi.mocked(capturesApi.createCapture).mockResolvedValue(mockCapture)
    vi.mocked(capturesApi.fetchCaptures).mockResolvedValue([mockCapture])
    
    const { unmount } = renderWithQueryClient(<App />)
    
    await waitFor(() => {
      expect(screen.getByPlaceholderText(/Capture anything/i)).toBeInTheDocument()
    })
    
    const textarea = screen.getByPlaceholderText(/Capture anything/i)
    
    // Capture an item
    fireEvent.change(textarea, { target: { value: 'Persisted capture' } })
    fireEvent.click(screen.getByRole('button', { name: /^Capture$/i }))
    
    await waitFor(() => {
      expect(screen.getByRole('button', { name: /View Captures \(1\)/i })).toBeInTheDocument()
    })
    
    // Unmount and remount
    unmount()
    renderWithQueryClient(<App />)
    
    // Should still show view captures button (data from API)
    await waitFor(() => {
      expect(screen.getByRole('button', { name: /View Captures \(1\)/i })).toBeInTheDocument()
    })
  })
})
