# Divergent Flow MVP - ADHD Brain Management Tool

An intelligent capture-first productivity system designed specifically for ADHD brains that starts with zero configuration and learns with the user.

**Experience Qualities**: 
1. **Frictionless** - Zero-barrier capture means thoughts are preserved before they evaporate
2. **Learns with You** - The system starts blank and builds its intelligence as you use it
3. **Calm** - Single-screen, distraction-free interface that grows organically with your needs

**Complexity Level**: Light Application (multiple features with basic state)
This MVP starts with pure capture to build a blank slate foundation. The system will learn patterns and build inference models based on user behavior, not pre-configured rules.

## Development Phases

### Phase 1: Blank Slate Foundation (Complete)
- Pure capture interface
- Persistent storage of raw text captures
- Zero configuration, zero assumptions
- Helpful prompts to guide discovery
- Simple capture counter to show the system is working

### Phase 2: Type Inference (Complete)
- Basic inference engine using keyword/phrase pattern matching
- Confidence scoring (0-100% numeric) for inferences
- User prompts for type confirmation when confidence < 90%
- Learning storage to improve future inferences
- Support for three item types: note, action, reminder

### Phase 3: Review Queue (Current)
- Priority-based review queue surfacing top 3 items needing attention
- Review criteria based purely on confidence percentage: no type OR confidence < 90% OR invalid confidence data
- Priority algorithm: no type (highest priority) > invalid/low confidence (< 90% or invalid data) > oldest unreviewed (by lastReviewedAt or createdAt)
- Items disappear from queue after review and confirmation (typeConfirmed=true)
- Queue only shows unconfirmed items, keeping focus on what needs attention
- Maximum 3 items shown to avoid overwhelming ADHD users
- Queue appears on page load and when there are unreviewed items (not when type confirmation dialog is open)
- Type confirmation sets typeConfirmed=true, typeConfidence=100, and lastReviewedAt
- High confidence items (≥90%) are auto-confirmed with typeConfirmed=true and never appear in review queue
- Visual indicators for review priority and reasons

## Essential Features (Phase 1)

### 1. Quick Capture
- **Functionality**: Large, always-accessible text input that accepts any natural language text with zero processing
- **Purpose**: Preserve fleeting thoughts instantly without any categorization overhead
- **Trigger**: User focuses on capture input (default focus on load)
- **Progression**: Focus input → Type thought → Press Cmd/Ctrl+Enter or click Capture → Visual confirmation → Input clears → Capture count updates → Ready for next capture
- **Success criteria**: Capture completes in under 1 second; no processing, no inference, just pure storage; helpful placeholder suggests asking about the app itself

### 2. Persistent Storage
- **Functionality**: All captures are automatically saved to browser storage
- **Purpose**: Build trust that thoughts won't be lost
- **Trigger**: Immediately on capture
- **Progression**: Capture created → Saved to KV store → UI updates to show count
- **Success criteria**: Captures persist across page refreshes; storage is transparent to user

### 3. Discovery Guidance
- **Functionality**: Helpful placeholder text that encourages users to ask about the app
- **Purpose**: Guide new users through discovering how to use the system organically
- **Trigger**: On first load
- **Progression**: User sees suggestion → Captures question about app → (Future: AI responds with guidance)
- **Success criteria**: Placeholder text invites exploration without overwhelming with instructions

### 4. Type Inference (Phase 2)
- **Functionality**: Analyze capture text using keyword/phrase patterns to determine if it's a note, action item, or reminder
- **Purpose**: Automatically categorize captures without manual user configuration
- **Trigger**: Immediately after capture is saved
- **Progression**: Capture saved → Pattern matching analysis → Confidence scoring → User prompt if needed → Type confirmation → Learning storage updated
- **Success criteria**: High confidence inferences (90%+) auto-confirm; medium/low confidence prompt user; learning improves accuracy over time

### 5. Type Confirmation Dialog
- **Functionality**: Interactive UI prompting user to confirm or correct inferred item type
- **Purpose**: Validate inferences and collect training data for learning system
- **Trigger**: When inference confidence is medium or low, or no type could be inferred
- **Progression**: Prompt appears → Shows inferred type (if any) with confidence badge → User selects correct type → Learning data saved → Capture updated
- **Success criteria**: Clear explanation of each type; easy single-click confirmation; dismissable without blocking workflow

