import { Item } from '@/lib/types'
import { ItemCard } from './ItemCard'
import { Card } from '@/components/ui/card'
import { ArrowRight, Lightning } from '@phosphor-icons/react'
import { Separator } from '@/components/ui/separator'

interface DashboardProps {
  nextAction: Item | null
  quickWins: Item[]
  onComplete: (id: string) => void
}

export function Dashboard({ nextAction, quickWins, onComplete }: DashboardProps) {
  return (
    <div className="w-full space-y-6">
      {nextAction && (
        <div className="space-y-3">
          <div className="flex items-center gap-2 text-primary">
            <ArrowRight size={24} weight="bold" />
            <h2 className="text-xl font-semibold">Next Action</h2>
          </div>
          <ItemCard item={nextAction} onComplete={onComplete} />
        </div>
      )}

      {quickWins.length > 0 && (
        <>
          <Separator />
          <div className="space-y-3">
            <div className="flex items-center gap-2 text-accent">
              <Lightning size={24} weight="fill" />
              <h2 className="text-xl font-semibold">Quick Wins</h2>
            </div>
            <div className="space-y-3">
              {quickWins.map((item) => (
                <ItemCard key={item.id} item={item} onComplete={onComplete} />
              ))}
            </div>
          </div>
        </>
      )}

      {!nextAction && quickWins.length === 0 && (
        <Card className="p-12 text-center">
          <div className="space-y-3">
            <div className="text-6xl">🎉</div>
            <h3 className="text-2xl font-semibold text-muted-foreground">All Clear!</h3>
            <p className="text-muted-foreground">
              No actions pending. Capture something new or take a well-deserved break.
            </p>
          </div>
        </Card>
      )}
    </div>
  )
}
