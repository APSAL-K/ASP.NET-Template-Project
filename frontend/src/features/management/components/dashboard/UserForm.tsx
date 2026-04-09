import { useState, useEffect } from 'react'
import { useAppDispatch, useAppSelector } from '../../../../app/hooks'
import { createUserThunk, updateUserThunk } from '../../managementThunks'
import { pushToast } from '../../../../features/ui/uiSlice'

interface UserFormProps {
  initialData?: any
  isSubmitting: boolean
  setIsSubmitting: (submitting: boolean) => void
  onSuccess: () => void
  onCancel: () => void
}

export function UserForm({ initialData, isSubmitting, setIsSubmitting, onSuccess, onCancel }: UserFormProps) {
  const dispatch = useAppDispatch()
  const { roles } = useAppSelector((state) => state.management)
  
  const [form, setForm] = useState({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    isActive: true,
    roleIds: [] as string[],
  })

  useEffect(() => {
    if (initialData) {
      setForm({
        firstName: initialData.firstName,
        lastName: initialData.lastName,
        email: initialData.email,
        password: '',
        isActive: initialData.isActive,
        roleIds: initialData.roles.map((r: any) => r.id),
      })
    }
  }, [initialData])

  const toggleRole = (roleId: string) => {
    setForm(prev => {
      const isSelected = prev.roleIds.includes(roleId)
      return {
        ...prev,
        roleIds: isSelected 
          ? prev.roleIds.filter(id => id !== roleId)
          : [...prev.roleIds, roleId]
      }
    })
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setIsSubmitting(true)
    try {
      const basePayload = { 
        firstName: form.firstName, 
        lastName: form.lastName, 
        email: form.email, 
        isActive: form.isActive, 
        roleIds: form.roleIds 
      }
      
      const action = initialData?.id
        ? dispatch(updateUserThunk({ id: initialData.id, payload: { ...basePayload, password: form.password || undefined } }))
        : dispatch(createUserThunk({ ...basePayload, password: form.password }))
        
      const result = await action
      if (createUserThunk.fulfilled.match(result) || updateUserThunk.fulfilled.match(result)) {
        dispatch(pushToast({ 
          kind: 'success', 
          title: 'Records Updated', 
          description: initialData?.id ? 'User identity modified successfully.' : 'New identity initialized in the cluster.'
        }))
        onSuccess()
      }
    } catch (error) {
      // Errors are handled by middleware toast logic typically
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="premium-card" style={{ maxWidth: '800px' }}>
      <div style={{ padding: '3rem' }}>
        <form onSubmit={handleSubmit}>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(300px, 1fr))', gap: '2rem', marginBottom: '2rem' }}>
            <div className="form-field">
              <label>First Name</label>
              <input type="text" value={form.firstName} onChange={e => setForm(c => ({...c, firstName: e.target.value}))} required />
            </div>
            <div className="form-field">
              <label>Last Name</label>
              <input type="text" value={form.lastName} onChange={e => setForm(c => ({...c, lastName: e.target.value}))} required />
            </div>
          </div>
          <div className="form-field">
            <label>Email Address</label>
            <input type="email" value={form.email} onChange={e => setForm(c => ({...c, email: e.target.value}))} required />
          </div>
          <div className="form-field">
            <label>Password {initialData?.id && '(Leave blank to keep current)'}</label>
            <input type="password" value={form.password} onChange={e => setForm(c => ({...c, password: e.target.value}))} required={!initialData?.id} />
          </div>
          
          <div style={{ marginTop: '2.5rem' }}>
            <label style={{ display: 'block', fontSize: '0.8rem', fontWeight: 700, marginBottom: '1rem', color: 'var(--text-soft)' }}>Role Assignment</label>
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(150px, 1fr))', gap: '1rem' }}>
              {roles.items.map(r => (
                <div 
                  key={r.id} 
                  onClick={() => toggleRole(r.id)}
                  style={{ 
                    padding: '1rem', 
                    borderRadius: '12px', 
                    border: '1px solid var(--line)', 
                    background: form.roleIds.includes(r.id) ? 'var(--accent-soft)' : 'var(--bg)',
                    color: form.roleIds.includes(r.id) ? 'var(--accent-strong)' : 'var(--text-soft)',
                    cursor: 'pointer',
                    fontSize: '0.85rem',
                    fontWeight: 700,
                    display: 'flex',
                    alignItems: 'center',
                    gap: '10px',
                    transition: 'all 0.2s ease'
                  }}
                >
                  <div style={{ width: '16px', height: '16px', borderRadius: '4px', border: '2px solid currentColor', background: form.roleIds.includes(r.id) ? 'currentColor' : 'transparent' }} />
                  {r.name}
                </div>
              ))}
            </div>
          </div>

          <div style={{ marginTop: '4rem', display: 'flex', gap: '1.5rem', flexWrap: 'wrap' }}>
            <button className="btn btn-primary" type="submit" disabled={isSubmitting} style={{ padding: '0.8rem 2.5rem' }}>
              {isSubmitting ? 'Processing...' : (initialData?.id ? 'Update identity' : 'Initialize Identity')}
            </button>
            <button className="btn btn-secondary" type="button" onClick={onCancel}>Cancel</button>
          </div>
        </form>
      </div>
    </div>
  )
}
