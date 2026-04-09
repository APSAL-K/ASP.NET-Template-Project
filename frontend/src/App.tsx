import { useEffect } from 'react'
import { Provider } from 'react-redux'
import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import { store } from './app/store'
import { useAppDispatch } from './app/hooks'
import { clearStoredSession, writeStoredApiUrl, writeStoredSession } from './app/localStorage'
import { ToastViewport } from './components/ToastViewport'
import { DashboardPage } from './pages/DashboardPage'
import { SignInPage } from './pages/SignInPage'
import { SignUpPage } from './pages/SignUpPage'
import './App.css'

function AppStateEffects() {
  useEffect(() => {
    const initial = store.getState()
    writeStoredApiUrl(initial.ui.apiBaseUrl)
    if (initial.auth.session) writeStoredSession(initial.auth.session)

    const unsubscribe = store.subscribe(() => {
      const state = store.getState()
      writeStoredApiUrl(state.ui.apiBaseUrl)
      if (state.auth.session) writeStoredSession(state.auth.session)
      else clearStoredSession()
    })

    return unsubscribe
  }, [])

  return null
}

function AppRouter() {
  const dispatch = useAppDispatch()

  useEffect(() => {
    void dispatch({ type: '@@app/booted' })
  }, [dispatch])

  return (
    <>
      <AppStateEffects />
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Navigate to="/dashboard" replace />} />
          <Route path="/sign-in" element={<SignInPage />} />
          <Route path="/sign-up" element={<SignUpPage />} />
          <Route path="/dashboard" element={<DashboardPage />} />
          <Route path="*" element={<Navigate to="/dashboard" replace />} />
        </Routes>
      </BrowserRouter>
      <ToastViewport />
    </>
  )
}

function App() {
  return (
    <Provider store={store}>
      <AppRouter />
    </Provider>
  )
}

export default App
