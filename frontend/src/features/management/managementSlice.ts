import { createSlice, isAnyOf } from '@reduxjs/toolkit'
import type { PermissionDto, RoleDto, UserDto } from '../../types'
import {
  createPermissionThunk,
  createRoleThunk,
  createUserThunk,
  deletePermissionThunk,
  deleteRoleThunk,
  deleteUserThunk,
  fetchOverviewThunk,
  fetchPermissionsThunk,
  fetchRolesThunk,
  fetchUsersThunk,
  updatePermissionThunk,
  updateRoleThunk,
  updateUserThunk,
} from './managementThunks'

type CollectionState<T> = {
  items: T[]
  totalItems: number
  pageNumber: number
  pageSize: number
  totalPages: number
  status: 'idle' | 'loading' | 'succeeded' | 'failed'
}

type ManagementState = {
  permissions: CollectionState<PermissionDto>
  roles: CollectionState<RoleDto>
  users: CollectionState<UserDto>
  overviewStatus: 'idle' | 'loading' | 'succeeded' | 'failed'
  mutationStatus: 'idle' | 'loading'
  error: string | null
}

const initialCollection = <T>(): CollectionState<T> => ({
  items: [],
  totalItems: 0,
  pageNumber: 1,
  pageSize: 10,
  totalPages: 0,
  status: 'idle',
})

const initialState: ManagementState = {
  permissions: initialCollection<PermissionDto>(),
  roles: initialCollection<RoleDto>(),
  users: initialCollection<UserDto>(),
  overviewStatus: 'idle',
  mutationStatus: 'idle',
  error: null,
}

const managementSlice = createSlice({
  name: 'management',
  initialState,
  reducers: {
    resetSectionStatus: (state, action: { payload: 'users' | 'roles' | 'permissions' }) => {
      state[action.payload].status = 'idle'
    }
  },
  extraReducers: (builder) => {
    builder
      // Overview
      .addCase(fetchOverviewThunk.pending, (state) => {
        state.overviewStatus = 'loading'
        state.error = null
      })
      .addCase(fetchOverviewThunk.fulfilled, (state, action) => {
        const { users, roles, permissions } = action.payload
        state.overviewStatus = 'succeeded'
        state.error = null
        
        state.users = { ...users, items: users.items, totalItems: users.totalCount, status: 'succeeded' }
        state.roles = { ...roles, items: roles.items, totalItems: roles.totalCount, status: 'succeeded' }
        state.permissions = { ...permissions, items: permissions.items, totalItems: permissions.totalCount, status: 'succeeded' }
      })
      .addCase(fetchOverviewThunk.rejected, (state, action) => {
        state.overviewStatus = 'failed'
        state.error = action.payload ?? 'Failed to load dashboard data.'
      })
      
      // Users
      .addCase(fetchUsersThunk.pending, (state) => {
        state.users.status = 'loading'
      })
      .addCase(fetchUsersThunk.fulfilled, (state, action) => {
        state.users = { 
          ...action.payload, 
          items: action.payload.items, 
          totalItems: action.payload.totalCount, 
          status: 'succeeded' 
        }
      })
      
      // Roles
      .addCase(fetchRolesThunk.pending, (state) => {
        state.roles.status = 'loading'
      })
      .addCase(fetchRolesThunk.fulfilled, (state, action) => {
        state.roles = { 
          ...action.payload, 
          items: action.payload.items, 
          totalItems: action.payload.totalCount, 
          status: 'succeeded' 
        }
      })
      
      // Permissions
      .addCase(fetchPermissionsThunk.pending, (state) => {
        state.permissions.status = 'loading'
      })
      .addCase(fetchPermissionsThunk.fulfilled, (state, action) => {
        state.permissions = { 
          ...action.payload, 
          items: action.payload.items, 
          totalItems: action.payload.totalCount, 
          status: 'succeeded' 
        }
      })

      .addMatcher(
        isAnyOf(
          createPermissionThunk.pending,
          updatePermissionThunk.pending,
          deletePermissionThunk.pending,
          createRoleThunk.pending,
          updateRoleThunk.pending,
          deleteRoleThunk.pending,
          createUserThunk.pending,
          updateUserThunk.pending,
          deleteUserThunk.pending,
        ),
        (state) => {
          state.mutationStatus = 'loading'
          state.error = null
        },
      )
      .addMatcher(
        isAnyOf(
          createPermissionThunk.fulfilled,
          updatePermissionThunk.fulfilled,
          deletePermissionThunk.fulfilled,
          createRoleThunk.fulfilled,
          updateRoleThunk.fulfilled,
          deleteRoleThunk.fulfilled,
          createUserThunk.fulfilled,
          updateUserThunk.fulfilled,
          deleteUserThunk.fulfilled,
        ),
        (state) => {
          state.mutationStatus = 'idle'
          state.error = null
        },
      )
      .addMatcher(
        isAnyOf(
          createPermissionThunk.rejected,
          updatePermissionThunk.rejected,
          deletePermissionThunk.rejected,
          createRoleThunk.rejected,
          updateRoleThunk.rejected,
          deleteRoleThunk.rejected,
          createUserThunk.rejected,
          updateUserThunk.rejected,
          deleteUserThunk.rejected,
        ),
        (state, action) => {
          state.mutationStatus = 'idle'
          state.error = action.payload ?? 'Dashboard update failed.'
        },
      )
  },
})

export const { resetSectionStatus } = managementSlice.actions
export default managementSlice.reducer