import { useState } from 'react'
import { Pencil, Trash2, Search, RefreshCw } from 'lucide-react'
import { useAppDispatch, useAppSelector } from '../../../../app/hooks'
import { fetchUsersThunk, deleteUserThunk } from '../../managementThunks'
import { Pagination } from '../../../../components/Pagination'
import { ConfirmModal } from '../../../../components/ConfirmModal'
import { pushToast } from '../../../../features/ui/uiSlice'

interface UserListProps {
  onEdit: (user: any) => void
}

export function UserList({ onEdit }: UserListProps) {
  const dispatch = useAppDispatch()
  const { users } = useAppSelector((state) => state.management)
  const [searchQuery, setSearchQuery] = useState('')
  const [deleteConfirm, setDeleteConfirm] = useState<{ isOpen: boolean; userId: string | null }>({
    isOpen: false,
    userId: null,
  })

  const handleDeleteClick = (userId: string) => {
    setDeleteConfirm({ isOpen: true, userId })
  }

  const handleConfirmDelete = async () => {
    if (deleteConfirm.userId) {
      const result = await dispatch(deleteUserThunk(deleteConfirm.userId))
      if (deleteUserThunk.fulfilled.match(result)) {
        dispatch(pushToast({ kind: 'success', title: 'Identity Revoked', description: 'User account has been successfully removed from the system.' }))
      }
    }
    setDeleteConfirm({ isOpen: false, userId: null })
  }

  return (
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
        <button className="btn btn-secondary" onClick={() => dispatch(fetchUsersThunk({ page: users.pageNumber }))}>
          <RefreshCw size={16} /> Sync
        </button>
      </div>
      <div className="table-wrapper">
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
            {users.items.filter(u => u.email.toLowerCase().includes(searchQuery.toLowerCase())).map(user => (
              <tr key={user.id}>
                <td data-label="Identity Name" style={{ fontWeight: 700 }}>{user.firstName} {user.lastName}</td>
                <td data-label="Network ID" style={{ color: 'var(--muted)', fontFamily: 'var(--mono)', fontSize: '0.8rem' }}>{user.email}</td>
                <td data-label="Status">
                  <span className={`premium-badge ${user.isActive ? 'badge-success' : 'badge-muted'}`} style={{ padding: '4px 10px', borderRadius: '6px', fontSize: '0.7rem', fontWeight: 800, background: user.isActive ? 'rgba(16, 185, 129, 0.1)' : 'rgba(255,255,255,0.05)', color: user.isActive ? '#10b981' : 'var(--muted)', border: `1px solid ${user.isActive ? 'rgba(16,185,129,0.2)' : 'var(--line)'}` }}>
                    {user.isActive ? 'ACTIVE' : 'IDLE'}
                  </span>
                </td>
                <td data-label="Privileges">
                  <div style={{ display: 'flex', gap: '4px', flexWrap: 'wrap', justifyContent: 'flex-end' }}>
                    {user.roles.map(r => (
                      <span key={r.id} style={{ fontSize: '0.65rem', fontWeight: 700, padding: '2px 6px', background: 'var(--bg)', borderRadius: '4px', border: '1px solid var(--line)' }}>{r.name}</span>
                    ))}
                  </div>
                </td>
                <td data-label="Actions" style={{ textAlign: 'right' }}>
                  <div style={{ display: 'flex', gap: '8px', justifyContent: 'flex-end' }}>
                    <button className="btn btn-ghost" onClick={() => onEdit(user)}>
                      <Pencil size={16} />
                    </button>
                    <button className="btn btn-ghost" style={{ color: 'var(--danger)' }} onClick={() => handleDeleteClick(user.id)}>
                      <Trash2 size={16} />
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      <Pagination 
        currentPage={users.pageNumber}
        totalPages={users.totalPages}
        totalItems={users.totalItems}
        pageSize={users.pageSize}
        onPageChange={(p) => dispatch(fetchUsersThunk({ page: p }))}
      />

      <ConfirmModal 
        isOpen={deleteConfirm.isOpen}
        title="Revoke Identity?"
        message="This action will permanently remove this user and all associated session tokens. This cannot be undone."
        confirmText="Revoke Access"
        onConfirm={handleConfirmDelete}
        onCancel={() => setDeleteConfirm({ isOpen: false, userId: null })}
      />
    </div>
  )
}
