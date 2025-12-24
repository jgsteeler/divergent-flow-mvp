import { describe, it, expect, beforeEach } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import App from './App'

describe('App - Basic Capture and View', () => {
  beforeEach(() => {
    // Clear localStorage before each test
    localStorage.clear()
  })

  it('should render the capture input', () => {
    render(<App />)
    
    expect(screen.getByText('Divergent Flow')).toBeInTheDocument()
    expect(screen.getByPlaceholderText(/Capture anything/i)).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /Capture/i })).toBeInTheDocument()
  })

  it('should disable capture button when input is empty', () => {
    render(<App />)
    
    const captureButton = screen.getByRole('button', { name: /^Capture$/i })
    expect(captureButton).toBeDisabled()
  })

  it('should enable capture button when input has text', () => {
    render(<App />)
    
    const textarea = screen.getByPlaceholderText(/Capture anything/i)
    const captureButton = screen.getByRole('button', { name: /^Capture$/i })
    
    fireEvent.change(textarea, { target: { value: 'Test capture' } })
    
    expect(captureButton).not.toBeDisabled()
  })

  it('should capture text and show view captures button', async () => {
    render(<App />)
    
    const textarea = screen.getByPlaceholderText(/Capture anything/i)
    const captureButton = screen.getByRole('button', { name: /^Capture$/i })
    
    // Type and capture
    fireEvent.change(textarea, { target: { value: 'My first capture' } })
    fireEvent.click(captureButton)
    
    // Should show view captures button
    await waitFor(() => {
      expect(screen.getByRole('button', { name: /View Captures \(1\)/i })).toBeInTheDocument()
    })
    
    // Textarea should be cleared
    expect(textarea).toHaveValue('')
  })

  it('should navigate to list view and back', async () => {
    render(<App />)
    
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
    expect(screen.getByText('Your Captures')).toBeInTheDocument()
    expect(screen.getByText('Test item')).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /Back to Capture/i })).toBeInTheDocument()
    
    // Click back
    fireEvent.click(screen.getByRole('button', { name: /Back to Capture/i }))
    
    // Should be back to capture view
    expect(screen.getByPlaceholderText(/Capture anything/i)).toBeInTheDocument()
  })

  it('should display multiple captures in list view', async () => {
    render(<App />)
    
    const textarea = screen.getByPlaceholderText(/Capture anything/i)
    
    // Capture first item
    fireEvent.change(textarea, { target: { value: 'First capture' } })
    fireEvent.click(screen.getByRole('button', { name: /^Capture$/i }))
    
    // Capture second item
    await waitFor(() => expect(textarea).toHaveValue(''))
    fireEvent.change(textarea, { target: { value: 'Second capture' } })
    fireEvent.click(screen.getByRole('button', { name: /^Capture$/i }))
    
    // View captures
    await waitFor(() => {
      const viewButton = screen.getByRole('button', { name: /View Captures \(2\)/i })
      fireEvent.click(viewButton)
    })
    
    // Should show both captures
    expect(screen.getByText('First capture')).toBeInTheDocument()
    expect(screen.getByText('Second capture')).toBeInTheDocument()
  })

  it('should show empty state when no captures exist', () => {
    render(<App />)
    
    // No view captures button should be shown initially
    expect(screen.queryByRole('button', { name: /View Captures/i })).not.toBeInTheDocument()
  })

  it('should persist captures in localStorage', async () => {
    const { unmount } = render(<App />)
    
    const textarea = screen.getByPlaceholderText(/Capture anything/i)
    
    // Capture an item
    fireEvent.change(textarea, { target: { value: 'Persisted capture' } })
    fireEvent.click(screen.getByRole('button', { name: /^Capture$/i }))
    
    await waitFor(() => {
      expect(screen.getByRole('button', { name: /View Captures \(1\)/i })).toBeInTheDocument()
    })
    
    // Unmount and remount
    unmount()
    render(<App />)
    
    // Should still show view captures button
    expect(screen.getByRole('button', { name: /View Captures \(1\)/i })).toBeInTheDocument()
  })
})
