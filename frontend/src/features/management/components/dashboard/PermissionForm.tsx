import { useState, useEffect } from 'react'
import { useAppDispatch } from '../../../../app/hooks'
import { createPermissionThunk, updatePermissionThunk } from '../../managementThunks'
import { pushToast } from '../../../../features/ui/uiSlice'

interface PermissionFormProps {
  initialData?: any
  isSubmitting: boolean
  setIsSubmitting: (submitting: boolean) => void
  onSuccess: () => void
  onCancel: () => void
}

export function PermissionForm({ initialData, isSubmitting, setIsSubmitting, onSuccess, onCancel }: PermissionFormProps) {
  const dispatch = useAppDispatch()
  const [form, setForm] = useState({ name: '', description: '' })

  useEffect(() => {
    if (initialData) {
      setForm({
        name: initialData.name,
        description: initialData.description,
      })
    }
  }, [initialData])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setIsSubmitting(true)
    try {
      const action = initialData?.id 
        ? dispatch(updatePermissionThunk({ id: initialData.id, payload: { name: form.name, description: form.description } }))
        : dispatch(createPermissionThunk({ name: form.name, description: form.description }))
        
      const result = await action
      if (createPermissionThunk.fulfilled.match(result) || updatePermissionThunk.fulfilled.match(result)) {
        dispatch(pushToast({ 
          kind: 'success', 
          title: 'Directives Saved', 
          description: initialData?.id ? 'Operational policy updated' : 'New permission key initialized.'
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
            <label>Permission Key</label>
            <input type="text" value={form.name} onChange={e => setForm(c => ({...c, name: e.target.value}))} required placeholder="e.g. storage.read" />
          </div>
          <div className="form-field">
            <label>Policy Description</label>
            <input type="text" value={form.description} onChange={e => setForm(c => ({...c, description: e.target.value}))} required placeholder="Describe what this policy allows" />
          </div>
          
          <div style={{ marginTop: '4rem', display: 'flex', gap: '1.5rem', flexWrap: 'wrap' }}>
            <button className="btn btn-primary" type="submit" disabled={isSubmitting} style={{ padding: '0.8rem 2.5rem' }}>
              {isSubmitting ? 'Processing...' : (initialData?.id ? 'Update policy' : 'Initialize key')}
            </button>
            <button className="btn btn-secondary" type="button" onClick={onCancel}>Cancel</button>
          </div>
        </form>
      </div>
    </div>
  )
}
