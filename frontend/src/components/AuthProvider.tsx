import { Auth0Provider } from '@auth0/auth0-react'
import { ReactNode } from 'react'
import { getAuth0Config, isAuth0Configured } from '@/lib/auth/authConfig'

interface AuthProviderProps {
  children: ReactNode
}

/**
 * Auth0 Provider wrapper that handles PKCE authentication
 * 
 * Features:
 * - PKCE flow enabled by default (no client secret needed)
 * - Offline access for refresh tokens
 * - Graceful fallback when Auth0 is not configured
 * - Automatic token caching
 * 
 * If Auth0 is not configured (missing env vars), the app will run
 * in unauthenticated mode with fallback to 'local' user.
 */
export function AuthProvider({ children }: AuthProviderProps) {
  // If Auth0 is not configured, render children without auth provider
  // This allows the app to run locally without Auth0 setup
  if (!isAuth0Configured()) {
    console.warn(
      'Auth0 is not configured. Running in unauthenticated mode. ' +
      'Set VITE_AUTH0_DOMAIN and VITE_AUTH0_CLIENT_ID to enable authentication.'
    )
    return <>{children}</>
  }

  const config = getAuth0Config()

  return (
    <Auth0Provider
      domain={config.domain}
      clientId={config.clientId}
      authorizationParams={{
        redirect_uri: config.redirectUri,
        ...(config.audience && { audience: config.audience }),
        // Request offline_access scope for refresh tokens
        scope: 'openid profile email offline_access',
      }}
      // Enable PKCE by default (recommended for SPAs)
      useRefreshTokens={true}
      // Cache tokens in memory for better performance
      cacheLocation="memory"
    >
      {children}
    </Auth0Provider>
  )
}
