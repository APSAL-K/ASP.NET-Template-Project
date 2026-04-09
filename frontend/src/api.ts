import type {
  CreatePermissionRequest,
  CreateRoleRequest,
  CreateUserRequest,
  LoginRequest,
  LoginResponse,
  PermissionDto,
  RegisterRequest,
  RoleDto,
  TokenResponse,
  UpdatePermissionRequest,
  UpdateRoleRequest,
  UpdateUserRequest,
  UserDto,
} from './types'

export class ApiError extends Error {
  status: number
  details: unknown

  constructor(message: string, status: number, details: unknown) {
    super(message)
    this.name = 'ApiError'
    this.status = status
    this.details = details
  }
}

type RequestOptions = {
  method?: 'GET' | 'POST' | 'PUT' | 'DELETE'
  body?: unknown
  accessToken?: string
}

function normalizeBaseUrl(baseUrl: string) {
  return baseUrl.trim().replace(/\/$/, '')
}

function buildHeaders(accessToken?: string) {
  const headers = new Headers({
    'Content-Type': 'application/json',
  })

  if (accessToken) {
    headers.set('Authorization', `Bearer ${accessToken}`)
  }

  return headers
}

async function requestJson<T>(baseUrl: string, path: string, options: RequestOptions = {}) {
  const response = await fetch(`${normalizeBaseUrl(baseUrl)}${path}`, {
    method: options.method ?? 'GET',
    headers: buildHeaders(options.accessToken),
    body: options.body === undefined ? undefined : JSON.stringify(options.body),
  })

  if (response.status === 204) {
    return undefined as T
  }

  const rawText = await response.text()
  const payload = rawText ? tryParseJson(rawText) : null

  if (!response.ok) {
    const message =
      payload?.error ?? payload?.title ?? payload?.message ?? `Request failed with ${response.status}`
    throw new ApiError(message, response.status, payload)
  }

  return payload as T
}

function tryParseJson(rawText: string) {
  try {
    return JSON.parse(rawText)
  } catch {
    return rawText
  }
}

export function describeApiError(error: unknown) {
  if (error instanceof ApiError) {
    if (typeof error.details === 'string' && error.details.trim().length > 0) {
      return error.details
    }

    if (error.details && typeof error.details === 'object') {
      const details = error.details as {
        title?: string
        message?: string
        error?: string
        errors?: Record<string, string[]>
      }

      const validationMessages = details.errors
        ? Object.values(details.errors)
            .flat()
            .join(' ')
        : ''

      return validationMessages || details.error || details.message || details.title || error.message
    }

    return error.message
  }

  if (error instanceof Error) {
    return error.message
  }

  return 'Unexpected request failure.'
}

export const authApi = {
  login(baseUrl: string, body: LoginRequest) {
    return requestJson<LoginResponse>(baseUrl, '/api/auth/login', { method: 'POST', body })
  },
  register(baseUrl: string, body: RegisterRequest) {
    return requestJson<LoginResponse>(baseUrl, '/api/auth/register', { method: 'POST', body })
  },
  refresh(baseUrl: string, refreshToken: string) {
    return requestJson<TokenResponse>(baseUrl, '/api/auth/refresh', {
      method: 'POST',
      body: { refreshToken },
    })
  },
}

export const managementApi = {
  listUsers(baseUrl: string, accessToken?: string) {
    return requestJson<UserDto[]>(baseUrl, '/api/users', { accessToken })
  },
  createUser(baseUrl: string, body: CreateUserRequest, accessToken?: string) {
    return requestJson<UserDto>(baseUrl, '/api/users', { method: 'POST', body, accessToken })
  },
  updateUser(baseUrl: string, id: string, body: UpdateUserRequest, accessToken?: string) {
    return requestJson<UserDto>(baseUrl, `/api/users/${id}`, {
      method: 'PUT',
      body,
      accessToken,
    })
  },
  deleteUser(baseUrl: string, id: string, accessToken?: string) {
    return requestJson<void>(baseUrl, `/api/users/${id}`, { method: 'DELETE', accessToken })
  },
  listRoles(baseUrl: string, accessToken?: string) {
    return requestJson<RoleDto[]>(baseUrl, '/api/roles', { accessToken })
  },
  createRole(baseUrl: string, body: CreateRoleRequest, accessToken?: string) {
    return requestJson<RoleDto>(baseUrl, '/api/roles', { method: 'POST', body, accessToken })
  },
  updateRole(baseUrl: string, id: string, body: UpdateRoleRequest, accessToken?: string) {
    return requestJson<RoleDto>(baseUrl, `/api/roles/${id}`, {
      method: 'PUT',
      body,
      accessToken,
    })
  },
  deleteRole(baseUrl: string, id: string, accessToken?: string) {
    return requestJson<void>(baseUrl, `/api/roles/${id}`, { method: 'DELETE', accessToken })
  },
  listPermissions(baseUrl: string, accessToken?: string) {
    return requestJson<PermissionDto[]>(baseUrl, '/api/permissions', { accessToken })
  },
  createPermission(baseUrl: string, body: CreatePermissionRequest, accessToken?: string) {
    return requestJson<PermissionDto>(baseUrl, '/api/permissions', {
      method: 'POST',
      body,
      accessToken,
    })
  },
  updatePermission(
    baseUrl: string,
    id: string,
    body: UpdatePermissionRequest,
    accessToken?: string,
  ) {
    return requestJson<PermissionDto>(baseUrl, `/api/permissions/${id}`, {
      method: 'PUT',
      body,
      accessToken,
    })
  },
  deletePermission(baseUrl: string, id: string, accessToken?: string) {
    return requestJson<void>(baseUrl, `/api/permissions/${id}`, {
      method: 'DELETE',
      accessToken,
    })
  },
}
