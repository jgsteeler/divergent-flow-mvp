import { useAuth } from '@/hooks/useAuth'
import { Button } from '@/components/ui/button'
import { SignIn, SignOut, User } from '@phosphor-icons/react'
import { isAuth0Configured } from '@/lib/auth/authConfig'

/**
 * Authentication button component
 * 
 * Shows:
 * - Login button when not authenticated
 * - User info and logout button when authenticated
 * - Nothing when Auth0 is not configured
 */
export function AuthButton() {
  const { isAuthenticated, isLoading, user, login, logout } = useAuth()

  // Don't show anything if Auth0 is not configured
  if (!isAuth0Configured()) {
    return null
  }

  // Show loading state
  if (isLoading) {
    return (
      <Button variant="ghost" size="sm" disabled>
        <User className="w-4 h-4" />
      </Button>
    )
  }

  // Show logout button when authenticated
  if (isAuthenticated && user) {
    return (
      <div className="flex items-center gap-2">
        <div className="flex items-center gap-2 text-sm text-muted-foreground">
          <User className="w-4 h-4" />
          <span className="hidden sm:inline">{user.email || user.name}</span>
        </div>
        <Button
          variant="ghost"
          size="sm"
          onClick={logout}
          className="flex items-center gap-2"
        >
          <SignOut className="w-4 h-4" />
          <span className="hidden sm:inline">Logout</span>
        </Button>
      </div>
    )
  }

  // Show login button when not authenticated
  return (
    <Button
      variant="default"
      size="sm"
      onClick={login}
      className="flex items-center gap-2"
    >
      <SignIn className="w-4 h-4" />
      <span>Login</span>
    </Button>
  )
}
