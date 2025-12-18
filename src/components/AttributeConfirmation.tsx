import { useState } from 'react'
import { ItemType, Priority, Item, LearningData } from '@/lib/types'
import { getTypeLabel, getTypeDescription } from '@/lib/typeInference'
import { getLearnedCollections } from '@/lib/collectionInference'
import { formatDate } from '@/lib/dateParser'
import { HIGH_CONFIDENCE_THRESHOLD, MEDIUM_CONFIDENCE_THRESHOLD, CONFIRMED_CONFIDENCE } from '@/lib/constants'
import { Card } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Check, X, Note, ListChecks, Bell } from '@phosphor-icons/react'
import { motion, AnimatePresence } from 'framer-motion'

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
  const [customCollection, setCustomCollection] = useState<string>('')
  const [selectedPriority, setSelectedPriority] = useState<Priority | undefined>(item.priority)

  const types: ItemType[] = ['note', 'action', 'reminder']
  const learnedCollections = getLearnedCollections(learningData)
  const priorities: Priority[] = ['low', 'medium', 'high']

  const getConfidenceBadgeColor = (conf: number) => {
    if (conf >= HIGH_CONFIDENCE_THRESHOLD) return 'bg-primary/10 text-primary border-primary/20'
    if (conf >= MEDIUM_CONFIDENCE_THRESHOLD) return 'bg-accent/10 text-accent-foreground border-accent/20'
    return 'bg-muted text-muted-foreground border-border'
  }

  const getConfidenceLabel = (conf: number) => {
    if (conf >= HIGH_CONFIDENCE_THRESHOLD) return 'High'
    if (conf >= MEDIUM_CONFIDENCE_THRESHOLD) return 'Medium'
    return 'Low'
  }

  const handleConfirm = () => {
    const updates: Partial<Item> = {
      inferredType: selectedType || undefined,
      typeConfidence: CONFIRMED_CONFIDENCE,
      collection: selectedCollection || customCollection || undefined,
      collectionConfidence: CONFIRMED_CONFIDENCE,
      priority: selectedPriority,
      lastReviewedAt: Date.now(),
    }
    onConfirm(item.id, updates)
  }

  const isValid = selectedType && (selectedCollection || customCollection)

  return (
    <AnimatePresence>
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        exit={{ opacity: 0, y: -20 }}
        transition={{ duration: 0.2 }}
      >
        <Card className="p-6 border-2 border-accent/40 bg-card shadow-lg">
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
              {learnedCollections.length > 0 && (
                <div className="grid grid-cols-2 gap-2">
                  {learnedCollections.map((collection) => {
                    const isSelected = collection === selectedCollection
                    
                    return (
                      <Button
                        key={collection}
                        variant={isSelected ? 'default' : 'outline'}
                        className={`justify-center ${
                          isSelected 
                            ? 'bg-primary hover:bg-primary/90 text-primary-foreground' 
                            : 'hover:bg-accent hover:text-accent-foreground'
                        }`}
                        onClick={() => {
                          setSelectedCollection(collection)
                          setCustomCollection('')
                        }}
                      >
                        {collection}
                        {isSelected && <Check size={16} weight="bold" className="ml-2" />}
                      </Button>
                    )
                  })}
                </div>
              )}
              <div className="space-y-2">
                <Label htmlFor="custom-collection" className="text-xs text-muted-foreground">
                  {learnedCollections.length > 0 ? 'Or create a new collection:' : 'Create a collection:'}
                </Label>
                <Input
                  id="custom-collection"
                  placeholder="e.g., Work, Personal, Health..."
                  value={customCollection}
                  onChange={(e) => {
                    setCustomCollection(e.target.value)
                    if (e.target.value) {
                      setSelectedCollection('')
                    }
                  }}
                />
              </div>
            </div>

            {/* Priority Selection (for actions) */}
            {selectedType === 'action' && (
              <div className="space-y-3">
                <Label className="text-sm font-medium">Priority</Label>
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
              </div>
            )}

            {/* Show extracted date/time if present */}
            {item.dueDate && (
              <div className="p-3 bg-muted/50 rounded-md border border-border">
                <div className="flex items-center gap-2">
                  <Label className="text-sm font-medium">Due Date:</Label>
                  <span className="text-sm text-foreground">{formatDate(item.dueDate)}</span>
                </div>
              </div>
            )}

            {/* Confirm Button */}
            <Button
              onClick={handleConfirm}
              disabled={!isValid}
              className="w-full"
              size="lg"
            >
              {isValid ? 'Confirm' : 'Select type and collection to continue'}
            </Button>
          </div>
        </Card>
      </motion.div>
    </AnimatePresence>
  )
}
