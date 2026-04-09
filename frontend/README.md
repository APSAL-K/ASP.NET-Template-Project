# Frontend

Separate React frontend for the ModuleDrivenFramwork auth module.

## What it covers

- `POST /api/auth/login`
- `POST /api/auth/register`
- `POST /api/auth/refresh`
- `GET/POST/PUT/DELETE /api/users`
- `GET/POST/PUT/DELETE /api/roles`
- `GET/POST/PUT/DELETE /api/permissions`

## Run locally

1. Start the backend from `ModuleDrivenFramwork`.
2. If needed, create `.env` from `.env.example` and point `VITE_API_BASE_URL` to your API.
3. Run `npm install`.
4. Run `npm run dev`.

The backend has been updated to allow the default Vite dev origin at `http://localhost:5173`.
