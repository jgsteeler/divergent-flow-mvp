import { Item } from '@/lib/types'
import { Card } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { CheckCircle, Tag, Calendar } from '@phosphor-icons/react'
import { motion } from 'framer-motion'
import { cn } from '@/lib/utils'
import { formatDate } from '@/lib/dateParser'

interface ItemCardProps {
  item: Item
  onComplete?: (id: string) => void
  showComplete?: boolean
}

export function ItemCard({ item, onComplete, showComplete = true }: ItemCardProps) {
  const handleComplete = () => {
    if (onComplete) {
      onComplete(item.id)
    }
  }

  const priorityColors = {
    high: 'bg-destructive text-destructive-foreground',
    medium: 'bg-accent text-accent-foreground',
    low: 'bg-secondary text-secondary-foreground'
  }

  const typeColors = {
    action: 'bg-primary text-primary-foreground',
    reminder: 'bg-accent text-accent-foreground',
    note: 'bg-secondary text-secondary-foreground'
  }

  const isOverdue = item.dueDate && item.dueDate < Date.now() && !item.completed
  const dueDateDisplay = item.dueDate ? formatDate(item.dueDate) : null

  return (
    <motion.div
      initial={{ opacity: 0, scale: 0.95 }}
      animate={{ opacity: 1, scale: 1 }}
      exit={{ opacity: 0, scale: 0.95, x: -100 }}
      transition={{ duration: 0.2 }}
    >
      <Card className={cn(
        "p-4 space-y-3 transition-all hover:shadow-md",
        item.completed && "opacity-60"
      )}>
        <div className="flex items-start justify-between gap-3">
          <div className="flex-1 space-y-2">
            <div className="flex items-center gap-2 flex-wrap">
              <Badge className={typeColors[item.type]}>
                {item.type}
              </Badge>
              {item.priority && (
                <Badge className={priorityColors[item.priority]}>
                  {item.priority}
                </Badge>
              )}
              {item.collection && (
                <Badge variant="outline" className="gap-1">
                  <Tag size={14} />
                  {item.collection}
                </Badge>
              )}
            </div>
            
            <p className="text-base leading-relaxed">{item.text}</p>
            
            {(item.dueDate || item.context || item.tags) && (
              <div className="flex items-center gap-3 text-sm text-muted-foreground font-mono">
                {item.dueDate && (
                  <span className={cn(
                    "flex items-center gap-1",
                    isOverdue && "text-destructive font-semibold"
                  )}>
                    <Calendar size={14} />
                    {dueDateDisplay}
                  </span>
                )}
                {item.context && (
                  <span>@{item.context}</span>
                )}
                {item.tags && item.tags.length > 0 && (
                  <span>#{item.tags.join(' #')}</span>
                )}
              </div>
            )}
          </div>

          {showComplete && !item.completed && (item.type === 'action' || item.type === 'reminder') && (
            <Button
              size="sm"
              variant="ghost"
              onClick={handleComplete}
              className="flex-shrink-0"
            >
              <CheckCircle size={20} />
            </Button>
          )}
        </div>
      </Card>
    </motion.div>
  )
}
