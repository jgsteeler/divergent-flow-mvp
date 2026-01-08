import { useAuth0 } from '@auth0/auth0-react'

/**
 * Hook to access authentication state and user information
 * 
 * Provides:
 * - Authentication status
 * - User information
 * - Login/logout functions
 * - Access token for API calls
 * - User ID for X-User-Id header
 */
export function useAuth() {
  const {
    isAuthenticated,
    isLoading,
    user,
    loginWithRedirect,
    logout,
    getAccessTokenSilently,
  } = useAuth0()

  /**
   * Get the user ID to send in API requests
   * Falls back to 'local' for development/testing
   */
  const getUserId = (): string => {
    return user?.sub || 'local'
  }

  /**
   * Get access token for API requests
   * Returns null if not authenticated or auth not configured
   */
  const getAccessToken = async (): Promise<string | null> => {
    if (!isAuthenticated) {
      return null
    }

    try {
      return await getAccessTokenSilently()
    } catch (error) {
      console.error('Failed to get access token:', error)
      return null
    }
  }

  /**
   * Login with redirect to Auth0
   */
  const login = async () => {
    await loginWithRedirect()
  }

  /**
   * Logout and redirect to home
   */
  const logoutUser = () => {
    logout({
      logoutParams: {
        returnTo: window.location.origin,
      },
    })
  }

  return {
    isAuthenticated,
    isLoading,
    user,
    login,
    logout: logoutUser,
    getAccessToken,
    getUserId,
  }
}
