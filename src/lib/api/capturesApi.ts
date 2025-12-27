import { Capture } from '@/lib/types'

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000'

export interface CreateCaptureRequest {
  text: string
  inferredType?: string
  typeConfidence?: number
}

/**
 * Fetch all captures from the API
 */
export async function fetchCaptures(): Promise<Capture[]> {
  const response = await fetch(`${API_URL}/api/captures`)
  
  if (!response.ok) {
    throw new Error(`Failed to fetch captures: ${response.statusText}`)
  }
  
  return response.json()
}

/**
 * Create a new capture via the API
 */
export async function createCapture(request: CreateCaptureRequest): Promise<Capture> {
  const response = await fetch(`${API_URL}/api/captures`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(request),
  })
  
  if (!response.ok) {
    throw new Error(`Failed to create capture: ${response.statusText}`)
  }
  
  return response.json()
}
