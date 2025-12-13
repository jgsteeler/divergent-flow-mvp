import { useState } from 'react'
import { useKV } from '@github/spark/hooks'
import { Item, ItemType, TypeLearningData } from '@/lib/types'
import { CaptureInput } from '@/components/CaptureInput'
import { TypeConfirmation } from '@/components/TypeConfirmation'
import { ReviewQueue } from '@/components/ReviewQueue'
import { toast } from 'sonner'
import { Toaster } from '@/components/ui/sonner'
import { inferType, saveTypeLearning } from '@/lib/typeInference'
import { getTopReviewItems } from '@/lib/reviewPriority'

function App() {
  const [items, setItems] = useKV<Item[]>('items', [])
  const [typeLearning, setTypeLearning] = useKV<TypeLearningData[]>('type-learning', [])
  const [isProcessing, setIsProcessing] = useState(false)
  const [pendingConfirmation, setPendingConfirmation] = useState<Item | null>(null)

  const itemsArray = items || []
  const learningArray = typeLearning || []

  const reviewItems = getTopReviewItems(itemsArray, 3)

  const handleCapture = async (text: string) => {
    setIsProcessing(true)
    
    const item: Item = {
      id: `item-${Date.now()}-${Math.random()}`,
      text,
      createdAt: Date.now(),
      migratedCapture: false
    }
    
    setItems((current) => [...(current || []), item])
    toast.success('Captured!')
    
    setTimeout(async () => {
      await processItem(item)
      setIsProcessing(false)
    }, 100)
  }

  const processItem = async (item: Item) => {
    const { type, confidence, reasoning } = inferType(item.text, learningArray)
    
    const updatedItem: Item = {
      ...item,
      inferredType: type || undefined,
      typeConfidence: confidence,
      confidenceReasoning: reasoning
    }
    
    setItems((current) => 
      (current || []).map(i => i.id === item.id ? updatedItem : i)
    )

    setPendingConfirmation(updatedItem)
  }

  const handleTypeConfirm = async (itemId: string, confirmedType: ItemType) => {
    const item = itemsArray.find(i => i.id === itemId)
    if (!item) return

    const updatedItem: Item = {
      ...item,
      inferredType: confirmedType,
      typeConfidence: 100,
      lastReviewedAt: Date.now()
    }

    setItems((current) =>
      (current || []).map(i => i.id === itemId ? updatedItem : i)
    )

    await saveTypeLearning(
      item.text,
      item.inferredType || null,
      confirmedType,
      item.typeConfidence || 0
    )
    
    setTypeLearning((current) => [...(current || [])])
    setPendingConfirmation(null)
    
    const wasCorrect = item.inferredType === confirmedType
    if (wasCorrect) {
      toast.success(`Confirmed as ${confirmedType}! I'm learning.`)
    } else {
      toast.success(`Updated to ${confirmedType}. I'll remember that!`)
    }
  }

  const handleDismiss = (itemId: string) => {
    setPendingConfirmation(null)
  }

  const handleReviewItem = (itemId: string) => {
    const item = itemsArray.find(i => i.id === itemId)
    if (item) {
      setPendingConfirmation(item)
    }
  }

  return (
    <div className="min-h-screen bg-background p-4 md:p-8">
      <Toaster />
      <div className="max-w-4xl mx-auto space-y-8">
        <header className="text-center space-y-2">
          <h1 className="text-3xl md:text-4xl font-bold text-primary">
            Divergent Flow
          </h1>
          <p className="text-muted-foreground">
            Your ADHD-friendly external brain
          </p>
        </header>

        <CaptureInput onCapture={handleCapture} isProcessing={isProcessing} />

        {pendingConfirmation && (
          <TypeConfirmation
            itemId={pendingConfirmation.id}
            text={pendingConfirmation.text}
            inferredType={pendingConfirmation.inferredType || null}
            confidence={pendingConfirmation.typeConfidence || 0}
            reasoning={pendingConfirmation.confidenceReasoning}
            onConfirm={handleTypeConfirm}
            onDismiss={handleDismiss}
          />
        )}

        {!pendingConfirmation && reviewItems.length > 0 && (
          <ReviewQueue
            items={reviewItems}
            onReviewItem={handleReviewItem}
          />
        )}

        {itemsArray.length > 0 && (
          <div className="text-center space-y-1">
            <div className="text-sm text-muted-foreground">
              {itemsArray.length} {itemsArray.length === 1 ? 'item' : 'items'} saved
            </div>
            {learningArray.length > 0 && (
              <div className="text-xs text-muted-foreground/70">
                Learning from {learningArray.length} {learningArray.length === 1 ? 'pattern' : 'patterns'}
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  )
}

export default App