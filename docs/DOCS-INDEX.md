# Documentation Index

Quick reference guide to all Divergent Flow documentation.

## 📖 Start Here

### New to the Project?
👉 **[README.md](../README.md)** - Project overview, quick start, and development setup

### Want to Understand the Architecture?
👉 **[ARCHITECTURE.md](./ARCHITECTURE.md)** - Current architecture, migration roadmap, and technical decisions

### Interested in the Long-Term Vision?
👉 **[unified.md](./unified.md)** - Complete vision for unified entity system SaaS platform

## 🏗️ Architecture & Design

| Document | Purpose | Audience |
|----------|---------|----------|
| [ARCHITECTURE.md](./ARCHITECTURE.md) | Current architecture, migration roadmap, ADRs | Developers, architects |
| [unified.md](./unified.md) | Target unified entity system vision | Architects, investors, contributors |
| [backend/README.md](../backend/README.md) | Backend API documentation | Backend developers |

## 🚀 Setup & Operations

| Document | Purpose | Audience |
|----------|---------|----------|
| [README.md](../README.md) | Quick start guide | All developers |
| [RUNNING-FULL-STACK.md](./RUNNING-FULL-STACK.md) | Full stack setup (frontend + backend) | Full-stack developers |
| [AUTH0-SETUP.md](./AUTH0-SETUP.md) | Authentication configuration | DevOps, developers |
| [backend/MONGODB-SETUP.md](../backend/MONGODB-SETUP.md) | MongoDB setup and configuration | Backend developers, DevOps |
| [.github/DEPLOYMENT-SETUP.md](../.github/DEPLOYMENT-SETUP.md) | CI/CD and deployment configuration | DevOps |

## 🧪 Testing & Quality

| Document | Purpose | Audience |
|----------|---------|----------|
| [TESTING.md](./TESTING.md) | Testing guidelines and practices | Developers |
| [SECURITY.md](../SECURITY.md) | Security policy and reporting | Security researchers |

## 🤝 Contributing

| Document | Purpose | Audience |
|----------|---------|----------|
| [CONTRIBUTING.md](../CONTRIBUTING.md) | Contribution guidelines | Contributors |
| [COMMIT-GUIDELINES.md](../COMMIT-GUIDELINES.md) | Conventional commits reference | All contributors |
| [.github/workflows/README.md](../.github/workflows/README.md) | CI/CD workflows documentation | Contributors, DevOps |

## 🗺️ Navigation Guide

### "I want to..."

**...understand what this project does**
→ Start with [README.md](../README.md)

**...understand the current technical architecture**
→ Read [ARCHITECTURE.md](./ARCHITECTURE.md) - Current Architecture section

**...understand where the project is going**
→ Read [unified.md](./unified.md) and [ARCHITECTURE.md](./ARCHITECTURE.md) - Migration Path section

**...set up my development environment**
→ Follow [README.md](../README.md) Development section

**...run the full stack locally**
→ Follow [RUNNING-FULL-STACK.md](./RUNNING-FULL-STACK.md)

**...set up authentication**
→ Follow [AUTH0-SETUP.md](./AUTH0-SETUP.md)

**...deploy to production**
→ Follow [.github/DEPLOYMENT-SETUP.md](../.github/DEPLOYMENT-SETUP.md)

**...contribute code**
→ Read [CONTRIBUTING.md](../CONTRIBUTING.md) and [COMMIT-GUIDELINES.md](../COMMIT-GUIDELINES.md)

**...write tests**
→ Read [TESTING.md](./TESTING.md)

**...understand architectural decisions**
→ Read [ARCHITECTURE.md](./ARCHITECTURE.md) - Architecture Decision Records section

**...understand the API**
→ Read [backend/README.md](../backend/README.md)

**...report a security issue**
→ Follow [SECURITY.md](../SECURITY.md)

## 📊 Document Relationship Map

```
README.md (Entry Point)
  ├── ARCHITECTURE.md (Current + Roadmap)
  │   └── unified.md (Vision)
  │
  ├── backend/README.md (API Docs)
  │   └── backend/MONGODB-SETUP.md
  │
  ├── CONTRIBUTING.md (Process)
  │   └── COMMIT-GUIDELINES.md
  │
  └── Setup Guides
      ├── RUNNING-FULL-STACK.md
      ├── AUTH0-SETUP.md
      ├── TESTING.md
      └── .github/DEPLOYMENT-SETUP.md
```

## 🔄 How Documents Relate to Each Other

### Core Triad

1. **README.md** = "What" (What is this project?)
2. **ARCHITECTURE.md** = "How" (How is it built? How will it evolve?)
3. **unified.md** = "Why" (Why this architecture? Where are we going?)

### Supporting Documentation

- **Setup guides** support getting started from README.md
- **CONTRIBUTING.md** + **COMMIT-GUIDELINES.md** support the development workflow
- **Backend/API docs** provide implementation details referenced by ARCHITECTURE.md
- **TESTING.md** supports quality practices mentioned in CONTRIBUTING.md

## 📅 Documentation Maintenance

**Review Frequency**:
- **ARCHITECTURE.md**: Before each migration phase kickoff
- **unified.md**: Quarterly or when major architectural decisions change
- **README.md**: With each release
- **Setup guides**: When tools/dependencies change
- **API docs**: With each API change

**Last Updated**: 2026-01-15
