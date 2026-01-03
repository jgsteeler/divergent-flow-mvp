import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { createItem, CreateItemRequest, fetchItems } from '@/lib/api/itemsApi'
import { Item } from '@/lib/types'

export const ITEMS_QUERY_KEY = ['items']

export function useItems() {
  return useQuery<Item[], Error>({
    queryKey: ITEMS_QUERY_KEY,
    queryFn: fetchItems,
  })
}

export function useCreateItem() {
  const queryClient = useQueryClient()

  return useMutation<Item, Error, CreateItemRequest>({
    mutationFn: createItem,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ITEMS_QUERY_KEY })
    },
  })
}
