# Divergent Flow Architecture

**Status**: MVP → Unified Entity System Migration

**Priority**: Get basic task-oriented workflow (Capture → Review → Dashboard) working ASAP for open source community launch

This document describes the current architecture and the roadmap for evolving to the **Unified Entity System** outlined in [unified.md](./unified.md).

## Table of Contents

- [Quick Win: Marketing-Ready MVP](#quick-win-marketing-ready-mvp) ⭐ **NEW**
- [Current Architecture (MVP)](#current-architecture-mvp)
- [Target Architecture (Unified Entity System)](#target-architecture-unified-entity-system)
- [Migration Path](#migration-path)
- [Open Source Strategy](#open-source-strategy) ⭐ **NEW**
- [Architecture Decision Records](#architecture-decision-records)

---

## Quick Win: Marketing-Ready MVP

**Goal**: Launch to open source community with complete basic workflow in 2-4 weeks

### What We Have ✅
- **Capture**: Working with persistent storage (Phase 1 complete)
- **Type Inference**: Pattern-based with confidence scoring (Phase 2 complete)
- **Backend API**: Clean architecture with .NET + MongoDB

### What We Need 🎯
- **Review Queue**: Complete basic implementation (3-5 days)
  - Simple priority sorting
  - 3 items per session
  - Confirm/defer actions
  - Mark items as reviewed
- **Dashboard**: Build task-oriented view (5-7 days)
  - Today's tasks (action items)
  - Overdue items
  - Quick stats
  - Simple navigation

### Timeline to Launch
```
Week 1:  Complete Review Queue + Start Dashboard
Week 2:  Complete Dashboard + Integration testing
Week 3:  Documentation + Screenshots + Demo
Week 4:  Open source launch + Marketing push
```

### Open Source Positioning
- **Base System**: Capture, Review, Dashboard (open source)
- **Plugin Architecture**: Foundation for proprietary/community extensions
- **Value Proposition**: ADHD-friendly task management that learns
- **Community Opportunity**: Build plugins, extend inference, contribute features

---

## Current Architecture (MVP)

### System Overview

**Current Stack**:
- **Frontend**: React 19 + TypeScript + Vite
- **Backend**: .NET 10 Web API
- **Database**: MongoDB (system of record)
- **Cache**: Redis (optional projection cache via Upstash)
- **State**: React hooks + local browser storage (Spark KV)

**Architecture Pattern**: Layered/Clean Architecture with CQRS-inspired patterns

```
┌─────────────────────────────────────────────────┐
│           React Frontend (SPA)                  │
│  - Components (UI)                              │
│  - Hooks (State)                                │
│  - Services (API clients)                       │
└───────────────────┬─────────────────────────────┘
                    │ HTTP/REST
                    ▼
┌─────────────────────────────────────────────────┐
│         .NET Web API (Backend)                  │
│  ┌─────────────────────────────────────────┐   │
│  │  Controllers (HTTP endpoints)           │   │
│  └──────────────┬──────────────────────────┘   │
│                 │ MediatR                       │
│  ┌──────────────▼──────────────────────────┐   │
│  │  Application Layer (Handlers)           │   │
│  │  - Use cases                             │   │
│  │  - Background services                   │   │
│  │  - Validation                            │   │
│  └──────────────┬──────────────────────────┘   │
│                 │                               │
│  ┌──────────────▼──────────────────────────┐   │
│  │  Domain Layer (Entities)                │   │
│  │  - Item                                  │   │
│  │  - Collection (partial)                  │   │
│  │  - Capture (legacy compatibility)       │   │
│  └──────────────┬──────────────────────────┘   │
│                 │                               │
│  ┌──────────────▼──────────────────────────┐   │
│  │  Infrastructure Layer                   │   │
│  │  - MongoDB repositories                  │   │
│  │  - Redis cache (optional)                │   │
│  │  - Type inference engine                 │   │
│  └──────────────────────────────────────────┘   │
└───────────────────┬─────────────────────────────┘
                    │
    ┌───────────────┴────────────────┐
    ▼                                ▼
┌─────────┐                    ┌─────────┐
│ MongoDB │                    │  Redis  │
│ (System │                    │ (Cache) │
│  of     │                    │(Optional)│
│ Record) │                    └─────────┘
└─────────┘
```

### Data Model (Current)

**Core Entities**:

1. **Item** (foundational entity)
   ```typescript
   {
     id: string           // UUID
     type: string         // "capture" (default)
     text: string         // Content
     createdAt: number    // Unix timestamp (ms)
     inferredType?: string    // e.g., "note", "action"
     typeConfidence?: number  // 0-100
     lastReviewedAt?: number  // Unix timestamp (ms)
     collectionId?: string    // Parent collection
     userId?: string      // Owner (WIP)
   }
   ```

2. **Collection** (partially implemented)
   ```typescript
   {
     id: string
     name: string
     // Additional fields TBD
   }
   ```

3. **Capture** (legacy, maps to Item)
   ```typescript
   {
     id: string
     text: string
     createdAt: number
   }
   ```

### Current Features (Phases 1-3)

#### ✅ Phase 1: Capture + Storage
- Quick text capture via web UI
- MongoDB persistence
- REST API endpoints (`/api/items`, `/api/captures`)
- Background sync with optional Redis cache

#### ✅ Phase 2: Type Inference
- Pattern-based type inference (rule-based MVP)
- Background processing queue
- Confidence scoring
- Learning from corrections (in progress)

#### 🚧 Phase 3: Review Queue (In Progress)
- Priority-based review queue
- 3 items per session
- Property validation (pending)

#### ⏭️ Phase 4-6: Planned
- LLM-powered inference
- Dashboard system
- Reflection/analytics

### Data Flow (Current)

**Write Path**:
```
User → React UI → POST /api/items
           ↓
    Controller → MediatR Handler
           ↓
    MongoDB (persist) → Enqueue for inference
           ↓
    Background worker → Type inference
           ↓
    Update MongoDB → (Optional) Sync to Redis
```

**Read Path**:
```
React UI → GET /api/items
           ↓
    Controller → MediatR Handler
           ↓
    MongoDB (direct read)
           ↓
    Return to UI
```

### Current Limitations

1. **Single Entity Type**: Only `Item` with hardcoded schema
2. **No Multi-Tenancy**: Single-user focused (userId field added but not enforced)
3. **No Plugin System**: Hardcoded inference rules
4. **Limited Relationships**: Basic parent-child (collectionId)
5. **No Versioning**: No audit trail or version history
6. **No Permissions**: No access control or sharing
7. **Basic Inference**: Rule-based, no ML training yet

---

## Target Architecture (Unified Entity System)

See [unified.md](./unified.md) for the complete vision. Key differences from current MVP:

### Core Concept: Entity-Centric Everything

**Target Entity Model**:
```typescript
Entity = {
  id: "entity:{type}:{uniqueId}",
  entityTypeId: "entityType:{typename}",
  tenantId: "tenant:{orgId}",        // Multi-tenant
  ownerId: "user:{userId}",          // Ownership
  attributes: {...},                 // Dynamic schema
  metadata: {
    createdDate: timestamp,
    updatedDate: timestamp,
    version: integer,                // Versioning
    visibility: "private|team|org"   // Permissions
  },
  relationships: {
    parentIds: [...],
    referencedByIds: [...],
    linkedEntities: {...}
  }
}
```

### Key Architectural Differences

| Aspect | Current MVP | Target (Unified) |
|--------|-------------|------------------|
| **Entity Types** | Hardcoded (Item, Collection) | Dynamic, extensible via EntityType definitions |
| **Schema** | Fixed C# classes | Dynamic attributes defined in EntityType |
| **Tenancy** | Single-user (userId field exists) | Multi-tenant by design (tenantId everywhere) |
| **Relationships** | Simple parent-child | Rich graph (parent, referenced, linked) |
| **Versioning** | None | Built-in version history |
| **Permissions** | None | Visibility + ownership model |
| **Inference** | Hardcoded rules | Plugin-based inference engines |
| **Extensibility** | Code changes required | Plugin marketplace |

### The Seven Pillars (Extended)

The unified system builds on the MVP's foundation and extends all 7 pillars:

1. **CAPTURE**: Multiple input channels (web, mobile, email, Slack, API)
2. **ORGANIZE**: Shared collections, workspaces, templates
3. **INFER**: Hosted ML models, custom model training
4. **REVIEW**: Collaborative review, approval workflows
5. **MACHINE LEARNING**: Model marketplace, federated learning
6. **ACTION/DASHBOARD**: Real-time dashboards, team visibility
7. **REFLECT**: Team insights, goal tracking

---

## Migration Path

### Guiding Principles

1. **Quick Wins First**: Get basic workflow working for open source launch
2. **Incremental Migration**: No "big bang" rewrite
3. **Backward Compatibility**: Existing data continues to work
4. **ADHD-First UX**: Never compromise user experience for architecture
5. **Community-Driven**: Enable plugin development early

### Phase-by-Phase Migration (UPDATED PRIORITIES)

#### Phase 3: Complete Basic Workflow (2-4 weeks) ⚡ **CURRENT PRIORITY**

**Goal**: Launch-ready MVP with Capture → Review → Dashboard workflow

**Tasks**:
- [x] Capture with persistent storage (DONE - Phase 1)
- [x] Type inference engine (DONE - Phase 2)
- [ ] **Review Queue** (3-5 days)
  - [ ] Simple priority calculation (unreviewed first, then by confidence)
  - [ ] 3 items per session UI
  - [ ] Confirm/defer/reject actions
  - [ ] Update `lastReviewedAt` field
  - [ ] Backend API endpoint: `GET /api/items/review-queue`
- [ ] **Dashboard** (5-7 days)
  - [ ] Task-oriented view (action items focus)
  - [ ] Today's tasks section
  - [ ] Overdue tasks section  
  - [ ] Quick stats (total, pending, completed)
  - [ ] Navigation between capture/review/dashboard
  - [ ] Backend API endpoint: `GET /api/items/dashboard`
- [ ] Integration testing
- [ ] Documentation + screenshots
- [ ] Demo video

**Deliverables**:
- Working Capture → Review → Dashboard workflow
- User can manage action items end-to-end
- Ready for open source community launch
- Contributor onboarding docs

**Success Criteria**:
- User can capture → review → see tasks in dashboard
- Mobile responsive
- ADHD-friendly UX maintained
- Clear path for contributors to extend

#### Phase 3.5: Plugin Foundation (4-6 weeks) 🔌 **BROUGHT FORWARD**

**Goal**: Enable plugin development for open source community

**Why Accelerated**: Support open source base + proprietary plugin strategy

**Tasks**:
- [ ] Design simple plugin interface (`IPlugin`)
- [ ] Plugin registry (in-memory, no persistence yet)
- [ ] Example: Convert existing inference rules to "built-in plugin"
- [ ] Plugin SDK documentation
- [ ] Example plugin for community developers
- [ ] Plugin loading mechanism (local file-based for MVP)

**Deliverables**:
- Basic plugin system (no marketplace yet)
- 2-3 example plugins (inference rules, dashboard widgets)
- Developer guide for creating plugins
- Clear separation: base system (open source) vs plugins (can be proprietary)

**Success Criteria**:
- At least 2 working example plugins
- Community developers can create plugins following guide
- Plugin execution is safe (basic validation)
- Clear license boundaries (MIT base, plugin authors choose license)

#### Phase 4: Entity Abstraction (6-8 weeks)

**Goal**: Introduce entity abstraction layer while maintaining current API

**Tasks**:
- [ ] Create `EntityType` model in Domain layer
- [ ] Add `EntityTypeRepository` with MongoDB backing
- [ ] Introduce `Entity` base class (extends current `Item`)
- [ ] Map current `Item` to `entityType:item:capture`
- [ ] Add `tenantId` support (single tenant initially)
- [ ] Implement version tracking
- [ ] Add relationship graph support

**Deliverables**:
- Dual data model (existing Item + new Entity coexist)
- Migration script: Item → Entity
- API remains unchanged (facade pattern)

**Success Criteria**:
- All existing tests pass
- New entities can be created alongside old items
- Performance does not degrade

#### Phase 5: Enhanced Plugin System (8-10 weeks)

**Goal**: Marketplace-ready plugin system with sandboxing

**Tasks**:
- [ ] Sandbox execution environment (WebWorker frontend, Process isolation backend)
- [ ] Plugin permissions model
- [ ] Plugin marketplace UI (basic)
- [ ] Plugin versioning
- [ ] Plugin discovery and installation

**Deliverables**:
- Plugin marketplace (basic)
- Safe plugin execution
- Plugin installation UI
- 10+ community plugins

#### Phase 6: Multi-Tenancy (8-12 weeks)

**Goal**: Support multiple organizations/workspaces

**Tasks**:
- [ ] Tenant model + management
- [ ] Update all queries to filter by `tenantId`
- [ ] Tenant isolation testing
- [ ] Tenant-specific configuration
- [ ] Billing/subscription model
- [ ] Tenant onboarding flow

**Deliverables**:
- Tenant registration/management
- Data isolation guarantees
- Subscription tiers (Free/Pro/Enterprise)

**Success Criteria**:
- No cross-tenant data leakage
- Query performance maintained with tenantId indexes
- Users can create/manage multiple workspaces

#### Phase 7: Collaboration Features (12-16 weeks)

**Goal**: Team collaboration, sharing, permissions

**Tasks**:
- [ ] Permission system (visibility, ownership)
- [ ] Team/workspace management
- [ ] Shared collections
- [ ] Real-time updates (WebSockets)
- [ ] Activity feeds
- [ ] @mentions and notifications

**Deliverables**:
- Team management UI
- Sharing workflows
- Real-time collaboration
- Notification system

**Success Criteria**:
- Users can invite team members
- Shared entities are visible to authorized users
- Real-time updates work reliably

#### Phase 7: Advanced Intelligence (16-20 weeks)

**Goal**: ML model training, model marketplace

**Tasks**:
- [ ] Model training infrastructure (GPU)
- [ ] Model versioning + A/B testing
- [ ] Model marketplace
- [ ] Transfer learning
- [ ] Explainability features
- [ ] Custom model training UI

**Deliverables**:
- Model training pipeline
- Model marketplace with community models
- Custom model training for Enterprise tier
- Explainability dashboard

**Success Criteria**:
- Users can train custom models
- Models can be shared/sold in marketplace
- Inference accuracy improves over time

#### Phase 8: Full SaaS Platform (Ongoing)

**Goal**: Complete unified entity system with all pillars

**Tasks**:
- [ ] File/media storage (S3, Azure Blob)
- [ ] Advanced dashboards
- [ ] Reflection analytics
- [ ] Compliance features (HIPAA, SOC2)
- [ ] API rate limiting
- [ ] Advanced monitoring

**Deliverables**:
- Production-ready SaaS platform
- Enterprise features
- Comprehensive documentation
- Developer ecosystem

---

## Open Source Strategy

### Vision: Community-Powered, Sustainably Monetized

**Core Principle**: Open source base system + plugin marketplace enables both community growth and business sustainability.

### What's Open Source (MIT License)

**Base System** (this repository):
- ✅ Capture, Review, Dashboard workflow
- ✅ Type inference engine (rule-based)
- ✅ Clean architecture foundation
- ✅ API endpoints and data models
- ✅ Basic plugin system
- ✅ UI components and patterns
- ✅ ADHD-optimized UX principles

**Why Open Source**:
- Attracts contributors and community
- Demonstrates commitment to transparency
- Allows self-hosting for privacy-conscious users
- Creates network effects through plugins
- Builds trust with ADHD community

### What Can Be Proprietary

**Plugins** (plugin authors choose license):
- 🔌 Advanced ML models (custom training, better accuracy)
- 🔌 Enterprise integrations (Salesforce, ServiceNow, etc.)
- 🔌 Specialized workflows (legal, medical, etc.)
- 🔌 Premium inference rules (industry-specific)
- 🔌 Advanced dashboard widgets
- 🔌 Team collaboration features
- 🔌 Compliance/audit plugins (HIPAA, SOC2)

**SaaS Features** (hosted service, proprietary):
- ☁️ Multi-tenancy infrastructure
- ☁️ Cloud hosting and scaling
- ☁️ Managed database and backups
- ☁️ API rate limiting and quotas
- ☁️ Billing and subscription management
- ☁️ Enterprise support and SLAs
- ☁️ Advanced security features

### Plugin Marketplace Business Model

**For Plugin Developers**:
- Create free or paid plugins
- Set your own pricing
- Revenue share: **70% developer, 30% platform**
- MIT, Apache, or proprietary license (your choice)
- Promotion opportunities for high-quality plugins

**For Users**:
- Browse community plugins (free)
- Purchase premium plugins (one-time or subscription)
- Plugin credits system
- Money-back guarantee for paid plugins

**For Platform**:
- Transaction fees sustain development
- Premium plugins drive engagement
- Community plugins increase adoption
- Network effects: more plugins = more users = more plugins

### Community Engagement Strategy

**Phase 3 (Current)**: Foundation
- ✅ Open source core repository
- ✅ Clear contribution guidelines
- ✅ Conventional commits enforced
- ✅ Good first issues labeled
- 📝 Plugin developer guide (coming)

**Phase 3.5**: Plugin Ecosystem Launch
- 🔌 Plugin SDK and examples
- 🔌 Plugin development tutorial
- 🔌 Community plugin showcase
- 🎁 Developer grants ($500-2000 for quality plugins)
- 📢 Open source launch announcement

**Phase 4+**: Community Growth
- 🏆 Monthly plugin competitions
- 📚 Plugin best practices guide
- 🤝 Plugin developer office hours
- 💬 Discord/Slack community
- 📰 Blog: Plugin spotlights, case studies

### Value Propositions

**For Individual Users**:
- Free core functionality (capture, review, dashboard)
- Self-host option for privacy
- Choose plugins that fit your needs
- Support developers you like

**For Enterprise Users**:
- Open source = security audit possible
- Self-host behind firewall
- Proprietary plugins for compliance
- Professional support available

**For Developers**:
- Contribute to meaningful open source project
- Build plugins, earn revenue
- Learn from clean architecture
- Portfolio piece + community recognition

**For ADHD Community**:
- Tool designed by/for ADHD individuals
- Transparent, not exploitative
- Community-driven feature development
- Affordable (free tier always available)

### License Boundaries

```
┌─────────────────────────────────────────────┐
│  Divergent Flow Core (MIT License)          │
│  - Capture/Review/Dashboard                 │
│  - Type inference (basic)                   │
│  - Plugin system (runtime)                  │
│  - API & data models                        │
│  - UI components                            │
└─────────────────────────────────────────────┘
                     ↓
        ┌────────────┴────────────┐
        │   Plugin Interface       │
        └────────────┬────────────┘
                     ↓
    ┌────────────────┴────────────────┐
    │                                  │
┌───▼────────────────┐   ┌────────────▼──────┐
│ Community Plugins  │   │ Proprietary       │
│ (Any License)      │   │ Plugins           │
│ - Free to use      │   │ (Author's License)│
│ - Open source      │   │ - Paid/Licensed   │
│ - Community-built  │   │ - Closed source   │
└────────────────────┘   └───────────────────┘
```

### Success Metrics

**Community Health**:
- GitHub stars, forks, contributors
- Plugin submissions per month
- Active Discord/community members
- Contribution frequency

**Business Health**:
- Paid plugin sales
- Hosted SaaS subscribers
- Enterprise contracts
- Plugin marketplace revenue

### Launch Readiness Checklist

For open source community launch (Phase 3 completion):
- [ ] Working capture → review → dashboard flow
- [ ] Clear README with screenshots
- [ ] Contributing guidelines
- [ ] Code of conduct
- [ ] Good first issues (5-10 labeled)
- [ ] Plugin architecture documented
- [ ] Demo video (2-3 minutes)
- [ ] License clearly stated (MIT)
- [ ] Security policy
- [ ] Community discussion enabled

---

## Architecture Decision Records

### ADR-001: Incremental Migration vs. Rewrite

**Status**: Accepted

**Context**: We have a working MVP with users. The unified entity system requires significant architectural changes.

**Decision**: Incremental migration with backward compatibility over complete rewrite.

**Rationale**:
- Users continue to have working system
- Reduced risk of introducing bugs
- Allows testing new architecture with real data
- Team can learn and adjust as we go

**Consequences**:
- More complex codebase during migration (dual models)
- Longer migration timeline
- Need to maintain legacy code paths temporarily

### ADR-002: MongoDB for Entity Storage

**Status**: Accepted

**Context**: The unified entity system needs flexible schema support for dynamic entity types.

**Decision**: Continue using MongoDB as primary data store.

**Rationale**:
- MongoDB's document model aligns with entity structure
- Dynamic schema eliminates need for migrations
- JSON-based queries match entity attributes
- Already proven in MVP
- Good performance for entity graph queries

**Alternatives Considered**:
- PostgreSQL with JSONB: More complex query syntax
- Neo4j: Better for graphs but adds complexity
- DynamoDB: Vendor lock-in concerns

**Consequences**:
- Must carefully design indexes for multi-tenant queries
- Need to handle schema evolution in application code
- Relationship traversal requires careful query design

### ADR-003: Plugin Sandbox Strategy

**Status**: Proposed

**Context**: Plugins will execute user-provided code. Security is critical.

**Options**:
1. **WebWorker (Frontend)**: Isolated threads, no DOM access
2. **Process Isolation (Backend)**: Separate processes, resource limits
3. **WASM**: Compile plugins to WebAssembly

**Decision**: Hybrid approach
- Frontend: WebWorker for UI plugins (dashboard widgets)
- Backend: Process isolation for inference plugins
- WASM for performance-critical plugins (future)

**Rationale**:
- Multiple layers of security
- Platform-appropriate isolation
- Performance trade-offs acceptable for security

### ADR-004: Pricing Model

**Status**: Accepted

**Context**: Need sustainable revenue model for SaaS platform.

**Decision**: Freemium with usage-based add-ons (per unified.md)

**Tiers**:
- **Free**: Single user, basic features, 1GB storage
- **Pro**: $15/user/month, team features, 10GB storage
- **Enterprise**: Custom pricing, unlimited features

**Rationale**:
- Low barrier to entry (free tier)
- Clear upgrade path
- Usage-based allows power users to pay more
- Enterprise tier for compliance/custom needs

### ADR-005: Open Source + Plugin Marketplace Strategy

**Status**: Accepted

**Context**: Need to balance community growth with sustainable business model. Want to attract open source contributors while building a profitable business.

**Decision**: Open source core system (MIT) + plugin marketplace with mixed free/paid plugins.

**Rationale**:
- **Community Growth**: Open source attracts contributors, builds trust, creates network effects
- **Business Sustainability**: Plugin marketplace (70/30 split) + hosted SaaS provides revenue
- **Flexibility**: Plugin authors choose their own licensing (MIT, proprietary, etc.)
- **Differentiation**: ADHD-focused UX is hard to replicate, core value isn't just in code
- **Self-Hosting**: Privacy-conscious users can self-host, enterprise can audit code
- **Network Effects**: More plugins attract more users, more users attract more plugin developers

**Alternatives Considered**:
- **Fully Open Source**: Harder to monetize, less sustainable
- **Fully Proprietary**: Slower adoption, less community trust
- **Open Core (arbitrary feature split)**: Complex to maintain, unclear boundaries
- **Dual License**: More complex, harder for contributors to understand

**What's Open Source** (MIT):
- Capture, Review, Dashboard workflow
- Type inference engine (rule-based)
- Plugin system runtime
- API and data models
- UI components

**What Can Be Proprietary**:
- Advanced ML models (plugins)
- Enterprise integrations (plugins)
- Hosted SaaS infrastructure
- Multi-tenancy features
- Enterprise support/SLAs

**Consequences**:
- Must maintain clear license boundaries
- Plugin system must be robust and well-documented
- Community management becomes critical
- Need developer relations resources
- Plugin marketplace infrastructure required
- 70/30 revenue split reduces margins but increases ecosystem growth

**Success Metrics**:
- GitHub stars, contributors, plugin submissions
- Plugin marketplace revenue
- Hosted SaaS adoption
- Community engagement

---

## Quick Reference: Current vs. Target

### Feature Matrix (UPDATED)

| Feature | MVP Status | Unified Target | Migration Phase |
|---------|-----------|----------------|-----------------|
| Quick capture | ✅ Live | ✅ + Multiple channels | Phase 6 |
| Type inference | ✅ Rule-based | ✅ ML + Plugins | Phase 3.5 (basic), Phase 5 (advanced) |
| Review queue | 🚧 Phase 3 | ✅ Collaborative | Phase 3 (basic), Phase 7 (collaborative) |
| **Dashboard** | **⚡ Phase 3** | ✅ Advanced | **Phase 3 (basic)**, Phase 8 (advanced) |
| Collections | 🚧 Partial | ✅ Shared + Templates | Phase 7 |
| **Plugins** | **⚡ Phase 3.5** | ✅ Marketplace | **Phase 3.5 (foundation)**, Phase 5 (marketplace) |
| Multi-tenancy | ❌ | ✅ Full | Phase 6 |
| Permissions | ❌ | ✅ RBAC | Phase 7 |
| ML Training | ❌ | ✅ Custom models | Phase 8+ |
| Versioning | ❌ | ✅ Audit trail | Phase 4 |
| File storage | ❌ | ✅ S3/Azure | Phase 8+ |
| Real-time | ❌ | ✅ WebSockets | Phase 7 |
| Reflection | ❌ | ✅ Team insights | Phase 8+ |

**Legend**:
- ✅ Complete
- 🚧 In Progress
- ⚡ Current Priority
- ❌ Not Started

### API Evolution

**Current (Phase 1-2)**:
```
POST /api/items
GET  /api/items
GET  /api/items/{id}
PUT  /api/items/{id}
DELETE /api/items/{id}

# Legacy compatibility
POST /api/captures
GET  /api/captures
```

**Phase 3** (Add review + dashboard endpoints):
```
# Existing endpoints continue to work
POST /api/items
GET  /api/items
GET  /api/items/{id}
PUT  /api/items/{id}
DELETE /api/items/{id}

# New endpoints
GET  /api/items/review-queue    # Get items needing review
PUT  /api/items/{id}/review     # Mark item as reviewed
GET  /api/items/dashboard       # Get dashboard data (tasks, stats)
```

**Phase 4** (Add entity endpoints, maintain compatibility):
```
POST /api/items             # Still works (facade)
POST /api/entities          # New generic endpoint
GET  /api/entities/{id}
PUT  /api/entities/{id}
DELETE /api/entities/{id}
GET  /api/entities?query=...
```

**Phase 5+** (Full unified API):
```
POST   /api/v1/entities
GET    /api/v1/entities/{id}
PUT    /api/v1/entities/{id}
DELETE /api/v1/entities/{id}
POST   /api/v1/query        # Complex queries
GET    /api/v1/entity-types
POST   /api/v1/infer
GET    /api/v1/plugins
```

---

## Getting Started with Migration

### For Contributors

1. **Read This Document**: Understand current state and target
2. **Review unified.md**: Understand the vision
3. **Check Phase Status**: See which phase is active
4. **Pick a Task**: Start with Phase 3.5 tasks
5. **Follow Patterns**: Use existing clean architecture patterns
6. **Write Tests**: All new code must have tests
7. **Document Decisions**: Update ADRs for significant choices

### For Architects

1. **Validate ADRs**: Review and approve architecture decisions
2. **Design Reviews**: Review design docs before implementation
3. **Risk Assessment**: Identify migration risks early
4. **Performance Testing**: Ensure scalability at each phase
5. **Security Review**: Verify plugin sandbox implementation

### For Users

Nothing changes! The migration is designed to be invisible to end users. Your data is safe, and features will only get better.

---

## Resources

- **[unified.md](./unified.md)**: Complete architectural vision
- **[backend/README.md](../backend/README.md)**: Current backend architecture
- **[README.md](../README.md)**: Project overview
- **[CONTRIBUTING.md](../CONTRIBUTING.md)**: Contribution guidelines

---

## Questions?

Open an issue with the `architecture` label or reach out to maintainers.

**Last Updated**: 2026-01-15
**Next Review**: Before Phase 3.5 kickoff
