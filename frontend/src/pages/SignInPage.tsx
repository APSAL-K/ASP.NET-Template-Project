import { useState, useEffect } from 'react'
import { Link, Navigate, useLocation } from 'react-router-dom'
import { LogIn, Mail, Lock, ShoppingBag, Eye, EyeOff, Shield } from 'lucide-react'
import { useAppDispatch, useAppSelector } from '../app/hooks'
import { signInThunk } from '../features/auth/authThunks'
import { publicApi, describeApiError } from '../api'

export function SignInPage() {
  const dispatch = useAppDispatch()
  const location = useLocation()
  const session = useAppSelector((state) => state.auth.session)
  const status = useAppSelector((state) => state.auth.status)
  const authError = useAppSelector((state) => state.auth.error)
  const apiBaseUrl = useAppSelector((state) => state.ui.apiBaseUrl)

  const [form, setForm] = useState({ email: '', password: '', roleId: '' })
  const [showPassword, setShowPassword] = useState(false)
  const [availableRoles, setAvailableRoles] = useState<{id: string, name: string}[]>([])
  const [_rolesError, setRolesError] = useState<string | null>(null)

  useEffect(() => {
    async function fetchRoles() {
      try {
        const roles = await publicApi.getRoles(apiBaseUrl)
        setAvailableRoles(roles)
      } catch (e) {
        setRolesError(describeApiError(e))
      }
    }
    void fetchRoles()
  }, [apiBaseUrl])

  if (session) {
    return <Navigate to="/dashboard" replace />
  }

  const redirectPath = (location.state as { from?: string } | null)?.from ?? '/dashboard'

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    const result = await dispatch(signInThunk({
      ...form,
      roleId: form.roleId || undefined
    }))

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
            <div style={{ position: 'relative' }}>
              <input
                type={showPassword ? 'text' : 'password'}
                value={form.password}
                onChange={(event) => setForm((current) => ({ ...current, password: event.target.value }))}
                placeholder="••••••••"
                autoComplete="current-password"
                required
                style={{ paddingRight: '3.5rem' }}
              />
              <button
                type="button"
                onClick={() => setShowPassword(!showPassword)}
                style={{
                  position: 'absolute',
                  right: '12px',
                  top: '50%',
                  transform: 'translateY(-50%)',
                  background: 'none',
                  border: 'none',
                  color: 'var(--muted)',
                  cursor: 'pointer',
                  display: 'flex',
                  alignItems: 'center',
                  padding: '4px',
                }}
              >
                {showPassword ? <EyeOff size={18} /> : <Eye size={18} />}
              </button>
            </div>
          </div>

          <div className="form-group">
            <label>
              <span style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                <Shield size={16} /> Session Context (Role)
              </span>
            </label>
            <select
              value={form.roleId}
              onChange={(event) => setForm((current) => ({ ...current, roleId: event.target.value }))}
              className="premium-select"
              style={{
                width: '100%',
                background: 'var(--bg)',
                border: '1px solid var(--line)',
                padding: '0.8rem 1.25rem',
                borderRadius: 'var(--radius-md)',
                color: 'var(--text)',
                fontWeight: 600,
                cursor: 'pointer'
              }}
            >
              <option value="">Auto-resolve Roles</option>
              {availableRoles.map(r => (
                <option key={r.id} value={r.id}>{r.name}</option>
              ))}
            </select>
          </div>

          {authError ? <div className="form-error">{authError}</div> : null}

          <button className="btn btn-primary" type="submit" style={{ width: '100%', height: '52px', fontSize: '1rem' }} disabled={status === 'loading'}>
            <LogIn size={20} /> {status === 'loading' ? 'Authenticating...' : 'Sign in'}
          </button>
        </form>

        <footer style={{ marginTop: '2.5rem', textAlign: 'center', fontSize: '0.9rem', color: 'var(--muted)', fontWeight: 600 }}>
          New operator? <Link to="/sign-up" style={{ color: 'var(--accent-strong)', fontWeight: '800', marginLeft: '6px' }}>Sign up</Link>
        </footer>
      </div>
    </main>
  )
}
