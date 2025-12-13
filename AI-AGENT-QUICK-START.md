# AI Agent Quick Start Guide

## TL;DR - Get Started in 5 Minutes

You're ready to stop using Spark and start using an AI coding agent. Here's how to get started **right now**.

---

## Method 1: GitHub Copilot (Easiest - You're Already Set Up!)

### If you have GitHub Copilot enabled:

1. **Open VS Code in this repo**
2. **Press `Cmd/Ctrl + I`** (Opens Copilot Chat)
3. **Type**: 
   ```
   @workspace Read PRD.md and implement Phase 4 LLM integration. 
   Create src/lib/llmInference.ts that uses OpenAI API to infer 
   collections from item text. Follow the pattern in typeInference.ts.
   ```
4. **Review** the suggested code
5. **Accept** or iterate

### If you DON'T have Copilot:
- Go to github.com/settings/copilot
- Subscribe ($10/month, first 30 days free)
- Install Copilot extension in VS Code
- Restart VS Code

---

## Method 2: Cursor IDE (Most Powerful)

### Setup (10 minutes):
```bash
# 1. Download Cursor
open https://cursor.sh

# 2. Install and open your project
# File -> Open Folder -> select divergent-flow-mvp

# 3. Create .cursorrules file (already provided below)
```

### Use Cursor:
1. **Press `Cmd/Ctrl + K`** anywhere in a file → Edit code
2. **Press `Cmd/Ctrl + L`** → Chat about code
3. **Press `Cmd/Ctrl + Shift + L`** → Composer (multi-file editing)

### Example Session:
```
1. Open Composer (Cmd+Shift+L)
2. Type: "Read PRD.md Phase 4. Implement LLM-powered inference 
   for collections using OpenAI. Add error handling and tests."
3. Review changes across multiple files
4. Accept or iterate
```

---

## Method 3: Aider (CLI - For Terminal Lovers)

### Setup (5 minutes):
```bash
# 1. Install
pip install aider-chat

# 2. Set API key (use your actual key, never commit it!)
export OPENAI_API_KEY=your-actual-openai-api-key-here

# 3. Run in your repo
cd divergent-flow-mvp
aider --model gpt-4-turbo

# 4. Start coding
/add PRD.md
/add src/lib/typeInference.ts

Now implement Phase 4 LLM integration...
```

---

## Your First 3 Tasks with AI Agent

Use these prompts to test your AI agent setup and make real progress:

### Task 1: Add Tests (30 minutes)
```
Set up Vitest for this React + TypeScript project. Then write 
comprehensive unit tests for src/lib/typeInference.ts and 
src/lib/reviewPriority.ts. Cover all edge cases mentioned in PRD.md.
```

**Why this first?** It's well-defined, low-risk, and proves the AI agent can understand your codebase.

### Task 2: Complete Phase 3 (1-2 hours)
```
Read PRD.md Phase 3 section on Property Validation. Implement 
validation that checks:
- Action items have priority property
- Reminders have due date property  
- Actions without due date have context property

Update ReviewQueue.tsx to show these missing properties as review reasons.
Write tests for the validation logic.
```

**Why second?** Builds on existing code, extends patterns you've already established.

### Task 3: Start Phase 4 (2-4 hours)
```
Read PRD.md Phase 4. Implement LLM-powered inference:

1. Create src/lib/llmService.ts with OpenAI client
2. Create src/lib/llmInference.ts with functions:
   - inferCollection(text, learningData)
   - inferPriority(text)
   - extractContext(text)
3. Add error handling, retry logic, and rate limiting
4. Create .env.example with VITE_OPENAI_API_KEY
5. Update App.tsx to use LLM inference after basic type inference
6. Add cost tracking (count tokens used)

Follow the existing patterns in typeInference.ts.
```

**Why third?** Major feature that unlocks intelligent processing.

---

## Project Context File

Create `.github/copilot-instructions.md`:

```markdown
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
- Store API key in .env (never commit!)
- Implement retry logic (3 attempts)
- Add rate limiting (user-based)
- Track costs (log token usage)
- Graceful degradation if API fails

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
```

Save this file, and Copilot/Cursor will use it as context!

---

## Cursor-Specific Setup

If you chose Cursor, also create `.cursorrules` in project root:

```
This is Divergent Flow, an ADHD productivity tool.

CRITICAL CONTEXT:
- Read PRD.md for all requirements
- Follow patterns in existing code
- Optimize for ADHD users (low friction, calm UI)
- TypeScript strict mode
- React 19 functional components
- Tailwind CSS + Radix UI
- Framer Motion animations (200-300ms)

CURRENT PRIORITY:
- Complete Phase 3: Property validation
- Implement Phase 4: LLM integration
- Add testing infrastructure

NEVER:
- Change color scheme
- Add complex workflows
- Remove animations
- Use any instead of proper types
- Commit secrets
```

