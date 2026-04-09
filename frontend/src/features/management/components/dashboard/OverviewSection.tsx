import { User as UserIcon, Shield, Activity } from 'lucide-react'
import { useAppSelector } from '../../../../app/hooks'

export function OverviewSection() {
  const { permissions, roles, users } = useAppSelector((state) => state.management)

  return (
    <>
      <section style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(240px, 1fr))', gap: '2rem', marginBottom: '3rem' }}>
        <div className="stat-card" style={{ background: 'var(--bg-soft)', padding: '2rem', borderRadius: '16px', border: '1px solid var(--line)' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem' }}>
            <div style={{ color: 'var(--muted)', fontSize: '0.75rem', fontWeight: 800, textTransform: 'uppercase' }}>Identities</div>
            <UserIcon size={20} style={{ color: 'var(--accent)' }} />
          </div>
          <div style={{ fontSize: '2.5rem', fontWeight: 900 }}>{users.totalItems}</div>
        </div>
        <div className="stat-card" style={{ background: 'var(--bg-soft)', padding: '2rem', borderRadius: '16px', border: '1px solid var(--line)' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem' }}>
            <div style={{ color: 'var(--muted)', fontSize: '0.75rem', fontWeight: 800, textTransform: 'uppercase' }}>Roles</div>
            <Shield size={20} style={{ color: 'var(--warning)' }} />
          </div>
          <div style={{ fontSize: '2.5rem', fontWeight: 900 }}>{roles.totalItems}</div>
        </div>
        <div className="stat-card" style={{ background: 'var(--bg-soft)', padding: '2rem', borderRadius: '16px', border: '1px solid var(--line)' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem' }}>
            <div style={{ color: 'var(--muted)', fontSize: '0.75rem', fontWeight: 800, textTransform: 'uppercase' }}>Policies</div>
            <Activity size={20} style={{ color: 'var(--success)' }} />
          </div>
          <div style={{ fontSize: '2.5rem', fontWeight: 900 }}>{permissions.totalItems}</div>
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
  )
}
