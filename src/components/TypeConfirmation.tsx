import { ItemType } from '@/lib/types'
import { getTypeLabel, getTypeDescription } from '@/lib/typeInference'
import { Card } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Check, X, Note, ListChecks, Bell } from '@phosphor-icons/react'
import { motion, AnimatePresence } from 'framer-motion'

interface TypeConfirmationProps {
  itemId: string
  text: string
  inferredType: ItemType | null
  confidence: number
  reasoning?: string
  onConfirm: (itemId: string, confirmedType: ItemType) => void
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
  onConfirm,
  onDismiss,
}: TypeConfirmationProps) {
  const types: ItemType[] = ['note', 'action', 'reminder']
  
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

  return (
    <AnimatePresence>
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        exit={{ opacity: 0, y: -20 }}
        transition={{ duration: 0.2 }}
      >
        <Card className="p-6 border-2 border-accent/40 bg-card shadow-lg">
          <div className="space-y-4">
            <div className="flex items-start justify-between gap-4">
              <div className="flex-1 space-y-2">
                <div className="flex items-center gap-2">
                  <h3 className="text-lg font-semibold text-foreground">
                    {inferredType ? 'Confirm Type' : 'What type is this?'}
                  </h3>
                  <Badge variant="outline" className={getConfidenceBadgeColor(confidence)}>
                    {confidence}% {getConfidenceLabel(confidence)}
                  </Badge>
                </div>
                <p className="text-sm text-muted-foreground line-clamp-2">
                  "{text}"
                </p>
                {reasoning && confidence > 0 && (
                  <p className="text-xs text-muted-foreground/80 italic">
                    {reasoning}
                  </p>
                )}
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

            {inferredType && (
              <div className="p-3 bg-muted/50 rounded-md border border-border">
                <div className="flex items-center gap-2 mb-1">
                  {(() => {
                    const Icon = TYPE_ICONS[inferredType]
                    return <Icon className="text-primary" weight="duotone" />
                  })()}
                  <span className="font-medium text-foreground">
                    {getTypeLabel(inferredType)}
                  </span>
                </div>
                <p className="text-sm text-muted-foreground ml-6">
                  {getTypeDescription(inferredType)}
                </p>
              </div>
            )}

            <div className="space-y-2">
              {!inferredType && (
                <p className="text-sm text-muted-foreground mb-3">
                  Choose the type that best fits:
                </p>
              )}
              
              <div className="grid gap-2">
                {types.map((type) => {
                  const Icon = TYPE_ICONS[type]
                  const isInferred = type === inferredType
                  
                  return (
                    <Button
                      key={type}
                      variant={isInferred ? 'default' : 'outline'}
                      className={`justify-start h-auto py-3 px-4 ${
                        isInferred 
                          ? 'bg-primary hover:bg-primary/90 text-primary-foreground' 
                          : 'hover:bg-accent hover:text-accent-foreground'
                      }`}
                      onClick={() => onConfirm(itemId, type)}
                    >
                      <div className="flex items-center gap-3 w-full">
                        <Icon 
                          size={20} 
                          weight="duotone"
                          className={isInferred ? 'text-primary-foreground' : 'text-muted-foreground'}
                        />
                        <div className="flex-1 text-left">
                          <div className="flex items-center gap-2">
                            <span className="font-medium">{getTypeLabel(type)}</span>
                            {isInferred && (
                              <Check size={16} weight="bold" />
                            )}
                          </div>
                          <span className={`text-xs ${
                            isInferred ? 'text-primary-foreground/80' : 'text-muted-foreground'
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
          </div>
        </Card>
      </motion.div>
    </AnimatePresence>
  )
}
