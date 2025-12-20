import { describe, it, expect, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { AttributeConfirmation } from './AttributeConfirmation'
import { Item, LearningData } from '@/lib/types'

describe('AttributeConfirmation', () => {
  const mockOnConfirm = vi.fn()
  const mockOnDismiss = vi.fn()
  const mockLearningData: LearningData[] = []

  const createMockItem = (overrides?: Partial<Item>): Item => ({
    id: 'test-1',
    text: 'Test item',
    createdAt: Date.now(),
    migratedCapture: false,
    inferredType: 'action',
    typeConfidence: 75,
    collection: 'Work',
    collectionConfidence: 80,
    priority: 'medium',
    priorityConfidence: 60,
    priorityReasoning: 'No priority indicators found, defaulting to medium',
    estimate: '30min',
    estimateConfidence: 50,
    estimateReasoning: 'Moderate description suggests standard task',
    ...overrides,
  })

  it('should render all property sections for an action item', () => {
    const item = createMockItem()
    render(
      <AttributeConfirmation
        item={item}
        learningData={mockLearningData}
        onConfirm={mockOnConfirm}
        onDismiss={mockOnDismiss}
      />
    )

    expect(screen.getByText('Review and Confirm')).toBeInTheDocument()
    expect(screen.getByText('Type')).toBeInTheDocument()
    expect(screen.getByText('Collection')).toBeInTheDocument()
    expect(screen.getByText('Priority')).toBeInTheDocument()
    expect(screen.getByText('Estimate')).toBeInTheDocument()
    expect(screen.getByText('Context (optional)')).toBeInTheDocument()
    expect(screen.getByText('Tags (optional)')).toBeInTheDocument()
  })

  it('should display confidence scores with correct styling', () => {
    const item = createMockItem()
    render(
      <AttributeConfirmation
        item={item}
        learningData={mockLearningData}
        onConfirm={mockOnConfirm}
        onDismiss={mockOnDismiss}
      />
    )

    // Type confidence (75% - Medium)
    expect(screen.getByText('75% Medium')).toBeInTheDocument()
    
    // Priority confidence (60% - Low)
    expect(screen.getByText('60% Low')).toBeInTheDocument()
    
    // Estimate confidence (50% - Low)
    expect(screen.getByText('50% Low')).toBeInTheDocument()
  })

  it('should display reasoning text for priority and estimate', () => {
    const item = createMockItem()
    render(
      <AttributeConfirmation
        item={item}
        learningData={mockLearningData}
        onConfirm={mockOnConfirm}
        onDismiss={mockOnDismiss}
      />
    )

    expect(screen.getByText('No priority indicators found, defaulting to medium')).toBeInTheDocument()
    expect(screen.getByText('Moderate description suggests standard task')).toBeInTheDocument()
  })

  it('should show priority and estimate for action items', () => {
    const item = createMockItem({ inferredType: 'action' })
    render(
      <AttributeConfirmation
        item={item}
        learningData={mockLearningData}
        onConfirm={mockOnConfirm}
        onDismiss={mockOnDismiss}
      />
    )

    expect(screen.getByText('Priority')).toBeInTheDocument()
    expect(screen.getByText('Estimate')).toBeInTheDocument()
  })

  it('should show priority but not estimate for reminders', () => {
    const item = createMockItem({ inferredType: 'reminder' })
    render(
      <AttributeConfirmation
        item={item}
        learningData={mockLearningData}
        onConfirm={mockOnConfirm}
        onDismiss={mockOnDismiss}
      />
    )

    expect(screen.getByText('Priority')).toBeInTheDocument()
    expect(screen.queryByText('Estimate')).not.toBeInTheDocument()
  })

  it('should not show priority or estimate for notes', () => {
    const item = createMockItem({ inferredType: 'note' })
    render(
      <AttributeConfirmation
        item={item}
        learningData={mockLearningData}
        onConfirm={mockOnConfirm}
        onDismiss={mockOnDismiss}
      />
    )

    expect(screen.queryByText('Priority')).not.toBeInTheDocument()
    expect(screen.queryByText('Estimate')).not.toBeInTheDocument()
  })

  it('should show "Required for reminders" badge when reminder has no due date', () => {
    const item = createMockItem({ inferredType: 'reminder', dueDate: undefined })
    render(
      <AttributeConfirmation
        item={item}
        learningData={mockLearningData}
        onConfirm={mockOnConfirm}
        onDismiss={mockOnDismiss}
      />
    )

    expect(screen.getByText('Required for reminders')).toBeInTheDocument()
  })

  it('should disable confirm button when reminder has no due date', () => {
    const item = createMockItem({ inferredType: 'reminder', dueDate: undefined })
    render(
      <AttributeConfirmation
        item={item}
        learningData={mockLearningData}
        onConfirm={mockOnConfirm}
        onDismiss={mockOnDismiss}
      />
    )

    const confirmButton = screen.getByRole('button', { name: /reminders require a due date/i })
    expect(confirmButton).toBeDisabled()
  })

  it('should enable confirm button when all required fields are filled', () => {
    const item = createMockItem()
    render(
      <AttributeConfirmation
        item={item}
        learningData={mockLearningData}
        onConfirm={mockOnConfirm}
        onDismiss={mockOnDismiss}
      />
    )

    const confirmButton = screen.getByRole('button', { name: 'Confirm' })
    expect(confirmButton).not.toBeDisabled()
  })

  it('should call onDismiss when close button is clicked', () => {
    const item = createMockItem()
    const { container } = render(
      <AttributeConfirmation
        item={item}
        learningData={mockLearningData}
        onConfirm={mockOnConfirm}
        onDismiss={mockOnDismiss}
      />
    )

    // Find close button (X icon button)
    const closeButton = container.querySelector('button[class*="h-8 w-8"]')
    expect(closeButton).toBeInTheDocument()
    
    if (closeButton) {
      fireEvent.click(closeButton)
      expect(mockOnDismiss).toHaveBeenCalledWith(item.id)
    }
  })

  it('should allow changing type selection', () => {
    const item = createMockItem({ inferredType: 'action' })
    render(
      <AttributeConfirmation
        item={item}
        learningData={mockLearningData}
        onConfirm={mockOnConfirm}
        onDismiss={mockOnDismiss}
      />
    )

    const noteButton = screen.getByRole('button', { name: /Note Information to remember/i })
    fireEvent.click(noteButton)

    // After clicking Note, priority and estimate should disappear
    expect(screen.queryByText('Priority')).not.toBeInTheDocument()
    expect(screen.queryByText('Estimate')).not.toBeInTheDocument()
  })

  it('should handle context input', () => {
    const item = createMockItem()
    render(
      <AttributeConfirmation
        item={item}
        learningData={mockLearningData}
        onConfirm={mockOnConfirm}
        onDismiss={mockOnDismiss}
      />
    )

    const contextInput = screen.getByPlaceholderText(/home, office, phone/i)
    fireEvent.change(contextInput, { target: { value: 'office' } })
    expect(contextInput).toHaveValue('office')
  })

  it('should handle tags input', () => {
    const item = createMockItem()
    render(
      <AttributeConfirmation
        item={item}
        learningData={mockLearningData}
        onConfirm={mockOnConfirm}
        onDismiss={mockOnDismiss}
      />
    )

    const tagsInput = screen.getByPlaceholderText(/urgent, meeting, personal/i)
    fireEvent.change(tagsInput, { target: { value: 'urgent, work' } })
    expect(tagsInput).toHaveValue('urgent, work')
  })

  it('should call onConfirm with all updated properties', async () => {
    const item = createMockItem()
    render(
      <AttributeConfirmation
        item={item}
        learningData={mockLearningData}
        onConfirm={mockOnConfirm}
        onDismiss={mockOnDismiss}
      />
    )

    // Add context
    const contextInput = screen.getByPlaceholderText(/home, office, phone/i)
    fireEvent.change(contextInput, { target: { value: 'office' } })

    // Add tags
    const tagsInput = screen.getByPlaceholderText(/urgent, meeting, personal/i)
    fireEvent.change(tagsInput, { target: { value: 'work, urgent' } })

    // Click confirm
    const confirmButton = screen.getByRole('button', { name: 'Confirm' })
    fireEvent.click(confirmButton)

    await waitFor(() => {
      expect(mockOnConfirm).toHaveBeenCalledWith(
        item.id,
        expect.objectContaining({
          context: 'office',
          tags: ['work', 'urgent'],
          typeConfidence: 100,
          collectionConfidence: 100,
          priorityConfidence: 100,
          estimateConfidence: 100,
        })
      )
    })
  })

  it('should parse tags correctly with comma separation', async () => {
    const item = createMockItem()
    render(
      <AttributeConfirmation
        item={item}
        learningData={mockLearningData}
        onConfirm={mockOnConfirm}
        onDismiss={mockOnDismiss}
      />
    )

    const tagsInput = screen.getByPlaceholderText(/urgent, meeting, personal/i)
    fireEvent.change(tagsInput, { target: { value: '  work , urgent  , meeting  ' } })

    const confirmButton = screen.getByRole('button', { name: 'Confirm' })
    fireEvent.click(confirmButton)

    await waitFor(() => {
      expect(mockOnConfirm).toHaveBeenCalledWith(
        item.id,
        expect.objectContaining({
          tags: ['work', 'urgent', 'meeting'],
        })
      )
    })
  })

  it('should display pre-filled due date from item', () => {
    const tomorrow = Date.now() + 86400000
    const item = createMockItem({ dueDate: tomorrow })
    render(
      <AttributeConfirmation
        item={item}
        learningData={mockLearningData}
        onConfirm={mockOnConfirm}
        onDismiss={mockOnDismiss}
      />
    )

    const dueDateInput = screen.getByPlaceholderText(/tomorrow at 3pm/i)
    // The date is formatted by formatDate function
    expect(dueDateInput.value).toBeTruthy()
    expect(dueDateInput.value.length).toBeGreaterThan(0)
  })

  it('should show natural language date hint', () => {
    const item = createMockItem()
    render(
      <AttributeConfirmation
        item={item}
        learningData={mockLearningData}
        onConfirm={mockOnConfirm}
        onDismiss={mockOnDismiss}
      />
    )

    const dueDateInput = screen.getByPlaceholderText(/tomorrow at 3pm/i)
    fireEvent.change(dueDateInput, { target: { value: 'next Tuesday' } })

    expect(screen.getByText(/Natural language dates are supported/i)).toBeInTheDocument()
  })
})
