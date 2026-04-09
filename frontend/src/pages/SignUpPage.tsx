import { useState, useEffect } from 'react'
import { Link, Navigate } from 'react-router-dom'
import { UserPlus, Mail, Lock, User, ShoppingBag, Eye, EyeOff, Shield } from 'lucide-react'
import { useAppDispatch, useAppSelector } from '../app/hooks'
import { signUpThunk } from '../features/auth/authThunks'
import { publicApi, describeApiError } from '../api'

export function SignUpPage() {
  const dispatch = useAppDispatch()
  const session = useAppSelector((state) => state.auth.session)
  const status = useAppSelector((state) => state.auth.status)
  const authError = useAppSelector((state) => state.auth.error)
  const apiBaseUrl = useAppSelector((state) => state.ui.apiBaseUrl)
  
  const [form, setForm] = useState({ firstName: '', lastName: '', email: '', password: '', roleId: '' })
  const [showPassword, setShowPassword] = useState(false)
  const [availableRoles, setAvailableRoles] = useState<{id: string, name: string}[]>([])
  const [rolesError, setRolesError] = useState<string | null>(null)

  useEffect(() => {
    async function fetchRoles() {
      try {
        const roles = await publicApi.getRoles(apiBaseUrl)
        setAvailableRoles(roles)
        if (roles.length > 0) {
          setForm(f => ({ ...f, roleId: roles[0].id }))
        }
      } catch (e) {
        setRolesError(describeApiError(e))
      }
    }
    void fetchRoles()
  }, [apiBaseUrl])

  if (session) {
    return <Navigate to="/dashboard" replace />
  }

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    await dispatch(signUpThunk({
      ...form,
      roleIds: form.roleId ? [form.roleId] : undefined
    }))
  }

  return (
    <main className="auth-layout">
      <div className="auth-card">
        <header className="auth-header">
           <div style={{ marginBottom: '2.5rem', display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
            <div className="sidebar-logo-box" style={{ width: '48px', height: '48px', marginBottom: '1.5rem' }}>
              <ShoppingBag size={24} strokeWidth={3} />
            </div>
            <h1>Initialize Identity</h1>
            <p>Register as a cluster operator</p>
          </div>
        </header>

        <form className="auth-form" onSubmit={handleSubmit}>
          <div className="grid-2">
            <div className="form-group">
              <label>
                <span style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                  <User size={16} /> First name
                </span>
              </label>
              <input
                type="text"
                value={form.firstName}
                onChange={(event) => setForm((current) => ({ ...current, firstName: event.target.value }))}
                placeholder="John"
                required
              />
            </div>
            <div className="form-group">
              <label>
                <span style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                  <User size={16} /> Last name
                </span>
              </label>
              <input
                type="text"
                value={form.lastName}
                onChange={(event) => setForm((current) => ({ ...current, lastName: event.target.value }))}
                placeholder="Doe"
                required
              />
            </div>
          </div>

          <div className="form-group">
            <label>
              <span style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                <Mail size={16} /> Enterprise Email
              </span>
            </label>
            <input
              type="email"
              value={form.email}
              onChange={(event) => setForm((current) => ({ ...current, email: event.target.value }))}
              placeholder="operator@enterprise.com"
              required
            />
          </div>

          <div className="form-group">
            <label>
              <span style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                <Lock size={16} /> Password
              </span>
            </label>
            <div style={{ position: 'relative' }}>
              <input
                type={showPassword ? 'text' : 'password'}
                value={form.password}
                onChange={(event) => setForm((current) => ({ ...current, password: event.target.value }))}
                placeholder="••••••••"
                minLength={6}
                autoComplete="new-password"
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
                <Shield size={16} /> Requested Role
              </span>
            </label>
            <select
              value={form.roleId}
              onChange={(event) => setForm((current) => ({ ...current, roleId: event.target.value }))}
              required
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
              {availableRoles.map(r => (
                <option key={r.id} value={r.id}>{r.name}</option>
              ))}
            </select>
            {rolesError && <div style={{ fontSize: '0.75rem', color: 'var(--danger)', marginTop: '0.5rem' }}>{rolesError}</div>}
          </div>

          {authError ? <div className="form-error">{authError}</div> : null}

          <button className="btn btn-primary" type="submit" style={{ width: '100%', height: '52px', fontSize: '1rem' }} disabled={status === 'loading'}>
            <UserPlus size={20} /> {status === 'loading' ? 'Requesting...' : 'Sign up'}
          </button>
        </form>

        <footer style={{ marginTop: '2.5rem', textAlign: 'center', fontSize: '0.9rem', color: 'var(--muted)', fontWeight: 600 }}>
          Already have access? <Link to="/sign-in" style={{ color: 'var(--accent-strong)', fontWeight: '800', marginLeft: '6px' }}>Sign in</Link>
        </footer>
      </div>
    </main>
  )
}
