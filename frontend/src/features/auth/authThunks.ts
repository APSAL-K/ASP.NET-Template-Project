import { createAsyncThunk } from '@reduxjs/toolkit'
import type { RootState } from '../../app/store'
import { authApi, describeApiError } from '../../api'
import type { AuthSession, LoginResponse, LoginRequest, RegisterRequest } from '../../types'
import { pushToast } from '../ui/uiSlice'

function mapAuthSession(response: LoginResponse, previousEmail?: string): AuthSession {
  return {
    userId: response.userId,
    email: response.email || previousEmail || '',
    accessToken: response.accessToken,
    refreshToken: response.refreshToken,
    expiresAt: response.tokenExpiresAt,
    roles: response.roles,
    permissions: response.permissions,
  }
}

export const signInThunk = createAsyncThunk<AuthSession, LoginRequest, { state: RootState; rejectValue: string }>(
  'auth/signIn',
  async (credentials, thunkApi) => {
    const apiBaseUrl = thunkApi.getState().ui.apiBaseUrl

    try {
      const response = await authApi.login(apiBaseUrl, credentials)
      thunkApi.dispatch(
        pushToast({
          kind: 'success',
          title: 'Signed in',
          description: 'Your account session is ready.',
        }),
      )
      return mapAuthSession(response)
    } catch (error) {
      const message = describeApiError(error)
      thunkApi.dispatch(pushToast({ kind: 'error', title: 'Sign in failed', description: message }))
      return thunkApi.rejectWithValue(message)
    }
  },
)

export const signUpThunk = createAsyncThunk<AuthSession, RegisterRequest, { state: RootState; rejectValue: string }>(
  'auth/signUp',
  async (payload, thunkApi) => {
    const apiBaseUrl = thunkApi.getState().ui.apiBaseUrl

    try {
      const response = await authApi.register(apiBaseUrl, payload)
      thunkApi.dispatch(
        pushToast({
          kind: 'success',
          title: 'Account created',
          description: 'You are signed in immediately after registration.',
        }),
      )
      return mapAuthSession(response)
    } catch (error) {
      const message = describeApiError(error)
      thunkApi.dispatch(pushToast({ kind: 'error', title: 'Sign up failed', description: message }))
      return thunkApi.rejectWithValue(message)
    }
  },
)

export const refreshSessionThunk = createAsyncThunk<AuthSession, void, { state: RootState; rejectValue: string }>(
  'auth/refreshSession',
  async (_, thunkApi) => {
    const state = thunkApi.getState()
    const apiBaseUrl = state.ui.apiBaseUrl
    const session = state.auth.session

    if (!session?.refreshToken) {
      const message = 'No refresh token is available.'
      thunkApi.dispatch(pushToast({ kind: 'error', title: 'Refresh unavailable', description: message }))
      return thunkApi.rejectWithValue(message)
    }

    try {
      const response = await authApi.refresh(apiBaseUrl, session.refreshToken)
      thunkApi.dispatch(
        pushToast({
          kind: 'success',
          title: 'Session renewed',
          description: 'Access and refresh tokens were updated.',
        }),
      )

      return {
        ...session,
        accessToken: response.accessToken,
        refreshToken: response.refreshToken,
        expiresAt: response.expiresAt,
        roles: response.roles,
        permissions: response.permissions,
      }
    } catch (error) {
      const message = describeApiError(error)
      thunkApi.dispatch(pushToast({ kind: 'error', title: 'Refresh failed', description: message }))
      return thunkApi.rejectWithValue(message)
    }
  },
)