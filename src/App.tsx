import { useState } from "react";
import { useLocalStorage } from "./hooks/useLocalStorage";
import { Item, ItemType, Priority, Estimate, TypeLearningData, PriorityLearningData, EstimateLearningData } from "@/lib/types";
import { CaptureInput } from "@/components/CaptureInput";
import { AttributeConfirmation } from "@/components/AttributeConfirmation";
import { ReviewQueue } from "@/components/ReviewQueue";
import { toast } from "sonner";
import { Toaster } from "@/components/ui/sonner";
import { inferAttributes } from "@/lib/inference";
import { inferPriority, inferEstimate, savePriorityLearning, saveEstimateLearning } from "@/lib/priorityEstimateInference";
import { getTopReviewItems } from "@/lib/reviewPriority";

function App() {
  const [items, setItems] = useLocalStorage<Item[]>("items", []);
  const [typeLearning, setTypeLearning] = useLocalStorage<TypeLearningData[]>("type-learning", []);
  const [attributeLearning, setAttributeLearning] = useLocalStorage("attribute-learning", []);
  const [priorityLearning, setPriorityLearning] = useLocalStorage<PriorityLearningData[]>("priority-learning", []);
  const [estimateLearning, setEstimateLearning] = useLocalStorage<EstimateLearningData[]>("estimate-learning", []);
  const [isProcessing, setIsProcessing] = useState(false);
  const [pendingConfirmation, setPendingConfirmation] = useState<Item | null>(null);

  const itemsArray = items || [];
  const typeLearningArray = typeLearning || [];
  const attributeLearningArray = attributeLearning || [];
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
    const attributes = await inferAttributes(item.text, attributeLearningArray);

    const { priority, confidence: priorityConf, reasoning: priorityReason } = inferPriority(
      item.text,
      attributes.type,
      priorityLearningArray
    );

    const { estimate, confidence: estimateConf, reasoning: estimateReason } = inferEstimate(
      item.text,
      attributes.type,
      estimateLearningArray
    );

    const updatedItem: Item = {
      ...item,
      inferredType: attributes.type || undefined,
      typeConfidence: attributes.typeConfidence,
      collection: attributes.collection || undefined,
      collectionConfidence: attributes.collectionConfidence,
      dueDate: attributes.dueDate || undefined,
      priority: priority || undefined,
      priorityConfidence: priorityConf,
      priorityReasoning: priorityReason,
      estimate: estimate || undefined,
      estimateConfidence: estimateConf,
      estimateReasoning: estimateReason,
      context: attributes.context || undefined,
      tags: attributes.tags || undefined,
    };

    setItems((current) =>
      (current || []).map((i) => (i.id === item.id ? updatedItem : i))
    );

    const needsReview =
      !attributes.type ||
      !attributes.collection ||
      (attributes.typeConfidence && attributes.typeConfidence < 85) ||
      (attributes.collectionConfidence && attributes.collectionConfidence < 85) ||
      (priorityConf < 85) ||
      (estimateConf < 85);

    if (needsReview) {
      setPendingConfirmation(updatedItem);
    } else {
      const reviewedItem = { ...updatedItem, lastReviewedAt: Date.now() };
      setItems((current) =>
        (current || []).map((i) => (i.id === item.id ? reviewedItem : i))
      );
    }
  };

  const handleAttributeConfirm = async (itemId: string, updates: Partial<Item>) => {
    const item = itemsArray.find((i) => i.id === itemId);
    if (!item) return;

    const updatedItem: Item = {
      ...item,
      ...updates,
    };

    setItems((current) =>
      (current || []).map((i) => (i.id === itemId ? updatedItem : i))
    );

    const learningData = {
      originalText: item.text,
      inferredAttributes: {
        type: item.inferredType,
        collection: item.collection,
        priority: item.priority,
        dueDate: item.dueDate,
        typeConfidence: item.typeConfidence,
        collectionConfidence: item.collectionConfidence,
      },
      correctedAttributes: {
        type: updates.inferredType,
        collection: updates.collection,
        priority: updates.priority,
        dueDate: updates.dueDate,
        typeConfidence: 100,
        collectionConfidence: 100,
      },
      timestamp: Date.now(),
      wasCorrect: item.inferredType === updates.inferredType && item.collection === updates.collection,
    };

    setAttributeLearning((current) => [...(current || []), learningData]);

    if (updates.priority) {
      const newPriorityLearning = await savePriorityLearning(
        item.text,
        item.priority || null,
        updates.priority,
        item.priorityConfidence || 0
      );
      if (newPriorityLearning) {
        setPriorityLearning((current) => [...(current || []), newPriorityLearning]);
      }
    }

    if (updates.estimate) {
      const newEstimateLearning = await saveEstimateLearning(
        item.text,
        item.estimate || null,
        updates.estimate,
        item.estimateConfidence || 0
      );
      if (newEstimateLearning) {
        setEstimateLearning((current) => [...(current || []), newEstimateLearning]);
      }
    }

    setPendingConfirmation(null);

    const wasTypeCorrect = item.inferredType === updates.inferredType;
    const wasCollectionCorrect = item.collection === updates.collection;

    if (wasTypeCorrect && wasCollectionCorrect) {
      toast.success("Confirmed! I'm learning from your input.");
    } else {
      toast.success("Updated! I'll remember that for next time.");
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
          <AttributeConfirmation
            item={pendingConfirmation}
            learningData={attributeLearningArray}
            onConfirm={handleAttributeConfirm}
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
      
      <footer className="mt-8 pb-4 text-center">
        <p className="text-xs text-muted-foreground">v{__APP_VERSION__}</p>
      </footer>
    </div>
  );
}

export default App;