# Phase 3: Collection Learning System Redesign

## Overview
Redesign the collection inference system to use keyword-based matching instead of full text matching, enabling more accurate and flexible collection suggestions.

## Current Problems

1. **Overly broad matching**: Everything matches previously learned collections with same confidence
2. **No keyword extraction**: System matches full text instead of meaningful keywords
3. **Single suggestion**: Only shows highest confidence collection
4. **No collection name detection**: Doesn't look for collection names within the text

## Requirements

### 1. Keyword Extraction System

**Concept:**
Break down text into meaningful keywords instead of matching full text.

**Example:**
- Input: "drain oil in the mgb"
- Keywords extracted: ["drain", "oil", "mgb"]
- Store keywords (not full text) with collection in learning table

**Implementation:**
- Extract keywords using natural language processing or rules-based approach
- Filter out common stop words (the, in, a, an, to, etc.)
- Keep nouns, verbs, and proper nouns
- Minimum word length: 3 characters
- Consider using simple NLP if needed (but keep it lightweight)

### 2. Collection Name Detection

**Rule:**
Always check if existing collection names appear in the keywords.

**Example:**
- Existing collections: ["MGB", "Henderson Project", "Jeep"]
- New text: "remember to update jill about the mgb progress"
- Keywords: ["remember", "update", "jill", "mgb", "progress"]
- **Detected:** "mgb" matches existing collection "MGB"
- **Action:** Suggest "MGB" with high confidence (e.g., 85%)

**Special Handling:**
- Case-insensitive matching
- Partial match support (e.g., "henderson" matches "Henderson Project")
- Collection names get higher confidence boost than keyword matches

### 3. Pattern Matching with Confidence Scoring

**Scenario 1: Keyword Overlap**
```
First entry: "drain oil in the mgb" → user selects "MGB"
  Stored: keywords=["drain", "oil", "mgb"], collection="MGB"

Second entry: "drain the oil in the jeep"
  Keywords: ["drain", "oil", "jeep"]
  Match: 2/3 keywords match ("drain", "oil")
  Confidence: 45% (decent overlap but new context)
  Action: Show "MGB" as option with 45% confidence
```

**Scenario 2: Collection Name in Text**
```
After user creates "Jeep" collection...
Third entry: "remember to update jill about the mgb progress"
  Keywords: ["remember", "update", "jill", "mgb", "progress"]
  Matches:
    - "mgb" = collection name match → "MGB" 85% confidence
    - "update" matches previous "Henderson Project" entry → 40% confidence
  Action: Show both "MGB" (85%) and "Henderson Project" (40%)
```

**Scenario 3: No Matches**
```
Entry: "remember to update the henderson project"
  Keywords: ["remember", "update", "henderson", "project"]
  No previous patterns or collection names match
  "project" is a collection-type keyword
  Action: Suggest "Henderson-Project" or "Henderson Project" with medium confidence (50%)
```

### 4. Confidence Calculation

**Base Confidence:**
- Collection name match: 85% base
- Strong keyword overlap (>75%): 70% base
- Medium keyword overlap (50-75%): 50% base
- Weak keyword overlap (25-50%): 30% base

**Confidence Boosts:**
- +10% if confirmed previously
- +5% per additional matching keyword
- +15% if collection name appears in text

**Maximum Confidence:** 100%

### 5. Multiple Collection Suggestions

**Display Rules:**

**Rule 1:** If any collection >= 95% confidence
- Select that collection automatically
- Still show confidence badge
- Allow user to edit (click to change)

**Rule 2:** If multiple collections match, none >= 95%
- Show up to 4 collections, highest confidence first
- Display each with its confidence percentage
- Show in edit mode with search/create option
- User can select any suggestion or create new

**Rule 3:** If single collection match, confidence < 95%
- Show that collection with confidence
- Show in edit mode with search/create option
- User can select suggestion or create new

**Rule 4:** If no matches
- Show create/search mode only
- No suggestions displayed

### 6. Learning Table Structure

**New Structure:**
```typescript
interface CollectionLearning {
  id: string
  keywords: string[]              // Extracted keywords
  collection: string               // Selected collection
  originalText: string             // Full text for reference
  timestamp: number
  wasCorrect: boolean             // Was the suggestion confirmed?
  confidence: number              // Confidence when suggested
}
```

**Migration:**
- Convert existing learning data to keyword-based format
- Extract keywords from originalText retroactively
- Or start fresh (simpler approach)

### 7. Review UI Display

**95%+ Confidence:**
```
Collection: 98% High
┌─────────────────────────────────┐
│ ✓ MGB                        ✏️ │
│ High confidence - Click to      │
│ change                           │
└─────────────────────────────────┘
```

**Multiple Suggestions (<95%):**
```
Collection: Suggested

┌─────────────────────────────────┐
│ MGB                          85% │
│ Henderson Project            40% │
│ Jeep                         30% │
└─────────────────────────────────┘

Or search/create collection:
[Type to search or create...      ]
```

**No Suggestions:**
```
Collection:

Search or create a collection:
[Type to search or create...      ]
```

## Auto-Skip Review Rule

**Condition:**
If BOTH type >= 95% AND collection >= 95%, skip immediate review.
Item goes directly to the normal review queue.

**Rationale:**
High confidence on both primary attributes means user likely doesn't need to review right away.

## Acceptance Criteria

- [ ] Keyword extraction system implemented and working
- [ ] Collection name detection functional
- [ ] Pattern matching uses keywords instead of full text
- [ ] Confidence scoring reflects keyword overlap accuracy
- [ ] Multiple collection suggestions displayed (up to 4)
- [ ] 95% threshold auto-selects but allows editing
- [ ] Auto-skip review works for high-confidence type + collection
- [ ] Learning table stores keywords
- [ ] Migration path for existing data (or fresh start)
- [ ] Tests cover all scenarios (keyword matching, name detection, multiple suggestions)
- [ ] Decimal places removed from confidence display (show "98%" not "98.33%")

## Technical Implementation

### Files to Modify

1. `src/lib/collectionInference.ts`
   - Complete rewrite with keyword-based approach
   - Implement keyword extraction function
   - Implement collection name detection
   - Update confidence calculation
   - Support multiple suggestions

2. `src/lib/types.ts`
   - Update `LearningData` or create `CollectionLearning` interface
   - Add keywords array field

3. `src/components/AttributeConfirmation.tsx`
   - Update collection display for multiple suggestions
   - Show up to 4 suggestions with confidence
   - Update auto-skip logic (both type and collection >= 95%)

4. `src/lib/collectionInference.test.ts`
   - Rewrite tests for keyword-based system
   - Test keyword extraction
   - Test collection name detection
   - Test multiple suggestions
   - Test confidence calculations
   - Test all display scenarios

### Testing Strategy

- Test keyword extraction with various inputs
- Test collection name detection (case-insensitive, partial matches)
- Test confidence scoring with different keyword overlaps
- Test multiple collection suggestions
- Test auto-skip review logic
- Test migration or fresh start approach
- Test decimal removal from confidence display

## Priority

High - This fixes the critical collection matching bug

## Estimated Effort

3-4 days

## Dependencies

- Phase 1 must be completed first (simplified UI)
- Phase 2 can run in parallel or before

## Notes

- This is a significant rewrite of the collection system
- Consider starting fresh with learning data (simpler than migration)
- Keep performance in mind (keyword extraction should be fast)
- Consider using a lightweight NLP library if needed
- Test thoroughly with real-world examples
