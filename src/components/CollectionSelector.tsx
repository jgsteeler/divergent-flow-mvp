import { useState, useMemo } from 'react'
import { CollectionInference } from '@/lib/types'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Check, MagnifyingGlass } from '@phosphor-icons/react'
import { MEDIUM_CONFIDENCE_THRESHOLD } from '@/lib/constants'

interface CollectionSelectorProps {
  inferences: CollectionInference[]
  allCollections: string[]
  selectedCollection: string
  onSelect: (collection: string) => void
  showConfidenceBadge?: boolean
}

export function CollectionSelector({
  inferences,
  allCollections,
  selectedCollection,
  onSelect,
  showConfidenceBadge = true,
}: CollectionSelectorProps) {
  const [searchQuery, setSearchQuery] = useState('')
  const [isCreatingNew, setIsCreatingNew] = useState(false)

  // Filter collections based on search
  const filteredCollections = useMemo(() => {
    if (!searchQuery) return []
    const query = searchQuery.toLowerCase()
    return allCollections
      .filter(c => c.toLowerCase().includes(query))
      .slice(0, 5) // Limit to 5 results
  }, [searchQuery, allCollections])

  const getConfidenceBadgeColor = (conf: number) => {
    if (conf >= 85) return 'bg-primary/10 text-primary border-primary/20'
    if (conf >= MEDIUM_CONFIDENCE_THRESHOLD) return 'bg-accent/10 text-accent-foreground border-accent/20'
    return 'bg-muted text-muted-foreground border-border'
  }

  const handleSearchChange = (value: string) => {
    setSearchQuery(value)
    setIsCreatingNew(false)
    
    // If exact match exists, select it
    const exactMatch = allCollections.find(c => c.toLowerCase() === value.toLowerCase())
    if (exactMatch) {
      onSelect(exactMatch)
    } else if (value) {
      // Clear selection when typing new name
      if (selectedCollection && !selectedCollection.toLowerCase().includes(value.toLowerCase())) {
        onSelect('')
      }
    }
  }

  const handleCreateNew = () => {
    if (searchQuery.trim()) {
      onSelect(searchQuery.trim())
      setIsCreatingNew(true)
    }
  }

  const handleSelectExisting = (collection: string) => {
    onSelect(collection)
    setSearchQuery(collection)
  }

  return (
    <div className="space-y-3">
      <div className="flex items-center gap-2">
        <Label className="text-sm font-medium">Collection</Label>
      </div>

      {/* Show inferred collections with medium+ confidence */}
      {inferences.length > 0 && (
        <div className="space-y-2">
          <Label className="text-xs text-muted-foreground">
            {inferences.length === 1 ? 'Suggested:' : 'Suggested collections:'}
          </Label>
          <div className="flex flex-wrap gap-2">
            {inferences.map((inference) => {
              const isSelected = inference.collection === selectedCollection
              return (
                <Button
                  key={inference.collection}
                  variant={isSelected ? 'default' : 'outline'}
                  size="sm"
                  className={`h-auto py-2 px-3 ${
                    isSelected 
                      ? 'bg-primary hover:bg-primary/90 text-primary-foreground' 
                      : 'hover:bg-accent hover:text-accent-foreground'
                  }`}
                  onClick={() => {
                    handleSelectExisting(inference.collection)
                  }}
                >
                  <span>{inference.collection}</span>
                  {showConfidenceBadge && (
                    <Badge 
                      variant="outline" 
                      className={`ml-2 text-xs ${getConfidenceBadgeColor(inference.confidence)}`}
                    >
                      {inference.confidence}%
                    </Badge>
                  )}
                  {isSelected && <Check size={14} weight="bold" className="ml-2" />}
                </Button>
              )
            })}
          </div>
        </div>
      )}

      {/* Search/Create input */}
      <div className="space-y-2">
        <Label htmlFor="collection-search" className="text-xs text-muted-foreground">
          {inferences.length > 0 ? 'Or search/create collection:' : 'Search or create a collection:'}
        </Label>
        <div className="relative">
          <MagnifyingGlass 
            className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" 
            size={16} 
          />
          <Input
            id="collection-search"
            placeholder="Type to search or create..."
            value={searchQuery}
            onChange={(e) => handleSearchChange(e.target.value)}
            className="pl-9"
          />
        </div>

        {/* Search results dropdown */}
        {searchQuery && filteredCollections.length > 0 && !isCreatingNew && (
          <div className="border rounded-md bg-card max-h-40 overflow-y-auto">
            {filteredCollections.map((collection) => {
              const isSelected = collection === selectedCollection
              return (
                <button
                  key={collection}
                  className={`w-full text-left px-3 py-2 text-sm hover:bg-accent transition-colors flex items-center justify-between ${
                    isSelected ? 'bg-accent/50' : ''
                  }`}
                  onClick={() => handleSelectExisting(collection)}
                >
                  <span>{collection}</span>
                  {isSelected && <Check size={14} weight="bold" />}
                </button>
              )
            })}
          </div>
        )}

        {/* Create new option */}
        {searchQuery && 
         !allCollections.some(c => c.toLowerCase() === searchQuery.toLowerCase()) && 
         filteredCollections.length === 0 && (
          <Button
            variant="outline"
            size="sm"
            className="w-full justify-start text-primary"
            onClick={handleCreateNew}
          >
            Create "{searchQuery}"
          </Button>
        )}
      </div>
    </div>
  )
}
