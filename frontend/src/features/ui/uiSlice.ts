import { createSlice, nanoid } from '@reduxjs/toolkit'
import type { PayloadAction } from '@reduxjs/toolkit'
import { readStoredApiUrl } from '../../app/localStorage'

export type ToastKind = 'success' | 'error' | 'info'

export type ToastMessage = {
  id: string
  kind: ToastKind
  title: string
  description?: string
}

type UiState = {
  apiBaseUrl: string
  toasts: ToastMessage[]
}

const initialState: UiState = {
  apiBaseUrl: readStoredApiUrl(),
  toasts: [],
}

const uiSlice = createSlice({
  name: 'ui',
  initialState,
  reducers: {
    setApiBaseUrl(state, action: PayloadAction<string>) {
      state.apiBaseUrl = action.payload
    },
    pushToast: {
      reducer(state, action: PayloadAction<ToastMessage>) {
        state.toasts.unshift(action.payload)
      },
      prepare(payload: Omit<ToastMessage, 'id'>) {
        return {
          payload: {
            id: nanoid(),
            ...payload,
          },
        }
      },
    },
    dismissToast(state, action: PayloadAction<string>) {
      state.toasts = state.toasts.filter((toast) => toast.id !== action.payload)
    },
  },
})

export const { dismissToast, pushToast, setApiBaseUrl } = uiSlice.actions
export default uiSlice.reducer