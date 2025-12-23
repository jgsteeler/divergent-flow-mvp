import { useState } from 'react'
import { ItemType, Priority, Estimate, Item, LearningData } from '@/lib/types'
import { getTypeLabel, getTypeDescription } from '@/lib/typeInference'
import { inferCollections, getLearnedCollections, getRelevantInferences } from '@/lib/collectionInference'
import { formatDate, extractDateTimeFromText } from '@/lib/dateParser'
import { getPriorityLabel, getPriorityDescription, getEstimateLabel, getEstimateDescription } from '@/lib/priorityEstimateInference'
import { HIGH_CONFIDENCE_THRESHOLD, MEDIUM_CONFIDENCE_THRESHOLD, CONFIRMED_CONFIDENCE, COLLECTION_HIGH_CONFIDENCE_THRESHOLD } from '@/lib/constants'
import { Card } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Label } from '@/components/ui/label'
import { Input } from '@/components/ui/input'
import { Textarea } from '@/components/ui/textarea'
import { Check, X, Note, ListChecks, Bell, Calendar, Clock, Tag, MapPin, Warning, Info, PencilSimple } from '@phosphor-icons/react'
import { motion, AnimatePresence } from 'framer-motion'
import { CollectionSelector } from '@/components/CollectionSelector'

interface AttributeConfirmationProps {
  item: Item
  learningData: LearningData[]
  onConfirm: (itemId: string, updates: Partial<Item>) => void
  onDismiss: (itemId: string) => void
}

const TYPE_ICONS = {
  note: Note,
  action: ListChecks,
  reminder: Bell,
}

