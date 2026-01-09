/**
 * Auth0 configuration
 * 
 * Auth0 is configured to use PKCE (Proof Key for Code Exchange) flow
 * which is the recommended approach for Single Page Applications.
 * 
 * PKCE provides enhanced security by:
 * - Not requiring client secrets in the frontend
 * - Protecting against authorization code interception attacks
 * - Using cryptographic proof for token exchange
 */

export interface Auth0Config {
  domain: string
  clientId: string
  audience?: string
  redirectUri: string
}

/**
 * Get Auth0 configuration from environment variables
 * 
 * @throws {Error} If required environment variables are not set
 */
export function getAuth0Config(): Auth0Config {
  const domain = import.meta.env.VITE_AUTH0_DOMAIN
  const clientId = import.meta.env.VITE_AUTH0_CLIENT_ID
  const audience = import.meta.env.VITE_AUTH0_AUDIENCE
  const redirectUri = import.meta.env.VITE_AUTH0_CALLBACK_URL || window.location.origin

  if (!domain || !clientId) {
    throw new Error(
      'Auth0 configuration is missing. Please set VITE_AUTH0_DOMAIN and VITE_AUTH0_CLIENT_ID environment variables.'
    )
  }

  return {
    domain,
    clientId,
    audience,
    redirectUri,
  }
}

/**
 * Check if Auth0 is configured
 */
export function isAuth0Configured(): boolean {
  try {
    getAuth0Config()
    return true
  } catch {
    return false
  }
}
