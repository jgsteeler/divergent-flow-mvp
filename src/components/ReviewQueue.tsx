import { ReviewItem, ItemType } from '@/lib/types'
import { Card } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Stack, Warning, CheckCircle, Note, ListChecks, Bell } from '@phosphor-icons/react'
import { motion } from 'framer-motion'
import { getTypeLabel } from '@/lib/typeInference'

interface ReviewQueueProps {
  items: ReviewItem[]
  onReviewItem: (captureId: string) => void
}

const TYPE_ICONS = {
  note: Note,
  action: ListChecks,
  reminder: Bell,
}

export function ReviewQueue({ items, onReviewItem }: ReviewQueueProps) {
  if (items.length === 0) return null

  const getPriorityColor = (priority: number) => {
    if (priority >= 900) return 'bg-destructive/10 text-destructive border-destructive/20'
    if (priority >= 800) return 'bg-orange-500/10 text-orange-600 border-orange-500/20'
    if (priority >= 700) return 'bg-accent/10 text-accent-foreground border-accent/20'
    return 'bg-muted text-muted-foreground border-border'
  }

  const getPriorityIcon = (priority: number) => {
    if (priority >= 900) return Warning
    if (priority >= 700) return CheckCircle
    return Stack
  }

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      className="w-full"
    >
      <Card className="p-6 space-y-4 bg-accent/5 border-2 border-accent/30">
        <div className="flex items-center justify-between gap-2">
          <div className="flex items-center gap-2 text-foreground">
            <Stack size={24} weight="duotone" className="text-accent" />
            <h3 className="text-lg font-semibold">Review Queue</h3>
            <Badge variant="outline" className="bg-accent/20 text-accent-foreground border-accent/30">
              {items.length} {items.length === 1 ? 'item' : 'items'}
            </Badge>
          </div>
        </div>

        <div className="space-y-3">
          {items.map((reviewItem, index) => {
            const PriorityIcon = getPriorityIcon(reviewItem.reviewPriority)
            const TypeIcon = reviewItem.item.inferredType ? TYPE_ICONS[reviewItem.item.inferredType] : null

            return (
              <motion.div
                key={reviewItem.item.id}
                initial={{ opacity: 0, x: -20 }}
                animate={{ opacity: 1, x: 0 }}
                transition={{ delay: index * 0.05 }}
              >
                <Card className="p-4 bg-card hover:bg-accent/5 transition-colors border border-border">
                  <div className="flex gap-4">
                    <div className="flex-shrink-0 pt-1">
                      <PriorityIcon 
                        size={20} 
                        weight="duotone"
                        className={reviewItem.reviewPriority >= 900 ? 'text-destructive' : reviewItem.reviewPriority >= 800 ? 'text-orange-500' : 'text-muted-foreground'}
                      />
                    </div>
                    
                    <div className="flex-1 space-y-2">
                      <p className="text-sm text-foreground line-clamp-2 leading-relaxed">
                        {reviewItem.item.text}
                      </p>
                      
                      <div className="flex flex-wrap items-center gap-2">
                        <Badge 
                          variant="outline" 
                          className={`text-xs ${getPriorityColor(reviewItem.reviewPriority)}`}
                        >
                          {reviewItem.reason}
                        </Badge>
                        
                        {TypeIcon && reviewItem.item.inferredType && (
                          <Badge variant="outline" className="text-xs bg-primary/10 text-primary border-primary/20">
                            <TypeIcon size={12} weight="duotone" className="mr-1" />
                            {getTypeLabel(reviewItem.item.inferredType)}
                          </Badge>
                        )}
                        
                        {reviewItem.item.typeConfidence !== undefined && (
                          <Badge variant="outline" className="text-xs bg-muted text-muted-foreground">
                            {reviewItem.item.typeConfidence}%
                          </Badge>
                        )}
                      </div>
                    </div>

                    <div className="flex-shrink-0">
                      <Button
                        size="sm"
                        variant="ghost"
                        onClick={() => onReviewItem(reviewItem.item.id)}
                        className="h-8 text-primary hover:text-primary hover:bg-primary/10"
                      >
                        Review
                      </Button>
                    </div>
                  </div>
                </Card>
              </motion.div>
            )
          })}
        </div>
      </Card>
    </motion.div>
  )
}
