import { useState } from 'react'
import { useKV } from '@github/spark/hooks'
import { Capture } from '@/lib/types'
import { CaptureInput } from '@/components/CaptureInput'
import { toast } from 'sonner'
import { Toaster } from '@/components/ui/sonner'

function App() {
  const [captures, setCaptures] = useKV<Capture[]>('captures', [])
  const [isProcessing, setIsProcessing] = useState(false)

  const capturesArray = captures || []

  const handleCapture = async (text: string) => {
    setIsProcessing(true)
    
    const capture: Capture = {
      id: `capture-${Date.now()}-${Math.random()}`,
      text,
      createdAt: Date.now()
    }
    
    setCaptures((current) => [...(current || []), capture])
    toast.success('Captured!')
    
    setIsProcessing(false)
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

        {capturesArray.length > 0 && (
          <div className="text-center text-sm text-muted-foreground">
            {capturesArray.length} {capturesArray.length === 1 ? 'capture' : 'captures'} saved
          </div>
        )}
      </div>
    </div>
  )
}

export default App