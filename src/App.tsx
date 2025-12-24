import { useState } from "react";
import { useLocalStorage } from "./hooks/useLocalStorage";
import { Capture } from "@/lib/types";
import { CaptureInput } from "@/components/CaptureInput";
import { CaptureList } from "@/components/CaptureList";
import { Button } from "@/components/ui/button";
import { toast } from "sonner";
import { Toaster } from "@/components/ui/sonner";
import { List } from "@phosphor-icons/react";

function App() {
  const [captures, setCaptures] = useLocalStorage<Capture[]>("captures", []);
  const [view, setView] = useState<"capture" | "list">("capture");

  const capturesArray = captures || [];

  const handleCapture = (text: string) => {
    const capture: Capture = {
      id: `capture-${Date.now()}-${crypto.randomUUID()}`,
      text,
      createdAt: Date.now(),
    };

    setCaptures((current) => [...(current || []), capture]);
    toast.success("Captured!");
  };

  return (
    <div className="min-h-screen bg-background p-4 md:p-8">
      <Toaster />
      <div className="max-w-4xl mx-auto space-y-8">
        <header className="text-center space-y-2">
          <h1 className="text-2xl font-bold">Divergent Flow</h1>
          <p className="text-sm text-muted-foreground">Capture your thoughts</p>
        </header>

        {view === "capture" ? (
          <>
            <CaptureInput onCapture={handleCapture} />
            
            {capturesArray.length > 0 && (
              <div className="flex justify-center">
                <Button
                  onClick={() => setView("list")}
                  variant="outline"
                  className="flex items-center gap-2"
                >
                  <List />
                  View Captures ({capturesArray.length})
                </Button>
              </div>
            )}
          </>
        ) : (
          <CaptureList 
            captures={capturesArray}
            onBack={() => setView("capture")}
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