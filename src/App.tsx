import { useState } from "react";
import { useLocalStorage } from "./hooks/useLocalStorage";
import { Item, ItemType, TypeLearningData, LearningData } from "@/lib/types";
import { CaptureInput } from "@/components/CaptureInput";
import { AttributeConfirmation } from "@/components/AttributeConfirmation";
import { ReviewQueue } from "@/components/ReviewQueue";
import { toast } from "sonner";
import { Toaster } from "@/components/ui/sonner";
import { inferType, saveTypeLearning } from "@/lib/typeInference";
import { inferAttributes } from "@/lib/inference";
import { getTopReviewItems } from "@/lib/reviewPriority";
import { HIGH_CONFIDENCE_THRESHOLD, CONFIRMED_CONFIDENCE } from "@/lib/constants";

function App() {
  const [items, setItems] = useLocalStorage<Item[]>("items", []);
  const [typeLearning, setTypeLearning] = useLocalStorage<TypeLearningData[]>(
    "type-learning",
    []
  );
  const [attributeLearning, setAttributeLearning] = useLocalStorage<LearningData[]>(
    "attribute-learning",
    []
  );
  const [isProcessing, setIsProcessing] = useState(false);
  const [pendingConfirmation, setPendingConfirmation] = useState<Item | null>(null);

  const itemsArray = items || [];
  const learningArray = typeLearning || [];
  const attributeLearningArray = attributeLearning || [];

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
    // Infer all attributes: type, collection, dates, etc.
    const attributes = await inferAttributes(item.text, attributeLearningArray);
    
    const updatedItem: Item = {
      ...item,
      inferredType: attributes.type || undefined,
      typeConfidence: attributes.typeConfidence,
      collection: attributes.collection || undefined,
      collectionConfidence: attributes.collectionConfidence,
      dueDate: attributes.dueDate || undefined,
      priority: attributes.priority || undefined,
      context: attributes.context || undefined,
      tags: attributes.tags || undefined,
    };

    setItems((current) =>
      (current || []).map((i) => (i.id === item.id ? updatedItem : i))
    );

    // Show confirmation dialog if confidence is low or missing critical fields
    const needsReview = 
      !attributes.type || 
      !attributes.collection ||
      (attributes.typeConfidence && attributes.typeConfidence < HIGH_CONFIDENCE_THRESHOLD) ||
      (attributes.collectionConfidence && attributes.collectionConfidence < HIGH_CONFIDENCE_THRESHOLD);
    
    if (needsReview) {
      setPendingConfirmation(updatedItem);
    } else {
      // High confidence - mark as reviewed
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

    // Save type learning if type was changed
    if (updates.inferredType) {
      const newLearning = await saveTypeLearning(
        item.text,
        item.inferredType || null,
        updates.inferredType,
        item.typeConfidence || 0
      );

      if (newLearning) {
        setTypeLearning((current) => [...(current || []), newLearning]);
      }
    }

    // Save attribute learning for collection and other properties
    const learningData: LearningData = {
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
        typeConfidence: CONFIRMED_CONFIDENCE,
        collectionConfidence: CONFIRMED_CONFIDENCE,
      },
      timestamp: Date.now(),
      wasCorrect: item.inferredType === updates.inferredType && item.collection === updates.collection,
    };

    setAttributeLearning((current) => [...(current || []), learningData]);

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
      setPendingConfirmation(item);
    }
  };

  return (
    <div className="min-h-screen bg-background p-4 md:p-8">
      <Toaster />
      <div className="max-w-4xl mx-auto space-y-8">
        <header className="text-center space-y-2">
          <h1 className="text-3xl md:text-4xl font-bold text-primary">
            Divergent Flow
          </h1>
          <p className="text-muted-foreground">
            Your ADHD-friendly external brain
          </p>
        </header>

        <CaptureInput onCapture={handleCapture} isProcessing={isProcessing} />

        {pendingConfirmation && (
          <AttributeConfirmation
            item={pendingConfirmation}
            onConfirm={handleAttributeConfirm}
            onDismiss={handleDismiss}
          />
        )}

        {!pendingConfirmation && reviewItems.length > 0 && (
          <ReviewQueue items={reviewItems} onReviewItem={handleReviewItem} />
        )}

        {itemsArray.length > 0 && (
          <div className="text-center space-y-1">
            <div className="text-sm text-muted-foreground">
              {itemsArray.length}{" "}
              {itemsArray.length === 1 ? "item" : "items"} saved
            </div>
            {learningArray.length > 0 && (
              <div className="text-xs text-muted-foreground/70">
                Learning from{" "}
                {learningArray.length === 1 ? "pattern" : "patterns"}
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
}

export default App;