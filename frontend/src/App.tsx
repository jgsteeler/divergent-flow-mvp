import { useState } from "react";
import { CaptureInput } from "@/components/CaptureInput";
import { CaptureList } from "@/components/CaptureList";
import { ReviewQueue } from "@/components/ReviewQueue";
import { Dashboard } from "@/components/Dashboard";
import { AuthButton } from "@/components/AuthButton";
import { Button } from "@/components/ui/button";
import { toast } from "sonner";
import { Toaster } from "@/components/ui/sonner";
import { List, SquaresFour, CheckSquare } from "@phosphor-icons/react";
import { useCaptures, useCreateCapture } from "@/hooks/useCaptures";

type View = "capture" | "list" | "review" | "dashboard";

function App() {
  const [view, setView] = useState<View>("capture");
  const { data: captures = [], isLoading, error } = useCaptures();
  const createCaptureMutation = useCreateCapture();

  const handleCapture = (text: string) => {
    createCaptureMutation.mutate(
      { text },
      {
        onSuccess: () => {
          toast.success("Captured!");
        },
        onError: (error) => {
          toast.error(`Failed to capture: ${error.message}`);
        },
      }
    );
  };

  return (
    <div className="min-h-screen bg-background p-4 md:p-8">
      <Toaster />
      <div className="max-w-6xl mx-auto space-y-8">
        <header className="flex justify-between items-center">
          <div className="text-center flex-1 space-y-2">
            <h1 className="text-2xl font-bold">Divergent Flow</h1>
            <p className="text-sm text-muted-foreground">Capture, Review, and Manage Your Thoughts</p>
          </div>
          <AuthButton />
        </header>

        {view === "capture" && (
          <>
            <CaptureInput 
              onCapture={handleCapture}
              isLoading={createCaptureMutation.isPending}
            />
            
            {isLoading && (
              <div className="text-center text-muted-foreground">
                Loading captures...
              </div>
            )}
            
            {error && (
              <div className="text-center text-destructive">
                Failed to load captures: {error.message}
              </div>
            )}
            
            {!isLoading && !error && captures.length > 0 && (
              <div className="flex justify-center gap-3 flex-wrap">
                <Button
                  onClick={() => setView("review")}
                  variant="outline"
                  className="flex items-center gap-2"
                >
                  <CheckSquare />
                  Review Queue
                </Button>
                <Button
                  onClick={() => setView("dashboard")}
                  variant="outline"
                  className="flex items-center gap-2"
                >
                  <SquaresFour />
                  Dashboard
                </Button>
                <Button
                  onClick={() => setView("list")}
                  variant="outline"
                  className="flex items-center gap-2"
                >
                  <List />
                  View All ({captures.length})
                </Button>
              </div>
            )}
          </>
        )}

        {view === "list" && (
          <CaptureList 
            captures={captures}
            onBack={() => setView("capture")}
            isLoading={isLoading}
            error={error}
          />
        )}

        {view === "review" && (
          <ReviewQueue onBack={() => setView("capture")} />
        )}

        {view === "dashboard" && (
          <Dashboard onBack={() => setView("capture")} />
        )}
      </div>
      
      <footer className="mt-8 pb-4 text-center">
        <p className="text-xs text-muted-foreground">v{__APP_VERSION__}</p>
      </footer>
    </div>
  );
}

export default App;