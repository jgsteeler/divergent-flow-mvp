# Divergent Flow 🧠

An ADHD-friendly brain management tool that helps you capture thoughts instantly and process them intelligently.

## Current Status

- ✅ **Phase 1 Complete**: Quick capture with persistent storage
- ✅ **Phase 2 Complete**: Type inference engine with learning
- 🚧 **Phase 3 In Progress**: Review queue with priority system
- ⏭️ **Phase 4 Next**: LLM-powered intelligent processing

## 🎯 Ready to Accelerate Development?

**If you're looking for guidance on transitioning from Spark to AI coding agents**, start here:

### 🎯 Quick Reference (2 minutes)
→ **[QUICK-REFERENCE.md](./QUICK-REFERENCE.md)** - Cheat sheet with timelines, prompts, and key info

### 📖 Quick Start (5 minutes)
→ **[SPARK-TRANSITION-SUMMARY.md](./SPARK-TRANSITION-SUMMARY.md)** - Executive summary answering your key questions

### 🚀 Setup AI Agent (10 minutes)
→ **[AI-AGENT-QUICK-START.md](./AI-AGENT-QUICK-START.md)** - Step-by-step setup for GitHub Copilot, Cursor, or Aider

### 📚 Complete Guide (30 minutes)
→ **[TRANSITION-GUIDE.md](./TRANSITION-GUIDE.md)** - Full roadmap, architecture, and scaling plan

### 📋 Project Requirements
→ **[PRD.md](./PRD.md)** - Product requirements document with all phases and features

## Development

### Prerequisites
```bash
npm install
```

### Run Locally
```bash
npm run dev
```

### Build
```bash
npm run build
```

## Tech Stack

- **Frontend**: React 19, TypeScript, Vite
- **UI**: Radix UI, Tailwind CSS, Framer Motion
- **State**: React hooks, Spark KV storage
- **Icons**: Phosphor Icons

## Project Structure

```
src/
├── components/          # React components
│   ├── ui/             # Reusable UI components
│   ├── CaptureInput.tsx
│   ├── ReviewQueue.tsx
│   └── TypeConfirmation.tsx
├── lib/                # Business logic
│   ├── types.ts
│   ├── typeInference.ts
│   └── reviewPriority.ts
└── App.tsx             # Main application
```

## Next Steps

1. Read [SPARK-TRANSITION-SUMMARY.md](./SPARK-TRANSITION-SUMMARY.md) to understand when/why to transition
2. Follow [AI-AGENT-QUICK-START.md](./AI-AGENT-QUICK-START.md) to set up AI coding assistant
3. Use AI agent to complete Phase 3-6 per [PRD.md](./PRD.md)
4. Scale to backend when ready using [TRANSITION-GUIDE.md](./TRANSITION-GUIDE.md)

## License

MIT License - See LICENSE file for details

---

## Original Spark Template Info

This project started with the GitHub Spark Template. The Spark Template files and resources from GitHub are licensed under the terms of the MIT license, Copyright GitHub, Inc.
