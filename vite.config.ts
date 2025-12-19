import tailwindcss from "@tailwindcss/vite";
import react from "@vitejs/plugin-react-swc";
import { defineConfig } from "vite";
import { resolve } from "path";

const projectRoot = process.env.PROJECT_ROOT || import.meta.dirname;

// Read version from package.json
const packageJson = await import('./package.json', { with: { type: 'json' } });
const version = packageJson.default.version;

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
