# transportadora-moura

This project was created with [Better-T-Stack](https://github.com/AmanVarshney01/create-better-t-stack), a modern TypeScript stack that combines React, TanStack Router, and more.

## Features

- **TypeScript** - For type safety and improved developer experience
- **TanStack Router** - File-based routing with full type safety
- **TailwindCSS** - Utility-first CSS for rapid UI development
- **Shared UI package** - shadcn/ui primitives live in `packages/ui`
- **Nx** - Smart monorepo task orchestration and caching

## Getting Started

First, install the dependencies:

```bash
pnpm install
```

Start the database only:

```bash
pnpm docker:postgres
```

Then, run the API and frontend already connected:

```bash
pnpm dev
```

Open [http://localhost:3001](http://localhost:3001) in your browser to see the web application.

To run the full stack in Docker:

```bash
pnpm docker-compose:up
```

To stop the full stack:

```bash
pnpm docker-compose:down
```

Local ports:

- Postgres: `localhost:5432`
- API: `http://localhost:5086`
- Swagger: `http://localhost:5086/swagger`
- Web: `http://localhost:3001`

Useful infrastructure commands:

```bash
pnpm docker:postgres
pnpm docker-compose:up
pnpm docker-compose:down
pnpm docker-compose:reset
```

Environment files:

- API: copy or edit `apps/api/.env.example` into `apps/api/.env`. Keep backend, database, CORS and JWT variables here (`JWT_SECRET` is required).
- Web: copy or edit `apps/web/.env.example` into `apps/web/.env`. Keep `WEB_PORT` and `VITE_API_BASE_URL` here.
- `VITE_API_BASE_URL` must point to the API URL exposed to the browser.
- If `pnpm dev` says port `5086` is already in use, stop the previous API process before starting it again.

### Login operacional

1. Acesse `http://localhost:3001/login`.
2. Use o usuario seed: `operador@moura.local` / `Moura@2026`.
3. A API grava o JWT em cookie `HttpOnly`; o frontend envia `credentials: 'include'` nas chamadas.
4. Endpoints de coletas e cadastros retornam `401` sem sessao valida.

## UI Customization

React web apps in this stack share shadcn/ui primitives through `packages/ui`.

- Change design tokens and global styles in `packages/ui/src/styles/globals.css`
- Update shared primitives in `packages/ui/src/components/*`
- Adjust shadcn aliases or style config in `packages/ui/components.json` and `apps/web/components.json`

### Add more shared components

Run this from the project root to add more primitives to the shared UI package:

```bash
npx shadcn@latest add accordion dialog popover sheet table -c packages/ui
```

Import shared components like this:

```tsx
import { Button } from "@transportadora-moura/ui/components/button";
```

### Add app-specific blocks

If you want to add app-specific blocks instead of shared primitives, run the shadcn CLI from `apps/web`.

## Project Structure

```text
transportadora-moura/
├── apps/
│   ├── web/         # Frontend application (React + TanStack Router)
├── packages/
│   ├── ui/          # Shared shadcn/ui components and styles
```

## Available Scripts

- `pnpm run dev`: Start all applications in development mode
- `pnpm run build`: Build all applications
- `pnpm run dev:web`: Start only the web application
- `pnpm run check-types`: Check TypeScript types across all apps
