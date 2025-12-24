import { defineConfig } from 'vitest/config'
import react from '@vitejs/plugin-react-swc'
import { resolve } from 'path'
import { readFileSync } from 'fs'

const projectRoot = process.env.PROJECT_ROOT || import.meta.dirname

// Read version from package.json for test environment
let version = '0.0.0'
try {
  const packageJsonPath = resolve(projectRoot, 'package.json')
  const packageJson = JSON.parse(readFileSync(packageJsonPath, 'utf-8'))
  version = packageJson.version
} catch (error) {
  console.warn('Warning: Could not read version from package.json for tests:', error instanceof Error ? error.message : 'Unknown error')
}

export default defineConfig({
  plugins: [react()],
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: './src/test/setup.ts',
    css: true,
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html'],
      exclude: [
        'node_modules/',
        'src/test/',
        '**/*.d.ts',
        '**/*.config.*',
        '**/mockData',
        'src/main.tsx',
      ],
    },
  },
  resolve: {
    alias: {
      '@': resolve(projectRoot, 'src'),
    },
  },
  define: {
    __APP_VERSION__: JSON.stringify(version),
  },
})
