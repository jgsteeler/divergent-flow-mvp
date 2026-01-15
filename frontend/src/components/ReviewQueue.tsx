import { Button } from '@/components/ui/button'
import { Card } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { ArrowLeft, Check, X } from '@phosphor-icons/react'
import { motion, AnimatePresence } from 'framer-motion'
import { useReviewQueue, useMarkItemReviewed } from '@/hooks/useItems'
import { Item } from '@/lib/types'
import { toast } from 'sonner'

interface ReviewQueueProps {
  onBack: () => void
}

export function ReviewQueue({ onBack }: ReviewQueueProps) {
  const { data: items = [], isLoading, error } = useReviewQueue()
  const markReviewedMutation = useMarkItemReviewed()
  
  const handleConfirm = (item: Item) => {
    markReviewedMutation.mutate(
      { 
        id: item.id,
        request: {
          confirmedType: item.inferredType || undefined,
          confirmedConfidence: item.typeConfidence || undefined
        }
      },
      {
        onSuccess: () => {
          toast.success('Item reviewed!')
        },
        onError: (error) => {
          toast.error(`Failed to review item: ${error.message}`)
        },
      }
    )
  }
  
  const handleDefer = (item: Item) => {
    // Just mark as reviewed with lower confidence to defer it
    markReviewedMutation.mutate(
      { 
        id: item.id,
        request: {
          confirmedType: item.inferredType || undefined,
          confirmedConfidence: 0.5 // Lower confidence so it might appear again
        }
      },
      {
        onSuccess: () => {
          toast.info('Item deferred')
        },
        onError: (error) => {
          toast.error(`Failed to defer item: ${error.message}`)
        },
      }
    )
  }

  return (
    <motion.div
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      className="w-full space-y-6"
    >
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold">Review Queue</h2>
          <p className="text-sm text-muted-foreground mt-1">
            Review and confirm your captured items
          </p>
        </div>
        <Button
          onClick={onBack}
          variant="outline"
          className="flex items-center gap-2"
        >
          <ArrowLeft />
          Back
        </Button>
      </div>

      {isLoading && (
        <div className="text-center py-12 text-muted-foreground">
          <p>Loading items to review...</p>
        </div>
      )}

      {error && (
        <div className="text-center py-12 text-destructive">
          <p>Failed to load review queue: {error.message}</p>
        </div>
      )}

      {!isLoading && !error && items.length === 0 && (
        <div className="text-center py-12 text-muted-foreground">
          <div className="text-4xl mb-4">🎉</div>
          <p className="text-lg font-medium">You're all caught up!</p>
          <p className="text-sm mt-2">No items need review right now.</p>
        </div>
      )}

      <AnimatePresence mode="popLayout">
        {!isLoading && !error && items.length > 0 && (
          <div className="space-y-4">
            {items.map((item, index) => (
              <motion.div
                key={item.id}
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                exit={{ opacity: 0, x: -100 }}
                transition={{ delay: index * 0.1 }}
              >
                <Card className="p-6 space-y-4">
                  <div className="space-y-2">
                    <div className="flex items-start justify-between gap-4">
                      <p className="text-base leading-relaxed flex-1">
                        {item.text}
                      </p>
                      {item.inferredType && (
                        <Badge variant="outline" className="shrink-0">
                          <span className="capitalize">{item.inferredType}</span>
                          {item.typeConfidence !== null && item.typeConfidence !== undefined && (
                            <span className="ml-2 text-xs opacity-75">
                              {Math.round(item.typeConfidence)}%
                            </span>
                          )}
                        </Badge>
                      )}
                    </div>
                    <time 
                      className="text-xs text-muted-foreground"
                      dateTime={new Date(item.createdAt).toISOString()}
                    >
                      Captured {new Date(item.createdAt).toLocaleDateString()}
                    </time>
                  </div>

                  <div className="flex gap-2 justify-end">
                    <Button
                      onClick={() => handleDefer(item)}
                      variant="outline"
                      size="sm"
                      disabled={markReviewedMutation.isPending}
                      className="flex items-center gap-2"
                    >
                      <X size={16} />
                      Defer
                    </Button>
                    <Button
                      onClick={() => handleConfirm(item)}
                      size="sm"
                      disabled={markReviewedMutation.isPending}
                      className="flex items-center gap-2 bg-accent hover:bg-accent/90"
                    >
                      <Check size={16} />
                      Confirm
                    </Button>
                  </div>
                </Card>
              </motion.div>
            ))}
          </div>
        )}
      </AnimatePresence>
    </motion.div>
  )
}
