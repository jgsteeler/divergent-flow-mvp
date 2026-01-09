# Auth0 Setup Guide

This guide explains how to set up Auth0 authentication for the Divergent Flow frontend application.

## Overview

The frontend uses Auth0 with PKCE (Proof Key for Code Exchange) flow for secure authentication. PKCE is the recommended approach for Single Page Applications because:

- No client secrets are needed in the frontend
- Protection against authorization code interception attacks
- Cryptographic proof for token exchange
- Refresh tokens enabled for persistent sessions
- Secure token storage in browser localStorage

## Prerequisites

- An Auth0 account (free tier works fine)
- Access to your Auth0 Dashboard

## Step 1: Create an Auth0 Application

1. Go to [Auth0 Dashboard](https://manage.auth0.com/)
2. Navigate to **Applications** → **Applications**
3. Click **Create Application**
4. Choose:
   - **Name**: `Divergent Flow` (or your preferred name)
   - **Application Type**: **Single Page Web Applications**
5. Click **Create**

## Step 2: Configure Application Settings

In your newly created application's settings:

### Allowed Callback URLs

Add the URLs where Auth0 should redirect after login:

```
http://localhost:5173, http://localhost:5000, https://your-production-domain.com
```

### Allowed Logout URLs

Add the URLs where Auth0 should redirect after logout:

```
http://localhost:5173, http://localhost:5000, https://your-production-domain.com
```

### Allowed Web Origins

Add the origins that can make requests:

```
http://localhost:5173, http://localhost:5000, https://your-production-domain.com
```

### Save Changes

Scroll down and click **Save Changes**

## Step 3: Configure Your Application

### Copy Auth0 Credentials

From your Auth0 Application settings page, copy:

1. **Domain** (e.g., `your-tenant.auth0.com`)
2. **Client ID** (long alphanumeric string)

### Create `.env.local` File

In the `frontend` directory, create a `.env.local` file:

```bash
cd frontend
cp .env.example .env.local
```

### Add Auth0 Configuration

Edit `.env.local` and add your Auth0 credentials:

```bash
# Auth0 Configuration
VITE_AUTH0_DOMAIN=your-tenant.auth0.com
VITE_AUTH0_CLIENT_ID=your-client-id-here
VITE_AUTH0_AUDIENCE=https://your-api-identifier (optional)
VITE_AUTH0_CALLBACK_URL=http://localhost:5173
```

**Notes:**
- `VITE_AUTH0_DOMAIN`: Your Auth0 tenant domain (without `https://`)
- `VITE_AUTH0_CLIENT_ID`: Your application's Client ID
- `VITE_AUTH0_AUDIENCE`: (Optional) Your API identifier if you have a custom API
- `VITE_AUTH0_CALLBACK_URL`: The URL Auth0 redirects to after login (defaults to `window.location.origin`)

## Step 4: (Optional) Create an API

If you want to secure your backend API:

1. Navigate to **Applications** → **APIs** in Auth0 Dashboard
2. Click **Create API**
3. Provide:
   - **Name**: `Divergent Flow API`
   - **Identifier**: `https://api.divergentflow.com` (can be any unique URL)
   - **Signing Algorithm**: `RS256`
4. Click **Create**
5. Copy the **Identifier** and add it to your `.env.local` as `VITE_AUTH0_AUDIENCE`

## Step 5: Test Authentication

1. Start the development server:
   ```bash
   cd frontend
   npm run dev
   ```

2. Open http://localhost:5173 in your browser

3. Click the **Login** button in the top-right corner

4. You should be redirected to Auth0's login page

5. Sign up or log in with an existing account

6. After successful authentication, you'll be redirected back to the app

7. You should see your email and a **Logout** button in the header

## Verification

To verify that authentication is working:

1. Open browser DevTools (F12)
2. Go to the **Console** tab
3. You should NOT see any Auth0-related errors
4. The user info should be displayed in the header
5. API requests should include the `X-User-Id` header with your user's ID

## Troubleshooting

### "Auth0 configuration is missing" warning

This means the environment variables are not set correctly. Check:
- `.env.local` file exists in the `frontend` directory
- Variables are prefixed with `VITE_` (required for Vite)
- No typos in variable names
- Restart the dev server after adding environment variables

### "Callback URL mismatch" error

This means the callback URL is not configured in Auth0. Check:
- Your current URL is in the **Allowed Callback URLs** list in Auth0
- No trailing slashes in URLs
- Correct protocol (http vs https)

### "Invalid state" error

This can happen if you:
- Have multiple tabs open during login
- Browser cookies are disabled
- Try to reuse an old login link

Solution: Clear browser cache and cookies, then try again

### User ID shows as "local" instead of Auth0 user ID

This means the app is running without Auth0 configured. This is intentional for development without Auth0. To use Auth0:
1. Set up Auth0 credentials in `.env.local`
2. Restart the dev server
3. Click Login

## Production Deployment

### Environment Variables

When deploying to production (e.g., Netlify), set these environment variables:

```bash
VITE_AUTH0_DOMAIN=your-tenant.auth0.com
VITE_AUTH0_CLIENT_ID=your-production-client-id
VITE_AUTH0_AUDIENCE=https://your-api-identifier
VITE_AUTH0_CALLBACK_URL=https://your-production-domain.com
```

### Auth0 Configuration

In your Auth0 Application settings, add your production URLs to:
- Allowed Callback URLs
- Allowed Logout URLs
- Allowed Web Origins

### Security Best Practices

1. **Never commit** `.env.local` to git (already in `.gitignore`)
2. Use different Auth0 applications for staging and production
3. Rotate credentials if accidentally exposed
4. Enable MFA (Multi-Factor Authentication) in Auth0 for added security
5. Review Auth0 logs regularly for suspicious activity

## Testing Without Auth0

The app is designed to work without Auth0 configuration for local development:

1. Don't set Auth0 environment variables (or leave them empty)
2. The app will run in "unauthenticated" mode
3. User ID will default to `'local'`
4. No login/logout buttons will be shown
5. API calls will still work with the default user ID

This is useful for:
- Quick local development
- Testing without Auth0 setup
- Running automated tests

## Additional Resources

- [Auth0 React SDK Documentation](https://auth0.com/docs/quickstart/spa/react)
- [PKCE Flow Explanation](https://auth0.com/docs/get-started/authentication-and-authorization-flow/authorization-code-flow-with-proof-key-for-code-exchange-pkce)
- [Auth0 Dashboard](https://manage.auth0.com/)

## Support

If you encounter issues:
1. Check the [Auth0 Community](https://community.auth0.com/)
2. Review [Auth0 Documentation](https://auth0.com/docs)
3. Open an issue in the project repository
