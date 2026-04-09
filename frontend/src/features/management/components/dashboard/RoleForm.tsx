import { useState, useEffect } from 'react'
import { useAppDispatch, useAppSelector } from '../../../../app/hooks'
import { createRoleThunk, updateRoleThunk } from '../../managementThunks'
import { pushToast } from '../../../../features/ui/uiSlice'

interface RoleFormProps {
  initialData?: any
  isSubmitting: boolean
  setIsSubmitting: (submitting: boolean) => void
  onSuccess: () => void
  onCancel: () => void
}

export function RoleForm({ initialData, isSubmitting, setIsSubmitting, onSuccess, onCancel }: RoleFormProps) {
  const dispatch = useAppDispatch()
  const { permissions } = useAppSelector((state) => state.management)
  
  const [form, setForm] = useState({ 
    name: '', 
    description: '', 
    permissionIds: [] as string[] 
  })

  useEffect(() => {
    if (initialData) {
      setForm({
        name: initialData.name,
        description: initialData.description || '',
        permissionIds: initialData.permissions.map((p: any) => p.id),
      })
    }
  }, [initialData])

  const togglePermission = (permId: string) => {
    setForm(prev => {
      const isSelected = prev.permissionIds.includes(permId)
      return {
        ...prev,
        permissionIds: isSelected 
          ? prev.permissionIds.filter(id => id !== permId)
          : [...prev.permissionIds, permId]
      }
    })
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setIsSubmitting(true)
    try {
      const action = initialData?.id
        ? dispatch(updateRoleThunk({ id: initialData.id, payload: { name: form.name, description: form.description, permissionIds: form.permissionIds } }))
        : dispatch(createRoleThunk({ name: form.name, description: form.description, permissionIds: form.permissionIds }))
        
      const result = await action
      if (createRoleThunk.fulfilled.match(result) || updateRoleThunk.fulfilled.match(result)) {
        dispatch(pushToast({ 
          kind: 'success', 
          title: 'Directives Saved', 
          description: initialData?.id ? 'Access role configuration updated.' : 'New role definition initialized.'
        }))
        onSuccess()
      }
    } catch (e: any) {
      // Error handled by thunk
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="premium-card" style={{ maxWidth: '800px' }}>
      <div style={{ padding: '3rem' }}>
        <form onSubmit={handleSubmit}>
          <div className="form-field">
            <label>Role Name</label>
            <input type="text" value={form.name} onChange={e => setForm(c => ({...c, name: e.target.value}))} required placeholder="e.g. System Administrator" />
          </div>
          
          <div style={{ marginTop: '2.5rem' }}>
            <label style={{ display: 'block', fontSize: '0.8rem', fontWeight: 700, marginBottom: '1rem', color: 'var(--text-soft)' }}>Attach Policies</label>
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(200px, 1fr))', gap: '1rem' }}>
              {permissions.items.map(p => (
                <div 
                  key={p.id} 
                  onClick={() => togglePermission(p.id)}
                  style={{ 
                    padding: '1rem', 
                    borderRadius: '12px', 
                    border: '1px solid var(--line)', 
                    background: form.permissionIds.includes(p.id) ? 'var(--accent-soft)' : 'var(--bg)',
                    color: form.permissionIds.includes(p.id) ? 'var(--accent-strong)' : 'var(--text-soft)',
                    cursor: 'pointer',
                    fontSize: '0.85rem',
                    fontWeight: 700,
                    display: 'flex',
                    alignItems: 'center',
                    gap: '10px',
                    transition: 'all 0.2s ease'
                  }}
                >
                  <div style={{ width: '16px', height: '16px', borderRadius: '4px', border: '2px solid currentColor', background: form.permissionIds.includes(p.id) ? 'currentColor' : 'transparent' }} />
                  {p.name}
                </div>
              ))}
            </div>
          </div>

          <div style={{ marginTop: '4rem', display: 'flex', gap: '1.5rem', flexWrap: 'wrap' }}>
            <button className="btn btn-primary" type="submit" disabled={isSubmitting} style={{ padding: '0.8rem 2.5rem' }}>
              {isSubmitting ? 'Processing...' : (initialData?.id ? 'Update Role' : 'Create Role')}
            </button>
            <button className="btn btn-secondary" type="button" onClick={onCancel}>Cancel</button>
          </div>
        </form>
      </div>
    </div>
  )
}
