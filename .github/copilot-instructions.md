# Divergent Flow - AI Agent Instructions

## Project Overview
ADHD-friendly brain management tool. See PRD.md for complete requirements.

## Current Status
- ✅ Phase 1 complete: Capture + storage
- ✅ Phase 2 complete: Type inference
- 🚧 Phase 3 in progress: Review queue (needs property validation)
- ⏭️ Phase 4 next: LLM-powered inference
- ⏭️ Phase 5-6: Dashboard + completion

## Tech Stack
- **Frontend**: React 19, TypeScript, Vite
- **UI**: Radix UI, Tailwind CSS, Framer Motion
- **State**: React hooks, Spark KV (will migrate to React Query + backend)
- **Icons**: Phosphor Icons
- **Testing**: None yet (needs Vitest + Playwright)

## Code Style
- TypeScript strict mode enabled
- Functional React components with hooks
- No class components
- Keep components under 200 lines
- Separate UI from logic (components/ vs lib/)
- Use existing shadcn/ui patterns

## Design Principles (Critical for ADHD users)
- **Low friction**: Minimize clicks, maximize speed
- **Calm UI**: Generous whitespace, soothing colors (OKLCH)
- **Clear feedback**: Toast messages, smooth animations (200-300ms)
- **Smart defaults**: Infer intent, only ask when necessary
- **Single focus**: One thing at a time, no overwhelming lists

## Naming Conventions
- Components: PascalCase (e.g., `CaptureInput.tsx`)
- Hooks: camelCase with 'use' prefix (e.g., `useItems.ts`)
- Utils/Lib: camelCase (e.g., `typeInference.ts`)
- Types: PascalCase (e.g., `Item`, `ItemType`)

## File Organization
```
src/
├── components/          # React components
│   ├── ui/             # Reusable UI components (Radix)
│   ├── CaptureInput.tsx
│   ├── ReviewQueue.tsx
│   └── TypeConfirmation.tsx
├── lib/                # Business logic
│   ├── types.ts        # TypeScript interfaces
│   ├── typeInference.ts
│   ├── reviewPriority.ts
│   └── utils.ts
├── hooks/              # Custom React hooks
└── App.tsx             # Main app component
```

## Testing Requirements
When adding tests:
- Use Vitest for unit/integration tests
- Use Playwright for E2E tests
- Test files: `*.test.ts` or `*.spec.ts`
- Cover edge cases mentioned in PRD.md
- Mock external APIs (LLM calls)

## API Integration Notes (Phase 4+)
- Use OpenAI API for LLM inference
- Store API key in .env.local (never commit!)
- For Vite projects: Use `.env.local` for local development (auto-ignored)
- For Next.js projects: Use `.env.local` for all local secrets
- Commit `.env.example` with dummy values as template
- Implement retry logic (3 attempts)
- Add rate limiting (user-based)
- Track costs (log token usage)
- Graceful degradation if API fails
- **Critical**: Add `.env.local`, `.env`, and `.env*.local` to `.gitignore`

## Database Migration Notes (Future)
- Current: Spark KV (browser storage)
- Future: .NET backend + PostgreSQL
- Keep data models compatible
- Plan for export/import functionality

## Common Patterns

### Adding a new item property:
1. Update `Item` interface in `src/lib/types.ts`
2. Update inference logic in `src/lib/typeInference.ts` or `src/lib/llmInference.ts`
3. Update UI components to display/edit property
4. Update review queue logic if property affects priority
5. Add tests for new property

### Adding a new inference type:
1. Add to `ItemType` union in `src/lib/types.ts`
2. Add patterns in `typeInference.ts` or LLM prompt
3. Add icon in `TYPE_ICONS` constants
4. Add label and description in `getTypeLabel`/`getTypeDescription`
5. Update type confirmation UI

### Adding a new feature:
1. Review PRD.md to understand requirements
2. Create component in `src/components/`
3. Create logic in `src/lib/`
4. Add to App.tsx
5. Write tests
6. Update documentation if public-facing

## Don't Do This
- ❌ Don't add dependencies without asking (project is lean)
- ❌ Don't change color scheme (carefully designed for ADHD)
- ❌ Don't add multi-step wizards (high friction)
- ❌ Don't remove animations (they provide crucial feedback)
- ❌ Don't make lists paginated (cognitive load)
- ❌ Don't commit .env files
- ❌ Don't use CSS-in-JS (we use Tailwind)

## Always Do This
- ✅ Check PRD.md for requirements
- ✅ Follow existing patterns (look at similar files)
- ✅ Add TypeScript types (no `any`)
- ✅ Handle loading/error states
- ✅ Add animations for user feedback (200-300ms)
- ✅ Test on mobile viewport (ADHD users are often on phones)
- ✅ Keep it simple (ADHD users are easily overwhelmed)
