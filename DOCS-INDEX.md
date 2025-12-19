# Documentation Index

## 🚨 Essential Reading for Contributors

### Commit Message Format (REQUIRED)
All commits and PR titles MUST follow Conventional Commits format. Non-compliance will cause CI to fail.

- 📖 **[COMMIT-GUIDELINES.md](./COMMIT-GUIDELINES.md)** - Quick reference for commit format
- 📋 **[CONTRIBUTING.md](./CONTRIBUTING.md)** - Complete contribution guidelines
- 🛠️ **[Setup Script](./scripts/setup-dev.sh)** - Configure local environment

---

## Project Documentation

### Getting Started
- 📘 **[README.md](./README.md)** - Project overview and quick start
- 🚀 **[AI-AGENT-QUICK-START.md](./AI-AGENT-QUICK-START.md)** - Set up AI coding assistants
- 📋 **[PRD.md](./PRD.md)** - Product requirements document

### For AI Agents & Developers
- 🤖 **[.github/copilot-instructions.md](./.github/copilot-instructions.md)** - Complete guide for AI agents
- 🎯 **[.cursorrules](./.cursorrules)** - Cursor IDE specific rules
- 💡 **[QUICK-REFERENCE.md](./QUICK-REFERENCE.md)** - Cheat sheet for development

### Architecture & Planning
- 🏗️ **[TRANSITION-GUIDE.md](./TRANSITION-GUIDE.md)** - Full roadmap and architecture
- 📊 **[SPARK-TRANSITION-SUMMARY.md](./SPARK-TRANSITION-SUMMARY.md)** - Executive summary
- ✅ **[IMPLEMENTATION-COMPLETE.md](./IMPLEMENTATION-COMPLETE.md)** - Completed features

### Testing & Quality
- 🧪 **[TESTING.md](./TESTING.md)** - Testing guidelines and practices
- 🔒 **[SECURITY.md](./SECURITY.md)** - Security policies

---

## GitHub Configuration

### Workflows
- 📁 **[.github/workflows/README.md](./.github/workflows/README.md)** - Workflow documentation
- ✅ **[.github/workflows/conventional-commits.yml](./.github/workflows/conventional-commits.yml)** - Commit validation
- 🚀 **[.github/workflows/release-please.yml](./.github/workflows/release-please.yml)** - Automated releases
- 🧪 **[.github/workflows/test.yml](./.github/workflows/test.yml)** - Test suite

### Templates & Configuration
- 📝 **[.github/pull_request_template.md](./.github/pull_request_template.md)** - PR template
- 💬 **[.github/commit_template.txt](./.github/commit_template.txt)** - Commit message template
- ⚙️ **[commitlint.config.cjs](./commitlint.config.cjs)** - Commitlint configuration
- 🔄 **[.github/release-please-config.json](./.github/release-please-config.json)** - Release config

---

## Quick Access by Topic

### 🎯 I want to...

#### Contribute Code
1. Read [CONTRIBUTING.md](./CONTRIBUTING.md)
2. Review [COMMIT-GUIDELINES.md](./COMMIT-GUIDELINES.md)
3. Run `./scripts/setup-dev.sh`
4. Check [.github/copilot-instructions.md](./.github/copilot-instructions.md)

#### Set Up AI Agent
1. Start with [AI-AGENT-QUICK-START.md](./AI-AGENT-QUICK-START.md)
2. Review [.github/copilot-instructions.md](./.github/copilot-instructions.md)
3. Check [PRD.md](./PRD.md) for requirements

#### Understand Architecture
1. Read [TRANSITION-GUIDE.md](./TRANSITION-GUIDE.md)
2. Review [PRD.md](./PRD.md)
3. Check [IMPLEMENTATION-COMPLETE.md](./IMPLEMENTATION-COMPLETE.md)

#### Fix CI/CD Issues
1. Check [.github/workflows/README.md](./.github/workflows/README.md)
2. Review specific workflow in [.github/workflows/](./.github/workflows/)
3. Validate commits with [COMMIT-GUIDELINES.md](./COMMIT-GUIDELINES.md)

#### Write Tests
1. Read [TESTING.md](./TESTING.md)
2. Check [.github/copilot-instructions.md](./.github/copilot-instructions.md) testing section
3. Review existing tests in `src/`

---

## File Purpose Summary

| File | Purpose |
|------|---------|
| `README.md` | Project overview and quick start |
| `CONTRIBUTING.md` | Complete contribution guidelines |
| `COMMIT-GUIDELINES.md` | Quick reference for commit format |
| `PRD.md` | Product requirements document |
| `AI-AGENT-QUICK-START.md` | Set up AI coding assistants |
| `TRANSITION-GUIDE.md` | Architecture and scaling roadmap |
| `TESTING.md` | Testing guidelines |
| `SECURITY.md` | Security policies |
| `.github/copilot-instructions.md` | AI agent instructions |
| `.cursorrules` | Cursor IDE rules |
| `commitlint.config.cjs` | Commit validation config |
| `.github/commit_template.txt` | Commit message template |
| `.github/pull_request_template.md` | PR template |
| `scripts/setup-dev.sh` | Local setup script |

---

## External Resources

- [Conventional Commits Specification](https://www.conventionalcommits.org/)
- [Release Please Documentation](https://github.com/googleapis/release-please)
- [Commitlint Documentation](https://commitlint.js.org/)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)

---

**Note**: All documentation is living and should be updated as the project evolves. If you find outdated information, please open an issue or PR.
