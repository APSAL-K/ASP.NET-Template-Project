import { createAsyncThunk } from '@reduxjs/toolkit'
import { managementApi, describeApiError } from '../../api'
import type { RootState } from '../../app/store'
import type {
  CreatePermissionRequest,
  CreateRoleRequest,
  CreateUserRequest,
  PermissionDto,
  RoleDto,
  UpdatePermissionRequest,
  UpdateRoleRequest,
  UpdateUserRequest,
  UserDto,
} from '../../types'
import { pushToast } from '../ui/uiSlice'

type OverviewPayload = {
  permissions: PermissionDto[]
  roles: RoleDto[]
  users: UserDto[]
}

function readSessionContext(state: RootState) {
  return {
    apiBaseUrl: state.ui.apiBaseUrl,
    accessToken: state.auth.session?.accessToken,
  }
}

function createMutationThunk<Arg>(
  typePrefix: string,
  action: (args: Arg, state: RootState) => Promise<void>,
  successTitle: string,
  successDescription: string,
) {
  return createAsyncThunk<void, Arg, { state: RootState; rejectValue: string }>(
    typePrefix,
    async (args, thunkApi) => {
      try {
        await action(args, thunkApi.getState())
        thunkApi.dispatch(pushToast({ kind: 'success', title: successTitle, description: successDescription }))
        await thunkApi.dispatch(fetchOverviewThunk()).unwrap()
      } catch (error) {
        const message = describeApiError(error)
        thunkApi.dispatch(pushToast({ kind: 'error', title: 'Request failed', description: message }))
        return thunkApi.rejectWithValue(message)
      }
    },
  )
}

export const fetchOverviewThunk = createAsyncThunk<OverviewPayload, void, { state: RootState; rejectValue: string }>(
  'management/fetchOverview',
  async (_, thunkApi) => {
    const { apiBaseUrl, accessToken } = readSessionContext(thunkApi.getState())

    try {
      const [users, roles, permissions] = await Promise.all([
        managementApi.listUsers(apiBaseUrl, accessToken),
        managementApi.listRoles(apiBaseUrl, accessToken),
        managementApi.listPermissions(apiBaseUrl, accessToken),
      ])

      return { users, roles, permissions }
    } catch (error) {
      const message = describeApiError(error)
      thunkApi.dispatch(pushToast({ kind: 'error', title: 'Dashboard load failed', description: message }))
      return thunkApi.rejectWithValue(message)
    }
  },
)

export const createPermissionThunk = createMutationThunk<CreatePermissionRequest>(
  'management/createPermission',
  async (payload, state) => {
    const { apiBaseUrl, accessToken } = readSessionContext(state)
    await managementApi.createPermission(apiBaseUrl, payload, accessToken)
  },
  'Permission created',
  'The permission is now available for roles and users.',
)

export const updatePermissionThunk = createMutationThunk<{ id: string; payload: UpdatePermissionRequest }>(
  'management/updatePermission',
  async ({ id, payload }, state) => {
    const { apiBaseUrl, accessToken } = readSessionContext(state)
    await managementApi.updatePermission(apiBaseUrl, id, payload, accessToken)
  },
  'Permission updated',
  'The permission changes were saved.',
)

export const deletePermissionThunk = createMutationThunk<string>(
  'management/deletePermission',
  async (id, state) => {
    const { apiBaseUrl, accessToken } = readSessionContext(state)
    await managementApi.deletePermission(apiBaseUrl, id, accessToken)
  },
  'Permission removed',
  'The permission was deleted from the catalog.',
)

export const createRoleThunk = createMutationThunk<CreateRoleRequest>(
  'management/createRole',
  async (payload, state) => {
    const { apiBaseUrl, accessToken } = readSessionContext(state)
    await managementApi.createRole(apiBaseUrl, payload, accessToken)
  },
  'Role created',
  'The role can now be assigned to users.',
)

export const updateRoleThunk = createMutationThunk<{ id: string; payload: UpdateRoleRequest }>(
  'management/updateRole',
  async ({ id, payload }, state) => {
    const { apiBaseUrl, accessToken } = readSessionContext(state)
    await managementApi.updateRole(apiBaseUrl, id, payload, accessToken)
  },
  'Role updated',
  'Role permissions were refreshed.',
)

export const deleteRoleThunk = createMutationThunk<string>(
  'management/deleteRole',
  async (id, state) => {
    const { apiBaseUrl, accessToken } = readSessionContext(state)
    await managementApi.deleteRole(apiBaseUrl, id, accessToken)
  },
  'Role removed',
  'The role was deleted from the workspace.',
)

export const createUserThunk = createMutationThunk<CreateUserRequest>(
  'management/createUser',
  async (payload, state) => {
    const { apiBaseUrl, accessToken } = readSessionContext(state)
    await managementApi.createUser(apiBaseUrl, payload, accessToken)
  },
  'User created',
  'The user account is now active in the dashboard.',
)

export const updateUserThunk = createMutationThunk<{ id: string; payload: UpdateUserRequest }>(
  'management/updateUser',
  async ({ id, payload }, state) => {
    const { apiBaseUrl, accessToken } = readSessionContext(state)
    await managementApi.updateUser(apiBaseUrl, id, payload, accessToken)
  },
  'User updated',
  'The user details were saved.',
)

export const deleteUserThunk = createMutationThunk<string>(
  'management/deleteUser',
  async (id, state) => {
    const { apiBaseUrl, accessToken } = readSessionContext(state)
    await managementApi.deleteUser(apiBaseUrl, id, accessToken)
  },
  'User removed',
  'The user was deleted from the roster.',
)
