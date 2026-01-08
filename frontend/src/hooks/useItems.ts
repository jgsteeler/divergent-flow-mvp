import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { createItem, CreateItemRequest, fetchItems } from '@/lib/api/itemsApi'
import { Item } from '@/lib/types'
import { useAuth } from './useAuth'

export const ITEMS_QUERY_KEY = ['items']

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
    },
  })
}
