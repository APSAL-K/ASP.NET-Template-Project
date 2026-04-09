import { useState } from 'react'
import { Pencil, Trash2, RefreshCw } from 'lucide-react'
import { useAppDispatch, useAppSelector } from '../../../../app/hooks'
import { fetchPermissionsThunk, deletePermissionThunk } from '../../managementThunks'
import { Pagination } from '../../../../components/Pagination'
import { ConfirmModal } from '../../../../components/ConfirmModal'
import { pushToast } from '../../../../features/ui/uiSlice'

interface PermissionListProps {
  onEdit: (permission: any) => void
}

export function PermissionList({ onEdit }: PermissionListProps) {
  const dispatch = useAppDispatch()
  const { permissions } = useAppSelector((state) => state.management)
  const [deleteConfirm, setDeleteConfirm] = useState<{ isOpen: boolean; permId: string | null }>({
    isOpen: false,
    permId: null,
  })

  const handleDeleteClick = (permId: string) => {
    setDeleteConfirm({ isOpen: true, permId })
  }

  const handleConfirmDelete = async () => {
    if (deleteConfirm.permId) {
      const result = await dispatch(deletePermissionThunk(deleteConfirm.permId))
      if (deletePermissionThunk.fulfilled.match(result)) {
        dispatch(pushToast({ kind: 'success', title: 'Policy Erased', description: 'The operational permission has been successfully removed from the platform.' }))
      }
    }
    setDeleteConfirm({ isOpen: false, permId: null })
  }

  return (
    <div className="premium-card">
      <div className="premium-card-header">
        <h3 style={{ fontWeight: 800, fontSize: '1.1rem', color: 'var(--muted)' }}>Operational Policies</h3>
        <button className="btn btn-secondary" onClick={() => dispatch(fetchPermissionsThunk({ page: permissions.pageNumber }))}>
          <RefreshCw size={16} /> Sync
        </button>
      </div>
      <div className="table-wrapper">
        <table className="premium-table">
          <thead>
            <tr>
              <th>Access Key</th>
              <th>Scope Description</th>
              <th style={{ textAlign: 'right' }}>Actions</th>
            </tr>
          </thead>
          <tbody>
            {permissions.items.map(p => (
              <tr key={p.id}>
                <td data-label="Access Key">
                  <span style={{ fontFamily: 'var(--mono)', fontSize: '0.8rem', fontWeight: 800, color: 'var(--accent-strong)', background: 'var(--accent-soft)', padding: '4px 8px', borderRadius: '4px' }}>{p.name}</span>
                </td>
                <td data-label="Scope Description" style={{ color: 'var(--text-soft)', fontSize: '0.9rem' }}>{p.description}</td>
                <td data-label="Actions" style={{ textAlign: 'right' }}>
                  <div style={{ display: 'flex', gap: '8px', justifyContent: 'flex-end' }}>
                    <button className="btn btn-ghost" onClick={() => onEdit(p)}>
                      <Pencil size={16} />
                    </button>
                    <button className="btn btn-ghost" style={{ color: 'var(--danger)' }} onClick={() => handleDeleteClick(p.id)}>
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
        currentPage={permissions.pageNumber}
        totalPages={permissions.totalPages}
        totalItems={permissions.totalItems}
        pageSize={permissions.pageSize}
        onPageChange={(p) => dispatch(fetchPermissionsThunk({ page: p }))}
      />

      <ConfirmModal 
        isOpen={deleteConfirm.isOpen}
        title="Delete Operational Policy?"
        message="Deleting this policy may impact multiple active roles and dependent services. please verify the scope before confirming."
        confirmText="Confirm Erasure"
        onConfirm={handleConfirmDelete}
        onCancel={() => setDeleteConfirm({ isOpen: false, permId: null })}
      />
    </div>
  )
}
