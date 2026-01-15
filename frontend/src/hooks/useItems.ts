import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { 
  createItem, 
  CreateItemRequest, 
  fetchItems, 
  fetchReviewQueue, 
  fetchDashboard,
  markItemReviewed,
  MarkItemReviewedRequest,
  DashboardData
} from '@/lib/api/itemsApi'
import { Item } from '@/lib/types'
import { useAuth } from './useAuth'

export const ITEMS_QUERY_KEY = ['items']
export const REVIEW_QUEUE_QUERY_KEY = ['items', 'review-queue']
export const DASHBOARD_QUERY_KEY = ['items', 'dashboard']

export function useItems() {
  const { getUserId, getAccessToken } = useAuth()
  
  return useQuery<Item[], Error>({
    queryKey: ITEMS_QUERY_KEY,
    queryFn: async () => {
      const userId = getUserId()
      const accessToken = await getAccessToken()
      return fetchItems({ userId, accessToken })
    },
  })
}

export function useReviewQueue(limit: number = 3, maxConfidence: number = 0.75) {
  const { getUserId, getAccessToken } = useAuth()
  
  return useQuery<Item[], Error>({
    queryKey: [...REVIEW_QUEUE_QUERY_KEY, limit, maxConfidence],
    queryFn: async () => {
      const userId = getUserId()
      const accessToken = await getAccessToken()
      return fetchReviewQueue(limit, maxConfidence, { userId, accessToken })
    },
  })
}

export function useDashboard() {
  const { getUserId, getAccessToken } = useAuth()
  
  return useQuery<DashboardData, Error>({
    queryKey: DASHBOARD_QUERY_KEY,
    queryFn: async () => {
      const userId = getUserId()
      const accessToken = await getAccessToken()
      return fetchDashboard({ userId, accessToken })
    },
  })
}

export function useCreateItem() {
  const queryClient = useQueryClient()
  const { getUserId, getAccessToken } = useAuth()

  return useMutation<Item, Error, CreateItemRequest>({
    mutationFn: async (request) => {
      const userId = getUserId()
      const accessToken = await getAccessToken()
      return createItem(request, { userId, accessToken })
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ITEMS_QUERY_KEY })
      queryClient.invalidateQueries({ queryKey: REVIEW_QUEUE_QUERY_KEY })
      queryClient.invalidateQueries({ queryKey: DASHBOARD_QUERY_KEY })
    },
  })
}

export function useMarkItemReviewed() {
  const queryClient = useQueryClient()
  const { getUserId, getAccessToken } = useAuth()

  return useMutation<Item, Error, { id: string; request?: MarkItemReviewedRequest }>({
    mutationFn: async ({ id, request }) => {
      const userId = getUserId()
      const accessToken = await getAccessToken()
      return markItemReviewed(id, request, { userId, accessToken })
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ITEMS_QUERY_KEY })
      queryClient.invalidateQueries({ queryKey: REVIEW_QUEUE_QUERY_KEY })
      queryClient.invalidateQueries({ queryKey: DASHBOARD_QUERY_KEY })
    },
  })
}