### 6. Learning System
- **Functionality**: Store patterns from user confirmations to improve future type inference
- **Purpose**: System becomes smarter with each correction, reducing need for manual confirmation over time
- **Trigger**: When user confirms or corrects a type
- **Progression**: User confirms type → Pattern extracted from text → Stored with type and confidence → Applied to future inferences
- **Success criteria**: Learning data persists; patterns influence future inferences; accuracy improves with usage

### 7. Review Queue (Phase 3)
- **Functionality**: Display top 3 items needing attention based on confidence-based priority algorithm
- **Purpose**: Surface items requiring user input in a manageable, non-overwhelming way
- **Trigger**: On page load and when not showing type confirmation dialog
- **Progression**: Algorithm calculates priorities → Top 3 items displayed → Visual indicators show priority and reason → User clicks Review → Opens type confirmation dialog → Item updated with typeConfirmed=true and removed from queue
- **Success criteria**: Priority algorithm correctly identifies items needing review; queue shows helpful context; limited to 3 items to avoid overwhelming; items disappear after confirmation and don't reappear

### 8. Priority Algorithm
- **Functionality**: Score all captures based on type confidence to determine review priority
- **Purpose**: Ensure items needing user input get attention, using simple confidence-based prioritization
- **Trigger**: Runs whenever review queue is displayed
- **Progression**: 
  1. Items without type: Priority 1000 (highest)
  2. Items with invalid confidence data (NaN, undefined, <0, >100): Priority 900
  3. Items with confidence < 90%: Priority 900
  4. Items with typeConfirmed=true are excluded from review queue entirely
  5. Within same priority level, older items (by lastReviewedAt or createdAt) appear first
- **Success criteria**: Critical items always surface first; confidence percentage is the sole determinant; confirmed items never reappear; maximum 3 items shown at once

### 9. Type Confirmation Flow
- **Functionality**: User confirms or corrects item type through interactive dialog
- **Purpose**: Build confidence in the system and train the learning model
- **Trigger**: When review item is clicked from queue
- **Progression**: Review button clicked → Type confirmation dialog opens → User selects type → Type is set with typeConfirmed=true and typeConfidence=100 → Learning data saved → Item marked as reviewed
- **Success criteria**: Confirmed types have 100% confidence; typeConfirmed flag prevents re-review of already-confirmed items; learning improves future accuracy

### 10. Property Validation (Future Phase)
- **Functionality**: Check that items have required properties based on their type (deferred to future phase)
- **Purpose**: Ensure items have necessary context to be actionable
- **Status**: Not yet implemented - Phase 3 focuses purely on type confirmation
- **Future Implementation**: Action items will be checked for priority; Reminders for due date; Actions for context when missing due date

## Future Features (Phase 4+)

### Intelligent Processing (Phase 4)
- LLM-powered inference for collection/priority/context
- Natural language date/time parsing
- Collection inference based on learned patterns
- Priority inference
- Context and tag extraction

### ADHD Dashboard (Phase 5)
- Next Action (single focus point)
- Quick Wins (easy completions)
- Review Queue integration
- Adaptive algorithms that learn user patterns

### Item Completion (Phase 6)
- Mark items complete
- Satisfying animations
- Completion history for learning

## Edge Case Handling (Phase 1)
- **Empty States**: Show encouraging prompts when no captures exist; never show empty lists without context
- **Network Failures**: LLM calls fail gracefully; items queue for retry; user can still capture offline
- **Ambiguous Captures**: Single word or vague entries route to review queue rather than guessing incorrectly
- **Duplicate Prevention**: Similar recent captures flagged during inference to avoid clutter
- **Date Parsing Failures**: Natural language date/time parser handles 20+ patterns (today, tomorrow, next [day], in X days/weeks, month day, MM/DD, times like 3pm, 5:30am, noon, midnight, etc.). Combines dates with times intelligently (e.g., "tomorrow at 3pm"). Unparseable dates/times show helpful error message in review queue.
- **Overdue Items**: Items with past due dates are highlighted in red to draw attention

## Design Direction
The design should feel like a trusted external brain - calm, organized, and always ready. It should reduce cognitive load through generous whitespace, clear visual hierarchy, and confidence-inspiring feedback. The aesthetic should be modern but warm, professional but not corporate, focused but not sterile.

## Color Selection
A calming yet energizing palette that promotes focus without feeling clinical.


