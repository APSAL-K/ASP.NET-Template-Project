import { useState } from 'react'
import { Globe, Shield, Save } from 'lucide-react'
import { useAppDispatch, useAppSelector } from '../../../../app/hooks'
import { setApiBaseUrl, pushToast } from '../../../../features/ui/uiSlice'

export function SettingsSection() {
  const dispatch = useAppDispatch()
  const apiBaseUrl = useAppSelector((state) => state.ui.apiBaseUrl)
  const [settingsForm, setSettingsForm] = useState({ apiBaseUrl })

  const onSettingsSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    dispatch(setApiBaseUrl(settingsForm.apiBaseUrl))
    dispatch(pushToast({ kind: 'success', title: 'Config Saved', description: 'System API endpoint updated successfully.' }))
  }

  return (
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

          <div style={{ display: 'flex', gap: '1.5rem', flexWrap: 'wrap' }}>
            <button className="btn btn-primary" type="submit" style={{ padding: '0.8rem 2rem' }}>
              <Save size={18} /> Apply Changes
            </button>
            <button className="btn btn-secondary" type="button" onClick={() => setSettingsForm({ apiBaseUrl })}>Revert</button>
          </div>
        </form>
      </div>
    </div>
  )
}
