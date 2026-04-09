import type { AuthSession } from '../types'

export const SESSION_STORAGE_KEY = 'mdf-auth-console-session'
export const API_URL_STORAGE_KEY = 'mdf-auth-console-api-url'

export function readStoredSession(): AuthSession | null {
  const raw = window.localStorage.getItem(SESSION_STORAGE_KEY)

  if (!raw) {
    return null
  }

  try {
    return JSON.parse(raw) as AuthSession
  } catch {
    window.localStorage.removeItem(SESSION_STORAGE_KEY)
    return null
  }
}

export function writeStoredSession(session: AuthSession) {
  window.localStorage.setItem(SESSION_STORAGE_KEY, JSON.stringify(session))
}

export function clearStoredSession() {
  window.localStorage.removeItem(SESSION_STORAGE_KEY)
}

export function readStoredApiUrl() {
  return window.localStorage.getItem(API_URL_STORAGE_KEY) ?? import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5019'
}

export function writeStoredApiUrl(apiBaseUrl: string) {
  window.localStorage.setItem(API_URL_STORAGE_KEY, apiBaseUrl)
}