---

## Environment Variables Setup

Create `.env.local`:

```env
# OpenAI API (for Phase 4 LLM integration)
VITE_OPENAI_API_KEY=your-actual-openai-api-key-here

# Optional: Use Anthropic instead
# VITE_ANTHROPIC_API_KEY=your-actual-anthropic-api-key-here

# API URL (for future backend)
# VITE_API_URL=http://localhost:5000
```

Create `.env.example`:

```env
# OpenAI API (required for Phase 4+)
VITE_OPENAI_API_KEY=your-actual-openai-api-key-here

# Optional: Anthropic API (alternative to OpenAI)
# VITE_ANTHROPIC_API_KEY=your-actual-anthropic-api-key-here

# Backend API URL (for production deployment)
# VITE_API_URL=https://api.divergentflow.com
```

Add to `.gitignore`:
```
.env.local
.env
.env*.local
```

**Security Best Practices**:
- Use `.env.local` for local development (automatically ignored by Vite)
- Use `.env.development` for development-specific config (commit this with dummy values)
- Never commit files containing real API keys
- Rotate keys immediately if accidentally committed

---

## Troubleshooting

### "AI agent doesn't understand my codebase"
**Solution**: Add more files to context
- Copilot: Use `@workspace` prefix in prompts
- Cursor: Add files with `@filename` in chat
- Aider: Use `/add path/to/file.ts` command

### "AI generates code that doesn't match my style"
**Solution**: Reference existing files
```
Follow the exact pattern used in src/lib/typeInference.ts. 
Use the same naming conventions, error handling style, 
and comment structure.
```

### "AI suggests installing new packages I don't want"
**Solution**: Be explicit
```
Implement this WITHOUT adding new dependencies. 
Use only the packages already in package.json.
```

### "Changes break existing functionality"
**Solution**: Ask for tests first
```
Before implementing this feature, write tests for the 
existing functionality that might be affected. Then 
implement the feature and ensure tests still pass.
```

---

## Success Metrics

You'll know the AI agent is working well when:

1. ✅ You can implement a PRD phase in 1-2 days instead of 1-2 weeks
2. ✅ Tests are written automatically with new features
3. ✅ Code quality is consistent across all files
4. ✅ Edge cases are handled that you didn't think of
5. ✅ Documentation is updated automatically

---

## What to Expect

### Week 1 with AI Agent
- Learning curve: figuring out good prompts
- Speed: 2-3x faster than manual coding
- Quality: Good but needs review
- **Tip**: Start with small, well-defined tasks

### Week 2-4 with AI Agent
- Prompts get better with practice
- Speed: 5-10x faster on routine tasks
- Quality: Excellent with clear instructions
- **Tip**: Build a prompt library for common tasks

### Month 2+ with AI Agent
- Prompts are second nature
- Speed: 10-20x faster overall
- Quality: Matches or exceeds manual coding
- **Tip**: Focus on architecture, let AI handle implementation

---

## Prompt Templates

Copy-paste these and fill in the blanks:

### New Feature
```
Based on PRD.md [Phase X], implement [feature name].

Requirements:
- [requirement 1]
- [requirement 2]
- [requirement 3]

Follow the pattern in [existing file].
Add tests covering [scenarios].
Update [related components].
```

### Bug Fix
```
There's a bug where [describe behavior].

Expected: [what should happen]
Actual: [what happens]
Reproduction: [steps to reproduce]

The bug is likely in [file/component].
Fix it and add a test to prevent regression.
```

### Refactor
```
Refactor [file/component] to [improvement].

Keep the same API/interface.
Maintain existing behavior.
Add tests to ensure nothing breaks.
Improve [specific aspect].
```

### Test Addition
```
Write comprehensive tests for [file/component].

Cover:
- Happy path
- Edge cases: [list specific cases]
- Error conditions
- [Any specific scenarios from PRD]

Use [testing library] and follow the pattern in [existing test file].
```

---

## Ready to Start?

Pick your tool, set it up (5-10 min), then try **Task 1: Add Tests** from above.

Once tests pass, you'll know your AI agent is working correctly and you can move on to more complex tasks.

**Remember**: The AI is a tool, not magic. You still need to:
- Review all code it generates
- Understand what it's doing
- Test thoroughly
- Make architectural decisions

But it will **dramatically speed up** implementation and help you maintain consistency.

---

## Questions?

If you get stuck:
1. Check TRANSITION-GUIDE.md for detailed explanations
2. Review PRD.md for requirements
3. Look at existing code for patterns
4. Ask the AI agent: "Explain how [feature] works in this codebase"

Good luck! You're about to accelerate your development significantly. 🚀
