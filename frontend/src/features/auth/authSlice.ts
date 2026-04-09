import { createSlice } from '@reduxjs/toolkit'
import { readStoredSession } from '../../app/localStorage'
import type { AuthSession } from '../../types'
import { refreshSessionThunk, signInThunk, signUpThunk } from './authThunks'

type AuthStatus = 'idle' | 'loading' | 'refreshing' | 'authenticated'

type AuthState = {
  session: AuthSession | null
  status: AuthStatus
  error: string | null
}

const initialState: AuthState = {
  session: readStoredSession(),
  status: readStoredSession() ? 'authenticated' : 'idle',
  error: null,
}

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    signOut(state) {
      state.session = null
      state.status = 'idle'
      state.error = null
    },
    clearAuthError(state) {
      state.error = null
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(signInThunk.pending, (state) => {
        state.status = 'loading'
        state.error = null
      })
      .addCase(signInThunk.fulfilled, (state, action) => {
        state.session = action.payload
        state.status = 'authenticated'
        state.error = null
      })
      .addCase(signInThunk.rejected, (state, action) => {
        state.status = 'idle'
        state.error = action.payload ?? 'Sign in failed.'
      })
      .addCase(signUpThunk.pending, (state) => {
        state.status = 'loading'
        state.error = null
      })
      .addCase(signUpThunk.fulfilled, (state, action) => {
        state.session = action.payload
        state.status = 'authenticated'
        state.error = null
      })
      .addCase(signUpThunk.rejected, (state, action) => {
        state.status = 'idle'
        state.error = action.payload ?? 'Sign up failed.'
      })
      .addCase(refreshSessionThunk.pending, (state) => {
        state.status = 'refreshing'
      })
      .addCase(refreshSessionThunk.fulfilled, (state, action) => {
        state.session = action.payload
        state.status = 'authenticated'
        state.error = null
      })
      .addCase(refreshSessionThunk.rejected, (state, action) => {
        state.status = state.session ? 'authenticated' : 'idle'
        state.error = action.payload ?? 'Session refresh failed.'
      })
  },
})

export const { clearAuthError, signOut } = authSlice.actions
export default authSlice.reducer