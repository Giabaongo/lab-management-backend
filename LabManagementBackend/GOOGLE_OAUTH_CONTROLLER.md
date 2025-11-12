# GoogleOAuthController Deep Dive

This document explains how `LabManagementBackend/LabManagement.API/Controllers/GoogleOAuthController.cs` implements the Google OAuth 2.0 authorization-code flow and how it interacts with the rest of the system.

## Overview

`GoogleOAuthController` exposes two endpoints:

| Endpoint | Purpose |
| --- | --- |
| `GET /api/auth/google/login` | Starts the Google login flow by redirecting the browser to Google’s consent screen. |
| `GET /api/auth/google/callback` | Handles Google’s redirect, exchanges the authorization code for tokens, and signs the user into our app. |

The controller relies on:

- `IConfiguration` to read configured values (Google client credentials, frontend URLs).
- `IMemoryCache` to store per-request `state` data for CSRF protection and frontend callback tracking.
- `IHttpClientFactory` to call Google’s token endpoint.
- `IGoogleAuthService` to reuse the existing ID-token verification + JWT issuance logic.
- `ILogger<GoogleOAuthController>` for troubleshooting.

## Login Endpoint

Relevant code: `GoogleOAuthController.Login()` lines 38-79.

1. Reads `Google:ClientId` and builds the backend redirect URI (`https://<api-domain>/api/auth/google/callback`).
2. Accepts an optional `returnUrl` query param pointing to the frontend route that should receive the final JWT. If it’s absent, falls back to `FrontendCallbackUrl` from configuration or a default of `http://localhost:3000/auth/callback`.
3. Validates that the callback is an absolute URI to avoid open redirect issues.
4. Creates a random `state` string, caches `{state -> callbackUrl}` for 5 minutes.
5. Redirects to Google’s authorization endpoint with the required query parameters (client id, redirect URI, scopes, state, etc.).

## Callback Endpoint

Relevant code: `GoogleOAuthController.Callback()` lines 86-159.

1. Checks if Google returned an `error` query param (user cancelled, invalid request, …). If so, redirects back to the frontend with the error attached.
2. Validates `code` and `state` query parameters.
3. Retrieves and removes the cached state entry. Missing state ⇒ 400 (“Invalid or expired OAuth state”).
4. Calls `ExchangeCodeForTokensAsync` to POST the authorization code to `https://oauth2.googleapis.com/token`. This requires both `Google:ClientId` and `Google:ClientSecret`, so production deployments must provide both (via appsettings, environment variables, or secrets). If Google responds with an error (most commonly `invalid_client` when client secrets are wrong), the controller redirects to the frontend with an error message.
5. When the token response includes an `id_token`, the controller invokes `IGoogleAuthService.LoginWithGoogleAsync` with that token. The service verifies the token via Google’s libraries, creates a user if needed, and returns the app’s JWT + user profile.
6. Builds the final frontend callback URL, adding query parameters for `token`, `userId`, `email`, `name`, `role`, and the original `state`. The browser is redirected there to finish the sign‑in.

## Helper Methods

- `BuildBackendRedirectUri()` ensures the redirect URI matches the actual API origin (scheme + host + optional path base). This helps local development (HTTP) and production (HTTPS) without extra config.
- `GetDefaultFrontendCallbackUrl()` uses `FrontendCallbackUrl` if present; otherwise appends `/auth/callback` to `FrontendUrl`.
- `ExchangeCodeForTokensAsync()` handles the HTTP POST to Google, logging warnings for non-2xx responses and errors for unexpected exceptions.
- `RedirectWithError()` appends an `error` query parameter to the callback URL.

## Configuration Checklist

To use this controller successfully, make sure:

1. `Google:ClientId` and `Google:ClientSecret` are set (e.g., via `appsettings.*.json` or environment variables `Google__ClientId`, `Google__ClientSecret`).
2. Google Cloud Console lists the backend redirect URI (e.g., `http://localhost:5162/api/auth/google/callback`) under **Authorized redirect URIs**.
3. `FrontendUrl` or `FrontendCallbackUrl` points to your SPA so the controller knows where to send the user after login.
4. The frontend route (`/auth/callback` by default) can read the query parameters we append and store the JWT appropriately.

## Control Flow Diagram

```
Browser  ──(GET /api/auth/google/login)──>  Controller ──> Google consent
Google   ──(redirect with code+state)──> Controller ──(POST code)──> Google token endpoint
Controller ──(id_token)──> GoogleAuthService ──> Issue JWT ──> Browser redirect with JWT
```

## Common Failure Modes

- **Missing Client Secret**: Google responds `invalid_client`. Ensure `Google:ClientSecret` is configured.
- **State mismatch**: If the callback happens after cache expiration or state wasn’t stored, the user gets `Invalid or expired OAuth state`.
- **Redirect mismatch**: Google rejects the `code` exchange if the redirect URI differs from what was registered in Google Cloud Console.

## Extensibility Ideas

- Swap `IMemoryCache` for a distributed cache (Redis) to support multi-instance deployments.
- Include refresh token handling (currently the refresh token is discarded).
- Encrypt the JWT before sending it through a query string, or switch to posting the result to the frontend to avoid exposing session data in URLs.
- Add PKCE support if native/mobile clients will reuse this flow.

This file should give you enough context to maintain or extend the `GoogleOAuthController`. Refer to the source (`LabManagementBackend/LabManagement.API/Controllers/GoogleOAuthController.cs`) for exact implementation details.
