# Phase 2: Type Inference Improvements

## Overview
Enhance the type inference system with preloaded phrase patterns and learning-based improvements for Actions, Reminders, and Notes.

## Requirements

### 1. Action Item Inference

**Preloaded Action Phrases:**
- "Create a..."
- "Take the..."
- "Build..."
- "Fix..."
- "Update..."
- "Review..."
- "Send..."
- "Call..."
- "Email..."
- "Schedule..."
- "Complete..."
- "Finish..."
- "Submit..."
- "Prepare..."
- "Order..."

**Learning Goals:**
- Learn how the user records action items
- Add user-specific action language to the learning system
- Main rule: If language implies an action that will be taken, it's probably an action item

### 2. Reminder Inference

**Preloaded Reminder Phrases:**
- "Remind me to..."
- "Remember to..."
- "Don't forget to..."
- "Need to remember..."
- "Reminder:"
- "Remember:"

**Special Handling:**
- Reminders often have action items implied
- If language is "do something at X date", lean towards reminder
- Consider date/time presence as a strong reminder indicator

### 3. Note Inference

**Catchall Logic:**
- If confidence is low for both action and reminder, confidence is high for note
- Notes are the default fallback type

**Note Characteristics:**
- Information to remember
- Observations
- Ideas
- References
- Could potentially turn into an action item or reminder later

### 4. Action-Specific Attributes (Data Model Only - No UI)

**Due Date:**
- Date when the action must be completed
- User-entered or inferred from text

**Review Date:**
- Date when the action should be reviewed
- Default: 90 days if no due date inferred
- If due date is > 90 days away: review date = 90 days from now
- If due date is within a week: review date = Sunday before that week starts
- If due date is specified: review date can be null or calculated based on due date

**Start Date:**
- Date when the action can/should begin
- Optional, can be inferred from text like "starting next week"

**Implementation Notes:**
- Add properties to Item interface
- No UI for editing these dates yet (wait for Phase 4)
- Focus this phase on type inference accuracy

### 5. Type Confidence Threshold

**95% Threshold Rule:**
- If type confidence >= 95%: auto-select that type, hide other types, show pencil icon to edit
- If type confidence < 95%: show review UI with inferred type selected, confidence displayed, edit mode enabled
- Always display confidence percentage in review

### 6. Priority and Tags (Future)

**Actions will have:**
- Priorities (low/medium/high)
- Tags for organization

**Reminders will have:**
- Reminder date (when to surface the reminder)
- Review date (if reminder is far in the future)

**Notes will have:**
- Review dates (default 90 days)

**Note:** These are NOT part of Phase 2 - focus on type inference only

## Acceptance Criteria

- [ ] Preloaded action phrases correctly identify action items
- [ ] Preloaded reminder phrases correctly identify reminders
- [ ] Notes serve as catchall when action/reminder confidence is low
- [ ] Learning system captures user-specific action language
- [ ] Type confidence threshold (95%) correctly controls review UI visibility
- [ ] Confidence percentage always displayed in review
- [ ] Data model includes due/review/start dates for actions (no UI yet)
- [ ] Type inference tests cover all three types with various phrases
- [ ] Edge cases handled: ambiguous text, multiple type indicators

## Technical Implementation

### Files to Modify

1. `src/lib/typeInference.ts`
   - Add preloaded phrase patterns for actions and reminders
   - Implement catchall logic for notes
   - Update confidence calculation
   - Add learning system for user-specific patterns

2. `src/lib/types.ts`
   - Add `dueDate`, `reviewDate`, `startDate` properties to Item interface

3. `src/components/AttributeConfirmation.tsx`
   - Update type display logic for 95% threshold
   - Add pencil icon for high-confidence types
   - Ensure confidence always displayed

4. `src/lib/typeInference.test.ts`
   - Add tests for preloaded phrases
   - Add tests for learning system
   - Add tests for confidence calculations
   - Add tests for catchall logic

### Testing Strategy

- Test each preloaded phrase pattern
- Test combinations (text with multiple type indicators)
- Test learning system with user corrections
- Test confidence threshold behavior
- Test edge cases (empty text, special characters, etc.)

## Priority

High - This is a foundational improvement needed before Phase 3

## Estimated Effort

2-3 days

## Dependencies

- Phase 1 must be completed first (simplified UI with Type and Collection only)

## Notes

- Phase 2 focuses exclusively on improving type inference
- No UI work for date attributes yet
- Keep the UI simple (Type and Collection only)
- Priority, tags, and other attributes come in later phases
