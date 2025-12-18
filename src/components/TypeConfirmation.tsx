import { useState } from 'react'
import { ItemType, Priority, Estimate, Item } from '@/lib/types'
import { getTypeLabel, getTypeDescription } from '@/lib/typeInference'
import { getPriorityLabel, getPriorityDescription, getEstimateLabel, getEstimateDescription } from '@/lib/priorityEstimateInference'
import { Card } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Check, X, Note, ListChecks, Bell, Warning, Clock } from '@phosphor-icons/react'
import { motion, AnimatePresence } from 'framer-motion'

interface TypeConfirmationProps {
  itemId: string
  text: string
  inferredType: ItemType | null
  confidence: number
  reasoning?: string
  priority?: Priority
  priorityConfidence?: number
  estimate?: Estimate
  estimateConfidence?: number
  onConfirm: (itemId: string, confirmedType: ItemType, confirmedPriority?: Priority, confirmedEstimate?: Estimate) => void
  onDismiss: (itemId: string) => void
}

const TYPE_ICONS = {
  note: Note,
  action: ListChecks,
  reminder: Bell,
}

export function TypeConfirmation({
  itemId,
  text,
  inferredType,
  confidence,
  reasoning,
  priority,
  priorityConfidence,
  estimate,
  estimateConfidence,
  onConfirm,
  onDismiss,
}: TypeConfirmationProps) {
  const [selectedType, setSelectedType] = useState<ItemType | null>(inferredType)
  const [selectedPriority, setSelectedPriority] = useState<Priority | undefined>(priority)
  const [selectedEstimate, setSelectedEstimate] = useState<Estimate | undefined>(estimate)

  const types: ItemType[] = ['note', 'action', 'reminder']
  const priorities: Priority[] = ['low', 'medium', 'high']
  const estimates: Estimate[] = ['5min', '15min', '30min', '1hour', '2hours', 'halfday', 'day', 'multiday']
  
  const getConfidenceBadgeColor = (conf: number) => {
    if (conf >= 90) return 'bg-primary/10 text-primary border-primary/20'
    if (conf >= 70) return 'bg-accent/10 text-accent-foreground border-accent/20'
    return 'bg-muted text-muted-foreground border-border'
  }

  const getConfidenceLabel = (conf: number) => {
    if (conf >= 90) return 'High'
    if (conf >= 70) return 'Medium'
    return 'Low'
  }

  const handleConfirm = () => {
    if (!selectedType) return
    onConfirm(itemId, selectedType, selectedPriority, selectedEstimate)
  }

  const showPrioritySection = selectedType === 'action' || selectedType === 'reminder'
  const showEstimateSection = selectedType === 'action'

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
            {/* Header */}
            <div className="flex items-start justify-between gap-4">
              <div className="flex-1 space-y-2">
                <div className="flex items-center gap-2">
                  <h3 className="text-lg font-semibold text-foreground">
                    Confirm Item Details
                  </h3>
                </div>
                <p className="text-sm text-muted-foreground line-clamp-2">
                  "{text}"
                </p>
              </div>
              <Button
                variant="ghost"
                size="icon"
                onClick={() => onDismiss(itemId)}
                className="h-8 w-8 text-muted-foreground hover:text-foreground"
              >
                <X />
              </Button>
            </div>

            {/* Type Selection */}
            <div className="space-y-3">
              <div className="flex items-center gap-2">
                <h4 className="text-sm font-medium text-foreground">Type</h4>
                {confidence > 0 && (
                  <Badge variant="outline" className={getConfidenceBadgeColor(confidence)}>
                    {confidence}% {getConfidenceLabel(confidence)}
                  </Badge>
                )}
              </div>
              {reasoning && confidence > 0 && (
                <p className="text-xs text-muted-foreground/80 italic">
                  {reasoning}
                </p>
              )}
              
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

            {/* Priority Selection */}
            {showPrioritySection && (
              <div className="space-y-3">
                <div className="flex items-center gap-2">
                  <Warning size={18} className="text-muted-foreground" />
                  <h4 className="text-sm font-medium text-foreground">Priority</h4>
                  {priorityConfidence !== undefined && priorityConfidence > 0 && (
                    <Badge variant="outline" className={getConfidenceBadgeColor(priorityConfidence)}>
                      {priorityConfidence}% {getConfidenceLabel(priorityConfidence)}
                    </Badge>
                  )}
                </div>
                
                <div className="grid grid-cols-3 gap-2">
                  {priorities.map((pri) => {
                    const isSelected = pri === selectedPriority
                    const color = pri === 'high' ? 'text-red-600' : pri === 'medium' ? 'text-yellow-600' : 'text-green-600'
                    
                    return (
                      <Button
                        key={pri}
                        variant={isSelected ? 'default' : 'outline'}
                        size="sm"
                        className={`${isSelected ? 'bg-primary text-primary-foreground' : ''}`}
                        onClick={() => setSelectedPriority(pri)}
                      >
                        <div className="flex flex-col items-center gap-1">
                          <span className={`text-xs font-medium ${isSelected ? '' : color}`}>
                            {getPriorityLabel(pri)}
                          </span>
                          {isSelected && <Check size={14} weight="bold" />}
                        </div>
                      </Button>
                    )
                  })}
                </div>
              </div>
            )}

            {/* Estimate Selection */}
            {showEstimateSection && (
              <div className="space-y-3">
                <div className="flex items-center gap-2">
                  <Clock size={18} className="text-muted-foreground" />
                  <h4 className="text-sm font-medium text-foreground">Time Estimate</h4>
                  {estimateConfidence !== undefined && estimateConfidence > 0 && (
                    <Badge variant="outline" className={getConfidenceBadgeColor(estimateConfidence)}>
                      {estimateConfidence}% {getConfidenceLabel(estimateConfidence)}
                    </Badge>
                  )}
                </div>
                
                <div className="grid grid-cols-4 gap-2">
                  {estimates.map((est) => {
                    const isSelected = est === selectedEstimate
                    
                    return (
                      <Button
                        key={est}
                        variant={isSelected ? 'default' : 'outline'}
                        size="sm"
                        className={`${isSelected ? 'bg-primary text-primary-foreground' : ''} text-xs`}
                        onClick={() => setSelectedEstimate(est)}
                      >
                        <div className="flex flex-col items-center gap-1">
                          <span className="font-medium">
                            {getEstimateLabel(est)}
                          </span>
                          {isSelected && <Check size={12} weight="bold" />}
                        </div>
                      </Button>
                    )
                  })}
                </div>
              </div>
            )}

            {/* Confirm Button */}
            <Button
              onClick={handleConfirm}
              disabled={!selectedType}
              className="w-full"
              size="lg"
            >
              <Check size={20} weight="bold" className="mr-2" />
              Confirm
            </Button>
          </div>
        </Card>
      </motion.div>
    </AnimatePresence>
  )
}
