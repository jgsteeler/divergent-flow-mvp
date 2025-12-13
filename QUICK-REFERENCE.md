# 🚀 Quick Reference Card

## When to Stop Using Spark?
**NOW** - You're at the perfect inflection point.

## Which AI Agent Should I Use?

| Tool | Best For | Cost | Setup Time |
|------|----------|------|------------|
| **GitHub Copilot** | Already using VS Code | $10/mo | 5 min |
| **Cursor** | Most powerful features | Free trial | 10 min |
| **Aider** | Terminal lovers | API costs | 5 min |

## Your Next 3 Tasks

### Task 1: Add Tests (30 min)
```
Set up Vitest and write unit tests for src/lib/typeInference.ts
covering all edge cases from PRD.md
```

### Task 2: Complete Phase 3 (2 hours)
```
Implement property validation per PRD.md Phase 3.
Check actions for priority, reminders for due date.
Update ReviewQueue to show missing properties.
```

### Task 3: Start Phase 4 (4 hours)
```
Implement OpenAI integration for LLM-powered inference.
Create src/lib/llmService.ts and src/lib/llmInference.ts.
Add collection, priority, and context extraction.
```

## Essential Prompt Patterns

### Feature Implementation
```
Based on PRD.md [Phase X], implement [feature].
Follow the pattern in [existing file].
Add tests and update [related components].
```

### Bug Fix
```
Fix bug where [describe issue].
Expected: [what should happen]
Actual: [what happens now]
Add test to prevent regression.
```

### Refactoring
```
Refactor [file] to [improvement].
Keep same API. Add tests first.
```

## MVP Timeline with AI Agent

| Phase | Time | Key Features |
|-------|------|--------------|
| Phase 3 | 1-2 weeks | Property validation |
| Phase 4 | 2-3 weeks | LLM integration, date parsing |
| Phase 5 | 1-2 weeks | Dashboard, Next Action, Quick Wins |
| Phase 6 | 1 week | Completion tracking |
| Testing | 1-2 weeks | E2E tests, polish |
| **Total** | **6-10 weeks** | Complete MVP |

## Backend Migration Timeline

| Step | Time | What You'll Build |
|------|------|-------------------|
| .NET Setup | 1-2 weeks | ASP.NET Core API, Entity Framework |
| API Design | 2-3 weeks | REST endpoints, authentication |
| Frontend Migration | 4-5 weeks | React Query, offline sync |
| Deployment | 5-6 weeks | Azure + Vercel, CI/CD |
| **Total** | **3-6 months** | Production-ready |

## Critical AI Agent Rules

### ✅ Always Do
- Review all generated code
- Check PRD.md for requirements
- Run tests after changes
- Keep code quality high
- Understand architectural decisions

### ❌ Never Do
- Blindly accept without review
- Skip testing
- Ignore edge cases
- Let AI make architecture decisions alone
- Commit without understanding

## Project Files Quick Reference

```
📁 Documentation
├── SPARK-TRANSITION-SUMMARY.md    # Start here
├── AI-AGENT-QUICK-START.md        # Setup guide
├── TRANSITION-GUIDE.md            # Complete roadmap
└── PRD.md                         # Requirements

📁 AI Agent Config
├── .github/copilot-instructions.md  # GitHub Copilot
├── .cursorrules                     # Cursor IDE
└── .env.example                     # Environment vars

📁 Source Code
├── src/
│   ├── components/              # UI components
│   ├── lib/                     # Business logic
│   └── App.tsx                  # Main app

📁 Key Files
├── src/lib/types.ts             # TypeScript types
├── src/lib/typeInference.ts     # Current inference
├── src/lib/reviewPriority.ts    # Queue algorithm
└── src/App.tsx                  # App structure
```

## Common AI Agent Prompts

### Start a Work Session
```
@workspace I'm working on Phase 4 of PRD.md.
Remind me of the requirements and current state.
```

### Review Current State
```
@workspace Analyze the current codebase.
What's complete? What's missing from PRD.md Phase 3?
```

### Get Architecture Advice
```
I need to implement [feature]. What's the best approach
given the current architecture in src/?
```

### Debug an Issue
```
This code in [file] isn't working as expected.
[Describe issue]. Help me debug it.
```

### Add Tests
```
Write comprehensive tests for [file].
Follow the testing patterns if any exist.
Cover happy path and edge cases.
```

## Speed vs. Quality Balance

| Speed Level | When to Use | Risk |
|-------------|-------------|------|
| **Fast** | Prototyping, exploring | High - Review carefully |
| **Balanced** | Feature development | Medium - Standard review |
| **Careful** | Core logic, security | Low - Extra validation |

**Recommendation**: Start with "Careful" mode until you trust your AI agent, then move to "Balanced".

## Troubleshooting Quick Fixes

### AI doesn't understand codebase
→ Add files to context: `@workspace` or `/add file.ts`

### AI suggests wrong patterns
→ Reference existing code: `Follow the pattern in [file]`

### AI adds unwanted dependencies
→ Be explicit: `Use only existing packages in package.json`

### Changes break tests
→ Test first: `Write tests before implementing this feature`

## Cost Estimates

### Development Phase
- AI Agent: $10-20/mo
- LLM API: $5-20/mo
- **Total**: ~$15-40/mo

### Production Phase
- Hosting: $100-200/mo
- LLM API: $50-500/mo (usage-based)
- Monitoring: $10-30/mo
- **Total**: ~$200-800/mo

## Emergency Contacts

### Getting Stuck?
1. Check TRANSITION-GUIDE.md troubleshooting
2. Read PRD.md for requirements
3. Review existing code patterns
4. Ask AI to explain: `@workspace explain how [feature] works`

### Need Human Help?
- GitHub Issues: Open an issue in your repo
- Community: ADHD tech communities, indie hacker forums
- Technical: Stack Overflow with specific questions

## Success Metrics

You're on the right track when:
- ✅ Implementing features 5-10x faster
- ✅ AI catches edge cases you missed
- ✅ Tests are written automatically
- ✅ Code quality stays consistent
- ✅ You focus on architecture, not syntax

## Next Action

**Right now**: Read [AI-AGENT-QUICK-START.md](./AI-AGENT-QUICK-START.md) and set up your AI agent.

**Today**: Complete Task 1 (add tests) to validate your setup.

**This week**: Complete Phase 3 property validation.

**This month**: Implement Phase 4 LLM integration.

---

## Remember

You're not replacing yourself with AI. You're **amplifying your capabilities** to build something meaningful for the ADHD community faster and better.

The AI agent is a **powerful tool in your hands**. Use it wisely! 🚀
