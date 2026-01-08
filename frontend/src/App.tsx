import { useState } from "react";
import { CaptureInput } from "@/components/CaptureInput";
import { CaptureList } from "@/components/CaptureList";
import { AuthButton } from "@/components/AuthButton";
import { Button } from "@/components/ui/button";
import { toast } from "sonner";
import { Toaster } from "@/components/ui/sonner";
import { List } from "@phosphor-icons/react";
import { useCaptures, useCreateCapture } from "@/hooks/useCaptures";

function App() {
  const [view, setView] = useState<"capture" | "list">("capture");
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
      <div className="max-w-4xl mx-auto space-y-8">
        <header className="flex justify-between items-center">
          <div className="text-center flex-1 space-y-2">
            <h1 className="text-2xl font-bold">Divergent Flow</h1>
            <p className="text-sm text-muted-foreground">Capture your thoughts</p>
          </div>
          <AuthButton />
        </header>

        {view === "capture" ? (
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
              <div className="flex justify-center">
                <Button
                  onClick={() => setView("list")}
                  variant="outline"
                  className="flex items-center gap-2"
                >
                  <List />
                  View Captures ({captures.length})
                </Button>
              </div>
            )}
          </>
        ) : (
          <CaptureList 
            captures={captures}
            onBack={() => setView("capture")}
            isLoading={isLoading}
            error={error}
          />
        )}
      </div>
      
      <footer className="mt-8 pb-4 text-center">
        <p className="text-xs text-muted-foreground">v{__APP_VERSION__}</p>
      </footer>
    </div>
  );
}

export default App;