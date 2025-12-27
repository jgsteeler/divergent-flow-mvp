import { useState, useRef, useEffect } from 'react'
import { Textarea } from '@/components/ui/textarea'
import { Button } from '@/components/ui/button'
import { Plus } from '@phosphor-icons/react'
import { motion } from 'framer-motion'

interface CaptureInputProps {
  onCapture: (text: string) => void
  isLoading?: boolean
}

export function CaptureInput({ onCapture, isLoading }: CaptureInputProps) {
  const [text, setText] = useState('')
  const textareaRef = useRef<HTMLTextAreaElement>(null)

  useEffect(() => {
    textareaRef.current?.focus()
  }, [])

  const handleSubmit = () => {
    if (text.trim()) {
      onCapture(text.trim())
      setText('')
      setTimeout(() => textareaRef.current?.focus(), 100)
    }
  }

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && (e.metaKey || e.ctrlKey)) {
      e.preventDefault()
      handleSubmit()
    }
  }

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      className="w-full"
    >
      <div className="bg-card rounded-lg border-2 border-accent/30 shadow-lg p-4 space-y-3">
        <Textarea
          ref={textareaRef}
          value={text}
          onChange={(e) => setText(e.target.value)}
          onKeyDown={handleKeyDown}
          placeholder="Capture anything... (Cmd/Ctrl+Enter to save)"
          className="min-h-[100px] resize-none text-base border-0 focus-visible:ring-0 focus-visible:ring-offset-0 bg-transparent"
        />
        <div className="flex justify-end">
          <Button
            onClick={handleSubmit}
            disabled={!text.trim() || isLoading}
            className="bg-accent hover:bg-accent/90 text-accent-foreground"
          >
            <Plus className="mr-2" />
            {isLoading ? 'Capturing...' : 'Capture'}
          </Button>
        </div>
      </div>
    </motion.div>
  )
}
