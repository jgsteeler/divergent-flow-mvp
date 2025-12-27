# Divergent Flow 🧠

An ADHD-friendly brain management tool that helps you capture thoughts instantly and process them intelligently.

> 📚 **New to the project?** See [local.docs/DOCS-INDEX.md](./local.docs/DOCS-INDEX.md) for a complete guide to all documentation.
>
> ⚠️ **Contributing?** All commits must follow [Conventional Commits](https://www.conventionalcommits.org/) format. See [COMMIT-GUIDELINES.md](./COMMIT-GUIDELINES.md).

## Current Status

- ✅ **Phase 1 Complete**: Quick capture with persistent storage
- ✅ **Phase 2 Complete**: Type inference engine with learning
- 🚧 **Phase 3 In Progress**: Review queue with priority system
- ⏭️ **Phase 4 Next**: LLM-powered intelligent processing

## 🎯 Ready to Accelerate Development?

**If you're looking for guidance on transitioning from Spark to AI coding agents**, start here:

### 🎯 Quick Reference (2 minutes)

→ **[local.docs/QUICK-REFERENCE.md](./local.docs/QUICK-REFERENCE.md)** - Cheat sheet with timelines, prompts, and key info

### 📖 Quick Start (5 minutes)

→ **[local.docs/SPARK-TRANSITION-SUMMARY.md](./local.docs/SPARK-TRANSITION-SUMMARY.md)** - Executive summary answering your key questions

### 🚀 Setup AI Agent (10 minutes)

→ **[local.docs/AI-AGENT-QUICK-START.md](./local.docs/AI-AGENT-QUICK-START.md)** - Step-by-step setup for GitHub Copilot, Cursor, or Aider

### 📚 Complete Guide (30 minutes)

→ **[local.docs/TRANSITION-GUIDE.md](./local.docs/TRANSITION-GUIDE.md)** - Full roadmap, architecture, and scaling plan

### 📋 Project Requirements

→ **[local.docs/PRD.md](./local.docs/PRD.md)** - Product requirements document with all phases and features

## Development

### Prerequisites

```bash
cd frontend
npm install

# Optional: Configure git for conventional commits
./scripts/setup-dev.sh
```

### Commit Message Format ⚠️

**All commits must follow [Conventional Commits](https://www.conventionalcommits.org/) format:**
```text
<type>(<scope>): <description>
```

Example: `feat(capture): add keyboard shortcut`

See [COMMIT-GUIDELINES.md](./COMMIT-GUIDELINES.md) for quick reference.

### Run Locally

```bash
cd frontend
npm run dev
```

### Build

```bash
cd frontend
npm run build
```

## Tech Stack

- **Frontend**: React 19, TypeScript, Vite
- **UI**: Radix UI, Tailwind CSS, Framer Motion
- **State**: React hooks, Spark KV storage
- **Icons**: Phosphor Icons

## Project Structure

```text
frontend/                # React + Vite app
└── src/
    ├── components/      # React components
    ├── hooks/           # Custom hooks
    └── lib/             # Business logic

backend/                 # .NET API
└── DivergentFlow.Api/
```

## Next Steps

1. Read [local.docs/SPARK-TRANSITION-SUMMARY.md](./local.docs/SPARK-TRANSITION-SUMMARY.md) to understand when/why to transition
2. Follow [local.docs/AI-AGENT-QUICK-START.md](./local.docs/AI-AGENT-QUICK-START.md) to set up AI coding assistant
3. Use AI agent to complete Phase 3-6 per [local.docs/PRD.md](./local.docs/PRD.md)
4. Scale the backend when ready using [local.docs/TRANSITION-GUIDE.md](./local.docs/TRANSITION-GUIDE.md)

## Contributing

We welcome contributions! Please read [CONTRIBUTING.md](./CONTRIBUTING.md) for guidelines.

**Important**: All commits and PR titles must follow [Conventional Commits](https://www.conventionalcommits.org/) format.

## License

MIT License - See LICENSE file for details

---

## Original Spark Template Info

This project started with the GitHub Spark Template. The Spark Template files and resources from GitHub are licensed under the terms of the MIT license, Copyright GitHub, Inc.
