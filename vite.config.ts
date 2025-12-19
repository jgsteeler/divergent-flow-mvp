import tailwindcss from "@tailwindcss/vite";
import react from "@vitejs/plugin-react-swc";
import { defineConfig } from "vite";
import { resolve } from "path";
import { readFileSync } from "fs";

const projectRoot = process.env.PROJECT_ROOT || import.meta.dirname;

// Read version from package.json
let version = '0.0.0';
try {
  const packageJsonPath = resolve(projectRoot, 'package.json');
  const packageJson = JSON.parse(readFileSync(packageJsonPath, 'utf-8'));
  version = packageJson.version;
} catch (error) {
  console.warn('Warning: Could not read version from package.json, using default version 0.0.0:', error instanceof Error ? error.message : 'Unknown error');
}

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), tailwindcss()],
  resolve: {
    alias: {
      "@": resolve(projectRoot, "src"),
    },
  },
  define: {
    '__APP_VERSION__': JSON.stringify(version),
  },
});
