import { useState, useEffect } from 'react'
import { useKV } from '@github/spark/hooks'
import { Capture, ItemType, TypeLearningData } from '@/lib/types'
import { CaptureInput } from '@/components/CaptureInput'
import { TypeConfirmation } from '@/components/TypeConfirmation'
import { ReviewQueue } from '@/components/ReviewQueue'
import { toast } from 'sonner'
import { Toaster } from '@/components/ui/sonner'
import { inferType, shouldPromptUser, saveTypeLearning } from '@/lib/typeInference'
import { getTopReviewItems } from '@/lib/reviewPriority'

function App() {
  const [captures, setCaptures] = useKV<Capture[]>('captures', [])
  const [typeLearning, setTypeLearning] = useKV<TypeLearningData[]>('type-learning', [])
  const [isProcessing, setIsProcessing] = useState(false)
  const [pendingConfirmation, setPendingConfirmation] = useState<Capture | null>(null)

  const capturesArray = captures || []
  const learningArray = typeLearning || []

  const reviewItems = getTopReviewItems(capturesArray, 5)

  const handleCapture = async (text: string) => {
    setIsProcessing(true)
    
    const capture: Capture = {
      id: `capture-${Date.now()}-${Math.random()}`,
      text,
      createdAt: Date.now()
    }
    
    setCaptures((current) => [...(current || []), capture])
    toast.success('Captured!')
    
    setTimeout(async () => {
      await processCapture(capture)
      setIsProcessing(false)
    }, 100)
  }

  const processCapture = async (capture: Capture) => {
    const { type, confidence } = inferType(capture.text, learningArray)
    
    const updatedCapture: Capture = {
      ...capture,
      inferredType: type || undefined,
      typeConfidence: confidence,
      needsTypeConfirmation: !type || shouldPromptUser(confidence),
      processedAt: Date.now()
    }
    
    setCaptures((current) => 
      (current || []).map(c => c.id === capture.id ? updatedCapture : c)
    )

    if (updatedCapture.needsTypeConfirmation) {
      setPendingConfirmation(updatedCapture)
    } else if (type) {
      await saveTypeLearning(capture.text, type, type, confidence)
      setTypeLearning((current) => [...(current || [])])
      toast.success(`Saved as ${type}!`)
    }
  }

  const handleTypeConfirm = async (captureId: string, confirmedType: ItemType) => {
    const capture = capturesArray.find(c => c.id === captureId)
    if (!capture) return

    const updatedCapture: Capture = {
      ...capture,
      inferredType: confirmedType,
      typeConfidence: 100,
      needsTypeConfirmation: false,
      lastReviewedAt: Date.now()
    }

    setCaptures((current) =>
      (current || []).map(c => c.id === captureId ? updatedCapture : c)
    )

    await saveTypeLearning(
      capture.text,
      capture.inferredType || null,
      confirmedType,
      capture.typeConfidence || 0
    )
    
    setTypeLearning((current) => [...(current || [])])
    setPendingConfirmation(null)
    
    const wasCorrect = capture.inferredType === confirmedType
    if (wasCorrect) {
      toast.success(`Confirmed as ${confirmedType}! I'm learning.`)
    } else {
      toast.success(`Updated to ${confirmedType}. I'll remember that!`)
    }
  }

  const handleDismiss = (captureId: string) => {
    setPendingConfirmation(null)
  }

  const handleReviewItem = (captureId: string) => {
    const capture = capturesArray.find(c => c.id === captureId)
    if (capture) {
      setPendingConfirmation(capture)
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
            captureId={pendingConfirmation.id}
            text={pendingConfirmation.text}
            inferredType={pendingConfirmation.inferredType || null}
            confidence={pendingConfirmation.typeConfidence || 0}
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

        {capturesArray.length > 0 && (
          <div className="text-center space-y-1">
            <div className="text-sm text-muted-foreground">
              {capturesArray.length} {capturesArray.length === 1 ? 'capture' : 'captures'} saved
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