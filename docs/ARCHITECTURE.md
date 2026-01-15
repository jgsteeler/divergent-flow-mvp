# Divergent Flow Architecture

**Status**: MVP → Unified Entity System Migration

This document describes the current architecture and the roadmap for evolving to the **Unified Entity System** outlined in [unified.md](./unified.md).

## Table of Contents

- [Current Architecture (MVP)](#current-architecture-mvp)
- [Target Architecture (Unified Entity System)](#target-architecture-unified-entity-system)
- [Migration Path](#migration-path)
- [Architecture Decision Records](#architecture-decision-records)

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

1. **Incremental Migration**: No "big bang" rewrite
2. **Backward Compatibility**: Existing data continues to work
3. **Zero Downtime**: Users never lose access
4. **Feature Flags**: New capabilities gated behind flags
5. **ADHD-First UX**: Never compromise user experience for architecture

### Phase-by-Phase Migration

#### Phase 3.5: Prepare for Entities (8-12 weeks)

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

#### Phase 4: Plugin-Based Inference (12-16 weeks)

**Goal**: Replace hardcoded inference with plugin system

**Tasks**:
- [ ] Design plugin interface (`IInferencePlugin`)
- [ ] Create plugin registry/loader
- [ ] Sandbox execution environment (WebWorker frontend, Process isolation backend)
- [ ] Plugin permissions model
- [ ] Migrate existing inference rules to plugins
- [ ] Plugin marketplace UI (basic)

**Deliverables**:
- Plugin SDK with examples
- 5-10 built-in plugins (current inference logic)
- Plugin installation UI
- Developer documentation

**Success Criteria**:
- Existing inference continues to work via plugins
- Community can create new plugins
- Plugin execution is isolated and safe

#### Phase 5: Multi-Tenancy (8-12 weeks)

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

#### Phase 6: Collaboration Features (12-16 weeks)

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

---

## Quick Reference: Current vs. Target

### Feature Matrix

| Feature | MVP Status | Unified Target | Migration Phase |
|---------|-----------|----------------|-----------------|
| Quick capture | ✅ Live | ✅ + Multiple channels | Phase 4 |
| Type inference | ✅ Rule-based | ✅ ML + Plugins | Phase 4 |
| Review queue | 🚧 Basic | ✅ Collaborative | Phase 6 |
| Collections | 🚧 Partial | ✅ Shared + Templates | Phase 6 |
| Multi-tenancy | ❌ | ✅ Full | Phase 5 |
| Permissions | ❌ | ✅ RBAC | Phase 6 |
| Plugins | ❌ | ✅ Marketplace | Phase 4 |
| ML Training | ❌ | ✅ Custom models | Phase 7 |
| Versioning | ❌ | ✅ Audit trail | Phase 3.5 |
| File storage | ❌ | ✅ S3/Azure | Phase 8 |
| Real-time | ❌ | ✅ WebSockets | Phase 6 |
| Dashboards | ⏭️ Planned | ✅ Advanced | Phase 8 |
| Reflection | ⏭️ Planned | ✅ Team insights | Phase 8 |

### API Evolution

**Current**:
```
POST /api/items
GET  /api/items
GET  /api/items/{id}
PUT  /api/items/{id}
DELETE /api/items/{id}
```

**Phase 3.5** (Add entity endpoints, maintain compatibility):
```
POST /api/items             # Still works (facade)
POST /api/entities          # New generic endpoint
GET  /api/entities/{id}
PUT  /api/entities/{id}
DELETE /api/entities/{id}
GET  /api/entities?query=...
```

**Phase 4+** (Full unified API):
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
