// Simplified capture interface (frontend currently uses /api/captures)
export interface Capture {
  id: string
  text: string
  createdAt: number
  updatedAt?: number | null
  inferredType?: string | null
  typeConfidence?: number | null
  isMigrated?: boolean
}

// Unified Item model (backend also exposes /api/items)
export interface Item {
  id: string
  type: string
  text: string
  createdAt: number
  inferredType?: string | null
  typeConfidence?: number | null
  lastReviewedAt?: number | null
  collectionId?: string | null
}


