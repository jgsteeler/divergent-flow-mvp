import { Capture } from '@/lib/types'

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5100'

export interface CreateCaptureRequest {
  text: string
  inferredType?: string
  typeConfidence?: number
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
 * Fetch all captures from the API
 */
export async function fetchCaptures(options?: ApiRequestOptions): Promise<Capture[]> {
  const response = await fetch(`${API_URL}/api/captures`, {
    headers: createHeaders(options),
  })
  
  if (!response.ok) {
    throw new Error(`Failed to fetch captures: ${response.statusText}`)
  }
  
  return response.json()
}

/**
 * Create a new capture via the API
 */
export async function createCapture(
  request: CreateCaptureRequest,
  options?: ApiRequestOptions
): Promise<Capture> {
  const response = await fetch(`${API_URL}/api/captures`, {
    method: 'POST',
    headers: createHeaders(options),
    body: JSON.stringify(request),
  })
  
  if (!response.ok) {
    throw new Error(`Failed to create capture: ${response.statusText}`)
  }
  
  return response.json()
}
