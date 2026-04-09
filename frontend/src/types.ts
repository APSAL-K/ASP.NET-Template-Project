export type LoginRequest = {
  email: string
  password: string
  roleId?: string
}

export type RegisterRequest = {
  firstName: string
  lastName: string
  email: string
  password: string
  roleIds?: string[]
}

export type LoginResponse = {
  userId: string
  email: string
  accessToken: string
  refreshToken: string
  tokenExpiresAt: string
  roles: string[]
  permissions: string[]
}

export type TokenResponse = {
  accessToken: string
  refreshToken: string
  expiresAt: string
  roles: string[]
  permissions: string[]
}

export type RoleAssignment = {
  id: string
  name: string
}

export type PermissionAssignment = {
  id: string
  name: string
}

export type PermissionDto = {
  id: string
  name: string
  description: string
}

export type RoleDto = {
  id: string
  name: string
  description: string
  permissions: PermissionAssignment[]
}

export type UserDto = {
  id: string
  firstName: string
  lastName: string
  email: string
  isActive: boolean
  createdAt: string
  lastLoginAt: string | null
  roles: RoleAssignment[]
  permissions: string[]
}

export type EditablePermission = PermissionDto

export type EditableRole = RoleDto

export type EditableUser = UserDto & {
  password: string
}

export type CreatePermissionRequest = {
  name: string
  description: string
}

export type CreateRoleRequest = {
  name: string
  description: string
  permissionIds: string[]
}

export type CreateUserRequest = {
  firstName: string
  lastName: string
  email: string
  password: string
  isActive: boolean
  roleIds: string[]
}

export type UpdatePermissionRequest = CreatePermissionRequest

export type UpdateRoleRequest = CreateRoleRequest

export type UpdateUserRequest = {
  firstName: string
  lastName: string
  email: string
  password?: string
  isActive: boolean
  roleIds: string[]
}

export type AuthSession = {
  userId?: string
  email: string
  accessToken: string
  refreshToken: string
  expiresAt: string
  roles: string[]
  permissions: string[]
}

export type PaginatedResult<T> = {
  items: T[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
  hasPreviousPage: boolean
  hasNextPage: boolean
}
