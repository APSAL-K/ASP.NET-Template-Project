import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { 
  Plus, 
  ChevronRight,
  ArrowLeft
} from 'lucide-react'
import { useAppDispatch, useAppSelector } from '../app/hooks'
import { clearStoredSession } from '../app/localStorage'
import { signOut } from '../features/auth/authSlice'
import { fetchOverviewThunk } from '../features/management/managementThunks'
import { Sidebar } from '../components/Sidebar'

// Refactored Components
import { OverviewSection } from '../features/management/components/dashboard/OverviewSection'
import { UserList } from '../features/management/components/dashboard/UserList'
import { UserForm } from '../features/management/components/dashboard/UserForm'
import { RoleList } from '../features/management/components/dashboard/RoleList'
import { RoleForm } from '../features/management/components/dashboard/RoleForm'
import { PermissionList } from '../features/management/components/dashboard/PermissionList'
import { PermissionForm } from '../features/management/components/dashboard/PermissionForm'
import { SettingsSection } from '../features/management/components/dashboard/SettingsSection'

export function DashboardPage() {
  const dispatch = useAppDispatch()
  const navigate = useNavigate()
  const session = useAppSelector((state) => state.auth.session)
  const { overviewStatus } = useAppSelector((state) => state.management)
  
  const [currentSection, setCurrentSection] = useState('overview')
  const [viewMode, setViewMode] = useState<'list' | 'form'>('list')
  const [editingData, setEditingData] = useState<any>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)

  useEffect(() => {
    if (overviewStatus === 'idle') {
      void dispatch(fetchOverviewThunk())
    }
  }, [dispatch, overviewStatus])

  useEffect(() => {
    setViewMode('list')
    setEditingData(null)
  }, [currentSection])

  const handleSignOut = () => {
    clearStoredSession()
    dispatch(signOut())
    navigate('/sign-in', { replace: true })
  }

  const handleEdit = (data: any) => {
    setEditingData(data)
    setViewMode('form')
  }

  const handleSuccess = () => {
    setEditingData(null)
    setViewMode('list')
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
            {editingData ? 'Edit' : 'Initialize'}
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
                ? `${editingData ? 'Configure' : 'Create'} ${currentSection.slice(0, -1)}`
                : (currentSection.charAt(0).toUpperCase() + currentSection.slice(1))
              }
            </h1>
            
            {viewMode === 'list' && currentSection !== 'overview' && currentSection !== 'settings' && (
              <button className="btn btn-primary" onClick={() => { setEditingData(null); setViewMode('form'); }}>
                <Plus size={18} strokeWidth={3} /> Create New
              </button>
            )}
            
            {viewMode === 'form' && (
              <button className="btn btn-secondary" onClick={() => { setEditingData(null); setViewMode('list'); }}>
                <ArrowLeft size={18} /> Back to List
              </button>
            )}
          </div>
        </div>

        {currentSection === 'overview' && <OverviewSection />}
        
        {currentSection === 'users' && (
          viewMode === 'list' 
            ? <UserList onEdit={handleEdit} />
            : <UserForm 
                initialData={editingData} 
                isSubmitting={isSubmitting} 
                setIsSubmitting={setIsSubmitting} 
                onSuccess={handleSuccess} 
                onCancel={handleSuccess} 
              />
        )}

        {currentSection === 'roles' && (
          viewMode === 'list'
            ? <RoleList onEdit={handleEdit} />
            : <RoleForm 
                initialData={editingData} 
                isSubmitting={isSubmitting} 
                setIsSubmitting={setIsSubmitting} 
                onSuccess={handleSuccess} 
                onCancel={handleSuccess} 
              />
        )}

        {currentSection === 'permissions' && (
          viewMode === 'list'
            ? <PermissionList onEdit={handleEdit} />
            : <PermissionForm 
                initialData={editingData} 
                isSubmitting={isSubmitting} 
                setIsSubmitting={setIsSubmitting} 
                onSuccess={handleSuccess} 
                onCancel={handleSuccess} 
              />
        )}

        {currentSection === 'settings' && <SettingsSection />}
      </main>
    </div>
  )
}
