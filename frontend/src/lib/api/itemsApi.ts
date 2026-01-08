import { Item } from '@/lib/types'

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5100'

export interface CreateItemRequest {
  text: string
  inferredType?: string
  typeConfidence?: number
  collectionId?: string | null
}

export interface ApiRequestOptions {
  userId?: string
  accessToken?: string | null
}

/**
 * Create headers for API requests
 * Includes authentication token and user ID when available
 */
function createHeaders(options?: ApiRequestOptions): HeadersInit {
  const headers: HeadersInit = {
    'Content-Type': 'application/json',
  }

  // Add user ID header (defaults to 'local' if not provided)
  headers['X-User-Id'] = options?.userId || 'local'

  // Add authorization header if access token is provided
  if (options?.accessToken) {
    headers['Authorization'] = `Bearer ${options.accessToken}`
  }

  return headers
}

/**
 * Fetch all items from the API
 */
export async function fetchItems(options?: ApiRequestOptions): Promise<Item[]> {
  const response = await fetch(`${API_URL}/api/items`, {
    headers: createHeaders(options),
  })

  if (!response.ok) {
    throw new Error(`Failed to fetch items: ${response.statusText}`)
  }

  return response.json()
}

/**
 * Create a new item via the API
 */
export async function createItem(
  request: CreateItemRequest,
  options?: ApiRequestOptions
): Promise<Item> {
  const response = await fetch(`${API_URL}/api/items`, {
    method: 'POST',
    headers: createHeaders(options),
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
