# Divergent Flow MVP - ADHD Brain Management Tool

An intelligent capture-first productivity system designed specifically for ADHD brains that eliminates configuration overhead and provides just-in-time task surfacing.

**Experience Qualities**: 
1. **Frictionless** - Zero-barrier capture means thoughts are preserved before they evaporate
2. **Intelligent** - The system learns and categorizes without requiring manual setup or rules
3. **Calm** - Single-screen, distraction-free interface that surfaces only what matters right now

**Complexity Level**: Light Application (multiple features with basic state)
This MVP focuses on capture → inference → review → dashboard workflow with persistent state, but avoids the complexity of multi-view navigation, heavy configuration, or advanced ML features planned for future phases.

## Essential Features

### 1. Quick Capture
- **Functionality**: Large, always-accessible text input that accepts natural language brain dumps
- **Purpose**: Preserve fleeting thoughts before ADHD causes them to vanish
- **Trigger**: User focuses on capture input (default focus on load)
- **Progression**: Focus input → Type thought → Press Enter/Submit → Visual confirmation → Input clears → Ready for next capture
- **Success criteria**: Capture completes in under 3 seconds; no fields required beyond the thought itself

### 2. Intelligent Processing
- **Functionality**: LLM-powered inference determines item type (note/reminder/action), collection, priority, dates, and context
- **Purpose**: Eliminate manual categorization that ADHD users struggle with
- **Trigger**: Immediately upon capture submission
- **Progression**: Raw capture stored → LLM analyzes text → Extracts type, collection, attributes → If type+collection known, migrates to structured item → Stores inference results for learning
- **Success criteria**: 70%+ captures successfully inferred; processing feels instant (< 2s); failed inferences route to review queue

### 3. ADHD Review Queue
- **Functionality**: Displays 3-5 highest-priority items needing human input
- **Purpose**: Handle edge cases where inference failed, without overwhelming the user
- **Trigger**: Items appear when: inference failed on type/collection, or key attributes missing
- **Progression**: Review item appears → User reads context → Provides missing info (type/collection/priority/dates) → Item migrates → Next review item surfaces
- **Success criteria**: Never shows more than 5 items; prioritizes un-migrated captures first; updates in real-time

### 4. Inference Learning
- **Functionality**: Stores user corrections from review queue to improve future inference accuracy
- **Purpose**: System gets smarter over time without explicit training
- **Trigger**: User completes a review item
- **Progression**: User correction captured → Paired with original text → Stored in learning dataset → Future inferences reference past patterns
- **Success criteria**: Learning data persists; similar future captures show improved inference accuracy

### 5. ADHD Dashboard
- **Functionality**: Smart surface showing: Review Queue (top 5), Next Action (single most important task), Quick Wins (3 easy completions), Recent Captures
- **Purpose**: Answer "what should I do right now?" without overwhelming choice paralysis
- **Trigger**: Always visible; updates reactively
- **Progression**: Dashboard loads → Algorithms calculate priorities → Surfaces 1 next action + 3 quick wins + review items → User acts → Dashboard recalculates
- **Success criteria**: Never more than 10 total items visible; "Next Action" is always singular; updates feel instant

### 6. Item Completion
- **Functionality**: Mark actions/reminders complete with single tap/click
- **Purpose**: Provide dopamine hit and reduce visual clutter
- **Trigger**: User clicks/taps completion indicator on any action or reminder
- **Progression**: User taps item → Completion animation → Item fades out → Dashboard recalculates → New item surfaces if available
- **Success criteria**: Completion feels satisfying (animation); completed items removed from view; total count updates

## Edge Case Handling
- **Empty States**: Show encouraging prompts when no captures exist; never show empty lists without context
- **Network Failures**: LLM calls fail gracefully; items queue for retry; user can still capture offline
- **Ambiguous Captures**: Single word or vague entries route to review queue rather than guessing incorrectly
- **Duplicate Prevention**: Similar recent captures flagged during inference to avoid clutter
- **Date Parsing Failures**: Partial date understanding (e.g., "tomorrow", "next week") should still work

## Design Direction
The design should feel like a trusted external brain - calm, organized, and always ready. It should reduce cognitive load through generous whitespace, clear visual hierarchy, and confidence-inspiring feedback. The aesthetic should be modern but warm, professional but not corporate, focused but not sterile.

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
