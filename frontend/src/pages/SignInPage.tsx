import { useState } from 'react'
import { Link, Navigate, useLocation } from 'react-router-dom'
import { LogIn, Mail, Lock, ShoppingBag } from 'lucide-react'
import { useAppDispatch, useAppSelector } from '../app/hooks'
import { signInThunk } from '../features/auth/authThunks'

export function SignInPage() {
  const dispatch = useAppDispatch()
  const location = useLocation()
  const session = useAppSelector((state) => state.auth.session)
  const status = useAppSelector((state) => state.auth.status)
  const authError = useAppSelector((state) => state.auth.error)
  const [form, setForm] = useState({ email: '', password: '' })

  if (session) {
    return <Navigate to="/dashboard" replace />
  }

  const redirectPath = (location.state as { from?: string } | null)?.from ?? '/dashboard'

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    const result = await dispatch(signInThunk(form))

    if (signInThunk.fulfilled.match(result)) {
      window.location.assign(redirectPath)
    }
  }

  return (
    <main className="auth-layout">
      <div className="auth-card">
        <header className="auth-header">
          <div style={{ marginBottom: '2.5rem', display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
            <div className="sidebar-logo-box" style={{ width: '48px', height: '48px', marginBottom: '1.5rem' }}>
              <ShoppingBag size={24} strokeWidth={3} />
            </div>
            <h1>Access Console</h1>
            <p>Login to manage enterprise cluster</p>
          </div>
        </header>

        <form className="auth-form" onSubmit={handleSubmit}>
          <div className="form-group">
            <label>
              <span style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                <Mail size={16} /> Identity Identifier (Email)
              </span>
            </label>
            <input
              type="email"
              value={form.email}
              onChange={(event) => setForm((current) => ({ ...current, email: event.target.value }))}
              placeholder="admin@enterprise.com"
              autoComplete="email"
              required
            />
          </div>

          <div className="form-group">
            <label>
              <span style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                <Lock size={16} /> Access Key (Password)
              </span>
            </label>
            <input
              type="password"
              value={form.password}
              onChange={(event) => setForm((current) => ({ ...current, password: event.target.value }))}
              placeholder="••••••••"
              autoComplete="current-password"
              required
            />
          </div>

          {authError ? <div className="form-error">{authError}</div> : null}

          <button className="btn btn-primary" type="submit" style={{ width: '100%', height: '52px', fontSize: '1rem' }} disabled={status === 'loading'}>
            <LogIn size={20} /> {status === 'loading' ? 'Authenticating...' : 'Sign in to Cluster'}
          </button>
        </form>

        <footer style={{ marginTop: '2.5rem', textAlign: 'center', fontSize: '0.9rem', color: 'var(--muted)', fontWeight: 600 }}>
          New operator? <Link to="/sign-up" style={{ color: 'var(--accent-strong)', fontWeight: '800', marginLeft: '6px' }}>Request Access</Link>
        </footer>
      </div>
    </main>
  )
}
