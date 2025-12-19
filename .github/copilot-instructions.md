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
- ✅ Write unit tests for all new features and changes
- ✅ Follow Conventional Commits specification
- ✅ Apply SOLID principles in architecture

## Conventional Commits

**ALL commits MUST follow the [Conventional Commits](https://www.conventionalcommits.org/) specification.**

### Commit Message Format
```
<type>(<scope>): <description>

[optional body]

[optional footer(s)]
```

### Commit Types
- **feat**: A new feature (triggers MINOR version bump)
- **fix**: A bug fix (triggers PATCH version bump)
- **docs**: Documentation only changes
- **style**: Code style changes (formatting, missing semi-colons, etc.)
- **refactor**: Code change that neither fixes a bug nor adds a feature
- **perf**: Performance improvement
- **test**: Adding or updating tests
- **chore**: Changes to build process, dependencies, or auxiliary tools
- **ci**: Changes to CI configuration files and scripts
- **build**: Changes that affect the build system or external dependencies

### Scope Examples
- `capture`: Capture input functionality
- `review`: Review queue feature
- `inference`: Type inference engine
- `ui`: UI components
- `storage`: Data persistence layer
- `types`: TypeScript type definitions
- `tests`: Test files

### Commit Message Examples
```bash
# New feature
feat(capture): add keyboard shortcut Cmd+Enter to submit

# Bug fix
fix(review): correct priority sorting in review queue

# Documentation
docs(readme): update installation instructions

# Refactoring
refactor(inference): extract pattern matching to separate function

# Breaking change
feat(storage)!: migrate from Spark KV to PostgreSQL

BREAKING CHANGE: Storage layer completely redesigned.
Migration script required for existing data.
```

### Breaking Changes
- Add `!` after type/scope: `feat(api)!: redesign inference API`
- Include `BREAKING CHANGE:` footer with description
- Triggers MAJOR version bump (when version >= 1.0.0)

### Best Practices
- Use imperative mood: "add feature" not "added feature"
- Keep description under 72 characters
- Don't end description with period
- Reference issues: `fix(review): correct null check (#123)`
- Use body to explain WHAT and WHY, not HOW

## Semantic Versioning & Releases

This project uses **[Release Please](https://github.com/googleapis/release-please)** for automated semantic versioning.

### Version Bumping Rules
- **MAJOR** (x.0.0): Breaking changes (commit with `!` or `BREAKING CHANGE:` footer)
- **MINOR** (0.x.0): New features (`feat:` commits)
- **PATCH** (0.0.x): Bug fixes (`fix:` commits)
- Other commit types (docs, chore, etc.) don't trigger releases

### Pre-1.0.0 Behavior
- Breaking changes bump MINOR version (not MAJOR)
- Features bump PATCH version
- Configured in `.github/release-please-config.json`

### Release Process
1. Commits following Conventional Commits are pushed
2. Release Please analyzes commits and creates/updates release PR
3. When release PR is merged, automated release is created
4. Version is bumped in `package.json` and `CHANGELOG.md` is updated

### Manual Release Notes
- Release Please generates changelog from commit messages
- Write clear commit descriptions - they become release notes!
- Group related changes in single commit when logical

## Unit Testing Requirements

**ALL code changes MUST include appropriate unit tests.**

### Testing Framework
- **Unit/Integration**: Vitest
- **E2E**: Playwright
- **File naming**: `*.test.ts` or `*.spec.ts`

### When to Write Tests
- ✅ New features: Test all public functions and components
- ✅ Bug fixes: Add test that reproduces bug, then fix it
- ✅ Refactoring: Ensure existing tests pass, add missing coverage
- ✅ API changes: Update tests to match new behavior

### What to Test
```typescript
// Component testing
- Rendering with different props
- User interactions (clicks, inputs, keyboard)
- State changes and side effects
- Error boundaries and loading states

// Logic testing (lib/)
- Pure functions with various inputs
- Edge cases and error conditions
- Type inference accuracy
- Priority calculations

// Integration testing
- Data flow between components
- Storage layer operations
- API calls and responses (mocked)
```

### Testing Best Practices
- **Arrange-Act-Assert**: Structure tests clearly
- **Test behavior, not implementation**: Don't test internal details
- **Mock external dependencies**: APIs, storage, timers
- **Use descriptive test names**: `it('should prioritize items with no type over low confidence')`
- **Cover edge cases**: Empty inputs, null values, extreme numbers
- **Keep tests fast**: Mock expensive operations
- **One assertion per test** (when logical): Makes failures clear

### Test Coverage Goals
- Aim for >80% coverage on business logic (`lib/`)
- Aim for >60% coverage on components
- 100% coverage on critical paths (data persistence, inference)

### Example Test Structure
```typescript
import { describe, it, expect, vi } from 'vitest'
import { calculatePriority } from './reviewPriority'

describe('calculatePriority', () => {
  it('should assign highest priority to items without type', () => {
    const item = { text: 'test', createdAt: Date.now() }
    const priority = calculatePriority(item)
    expect(priority).toBe(3)
  })

  it('should handle null confidence gracefully', () => {
    const item = { 
      text: 'test', 
      inferredType: 'note',
      typeConfidence: null 
    }
    expect(() => calculatePriority(item)).not.toThrow()
  })
})
```

### Running Tests
```bash
# Run all tests
npm test

# Run tests in watch mode
npm test -- --watch

# Run with coverage
npm test -- --coverage

# Run specific test file
npm test src/lib/typeInference.test.ts
```

## SOLID Principles

**Apply SOLID principles to maintain clean, maintainable architecture.**

### Single Responsibility Principle (SRP)
Each module/component should have ONE reason to change.

**✅ Good:**
```typescript
// lib/typeInference.ts - ONLY handles type inference logic
export function inferType(text: string): ItemType { }

// lib/storage.ts - ONLY handles data persistence
export function saveItem(item: Item): Promise<void> { }

// components/CaptureInput.tsx - ONLY handles capture input UI
export function CaptureInput() { }
```

**❌ Bad:**
```typescript
// CaptureInput.tsx doing too much
export function CaptureInput() {
  // ❌ Handles UI, inference, AND storage
  const inferType = (text) => { /* inference logic */ }
  const saveToStorage = (item) => { /* storage logic */ }
}
```

### Open/Closed Principle (OCP)
Open for extension, closed for modification.

**✅ Good:**
```typescript
// lib/typeInference.ts - Easy to add new patterns
const PATTERNS: Record<ItemType, RegExp[]> = {
  action: [/\b(do|task|todo)\b/i],
  note: [/\b(note|idea|thought)\b/i],
  // Add new type here without modifying logic
}

export function inferType(text: string): ItemType {
  // Generic logic works with any pattern
  for (const [type, patterns] of Object.entries(PATTERNS)) {
    if (patterns.some(p => p.test(text))) return type
  }
}
```

**❌ Bad:**
```typescript
// Hard-coded logic that requires modification for each new type
export function inferType(text: string): ItemType {
  if (text.includes('do') || text.includes('task')) return 'action'
  if (text.includes('note')) return 'note'
  // ❌ Must modify this function for each new type
}
```

### Liskov Substitution Principle (LSP)
Subtypes must be substitutable for their base types.

**✅ Good:**
```typescript
// Common interface
interface StorageProvider {
  save(item: Item): Promise<void>
  load(id: string): Promise<Item>
}

// Both implementations are interchangeable
class SparkKVStorage implements StorageProvider { }
class PostgresStorage implements StorageProvider { }

// Can swap implementations without breaking code
const storage: StorageProvider = new SparkKVStorage()
```

### Interface Segregation Principle (ISP)
Don't force clients to depend on interfaces they don't use.

**✅ Good:**
```typescript
// Separate interfaces for different concerns
interface Readable {
  load(id: string): Promise<Item>
}

interface Writable {
  save(item: Item): Promise<void>
}

// Components use only what they need
function ReviewQueue({ storage }: { storage: Readable }) { }
function CaptureInput({ storage }: { storage: Writable }) { }
```

**❌ Bad:**
```typescript
// Bloated interface
interface Storage {
  save(item: Item): Promise<void>
  load(id: string): Promise<Item>
  delete(id: string): Promise<void>
  backup(): Promise<void>
  restore(): Promise<void>
  // ❌ Not all components need all methods
}
```

### Dependency Inversion Principle (DIP)
Depend on abstractions, not concretions.

**✅ Good:**
```typescript
// components/ReviewQueue.tsx - Depends on abstraction
interface ItemRepository {
  getItemsNeedingReview(): Promise<Item[]>
}

export function ReviewQueue({ repository }: { repository: ItemRepository }) {
  // Works with ANY implementation of ItemRepository
}
```

**❌ Bad:**
```typescript
// components/ReviewQueue.tsx - Depends on concrete implementation
import { sparkKV } from '../lib/sparkStorage'

export function ReviewQueue() {
  // ❌ Tightly coupled to Spark KV
  const items = sparkKV.getItems()
}
```

### Applying SOLID in This Project

**Component Design:**
- Keep components under 200 lines (SRP)
- Extract business logic to `lib/` (SRP, DIP)
- Use props/context for dependencies (DIP)
- Create small, focused hooks (SRP, ISP)

**State Management:**
- Separate UI state from business logic (SRP)
- Use custom hooks for reusable logic (OCP)
- Keep reducers/actions focused (SRP)

**Type System:**
- Define clear interfaces (ISP)
- Use discriminated unions for variants (LSP)
- Export types for extensibility (OCP, DIP)

**File Structure:**
```
src/
├── components/       # UI components (SRP)
├── lib/             # Business logic (SRP, DIP)
├── hooks/           # Reusable hooks (SRP, ISP)
├── types/           # Shared interfaces (OCP, ISP)
└── services/        # External integrations (DIP)
```