export function AttributeConfirmation({
  item,
  learningData,
  onConfirm,
  onDismiss,
}: AttributeConfirmationProps) {
  const [selectedType, setSelectedType] = useState<ItemType | null>(item.inferredType || null)
  const [selectedCollection, setSelectedCollection] = useState<string>(item.collection || '')
  const [selectedPriority, setSelectedPriority] = useState<Priority | undefined>(item.priority)
  const [selectedEstimate, setSelectedEstimate] = useState<Estimate | undefined>(item.estimate)
  const [selectedContext, setSelectedContext] = useState<string>(item.context || '')
  const [selectedTags, setSelectedTags] = useState<string>(item.tags?.join(', ') || '')
  const [selectedDueDate, setSelectedDueDate] = useState<string>(
    item.dueDate ? formatDate(item.dueDate) : ''
  )
  const [dueDateError, setDueDateError] = useState<string>('')
  const [showCollectionEditor, setShowCollectionEditor] = useState<boolean>(false)
  const [showTypeEditor, setShowTypeEditor] = useState<boolean>(false)

  const types: ItemType[] = ['note', 'action', 'reminder']
  const allCollections = getLearnedCollections(learningData)
  const collectionInferences = inferCollections(item.text, learningData)
  const relevantInferences = getRelevantInferences(collectionInferences)
  const priorities: Priority[] = ['low', 'medium', 'high']
  const estimates: Estimate[] = ['5min', '15min', '30min', '1hour', '2hours', 'halfday', 'day', 'multiday']

  // Check if we have a high-confidence collection that should be auto-displayed
  const hasHighConfidenceCollection = item.collectionConfidence !== undefined && 
    item.collectionConfidence >= COLLECTION_HIGH_CONFIDENCE_THRESHOLD &&
    item.collection
  
  // Check if we have a high-confidence type that should be auto-displayed
  const hasHighConfidenceType = item.typeConfidence !== undefined && 
    item.typeConfidence >= HIGH_CONFIDENCE_THRESHOLD &&
    item.inferredType
  
  // Show editor if: no high confidence collection OR user explicitly wants to edit
  const shouldShowEditor = !hasHighConfidenceCollection || showCollectionEditor
  
  // Show type editor if: no high confidence type OR user explicitly wants to edit
  const shouldShowTypeEditor = !hasHighConfidenceType || showTypeEditor

  const getConfidenceBadgeColor = (conf: number) => {
    if (conf >= HIGH_CONFIDENCE_THRESHOLD) return 'bg-primary/10 text-primary border-primary/20'
    if (conf >= MEDIUM_CONFIDENCE_THRESHOLD) return 'bg-accent/10 text-accent-foreground border-accent/20'
    return 'bg-orange-500/10 text-orange-600 border-orange-500/20'
  }

  const getConfidenceLabel = (conf: number) => {
    if (conf >= HIGH_CONFIDENCE_THRESHOLD) return 'High'
    if (conf >= MEDIUM_CONFIDENCE_THRESHOLD) return 'Medium'
    return 'Low'
  }

  const handleDueDateChange = (value: string) => {
    setSelectedDueDate(value)
    setDueDateError('')
    
    // Only validate if there's a value
    if (value.trim()) {
      const { dateTime } = extractDateTimeFromText(value)
      if (!dateTime) {
        setDueDateError('Could not parse date. Try formats like "tomorrow", "next Tuesday at 3pm", or "in 2 days"')
      }
    }
  }

  const handleConfirm = () => {
    // Parse due date if provided
    let parsedDueDate = item.dueDate
    if (selectedDueDate.trim()) {
      const { dateTime } = extractDateTimeFromText(selectedDueDate)
      if (!dateTime) {
        setDueDateError('Could not parse date. Try formats like "tomorrow", "next Tuesday at 3pm", or "in 2 days"')
        return
      }
      parsedDueDate = dateTime
    }

    // Parse tags
    const parsedTags = selectedTags
      .split(',')
      .map(t => t.trim())
      .filter(t => t.length > 0)

    const updates: Partial<Item> = {
      inferredType: selectedType || undefined,
      typeConfidence: CONFIRMED_CONFIDENCE,
      collection: selectedCollection || undefined,
      collectionConfidence: CONFIRMED_CONFIDENCE,
      priority: selectedPriority,
      priorityConfidence: selectedPriority ? CONFIRMED_CONFIDENCE : undefined,
      estimate: selectedEstimate,
      estimateConfidence: selectedEstimate ? CONFIRMED_CONFIDENCE : undefined,
      context: selectedContext || undefined,
      tags: parsedTags.length > 0 ? parsedTags : undefined,
      dueDate: parsedDueDate,
      lastReviewedAt: Date.now(),
    }
    onConfirm(item.id, updates)
  }

  const isReminderWithoutDate = selectedType === 'reminder' && !selectedDueDate
  const hasRequiredFields = selectedType && selectedCollection
  const isValid = hasRequiredFields && !isReminderWithoutDate

  return (
    <AnimatePresence>
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        exit={{ opacity: 0, y: -20 }}
        transition={{ duration: 0.2 }}
      >
        <Card className="p-6 border-2 border-accent/40 bg-card shadow-lg max-h-[80vh] overflow-y-auto">
          <div className="space-y-6">
            <div className="flex items-start justify-between gap-4">
              <div className="flex-1 space-y-2">
                <h3 className="text-lg font-semibold text-foreground">
                  Review and Confirm
                </h3>
                <p className="text-sm text-muted-foreground line-clamp-2">
                  "{item.text}"
                </p>
              </div>
              <Button
                variant="ghost"
                size="icon"
                onClick={() => onDismiss(item.id)}
                className="h-8 w-8 text-muted-foreground hover:text-foreground"
              >
                <X />
              </Button>
            </div>

            {/* Type Selection */}
            <div className="space-y-3">
              <div className="flex items-center gap-2">
                <Label className="text-sm font-medium">Type</Label>
                {item.typeConfidence !== undefined && item.typeConfidence > 0 && (
                  <Badge variant="outline" className={getConfidenceBadgeColor(item.typeConfidence)}>
                    {item.typeConfidence}% {getConfidenceLabel(item.typeConfidence)}
                  </Badge>
                )}
              </div>
              
              {hasHighConfidenceType && !showTypeEditor ? (
                // High confidence: Show type directly with edit button
                <div className="space-y-2">
                  <div 
                    className="flex items-center justify-between p-3 rounded-md border-2 border-primary/40 bg-primary/5 cursor-pointer hover:bg-primary/10 transition-colors"
                    onClick={() => setShowTypeEditor(true)}
                  >
                    <div className="flex items-center gap-3">
                      {selectedType && (() => {
                        const Icon = TYPE_ICONS[selectedType]
                        return <Icon size={20} weight="duotone" className="text-primary" />
                      })()}
                      <div>
                        <div className="font-medium text-foreground">
                          {selectedType && getTypeLabel(selectedType)}
                        </div>
                        <div className="text-xs text-muted-foreground">
                          High confidence - Click to change
                        </div>
                      </div>
                    </div>
                    <PencilSimple size={18} className="text-muted-foreground" />
                  </div>
                </div>
              ) : (
                // Normal flow: Show type selector
                <div className="grid gap-2">
                  {types.map((type) => {
                    const Icon = TYPE_ICONS[type]
                    const isSelected = type === selectedType
                    
                    return (
                      <Button
                        key={type}
                        variant={isSelected ? 'default' : 'outline'}
                        className={`justify-start h-auto py-3 px-4 ${
                          isSelected 
                            ? 'bg-primary hover:bg-primary/90 text-primary-foreground' 
                            : 'hover:bg-accent hover:text-accent-foreground'
                        }`}
                        onClick={() => setSelectedType(type)}
                      >
                        <div className="flex items-center gap-3 w-full">
                          <Icon 
                            size={20} 
                            weight="duotone"
                            className={isSelected ? 'text-primary-foreground' : 'text-muted-foreground'}
                          />
                          <div className="flex-1 text-left">
                            <div className="flex items-center gap-2">
                              <span className="font-medium">{getTypeLabel(type)}</span>
                              {isSelected && (
                                <Check size={16} weight="bold" />
                              )}
                            </div>
                            <span className={`text-xs ${
                              isSelected ? 'text-primary-foreground/80' : 'text-muted-foreground'
                            }`}>
                              {getTypeDescription(type)}
                            </span>
                          </div>
                        </div>
                      </Button>
                    )
                  })}
                </div>
              )}
            </div>

            {/* Collection Selection */}
            <div className="space-y-3">
              <div className="flex items-center gap-2">
                <Label className="text-sm font-medium">Collection</Label>
                {item.collectionConfidence !== undefined && item.collectionConfidence > 0 && (
                  <Badge variant="outline" className={getConfidenceBadgeColor(item.collectionConfidence)}>
                    {item.collectionConfidence}% {getConfidenceLabel(item.collectionConfidence)}
                  </Badge>
                )}
              </div>
              
              {hasHighConfidenceCollection && !showCollectionEditor ? (
                // High confidence: Show collection directly with edit button
                <div className="space-y-2">
                  <div 
                    className="flex items-center justify-between p-3 rounded-md border-2 border-primary/40 bg-primary/5 cursor-pointer hover:bg-primary/10 transition-colors"
                    onClick={() => setShowCollectionEditor(true)}
                  >
                    <div className="flex items-center gap-3">
                      <Check size={20} weight="bold" className="text-primary" />
                      <div>
                        <div className="font-medium text-foreground">{selectedCollection}</div>
                        <div className="text-xs text-muted-foreground">
                          High confidence - Click to change
                        </div>
                      </div>
                    </div>
                    <PencilSimple size={18} className="text-muted-foreground" />
                  </div>
                </div>
              ) : (
                // Normal flow: Show collection selector
                <CollectionSelector
                  inferences={relevantInferences}
                  allCollections={allCollections}
                  selectedCollection={selectedCollection}
                  onSelect={setSelectedCollection}
                  showLabel={false}
                />
              )}
            </div>

            {/* Priority Selection (for actions and reminders) */}
            {(selectedType === 'action' || selectedType === 'reminder') && (
              <div className="space-y-3">
                <div className="flex items-center gap-2">
                  <Label className="text-sm font-medium">Priority</Label>
                  {item.priorityConfidence !== undefined && item.priorityConfidence > 0 && (
                    <Badge variant="outline" className={getConfidenceBadgeColor(item.priorityConfidence)}>
                      {item.priorityConfidence}% {getConfidenceLabel(item.priorityConfidence)}
                    </Badge>
                  )}
                  {item.priorityReasoning && (
                    <Badge variant="outline" className="bg-muted text-muted-foreground text-xs">
                      <Info size={12} className="mr-1" /> {item.priorityReasoning}
                    </Badge>
                  )}
                </div>
                <div className="grid grid-cols-3 gap-2">
                  {priorities.map((priority) => {
                    const isSelected = priority === selectedPriority
                    
                    return (
                      <Button
                        key={priority}
                        variant={isSelected ? 'default' : 'outline'}
                        className={`justify-center capitalize ${
                          isSelected 
                            ? 'bg-primary hover:bg-primary/90 text-primary-foreground' 
                            : 'hover:bg-accent hover:text-accent-foreground'
                        }`}
                        onClick={() => setSelectedPriority(priority)}
                      >
                        {priority}
                        {isSelected && <Check size={16} weight="bold" className="ml-2" />}
                      </Button>
                    )
                  })}
                </div>
                <p className="text-xs text-muted-foreground">
                  {selectedPriority ? getPriorityDescription(selectedPriority) : 'Select a priority level'}
                </p>
              </div>
            )}

            {/* Estimate Selection (for actions) */}
            {selectedType === 'action' && (
              <div className="space-y-3">
                <div className="flex items-center gap-2">
                  <Clock size={16} weight="duotone" className="text-muted-foreground" />
                  <Label className="text-sm font-medium">Estimate</Label>
                  {item.estimateConfidence !== undefined && item.estimateConfidence > 0 && (
                    <Badge variant="outline" className={getConfidenceBadgeColor(item.estimateConfidence)}>
                      {item.estimateConfidence}% {getConfidenceLabel(item.estimateConfidence)}
                    </Badge>
                  )}
                  {item.estimateReasoning && (
                    <Badge variant="outline" className="bg-muted text-muted-foreground text-xs">
                      <Info size={12} className="mr-1" /> {item.estimateReasoning}
                    </Badge>
                  )}
                </div>
                <div className="grid grid-cols-4 gap-2">
                  {estimates.map((estimate) => {
                    const isSelected = estimate === selectedEstimate
                    
                    return (
                      <Button
                        key={estimate}
                        variant={isSelected ? 'default' : 'outline'}
                        size="sm"
                        className={`justify-center text-xs ${
                          isSelected 
                            ? 'bg-primary hover:bg-primary/90 text-primary-foreground' 
                            : 'hover:bg-accent hover:text-accent-foreground'
                        }`}
                        onClick={() => setSelectedEstimate(estimate)}
                      >
                        {getEstimateLabel(estimate)}
                        {isSelected && <Check size={12} weight="bold" className="ml-1" />}
                      </Button>
                    )
                  })}
                </div>
                {selectedEstimate && (
                  <p className="text-xs text-muted-foreground">
                    {getEstimateDescription(selectedEstimate)}
                  </p>
                )}
              </div>
            )}

            {/* Due Date/Time (for reminders and actions) */}
            {(selectedType === 'reminder' || selectedType === 'action') && (
              <div className="space-y-3">
                <div className="flex items-center gap-2">
                  <Calendar size={16} weight="duotone" className="text-muted-foreground" />
                  <Label className="text-sm font-medium">
                    {selectedType === 'reminder' ? 'Due Date/Time' : 'Due Date (optional)'}
                  </Label>
                  {selectedType === 'reminder' && !selectedDueDate && (
                    <Badge variant="outline" className="bg-orange-500/10 text-orange-600 border-orange-500/20 text-xs">
                      <Warning size={12} className="mr-1" /> Required for reminders
                    </Badge>
                  )}
                </div>
                <Input
                  value={selectedDueDate}
                  onChange={(e) => handleDueDateChange(e.target.value)}
                  placeholder="e.g., tomorrow at 3pm, next Tuesday, in 2 days"
                  className={dueDateError ? 'border-destructive' : ''}
                />
                {dueDateError && (
                  <p className="text-xs text-destructive flex items-center gap-1">
                    <Warning size={12} /> {dueDateError}
                  </p>
                )}
                {selectedDueDate && !dueDateError && (
                  <p className="text-xs text-muted-foreground">
                    Natural language dates are supported (e.g., "tomorrow at 3pm")
                  </p>
                )}
              </div>
            )}

            {/* Context (for actions) */}
            {selectedType === 'action' && (
              <div className="space-y-3">
                <div className="flex items-center gap-2">
                  <MapPin size={16} weight="duotone" className="text-muted-foreground" />
                  <Label className="text-sm font-medium">Context (optional)</Label>
                </div>
                <Input
                  value={selectedContext}
                  onChange={(e) => setSelectedContext(e.target.value)}
                  placeholder="e.g., home, office, phone, computer"
                />
                <p className="text-xs text-muted-foreground">
                  Where or how should this be done?
                </p>
              </div>
            )}

            {/* Tags (for all types) */}
            <div className="space-y-3">
              <div className="flex items-center gap-2">
                <Tag size={16} weight="duotone" className="text-muted-foreground" />
                <Label className="text-sm font-medium">Tags (optional)</Label>
              </div>
              <Input
                value={selectedTags}
                onChange={(e) => setSelectedTags(e.target.value)}
                placeholder="e.g., urgent, meeting, personal (comma-separated)"
              />
              <p className="text-xs text-muted-foreground">
                Add keywords to help organize and find this item later
              </p>
            </div>

            {/* Confirm Button */}
            <Button
              onClick={handleConfirm}
              disabled={!isValid || dueDateError.length > 0}
              className="w-full"
              size="lg"
            >
              {!selectedType || !selectedCollection 
                ? 'Select type and collection to continue'
                : selectedType === 'reminder' && !selectedDueDate
                ? 'Reminders require a due date'
                : dueDateError.length > 0
                ? 'Fix date error to continue'
                : 'Confirm'}
            </Button>
          </div>
        </Card>
      </motion.div>
    </AnimatePresence>
  )
}
