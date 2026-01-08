import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { fetchCaptures, createCapture, CreateCaptureRequest } from '@/lib/api/capturesApi'
import { Capture } from '@/lib/types'
import { useAuth } from './useAuth'

/**
 * Query key for captures
 */
export const CAPTURES_QUERY_KEY = ['captures']

/**
 * Hook to fetch all captures
 */
export function useCaptures() {
  const { getUserId, getAccessToken } = useAuth()
  
  return useQuery<Capture[], Error>({
    queryKey: CAPTURES_QUERY_KEY,
    queryFn: async () => {
      const userId = getUserId()
      const accessToken = await getAccessToken()
      return fetchCaptures({ userId, accessToken })
    },
  })
}

/**
 * Hook to create a new capture
 */
export function useCreateCapture() {
  const queryClient = useQueryClient()
  const { getUserId, getAccessToken } = useAuth()
  
  return useMutation<Capture, Error, CreateCaptureRequest>({
    mutationFn: async (request) => {
      const userId = getUserId()
      const accessToken = await getAccessToken()
      return createCapture(request, { userId, accessToken })
    },
    onSuccess: () => {
      // Invalidate captures query to refetch the list
      queryClient.invalidateQueries({ queryKey: CAPTURES_QUERY_KEY })
    },
  })
}
