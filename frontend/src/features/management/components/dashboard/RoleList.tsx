import { useState } from 'react'
import { Pencil, Trash2, RefreshCw } from 'lucide-react'
import { useAppDispatch, useAppSelector } from '../../../../app/hooks'
import { fetchRolesThunk, deleteRoleThunk } from '../../managementThunks'
import { Pagination } from '../../../../components/Pagination'
import { ConfirmModal } from '../../../../components/ConfirmModal'
import { pushToast } from '../../../../features/ui/uiSlice'

interface RoleListProps {
  onEdit: (role: any) => void
}

export function RoleList({ onEdit }: RoleListProps) {
  const dispatch = useAppDispatch()
  const { roles } = useAppSelector((state) => state.management)
  const [deleteConfirm, setDeleteConfirm] = useState<{ isOpen: boolean; roleId: string | null }>({
    isOpen: false,
    roleId: null,
  })

  const handleDeleteClick = (roleId: string) => {
    setDeleteConfirm({ isOpen: true, roleId })
  }

  const handleConfirmDelete = async () => {
    if (deleteConfirm.roleId) {
      const result = await dispatch(deleteRoleThunk(deleteConfirm.roleId))
      if (deleteRoleThunk.fulfilled.match(result)) {
        dispatch(pushToast({ kind: 'success', title: 'Role Purged', description: 'The access role has been successfully removed from the cluster configuration.' }))
      }
    }
    setDeleteConfirm({ isOpen: false, roleId: null })
  }

  return (
    <div className="premium-card">
      <div className="premium-card-header">
        <h3 style={{ fontWeight: 800, fontSize: '1.1rem', color: 'var(--muted)' }}>Access Role Definitions</h3>
        <button className="btn btn-secondary" onClick={() => dispatch(fetchRolesThunk({ page: roles.pageNumber }))}>
          <RefreshCw size={16} /> Sync
        </button>
      </div>
      <div className="table-wrapper">
        <table className="premium-table">
          <thead>
            <tr>
              <th>Role Identifier</th>
              <th>Policy Count</th>
              <th style={{ textAlign: 'right' }}>Actions</th>
            </tr>
          </thead>
          <tbody>
            {roles.items.map(role => (
              <tr key={role.id}>
                <td data-label="Role Identifier" style={{ fontWeight: 700 }}>{role.name}</td>
                <td data-label="Policy Count">
                  <div style={{ display: 'flex', alignItems: 'center', gap: '8px', justifyContent: 'flex-end' }}>
                    <span style={{ padding: '2px 8px', background: 'var(--bg)', borderRadius: '4px', border: '1px solid var(--line)', fontWeight: 800, fontSize: '0.8rem' }}>{role.permissions.length}</span>
                    <span style={{ color: 'var(--muted)', fontSize: '0.8rem', fontWeight: 600 }}>active policies</span>
                  </div>
                </td>
                <td data-label="Actions" style={{ textAlign: 'right' }}>
                  <div style={{ display: 'flex', gap: '8px', justifyContent: 'flex-end' }}>
                    <button className="btn btn-ghost" onClick={() => onEdit(role)}>
                      <Pencil size={16} />
                    </button>
                    <button className="btn btn-ghost" style={{ color: 'var(--danger)' }} onClick={() => handleDeleteClick(role.id)}>
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
        currentPage={roles.pageNumber}
        totalPages={roles.totalPages}
        totalItems={roles.totalItems}
        pageSize={roles.pageSize}
        onPageChange={(p) => dispatch(fetchRolesThunk({ page: p }))}
      />

      <ConfirmModal 
        isOpen={deleteConfirm.isOpen}
        title="Delete Access Role?"
        message="This will immediately revoke access for all users currently assigned to this role. This action is irreversible."
        confirmText="Purge Role"
        onConfirm={handleConfirmDelete}
        onCancel={() => setDeleteConfirm({ isOpen: false, roleId: null })}
      />
    </div>
  )
}
