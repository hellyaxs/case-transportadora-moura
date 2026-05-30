## 1. Auth Route Guard

- [x] 1.1 Create a reusable frontend auth guard helper in `apps/web/src/modules/auth` that validates the current session with `authService.me()`.
- [x] 1.2 Make the guard redirect unauthenticated users to `/login` before protected route components render.
- [x] 1.3 Preserve the original internal destination during the redirect when the attempted route can be safely represented.
- [x] 1.4 Reject unsafe or external return destinations and fall back to `/`.

## 2. Route Integration

- [x] 2.1 Apply the auth guard to the `/` route that renders the collections operational page.
- [x] 2.2 Keep `/login` publicly accessible for anonymous users.
- [x] 2.3 Keep or adjust the `/login` authenticated-user redirect so an already authenticated user does not see the login form.
- [x] 2.4 After successful login, navigate to the preserved destination when valid, otherwise navigate to `/`.

## 3. Loading and Error Behavior

- [x] 3.1 Ensure protected route pages do not start collections/catalog API loading before the auth guard resolves.
- [x] 3.2 Ensure an anonymous visit to a protected route shows the login page instead of operational `401 Unauthorized` errors.
- [x] 3.3 Ensure expired or invalid sessions follow the same redirect behavior as missing sessions.

## 4. Validation

- [x] 4.1 Manually verify anonymous access to `/` redirects to `/login`.
- [x] 4.2 Manually verify successful login returns to the intended protected route or `/`.
- [x] 4.3 Manually verify authenticated access to `/` loads collections normally.
- [x] 4.4 Run `pnpm check-types` and fix TypeScript errors introduced by the change.
