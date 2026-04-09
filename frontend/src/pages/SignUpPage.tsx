import { useState } from 'react'
import { Link, Navigate } from 'react-router-dom'
import { UserPlus, Mail, Lock, User, ShoppingBag } from 'lucide-react'
import { useAppDispatch, useAppSelector } from '../app/hooks'
import { signUpThunk } from '../features/auth/authThunks'

export function SignUpPage() {
  const dispatch = useAppDispatch()
  const session = useAppSelector((state) => state.auth.session)
  const status = useAppSelector((state) => state.auth.status)
  const authError = useAppSelector((state) => state.auth.error)
  const [form, setForm] = useState({ firstName: '', lastName: '', email: '', password: '' })

  if (session) {
    return <Navigate to="/dashboard" replace />
  }

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    await dispatch(signUpThunk(form))
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
                <Lock size={16} /> Master Access Key
              </span>
            </label>
            <input
              type="password"
              value={form.password}
              onChange={(event) => setForm((current) => ({ ...current, password: event.target.value }))}
              placeholder="••••••••"
              minLength={6}
              autoComplete="new-password"
              required
            />
          </div>

          {authError ? <div className="form-error">{authError}</div> : null}

          <button className="btn btn-primary" type="submit" style={{ width: '100%', height: '52px', fontSize: '1rem' }} disabled={status === 'loading'}>
            <UserPlus size={20} /> {status === 'loading' ? 'Requesting...' : 'Request Cluster Access'}
          </button>
        </form>

        <footer style={{ marginTop: '2.5rem', textAlign: 'center', fontSize: '0.9rem', color: 'var(--muted)', fontWeight: 600 }}>
          Already have access? <Link to="/sign-in" style={{ color: 'var(--accent-strong)', fontWeight: '800', marginLeft: '6px' }}>Sign in to Cluster</Link>
        </footer>
      </div>
    </main>
  )
}
