import { Item } from '@/lib/types'

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5100'

export interface CreateItemRequest {
  text: string
  inferredType?: string
  typeConfidence?: number
  collectionId?: string | null
}

/**
 * Fetch all items from the API
 */
export async function fetchItems(): Promise<Item[]> {
  const response = await fetch(`${API_URL}/api/items`)

  if (!response.ok) {
    throw new Error(`Failed to fetch items: ${response.statusText}`)
  }

  return response.json()
}

/**
 * Create a new item via the API
 */
export async function createItem(request: CreateItemRequest): Promise<Item> {
  const response = await fetch(`${API_URL}/api/items`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(request),
  })

  if (!response.ok) {
    const bodyText = await response.text().catch(() => '')
    throw new Error(
      `Failed to create item: ${response.status} ${response.statusText}${bodyText ? ` - ${bodyText}` : ''}`
    )
  }

  return response.json()
}
