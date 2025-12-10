import { useState, useEffect } from 'react'
import { useKV } from '@github/spark/hooks'
import { Capture, Item, InferredAttributes, LearningData } from '@/lib/types'
import { inferAttributes, getMissingFields } from '@/lib/inference'
import { getReviewQueue, getNextAction, getQuickWins } from '@/lib/dashboard'
import { CaptureInput } from '@/components/CaptureInput'
import { ReviewQueue } from '@/components/ReviewQueue'
import { Dashboard } from '@/components/Dashboard'
import { Separator } from '@/components/ui/separator'
import { toast } from 'sonner'
import { Toaster } from '@/components/ui/sonner'

function App() {
  const [captures, setCaptures] = useKV<Capture[]>('captures', [])
  const [items, setItems] = useKV<Item[]>('items', [])
  const [learningData, setLearningData] = useKV<LearningData[]>('learning', [])
  const [isProcessing, setIsProcessing] = useState(false)

  const capturesArray = captures || []
  const itemsArray = items || []
  const learningArray = learningData || []

  const handleCapture = async (text: string) => {
    setIsProcessing(true)
    
    const capture: Capture = {
      id: `capture-${Date.now()}-${Math.random()}`,
      text,
      createdAt: Date.now(),
      processed: false
    }
    
    setCaptures((current) => [...(current || []), capture])
    toast.success('Captured!')
    
    try {
      const inferred = await inferAttributes(text, learningArray)
      const missing = getMissingFields(inferred)
      
      if (missing.length === 0 && inferred.type && inferred.collection) {
        const newItem: Item = {
          id: `item-${Date.now()}-${Math.random()}`,
          type: inferred.type,
          text,
          collection: inferred.collection,
          priority: inferred.priority,
          dueDate: inferred.dueDate,
          context: inferred.context,
          tags: inferred.tags,
          completed: false,
          createdAt: Date.now()
        }
        
        setItems((current) => [...(current || []), newItem])
        setCaptures((current) => 
          (current || []).map(c => 
            c.id === capture.id 
              ? { ...c, processed: true, inferredAttributes: inferred, migratedTo: newItem.id }
              : c
          )
        )
        
        toast.success(`Organized as ${inferred.type} in ${inferred.collection}!`)
      } else {
        setCaptures((current) => 
          (current || []).map(c => 
            c.id === capture.id 
              ? { ...c, processed: true, inferredAttributes: inferred }
              : c
          )
        )
        toast.info('Needs review - some details unclear')
      }
    } catch (error) {
      console.error('Processing error:', error)
      toast.error('Processing failed, but capture is saved')
    } finally {
      setIsProcessing(false)
    }
  }

  const handleReview = (captureId: string, attributes: InferredAttributes) => {
    const capture = capturesArray.find(c => c.id === captureId)
    if (!capture) return
    
    if (attributes.type && attributes.collection) {
      const newItem: Item = {
        id: `item-${Date.now()}-${Math.random()}`,
        type: attributes.type,
        text: capture.text,
        collection: attributes.collection,
        priority: attributes.priority,
        dueDate: attributes.dueDate,
        context: attributes.context,
        tags: attributes.tags,
        completed: false,
        createdAt: capture.createdAt
      }
      
      setItems((current) => [...(current || []), newItem])
      setCaptures((current) => 
        (current || []).map(c => 
          c.id === captureId 
            ? { ...c, migratedTo: newItem.id }
            : c
        )
      )
      
      const learning: LearningData = {
        originalText: capture.text,
        inferredAttributes: capture.inferredAttributes || {},
        correctedAttributes: attributes,
        timestamp: Date.now()
      }
      setLearningData((current) => [...(current || []), learning])
      
      toast.success('Review complete!')
    }
  }

  const handleComplete = (itemId: string) => {
    setItems((current) => 
      (current || []).map(item => 
        item.id === itemId 
          ? { ...item, completed: true, completedAt: Date.now() }
          : item
      )
    )
    toast.success('Completed! 🎉')
  }

  const reviewQueue = getReviewQueue(capturesArray, itemsArray)
  const nextAction = getNextAction(itemsArray)
  const quickWins = getQuickWins(itemsArray)

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

        {reviewQueue.length > 0 && (
          <>
            <Separator />
            <ReviewQueue items={reviewQueue} onReview={handleReview} />
          </>
        )}

        <Separator />

        <Dashboard 
          nextAction={nextAction}
          quickWins={quickWins}
          onComplete={handleComplete}
        />
      </div>
    </div>
  )
}

export default App