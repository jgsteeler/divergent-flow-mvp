import { describe, it, expect, vi } from 'vitest'
import { render, screen, fireEvent } from '@testing-library/react'
import { CaptureList } from './CaptureList'
import type { Capture } from '@/lib/types'

describe('CaptureList', () => {
  const mockOnBack = vi.fn()

  it('should display capture text', () => {
    const captures: Capture[] = [
      {
        id: '1',
        text: 'Test capture',
        createdAt: Date.now(),
      },
    ]

    render(<CaptureList captures={captures} onBack={mockOnBack} />)
    
    expect(screen.getByText('Test capture')).toBeInTheDocument()
  })

  it('should display inferred type when available', () => {
    const captures: Capture[] = [
      {
        id: '1',
        text: 'Test capture',
        createdAt: Date.now(),
        inferredType: 'action',
      },
    ]

    render(<CaptureList captures={captures} onBack={mockOnBack} />)
    
    expect(screen.getByText('action')).toBeInTheDocument()
  })

  it('should display type confidence when available', () => {
    const captures: Capture[] = [
      {
        id: '1',
        text: 'Test capture',
        createdAt: Date.now(),
        inferredType: 'action',
        typeConfidence: 85.5,
      },
    ]

    render(<CaptureList captures={captures} onBack={mockOnBack} />)
    
    // Should display rounded confidence percentage
    expect(screen.getByText('86%')).toBeInTheDocument()
  })

  it('should display updatedAt when different from createdAt', () => {
    const createdAt = new Date('2024-01-01').getTime()
    const updatedAt = new Date('2024-01-15').getTime()
    
    const captures: Capture[] = [
      {
        id: '1',
        text: 'Test capture',
        createdAt,
        updatedAt,
      },
    ]

    render(<CaptureList captures={captures} onBack={mockOnBack} />)
    
    // Should show "Updated" text
    expect(screen.getByText(/Updated/)).toBeInTheDocument()
  })

  it('should not display updatedAt when same as createdAt', () => {
    const timestamp = Date.now()
    
    const captures: Capture[] = [
      {
        id: '1',
        text: 'Test capture',
        createdAt: timestamp,
        updatedAt: timestamp,
      },
    ]

    render(<CaptureList captures={captures} onBack={mockOnBack} />)
    
    // Should not show "Updated" text
    expect(screen.queryByText(/Updated/)).not.toBeInTheDocument()
  })

  it('should not display updatedAt when not set', () => {
    const captures: Capture[] = [
      {
        id: '1',
        text: 'Test capture',
        createdAt: Date.now(),
      },
    ]

    render(<CaptureList captures={captures} onBack={mockOnBack} />)
    
    // Should not show "Updated" text
    expect(screen.queryByText(/Updated/)).not.toBeInTheDocument()
  })

  it('should display both type confidence and updatedAt when available', () => {
    const createdAt = new Date('2024-01-01').getTime()
    const updatedAt = new Date('2024-01-15').getTime()
    
    const captures: Capture[] = [
      {
        id: '1',
        text: 'Test capture',
        createdAt,
        updatedAt,
        inferredType: 'note',
        typeConfidence: 92.3,
      },
    ]

    render(<CaptureList captures={captures} onBack={mockOnBack} />)
    
    expect(screen.getByText('note')).toBeInTheDocument()
    expect(screen.getByText('92%')).toBeInTheDocument()
    expect(screen.getByText(/Updated/)).toBeInTheDocument()
  })

  it('should call onBack when back button is clicked', () => {
    const captures: Capture[] = [
      {
        id: '1',
        text: 'Test capture',
        createdAt: Date.now(),
      },
    ]

    render(<CaptureList captures={captures} onBack={mockOnBack} />)
    
    const backButton = screen.getByRole('button', { name: /Back to Capture/i })
    fireEvent.click(backButton)
    
    expect(mockOnBack).toHaveBeenCalledTimes(1)
  })

  it('should show empty state when no captures', () => {
    render(<CaptureList captures={[]} onBack={mockOnBack} />)
    
    expect(screen.getByText(/No captures yet/i)).toBeInTheDocument()
  })

  it('should show loading state', () => {
    render(<CaptureList captures={[]} onBack={mockOnBack} isLoading={true} />)
    
    expect(screen.getByText(/Loading captures/i)).toBeInTheDocument()
  })

  it('should show error state', () => {
    const error = new Error('Failed to load')
    render(<CaptureList captures={[]} onBack={mockOnBack} error={error} />)
    
    expect(screen.getByText(/Failed to load captures/i)).toBeInTheDocument()
  })
})
