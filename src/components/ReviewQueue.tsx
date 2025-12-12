import { useState, useEffect } from 'react'
import { ReviewQueueItem } from '@/lib/dashboard'
import { ItemType, Priority, InferredAttributes } from '@/lib/types'
import { Card } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Question } from '@phosphor-icons/react'
import { motion } from 'framer-motion'
import { parseNaturalDate, formatDate } from '@/lib/dateParser'

interface ReviewQueueProps {
  items: ReviewQueueItem[]
  onReview: (captureId: string, attributes: InferredAttributes) => void
}

export function ReviewQueue({ items, onReview }: ReviewQueueProps) {
  const [currentIndex, setCurrentIndex] = useState(0)
  const [formData, setFormData] = useState<InferredAttributes>({})
  const [dateInput, setDateInput] = useState('')
  const [dateError, setDateError] = useState<string | null>(null)

  useEffect(() => {
    if (currentIndex >= items.length && items.length > 0) {
      setCurrentIndex(0)
    }
  }, [items.length, currentIndex])

  if (items.length === 0) return null

  const currentItem = items[currentIndex]
  
  if (!currentItem) {
    return null
  }

  const handleDateChange = (value: string) => {
    setDateInput(value)
    setDateError(null)
    
    if (!value.trim()) {
      setFormData({ ...formData, dueDate: undefined })
      return
    }
    
    const parsedDate = parseNaturalDate(value)
    if (parsedDate !== null) {
      setFormData({ ...formData, dueDate: parsedDate })
      setDateError(null)
    } else {
      setDateError('Could not parse date')
    }
  }

  const handleSubmit = () => {
    const attributes: InferredAttributes = {
      ...(currentItem.inferredAttributes || {}),
      ...formData
    }
    
    onReview(currentItem.captureId, attributes)
    setFormData({})
    setDateInput('')
    setDateError(null)
    
    if (currentIndex < items.length - 1) {
      setCurrentIndex(currentIndex + 1)
    } else {
      setCurrentIndex(0)
    }
  }

  const needsType = currentItem.missingFields?.includes('type') || false
  const needsCollection = currentItem.missingFields?.includes('collection') || false
  const needsPriority = currentItem.missingFields?.includes('priority') || false
  const showDate = formData.type === 'action' || formData.type === 'reminder' || currentItem.inferredAttributes?.type === 'action' || currentItem.inferredAttributes?.type === 'reminder'

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      className="w-full"
    >
      <Card className="p-6 space-y-4 bg-accent/5 border-accent/30">
        <div className="flex items-center gap-2 text-accent">
          <Question size={24} weight="fill" />
          <h3 className="text-lg font-semibold">Needs Review ({items.length})</h3>
        </div>

        <div className="space-y-4">
          <div className="p-4 bg-card rounded-lg">
            <p className="text-base leading-relaxed">{currentItem.text}</p>
          </div>

          <div className="space-y-3">
            {needsType && (
              <div className="space-y-2">
                <Label htmlFor="type">What type of item is this?</Label>
                <Select
                  value={formData.type}
                  onValueChange={(value) => setFormData({ ...formData, type: value as ItemType })}
                >
                  <SelectTrigger id="type">
                    <SelectValue placeholder="Select type..." />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="note">Note</SelectItem>
                    <SelectItem value="action">Action</SelectItem>
                    <SelectItem value="reminder">Reminder</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            )}

            {needsCollection && (
              <div className="space-y-2">
                <Label htmlFor="collection">What collection does this belong to?</Label>
                <Input
                  id="collection"
                  value={formData.collection || ''}
                  onChange={(e) => setFormData({ ...formData, collection: e.target.value })}
                  placeholder="e.g., Work, Personal, Health..."
                />
              </div>
            )}

            {needsPriority && (
              <div className="space-y-2">
                <Label htmlFor="priority">Priority</Label>
                <Select
                  value={formData.priority}
                  onValueChange={(value) => setFormData({ ...formData, priority: value as Priority })}
                >
                  <SelectTrigger id="priority">
                    <SelectValue placeholder="Select priority..." />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="low">Low</SelectItem>
                    <SelectItem value="medium">Medium</SelectItem>
                    <SelectItem value="high">High</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            )}

            {showDate && (
              <div className="space-y-2">
                <Label htmlFor="due-date">Due Date (optional)</Label>
                <Input
                  id="due-date"
                  value={dateInput}
                  onChange={(e) => handleDateChange(e.target.value)}
                  placeholder="e.g., tomorrow, next Tuesday, in 3 days, 3/20"
                  className={dateError ? 'border-destructive' : ''}
                />
                {dateError && (
                  <p className="text-sm text-destructive">{dateError}</p>
                )}
                {formData.dueDate && !dateError && (
                  <p className="text-sm text-muted-foreground">
                    Parsed as: {formatDate(formData.dueDate)}
                  </p>
                )}
              </div>
            )}
          </div>

          <div className="flex justify-between items-center pt-2">
            <span className="text-sm text-muted-foreground font-mono">
              {currentIndex + 1} of {items.length}
            </span>
            <Button
              onClick={handleSubmit}
              disabled={
                (needsType && !formData.type) ||
                (needsCollection && !formData.collection) ||
                (needsPriority && !formData.priority)
              }
              className="bg-primary hover:bg-primary/90"
            >
              Submit Review
            </Button>
          </div>
        </div>
      </Card>
    </motion.div>
  )
}
