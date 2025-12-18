import { useState } from "react";
import { useLocalStorage } from "./hooks/useLocalStorage";
import { Item, ItemType, Priority, Estimate, TypeLearningData, PriorityLearningData, EstimateLearningData } from "@/lib/types";
import { CaptureInput } from "@/components/CaptureInput";
import { TypeConfirmation } from "@/components/TypeConfirmation";
import { ReviewQueue } from "@/components/ReviewQueue";
import { toast } from "sonner";
import { Toaster } from "@/components/ui/sonner";
import { inferType, saveTypeLearning } from "@/lib/typeInference";
import { inferPriority, inferEstimate, savePriorityLearning, saveEstimateLearning } from "@/lib/priorityEstimateInference";
import { getTopReviewItems } from "@/lib/reviewPriority";

function App() {
  const [items, setItems] = useLocalStorage<Item[]>("items", []);
  const [typeLearning, setTypeLearning] = useLocalStorage<TypeLearningData[]>("type-learning", []);
  const [priorityLearning, setPriorityLearning] = useLocalStorage<PriorityLearningData[]>("priority-learning", []);
  const [estimateLearning, setEstimateLearning] = useLocalStorage<EstimateLearningData[]>("estimate-learning", []);
  const [isProcessing, setIsProcessing] = useState(false);
  const [pendingConfirmation, setPendingConfirmation] = useState<Item | null>(null);

  const itemsArray = items || [];
  const typeLearningArray = typeLearning || [];
  const priorityLearningArray = priorityLearning || [];
  const estimateLearningArray = estimateLearning || [];

  const reviewItems = getTopReviewItems(itemsArray, 3);

  const handleCapture = async (text: string) => {
    setIsProcessing(true);

    const item: Item = {
      id: `item-${Date.now()}-${Math.random()}`,
      text,
      createdAt: Date.now(),
      migratedCapture: false,
    };

    setItems((current) => [...(current || []), item]);
    toast.success("Captured!");

    setTimeout(async () => {
      await processItem(item);
      setIsProcessing(false);
    }, 100);
  };

  const processItem = async (item: Item) => {
    const { type, confidence, reasoning } = inferType(item.text, typeLearningArray);

    const { priority, confidence: priorityConf, reasoning: priorityReason } = inferPriority(
      item.text,
      type,
      priorityLearningArray
    );

    const { estimate, confidence: estimateConf, reasoning: estimateReason } = inferEstimate(
      item.text,
      type,
      estimateLearningArray
    );

    const updatedItem: Item = {
      ...item,
      inferredType: type || undefined,
      typeConfidence: confidence,
      confidenceReasoning: reasoning,
      priority: priority || undefined,
      priorityConfidence: priorityConf,
      priorityReasoning: priorityReason,
      estimate: estimate || undefined,
      estimateConfidence: estimateConf,
      estimateReasoning: estimateReason,
    };

    setItems((current) =>
      (current || []).map((i) => (i.id === item.id ? updatedItem : i))
    );

    const needsConfirmation =
      confidence < 85 ||
      (type === "action" || type === "reminder") && priorityConf < 85 ||
      type === "action" && estimateConf < 85;

    if (needsConfirmation || !type) {
      setPendingConfirmation(updatedItem);
    } else {
      const autoSavedItem: Item = {
        ...updatedItem,
        lastReviewedAt: Date.now(),
      };
      setItems((current) =>
        (current || []).map((i) => (i.id === item.id ? autoSavedItem : i))
      );
    }
  };

  const handleTypeConfirm = async (
    itemId: string,
    confirmedType: ItemType,
    confirmedPriority?: Priority,
    confirmedEstimate?: Estimate
  ) => {
    const item = itemsArray.find((i) => i.id === itemId);
    if (!item) return;

    const updatedItem: Item = {
      ...item,
      inferredType: confirmedType,
      typeConfidence: 100,
      lastReviewedAt: Date.now(),
    };

    if (confirmedPriority !== undefined) {
      updatedItem.priority = confirmedPriority;
      updatedItem.priorityConfidence = 100;
    }

    if (confirmedEstimate !== undefined) {
      updatedItem.estimate = confirmedEstimate;
      updatedItem.estimateConfidence = 100;
    }

    setItems((current) =>
      (current || []).map((i) => (i.id === itemId ? updatedItem : i))
    );

    const newLearning = await saveTypeLearning(
      item.text,
      item.inferredType || null,
      confirmedType,
      item.typeConfidence || 0
    );

    if (newLearning) {
      setTypeLearning((current) => [...(current || []), newLearning]);
    }

    if (confirmedPriority !== undefined) {
      const newPriorityLearning = await savePriorityLearning(
        item.text,
        item.priority || null,
        confirmedPriority,
        item.priorityConfidence || 0
      );
      if (newPriorityLearning) {
        setPriorityLearning((current) => [...(current || []), newPriorityLearning]);
      }
    }

    if (confirmedEstimate !== undefined) {
      const newEstimateLearning = await saveEstimateLearning(
        item.text,
        item.estimate || null,
        confirmedEstimate,
        item.estimateConfidence || 0
      );
      if (newEstimateLearning) {
        setEstimateLearning((current) => [...(current || []), newEstimateLearning]);
      }
    }

    setPendingConfirmation(null);

    const wasCorrect = item.inferredType === confirmedType;
    if (wasCorrect) {
      toast.success("Confirmed! I'm learning.");
    } else {
      toast.success("Updated! I'll remember that.");
    }
  };

  const handleDismiss = (itemId: string) => {
    setPendingConfirmation(null);
  };

  const handleReviewItem = (itemId: string) => {
    const item = itemsArray.find((i) => i.id === itemId);
    if (item) {
      processItem(item);
    }
  };

  return (
    <div className="min-h-screen bg-background p-4 md:p-8">
      <Toaster />
      <div className="max-w-4xl mx-auto space-y-8">
        <header className="text-center space-y-2">
          <h1 className="text-2xl font-bold">Divergent Flow</h1>
        </header>

        <CaptureInput onCapture={handleCapture} isProcessing={isProcessing} />

        {pendingConfirmation && (
          <TypeConfirmation
            itemId={pendingConfirmation.id}
            text={pendingConfirmation.text}
            inferredType={pendingConfirmation.inferredType || null}
            confidence={pendingConfirmation.typeConfidence || 0}
            reasoning={pendingConfirmation.confidenceReasoning}
            priority={pendingConfirmation.priority}
            priorityConfidence={pendingConfirmation.priorityConfidence}
            priorityReasoning={pendingConfirmation.priorityReasoning}
            estimate={pendingConfirmation.estimate}
            estimateConfidence={pendingConfirmation.estimateConfidence}
            estimateReasoning={pendingConfirmation.estimateReasoning}
            onConfirm={handleTypeConfirm}
            onDismiss={handleDismiss}
          />
        )}

        {!pendingConfirmation && reviewItems.length > 0 && (
          <ReviewQueue items={reviewItems} onReviewItem={handleReviewItem} />
        )}

        {itemsArray.length > 0 && (
          <div className="space-y-4">
            {itemsArray.map((item) => (
              <div key={item.id} className="p-4 bg-white rounded shadow">
                <p>{item.text}</p>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

export default App;