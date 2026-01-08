import { createRoot } from "react-dom/client";
import { ErrorBoundary } from "react-error-boundary";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";

import App from "./App.tsx";
import { ErrorFallback } from "./ErrorFallback.tsx";
import { AuthProvider } from "./components/AuthProvider.tsx";

import "./main.css";
import "./styles/theme.css";
import "./index.css";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      refetchOnWindowFocus: false,
    },
  },
});

const container = document.getElementById("root");
if (container) {
  const root = createRoot(container);
  root.render(
    <ErrorBoundary FallbackComponent={ErrorFallback}>
      <AuthProvider>
        <QueryClientProvider client={queryClient}>
          <App />
        </QueryClientProvider>
      </AuthProvider>
    </ErrorBoundary>
  );
}
