import { create } from 'zustand'
import { produce, Draft } from 'immer'

interface NotificationStore {
    notifications: Array<{
        type: string
        message: string
        description?: string
        txid?: string
    }>
    set: (fn: (draft: Draft<NotificationStore>) => void) => void
}

const useNotificationStore = create<NotificationStore>((set) => ({
    notifications: [],
    set: (fn) =>
        set(
            produce((draft) => {
                fn(draft)
            })
        ),
}))

export default useNotificationStore
