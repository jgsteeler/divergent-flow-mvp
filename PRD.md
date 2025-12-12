# Divergent Flow MVP - ADHD Brain Management Tool

An intelligent capture-first productivity system designed specifically for ADHD brains that starts with zero configuration and learns with the user.

**Experience Qualities**: 
1. **Frictionless** - Zero-barrier capture means thoughts are preserved before they evaporate
2. **Learns with You** - The system starts blank and builds its intelligence as you use it
3. **Calm** - Single-screen, distraction-free interface that grows organically with your needs

**Complexity Level**: Light Application (multiple features with basic state)
This MVP starts with pure capture to build a blank slate foundation. The system will learn patterns and build inference models based on user behavior, not pre-configured rules.

## Development Phases

### Phase 1: Blank Slate Foundation (Current)
- Pure capture interface
- Persistent storage of raw text captures
- Zero configuration, zero assumptions
- Helpful prompts to guide discovery
- Simple capture counter to show the system is working

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

## Future Features (Phase 2+)

### Intelligent Processing (Phase 2)
- LLM-powered inference that learns from user patterns
- Natural language date/time parsing
- Automatic item type detection (note/action/reminder)
- Collection inference based on learned patterns
- Confidence scoring for all inferences

### ADHD Review Queue (Phase 3)
- Surface items needing clarification
- Learn from user corrections
- Build inference model over time

### ADHD Dashboard (Phase 4)
- Next Action (single focus point)
- Quick Wins (easy completions)
- Review Queue integration
- Adaptive algorithms that learn user patterns

### Item Completion (Phase 5)
- Mark items complete
- Satisfying animations
- Completion history for learning

## Edge Case Handling (Phase 1)
- **Empty State**: Encouraging header and helpful placeholder; system feels welcoming even with zero data
- **First Capture**: Instant feedback with toast notification builds confidence
- **Capture Counter**: Shows system is working and building history

## Edge Case Handling (Future Phases)
- **Network Failures**: LLM calls fail gracefully; items queue for retry
- **Ambiguous Captures**: Route to review queue rather than guessing
- **Duplicate Prevention**: Similar recent captures flagged during inference
- **Date Parsing Failures**: Helpful error messages with examples
- **Overdue Items**: Visual highlights for past-due dates

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
