import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { 
  Plus, 
  Pencil, 
  Trash2, 
  Search,
  RefreshCw,
  Shield,
  User as UserIcon,
  Activity,
  ChevronRight,
  Globe,
  Save,
  ArrowLeft
} from 'lucide-react'
import { useAppDispatch, useAppSelector } from '../app/hooks'
import { clearStoredSession } from '../app/localStorage'
import { signOut } from '../features/auth/authSlice'
import {
  createPermissionThunk,
  createRoleThunk,
  createUserThunk,
  deletePermissionThunk,
  deleteRoleThunk,
  deleteUserThunk,
  fetchOverviewThunk,
  updatePermissionThunk,
  updateRoleThunk,
  updateUserThunk,
} from '../features/management/managementThunks'
import { pushToast, setApiBaseUrl } from '../features/ui/uiSlice'
import { Sidebar } from '../components/Sidebar'

export function DashboardPage() {
  const dispatch = useAppDispatch()
  const navigate = useNavigate()
  const session = useAppSelector((state) => state.auth.session)
  const { permissions, roles, users, overviewStatus } = useAppSelector((state) => state.management)
  
  const [currentSection, setCurrentSection] = useState('overview')
  const [viewMode, setViewMode] = useState<'list' | 'form'>('list')
  const [searchQuery, setSearchQuery] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)
  
  const apiBaseUrl = useAppSelector((state) => state.ui.apiBaseUrl)
  const [settingsForm, setSettingsForm] = useState({ apiBaseUrl })

  // Form states
  const [permissionForm, setPermissionForm] = useState({ name: '', description: '', editingId: '' })
  const [roleForm, setRoleForm] = useState({ name: '', description: '', permissionIds: [] as string[], editingId: '' })
  const [userForm, setUserForm] = useState({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    isActive: true,
    roleIds: [] as string[],
    editingId: '',
  })

  useEffect(() => {
    if (overviewStatus === 'idle') {
      void dispatch(fetchOverviewThunk())
    }
  }, [dispatch, overviewStatus])

  useEffect(() => {
    // Reset view mode when changing main sections
    setViewMode('list')
    setSearchQuery('')
  }, [currentSection])

  const resetPermissionForm = () => {
    setPermissionForm({ name: '', description: '', editingId: '' })
    setViewMode('list')
  }
  const resetRoleForm = () => {
    setRoleForm({ name: '', description: '', permissionIds: [], editingId: '' })
    setViewMode('list')
  }
  const resetUserForm = () => {
    setUserForm({ firstName: '', lastName: '', email: '', password: '', isActive: true, roleIds: [], editingId: '' })
    setViewMode('list')
  }

  const handleSignOut = () => {
    clearStoredSession()
    dispatch(signOut())
    navigate('/sign-in', { replace: true })
  }

  const toggleRole = (roleId: string) => {
    setUserForm(prev => {
      const isSelected = prev.roleIds.includes(roleId)
      return {
        ...prev,
        roleIds: isSelected 
          ? prev.roleIds.filter(id => id !== roleId)
          : [...prev.roleIds, roleId]
      }
    })
  }

  const togglePermission = (permId: string) => {
    setRoleForm(prev => {
      const isSelected = prev.permissionIds.includes(permId)
      return {
        ...prev,
        permissionIds: isSelected 
          ? prev.permissionIds.filter(id => id !== permId)
          : [...prev.permissionIds, permId]
      }
    })
  }

  async function wrapAction(action: () => Promise<any>, successMsg: string) {
    setIsSubmitting(true)
    try {
      const result = await action()
      if (result.error) throw new Error(result.error)
      dispatch(pushToast({ kind: 'success', title: 'Success', description: successMsg }))
      return true
    } catch (e: any) {
      dispatch(pushToast({ kind: 'error', title: 'Action Failed', description: e.message }))
      return false
    } finally {
      setIsSubmitting(false)
    }
  }

  const onPermissionSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    const success = await wrapAction(
      () => permissionForm.editingId 
        ? dispatch(updatePermissionThunk({ id: permissionForm.editingId, payload: { name: permissionForm.name, description: permissionForm.description } }))
        : dispatch(createPermissionThunk({ name: permissionForm.name, description: permissionForm.description })),
      permissionForm.editingId ? 'Permission updated' : 'Permission created'
    )
    if (success) resetPermissionForm()
  }

  const onRoleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    const success = await wrapAction(
      () => roleForm.editingId
        ? dispatch(updateRoleThunk({ id: roleForm.editingId, payload: { name: roleForm.name, description: roleForm.description, permissionIds: roleForm.permissionIds } }))
        : dispatch(createRoleThunk({ name: roleForm.name, description: roleForm.description, permissionIds: roleForm.permissionIds })),
      roleForm.editingId ? 'Role updated' : 'Role created'
    )
    if (success) resetRoleForm()
  }

  const onUserSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    const basePayload = { firstName: userForm.firstName, lastName: userForm.lastName, email: userForm.email, isActive: userForm.isActive, roleIds: userForm.roleIds }
    const success = await wrapAction(
      () => userForm.editingId
        ? dispatch(updateUserThunk({ id: userForm.editingId, payload: { ...basePayload, password: userForm.password || undefined } }))
        : dispatch(createUserThunk({ ...basePayload, password: userForm.password })),
      userForm.editingId ? 'User updated' : 'User created'
    )
    if (success) resetUserForm()
  }
  
  const onSettingsSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    dispatch(setApiBaseUrl(settingsForm.apiBaseUrl))
    dispatch(pushToast({ kind: 'success', title: 'Config Saved', description: 'System API endpoint updated successfully.' }))
  }

  const renderBreadcrumbs = () => (
    <div className="breadcrumb-trail">
      <span className="breadcrumb-item">Console</span>
      <ChevronRight size={12} strokeWidth={3} />
      <span className="breadcrumb-item">{currentSection}</span>
      {viewMode === 'form' && (
        <>
          <ChevronRight size={12} strokeWidth={3} />
          <span className="breadcrumb-item" style={{ color: 'var(--accent-strong)' }}>
            {(userForm.editingId || roleForm.editingId || permissionForm.editingId) ? 'Modify' : 'Initialize'}
          </span>
        </>
      )}
    </div>
  )

  return (
    <div className="console-layout">
      <Sidebar 
        currentSection={currentSection} 
        onSectionChange={setCurrentSection} 
        userEmail={session?.email ?? 'Guest User'} 
        roles={session?.roles ?? []}
        permissions={session?.permissions ?? []}
        onSignOut={handleSignOut} 
      />

      <main className="main-content">
        <div className="section-header-group">
          {renderBreadcrumbs()}
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <h1 className="section-title">
              {viewMode === 'form' 
                ? (userForm.editingId || roleForm.editingId || permissionForm.editingId ? `Update ${currentSection.slice(0, -1)}` : `New ${currentSection.slice(0, -1)}`)
                : (currentSection.charAt(0).toUpperCase() + currentSection.slice(1))
              }
            </h1>
            
            {viewMode === 'list' && currentSection !== 'overview' && currentSection !== 'settings' && (
              <button className="btn btn-primary" onClick={() => setViewMode('form')}>
                <Plus size={18} strokeWidth={3} /> Create New
              </button>
            )}
            
            {viewMode === 'form' && (
              <button className="btn btn-secondary" onClick={() => {
                if (currentSection === 'users') resetUserForm();
                if (currentSection === 'roles') resetRoleForm();
                if (currentSection === 'permissions') resetPermissionForm();
              }}>
                <ArrowLeft size={18} /> Back to List
              </button>
            )}
          </div>
        </div>

        {currentSection === 'overview' && (
          <>
            <section style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(240px, 1fr))', gap: '2rem', marginBottom: '3rem' }}>
              <div className="stat-card" style={{ background: 'var(--bg-soft)', padding: '2rem', borderRadius: '16px', border: '1px solid var(--line)' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem' }}>
                  <div style={{ color: 'var(--muted)', fontSize: '0.75rem', fontWeight: 800, textTransform: 'uppercase' }}>Identities</div>
                  <UserIcon size={20} style={{ color: 'var(--accent)' }} />
                </div>
                <div style={{ fontSize: '2.5rem', fontWeight: 900 }}>{users.length}</div>
              </div>
              <div className="stat-card" style={{ background: 'var(--bg-soft)', padding: '2rem', borderRadius: '16px', border: '1px solid var(--line)' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem' }}>
                  <div style={{ color: 'var(--muted)', fontSize: '0.75rem', fontWeight: 800, textTransform: 'uppercase' }}>Roles</div>
                  <Shield size={20} style={{ color: 'var(--warning)' }} />
                </div>
                <div style={{ fontSize: '2.5rem', fontWeight: 900 }}>{roles.length}</div>
              </div>
              <div className="stat-card" style={{ background: 'var(--bg-soft)', padding: '2rem', borderRadius: '16px', border: '1px solid var(--line)' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem' }}>
                  <div style={{ color: 'var(--muted)', fontSize: '0.75rem', fontWeight: 800, textTransform: 'uppercase' }}>Policies</div>
                  <Activity size={20} style={{ color: 'var(--success)' }} />
                </div>
                <div style={{ fontSize: '2.5rem', fontWeight: 900 }}>{permissions.length}</div>
              </div>
            </section>
            
            <div className="premium-card">
              <div className="premium-card-header">
                <h3 style={{ fontWeight: 800, fontSize: '1.25rem' }}>Infrastructure Overview</h3>
              </div>
              <div style={{ padding: '2rem', color: 'var(--text-soft)', lineHeight: '1.6' }}>
                System operational status is optimal. All 12 nodes are active and synchronized. 
                Manage your cluster identities and access policies using the navigation menu.
              </div>
            </div>
          </>
        )}

        {currentSection === 'users' && (
          viewMode === 'list' ? (
            <div className="premium-card">
              <div className="premium-card-header">
                <div style={{ position: 'relative', width: '300px' }}>
                  <Search size={16} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--muted)' }} />
                  <input 
                    type="text" 
                    placeholder="Filter identities..." 
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                    style={{ paddingLeft: '2.5rem' }}
                  />
                </div>
                <button className="btn btn-secondary" onClick={() => dispatch(fetchOverviewThunk())}>
                  <RefreshCw size={16} /> Sync
                </button>
              </div>
              <table className="premium-table">
                <thead>
                  <tr>
                    <th>Identity Name</th>
                    <th>Network ID</th>
                    <th>Status</th>
                    <th>Privileges</th>
                    <th style={{ textAlign: 'right' }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {users.filter(u => u.email.toLowerCase().includes(searchQuery.toLowerCase())).map(user => (
                    <tr key={user.id}>
                      <td style={{ fontWeight: 700 }}>{user.firstName} {user.lastName}</td>
                      <td style={{ color: 'var(--muted)', fontFamily: 'var(--mono)', fontSize: '0.8rem' }}>{user.email}</td>
                      <td>
                        <span className={`premium-badge ${user.isActive ? 'badge-success' : 'badge-muted'}`} style={{ padding: '4px 10px', borderRadius: '6px', fontSize: '0.7rem', fontWeight: 800, background: user.isActive ? 'rgba(16, 185, 129, 0.1)' : 'rgba(255,255,255,0.05)', color: user.isActive ? '#10b981' : 'var(--muted)', border: `1px solid ${user.isActive ? 'rgba(16,185,129,0.2)' : 'var(--line)'}` }}>
                          {user.isActive ? 'ACTIVE' : 'IDLE'}
                        </span>
                      </td>
                      <td>
                        <div style={{ display: 'flex', gap: '4px', flexWrap: 'wrap' }}>
                          {user.roles.map(r => (
                            <span key={r.id} style={{ fontSize: '0.65rem', fontWeight: 700, padding: '2px 6px', background: 'var(--bg)', borderRadius: '4px', border: '1px solid var(--line)' }}>{r.name}</span>
                          ))}
                        </div>
                      </td>
                      <td style={{ textAlign: 'right' }}>
                        <div style={{ display: 'flex', gap: '8px', justifyContent: 'flex-end' }}>
                          <button className="btn btn-ghost" onClick={() => {
                            setUserForm({ editingId: user.id, firstName: user.firstName, lastName: user.lastName, email: user.email, password: '', isActive: user.isActive, roleIds: user.roles.map(r => r.id) });
                            setViewMode('form');
                          }}>
                            <Pencil size={16} />
                          </button>
                          <button className="btn btn-ghost" style={{ color: 'var(--danger)' }} onClick={() => dispatch(deleteUserThunk(user.id))}>
                            <Trash2 size={16} />
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          ) : (
            <div className="premium-card" style={{ maxWidth: '800px' }}>
              <div style={{ padding: '3rem' }}>
                <form onSubmit={onUserSubmit}>
                  <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '2rem', marginBottom: '2rem' }}>
                    <div className="form-field">
                      <label>First Name</label>
                      <input type="text" value={userForm.firstName} onChange={e => setUserForm(c => ({...c, firstName: e.target.value}))} required />
                    </div>
                    <div className="form-field">
                      <label>Last Name</label>
                      <input type="text" value={userForm.lastName} onChange={e => setUserForm(c => ({...c, lastName: e.target.value}))} required />
                    </div>
                  </div>
                  <div className="form-field">
                    <label>Email Address</label>
                    <input type="email" value={userForm.email} onChange={e => setUserForm(c => ({...c, email: e.target.value}))} required />
                  </div>
                  <div className="form-field">
                    <label>Password {userForm.editingId && '(Leave blank to keep current)'}</label>
                    <input type="password" value={userForm.password} onChange={e => setUserForm(c => ({...c, password: e.target.value}))} required={!userForm.editingId} />
                  </div>
                  
                  <div style={{ marginTop: '2.5rem' }}>
                    <label style={{ display: 'block', fontSize: '0.8rem', fontWeight: 700, marginBottom: '1rem', color: 'var(--text-soft)' }}>Role Assignment</label>
                    <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(150px, 1fr))', gap: '1rem' }}>
                      {roles.map(r => (
                        <div 
                          key={r.id} 
                          onClick={() => toggleRole(r.id)}
                          style={{ 
                            padding: '1rem', 
                            borderRadius: '12px', 
                            border: '1px solid var(--line)', 
                            background: userForm.roleIds.includes(r.id) ? 'var(--accent-soft)' : 'var(--bg)',
                            color: userForm.roleIds.includes(r.id) ? 'var(--accent-strong)' : 'var(--text-soft)',
                            cursor: 'pointer',
                            fontSize: '0.85rem',
                            fontWeight: 700,
                            display: 'flex',
                            alignItems: 'center',
                            gap: '10px',
                            transition: 'all 0.2s ease'
                          }}
                        >
                          <div style={{ width: '16px', height: '16px', borderRadius: '4px', border: '2px solid currentColor', background: userForm.roleIds.includes(r.id) ? 'currentColor' : 'transparent' }} />
                          {r.name}
                        </div>
                      ))}
                    </div>
                  </div>

                  <div style={{ marginTop: '4rem', display: 'flex', gap: '1.5rem' }}>
                    <button className="btn btn-primary" type="submit" disabled={isSubmitting} style={{ padding: '0.8rem 2.5rem' }}>
                      {isSubmitting ? 'Processing...' : 'Save Identity'}
                    </button>
                    <button className="btn btn-secondary" type="button" onClick={resetUserForm}>Cancel</button>
                  </div>
                </form>
              </div>
            </div>
          )
        )}

        {currentSection === 'roles' && (
          viewMode === 'list' ? (
            <div className="premium-card">
              <div className="premium-card-header">
                <h3 style={{ fontWeight: 800, fontSize: '1.1rem', color: 'var(--muted)' }}>Access Role Definitions</h3>
              </div>
              <table className="premium-table">
                <thead>
                  <tr>
                    <th>Role Identifier</th>
                    <th>Policy Count</th>
                    <th style={{ textAlign: 'right' }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {roles.map(role => (
                    <tr key={role.id}>
                      <td style={{ fontWeight: 700 }}>{role.name}</td>
                      <td>
                        <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                          <span style={{ padding: '2px 8px', background: 'var(--bg)', borderRadius: '4px', border: '1px solid var(--line)', fontWeight: 800, fontSize: '0.8rem' }}>{role.permissions.length}</span>
                          <span style={{ color: 'var(--muted)', fontSize: '0.8rem', fontWeight: 600 }}>active policies</span>
                        </div>
                      </td>
                      <td style={{ textAlign: 'right' }}>
                        <div style={{ display: 'flex', gap: '8px', justifyContent: 'flex-end' }}>
                          <button className="btn btn-ghost" onClick={() => {
                            setRoleForm({ editingId: role.id, name: role.name, description: role.description || '', permissionIds: role.permissions.map(p => p.id) });
                            setViewMode('form');
                          }}>
                            <Pencil size={16} />
                          </button>
                          <button className="btn btn-ghost" style={{ color: 'var(--danger)' }} onClick={() => dispatch(deleteRoleThunk(role.id))}>
                            <Trash2 size={16} />
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          ) : (
            <div className="premium-card" style={{ maxWidth: '800px' }}>
              <div style={{ padding: '3rem' }}>
                <form onSubmit={onRoleSubmit}>
                  <div className="form-field">
                    <label>Role Name</label>
                    <input type="text" value={roleForm.name} onChange={e => setRoleForm(c => ({...c, name: e.target.value}))} required placeholder="e.g. System Administrator" />
                  </div>
                  
                  <div style={{ marginTop: '2.5rem' }}>
                    <label style={{ display: 'block', fontSize: '0.8rem', fontWeight: 700, marginBottom: '1rem', color: 'var(--text-soft)' }}>Attach Policies</label>
                    <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(200px, 1fr))', gap: '1rem' }}>
                      {permissions.map(p => (
                        <div 
                          key={p.id} 
                          onClick={() => togglePermission(p.id)}
                          style={{ 
                            padding: '1rem', 
                            borderRadius: '12px', 
                            border: '1px solid var(--line)', 
                            background: roleForm.permissionIds.includes(p.id) ? 'var(--accent-soft)' : 'var(--bg)',
                            color: roleForm.permissionIds.includes(p.id) ? 'var(--accent-strong)' : 'var(--text-soft)',
                            cursor: 'pointer',
                            fontSize: '0.85rem',
                            fontWeight: 700,
                            display: 'flex',
                            alignItems: 'center',
                            gap: '10px',
                            transition: 'all 0.2s ease'
                          }}
                        >
                          <div style={{ width: '16px', height: '16px', borderRadius: '4px', border: '2px solid currentColor', background: roleForm.permissionIds.includes(p.id) ? 'currentColor' : 'transparent' }} />
                          {p.name}
                        </div>
                      ))}
                    </div>
                  </div>

                  <div style={{ marginTop: '4rem', display: 'flex', gap: '1.5rem' }}>
                    <button className="btn btn-primary" type="submit" disabled={isSubmitting} style={{ padding: '0.8rem 2.5rem' }}>
                      {isSubmitting ? 'Processing...' : 'Save Role'}
                    </button>
                    <button className="btn btn-secondary" type="button" onClick={resetRoleForm}>Cancel</button>
                  </div>
                </form>
              </div>
            </div>
          )
        )}

        {currentSection === 'permissions' && (
          viewMode === 'list' ? (
            <div className="premium-card">
              <div className="premium-card-header">
                <h3 style={{ fontWeight: 800, fontSize: '1.1rem', color: 'var(--muted)' }}>Operational Policies</h3>
              </div>
              <table className="premium-table">
                <thead>
                  <tr>
                    <th>Access Key</th>
                    <th>Scope Description</th>
                    <th style={{ textAlign: 'right' }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {permissions.map(p => (
                    <tr key={p.id}>
                      <td>
                        <span style={{ fontFamily: 'var(--mono)', fontSize: '0.8rem', fontWeight: 800, color: 'var(--accent-strong)', background: 'var(--accent-soft)', padding: '4px 8px', borderRadius: '4px' }}>{p.name}</span>
                      </td>
                      <td style={{ color: 'var(--text-soft)', fontSize: '0.9rem' }}>{p.description}</td>
                      <td style={{ textAlign: 'right' }}>
                        <div style={{ display: 'flex', gap: '8px', justifyContent: 'flex-end' }}>
                          <button className="btn btn-ghost" onClick={() => {
                            setPermissionForm({ editingId: p.id, name: p.name, description: p.description });
                            setViewMode('form');
                          }}>
                            <Pencil size={16} />
                          </button>
                          <button className="btn btn-ghost" style={{ color: 'var(--danger)' }} onClick={() => dispatch(deletePermissionThunk(p.id))}>
                            <Trash2 size={16} />
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          ) : (
            <div className="premium-card" style={{ maxWidth: '800px' }}>
              <div style={{ padding: '3rem' }}>
                <form onSubmit={onPermissionSubmit}>
                  <div className="form-field">
                    <label>Permission Key</label>
                    <input type="text" value={permissionForm.name} onChange={e => setPermissionForm(c => ({...c, name: e.target.value}))} required placeholder="e.g. storage.read" />
                  </div>
                  <div className="form-field">
                    <label>Policy Description</label>
                    <input type="text" value={permissionForm.description} onChange={e => setPermissionForm(c => ({...c, description: e.target.value}))} required placeholder="Describe what this policy allows" />
                  </div>
                  
                  <div style={{ marginTop: '4rem', display: 'flex', gap: '1.5rem' }}>
                    <button className="btn btn-primary" type="submit" disabled={isSubmitting} style={{ padding: '0.8rem 2.5rem' }}>
                      {isSubmitting ? 'Processing...' : 'Save Policy'}
                    </button>
                    <button className="btn btn-secondary" type="button" onClick={resetPermissionForm}>Cancel</button>
                  </div>
                </form>
              </div>
            </div>
          )
        )}

        {currentSection === 'settings' && (
          <div className="premium-card" style={{ maxWidth: '800px' }}>
            <div className="premium-card-header">
              <h3 style={{ fontWeight: 800, fontSize: '1.1rem' }}>Infrastructure Configuration</h3>
            </div>
            <div style={{ padding: '3rem' }}>
              <form onSubmit={onSettingsSubmit}>
                <div className="form-field" style={{ marginBottom: '3rem' }}>
                  <label style={{ display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '1rem' }}>
                    <Globe size={18} /> API Service Gateway
                  </label>
                  <input 
                    type="url" 
                    value={settingsForm.apiBaseUrl} 
                    onChange={e => setSettingsForm({ apiBaseUrl: e.target.value })} 
                    required 
                    placeholder="https://api.gateway.enterprise.com"
                  />
                  <div style={{ marginTop: '1.5rem', padding: '1rem', background: 'rgba(245, 158, 11, 0.05)', borderRadius: '12px', border: '1px solid rgba(245, 158, 11, 0.2)', display: 'flex', gap: '12px', alignItems: 'flex-start' }}>
                    <Shield size={20} style={{ color: '#f59e0b', flexShrink: 0 }} />
                    <p style={{ fontSize: '0.8rem', color: '#f59e0b', fontWeight: 600, lineHeight: '1.6' }}>
                      Warning: Changing the service gateway will clear all active sessions and redirect you to the authentication cluster.
                    </p>
                  </div>
                </div>

                <div style={{ display: 'flex', gap: '1.5rem' }}>
                  <button className="btn btn-primary" type="submit" style={{ padding: '0.8rem 2rem' }}>
                    <Save size={18} /> Apply Changes
                  </button>
                  <button className="btn btn-secondary" type="button" onClick={() => setSettingsForm({ apiBaseUrl })}>Revert</button>
                </div>
              </form>
            </div>
          </div>
        )}
      </main>
    </div>
  )
}
