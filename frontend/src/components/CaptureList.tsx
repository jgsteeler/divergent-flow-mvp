import { Capture } from '@/lib/types'
import { Button } from '@/components/ui/button'
import { ArrowLeft } from '@phosphor-icons/react'
import { motion } from 'framer-motion'

interface CaptureListProps {
  captures: Capture[]
  onBack: () => void
  isLoading?: boolean
  error?: Error | null
}

export function CaptureList({ captures, onBack, isLoading, error }: CaptureListProps) {
  return (
    <motion.div
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      className="w-full space-y-4"
    >
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-xl font-semibold">Your Captures</h2>
        <Button
          onClick={onBack}
          variant="outline"
          className="flex items-center gap-2"
        >
          <ArrowLeft />
          Back to Capture
        </Button>
      </div>

      {isLoading && (
        <div className="text-center py-12 text-muted-foreground">
          <p>Loading captures...</p>
        </div>
      )}

      {error && (
        <div className="text-center py-12 text-destructive">
          <p>Failed to load captures: {error.message}</p>
        </div>
      )}

      {!isLoading && !error && captures.length === 0 && (
        <div className="text-center py-12 text-muted-foreground">
          <p>No captures yet. Start capturing your thoughts!</p>
        </div>
      )}
      
      {!isLoading && !error && captures.length > 0 && (
        <div className="space-y-3">
          {captures.map((capture) => (
            <motion.div
              key={capture.id}
              initial={{ opacity: 0, y: 10 }}
              animate={{ opacity: 1, y: 0 }}
              className="bg-card rounded-lg border p-4 shadow-sm hover:shadow-md transition-shadow"
            >
              <p className="text-sm text-foreground whitespace-pre-wrap">
                {capture.text}
              </p>
              <time 
                dateTime={new Date(capture.createdAt).toISOString()}
                className="text-xs text-muted-foreground mt-2 block"
              >
                {new Date(capture.createdAt).toLocaleString()}
              </time>
            </motion.div>
          ))}
        </div>
      )}
    </motion.div>
  )
}