## Color Selection
A calming yet energizing palette that promotes focus without feeling clinical.

- **Primary Color**: Deep teal `oklch(0.55 0.12 200)` - Communicates trust, calm, and focus without the coldness of blue
- **Secondary Colors**: 
  - Warm sand `oklch(0.92 0.02 80)` for backgrounds - Creates comfortable, low-stress environment
  - Charcoal `oklch(0.25 0.01 260)` for structure - Professional without harshness
- **Accent Color**: Energizing coral `oklch(0.70 0.15 25)` - Draws attention to capture input and quick wins without anxiety
- **Foreground/Background Pairings**: 
  - Background (Warm Sand #F5F3EE `oklch(0.92 0.02 80)`): Charcoal text `oklch(0.25 0.01 260)` - Ratio 11.2:1 ✓
  - Primary (Deep Teal): White text `oklch(1 0 0)` - Ratio 5.8:1 ✓
  - Accent (Coral `oklch(0.70 0.15 25)`): Charcoal text `oklch(0.25 0.01 260)` - Ratio 6.4:1 ✓
  - Cards (White): Charcoal text - Ratio 14.8:1 ✓

## Font Selection
Typography should be clear, friendly, and reduce reading friction for users who may have attention or processing challenges.

- **Primary**: Outfit (Google Fonts) - Geometric sans with personality; excellent readability without feeling corporate
- **Monospace**: JetBrains Mono - For timestamps and metadata; technical without being cold

- **Typographic Hierarchy**: 
  - H1 (App Title): Outfit Bold/32px/tight letter spacing - Used sparingly for "Divergent Flow"
  - H2 (Section Headers): Outfit SemiBold/20px/normal spacing - Dashboard sections
  - H3 (Item Titles): Outfit Medium/16px/normal spacing - Individual captures and actions
  - Body (Capture Text): Outfit Regular/15px/relaxed line-height (1.6) - Primary content
  - Meta (Timestamps, Counts): JetBrains Mono Regular/12px/tracked - Secondary info
  - Button Text: Outfit Medium/14px/slight tracking - All interactive elements

## Animations
Animations should provide feedback and celebrate progress without causing distraction or delay.

- **Capture Submission**: Gentle upward fade (200ms) gives sense of thought being "saved to the cloud"
- **Item Completion**: Satisfying check animation + fade out (300ms) provides dopamine hit
- **Dashboard Updates**: Subtle fade-in for new items (150ms) indicates change without jarring
- **Review Queue**: Smooth slide-in from bottom (250ms) when items need attention
- **Loading States**: Pulsing skeleton screens (no spinners) maintain calm during inference

## Component Selection
- **Components**: 
  - `Textarea` for capture input (auto-growing, no scroll)
  - `Card` for dashboard sections and individual items
  - `Badge` for item types (note/action/reminder), priority indicators
  - `Button` for completion actions, review submissions
  - `Separator` for visual breathing room between sections
  - `ScrollArea` for review queue and completed items list
  - `Skeleton` for loading states during LLM inference
- **Customizations**: 
  - Custom FloatingCaptureInput component (always-accessible, grows with content)
  - Custom PriorityIndicator component (color-coded dots/bars)
  - Custom ItemCard component (swipeable on mobile for completion)
- **States**: 
  - Buttons: Subtle shadow on rest, lift on hover, press animation, disabled with 50% opacity
  - Inputs: Soft border on rest, accent border + subtle glow on focus, error state with coral border
  - Cards: Flat on rest, subtle lift on hover for interactive cards, pressed state for swipe-to-complete
- **Icon Selection**: 
  - `Plus` for new capture
  - `CheckCircle` for completion
  - `Lightning` for quick wins
  - `ArrowRight` for next action
  - `Question` for items needing review
  - `Tag` for collections
  - `Calendar` for date-related items
- **Spacing**: 
  - Base unit: 4px (Tailwind default)
  - Section padding: 6-8 (24-32px)
  - Card gaps: 4 (16px)
  - Card padding: 4-6 (16-24px)
  - Button padding: px-6 py-3
- **Mobile**: 
  - Single column layout (no sidebar)
  - Sticky capture input at bottom on mobile
  - Swipe gestures for completion
  - Larger touch targets (min 44px)
  - Collapsible sections to manage vertical space
  - Bottom sheet for review queue on mobile
