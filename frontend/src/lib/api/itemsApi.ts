import { Item } from '@/lib/types'

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5100'

export interface CreateItemRequest {
  text: string
  inferredType?: string
  typeConfidence?: number
  collectionId?: string | null
}

export interface MarkItemReviewedRequest {
  confirmedType?: string
  confirmedConfidence?: number
}

export interface DashboardData {
  metrics: {
    totalItems: number
    pendingReview: number
    actionItems: number
    completedToday: number
  }
  todayTasks: TaskItem[]
  overdueTasks: TaskItem[]
  upcomingTasks: TaskItem[]
}

export interface TaskItem {
  id: string
  text: string
  createdAt: number
  inferredType?: string | null
  typeConfidence?: number | null
  lastReviewedAt?: number | null
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
 * Fetch review queue items from the API
 */
export async function fetchReviewQueue(
  limit: number = 3,
  maxConfidence: number = 0.75,
  options?: ApiRequestOptions
): Promise<Item[]> {
  const params = new URLSearchParams({
    limit: limit.toString(),
    ...(maxConfidence !== null && { maxConfidence: maxConfidence.toString() }),
  })
  
  const response = await fetch(`${API_URL}/api/items/review-queue?${params}`, {
    headers: createHeaders(options),
  })

  if (!response.ok) {
    throw new Error(`Failed to fetch review queue: ${response.statusText}`)
  }

  return response.json()
}

/**
 * Fetch dashboard data from the API
 */
export async function fetchDashboard(options?: ApiRequestOptions): Promise<DashboardData> {
  const response = await fetch(`${API_URL}/api/items/dashboard`, {
    headers: createHeaders(options),
  })

  if (!response.ok) {
    throw new Error(`Failed to fetch dashboard: ${response.statusText}`)
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

/**
 * Mark an item as reviewed
 */
export async function markItemReviewed(
  id: string,
  request?: MarkItemReviewedRequest,
  options?: ApiRequestOptions
): Promise<Item> {
  const response = await fetch(`${API_URL}/api/items/${id}/review`, {
    method: 'PUT',
    headers: createHeaders(options),
    body: JSON.stringify(request || {}),
  })

  if (!response.ok) {
    throw new Error(`Failed to mark item as reviewed: ${response.statusText}`)
  }

  return response.json()
}
