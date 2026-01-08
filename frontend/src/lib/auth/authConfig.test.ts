import { describe, it, expect } from 'vitest'
import { getAuth0Config, isAuth0Configured } from '@/lib/auth/authConfig'

describe('authConfig', () => {
  describe('getAuth0Config', () => {
    it('should throw error when Auth0 is not configured', () => {
      expect(() => getAuth0Config()).toThrow(
        'Auth0 configuration is missing'
      )
    })
  })

  describe('isAuth0Configured', () => {
    it('should return false when Auth0 is not configured', () => {
      expect(isAuth0Configured()).toBe(false)
    })
  })
})
