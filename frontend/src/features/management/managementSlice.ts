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
  updatePermissionThunk,
  updateRoleThunk,
  updateUserThunk,
} from './managementThunks'

type ManagementState = {
  permissions: PermissionDto[]
  roles: RoleDto[]
  users: UserDto[]
  overviewStatus: 'idle' | 'loading' | 'succeeded' | 'failed'
  mutationStatus: 'idle' | 'loading'
  error: string | null
}

const initialState: ManagementState = {
  permissions: [],
  roles: [],
  users: [],
  overviewStatus: 'idle',
  mutationStatus: 'idle',
  error: null,
}

const managementSlice = createSlice({
  name: 'management',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchOverviewThunk.pending, (state) => {
        state.overviewStatus = 'loading'
        state.error = null
      })
      .addCase(fetchOverviewThunk.fulfilled, (state, action) => {
        state.overviewStatus = 'succeeded'
        state.permissions = action.payload.permissions
        state.roles = action.payload.roles
        state.users = action.payload.users
        state.error = null
      })
      .addCase(fetchOverviewThunk.rejected, (state, action) => {
        state.overviewStatus = 'failed'
        state.error = action.payload ?? 'Failed to load dashboard data.'
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

export default managementSlice.reducer