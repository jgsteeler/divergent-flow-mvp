import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { fetchCaptures, createCapture, CreateCaptureRequest } from '@/lib/api/capturesApi'
import { Capture } from '@/lib/types'

/**
 * Query key for captures
 */
export const CAPTURES_QUERY_KEY = ['captures']

/**
 * Hook to fetch all captures
 */
export function useCaptures() {
  return useQuery<Capture[], Error>({
    queryKey: CAPTURES_QUERY_KEY,
    queryFn: fetchCaptures,
  })
}

/**
 * Hook to create a new capture
 */
export function useCreateCapture() {
  const queryClient = useQueryClient()
  
  return useMutation<Capture, Error, CreateCaptureRequest>({
    mutationFn: createCapture,
    onSuccess: () => {
      // Invalidate captures query to refetch the list
      queryClient.invalidateQueries({ queryKey: CAPTURES_QUERY_KEY })
    },
  })
}
