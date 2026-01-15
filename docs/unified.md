# Unified Entity System as SaaS - Architecture & Monetization

> **Implementation Status**: This is the **target vision** for Divergent Flow's evolution into a full SaaS platform.
> 
> 📋 **See [ARCHITECTURE.md](./ARCHITECTURE.md)** for:
> - Current MVP implementation status
> - Gap analysis between current state and this vision
> - Phased migration roadmap
> - Architecture decision records
>
> **Current MVP**: We have a working single-user system with basic capture, inference, and review features. This document describes the end-state unified entity system we're building toward incrementally.

**Goal**: Design a multi-user, multi-tenant SaaS platform based on the unified entity architecture, with pricing tiers, extensibility through plugin systems, and community-driven feature development.

**Stack**: Node.js React Frontend | .NET Web API Backend | MongoDB Persistence

**Philosophy**: Everything is an entity. Everything is extensible. Users pay for capabilities, not features.

---

## Part 1: Foundational Architecture (from PowerShell MVP)

### Core Concept: Entity-Centric Everything

The unified entity system recognizes that everything in a productivity system is fundamentally the same:

```
Entity = {
  id: "entity:{type}:{uniqueId}",
  entityTypeId: "entityType:{typename}",
  tenantId: "tenant:{orgId}",        # Multi-tenant: all entities scoped to tenant
  ownerId: "user:{userId}",           # Permission: who owns this entity
  attributes: {...},                  # Defined by entityTypeId schema
  metadata: {
    createdDate: timestamp,
    updatedDate: timestamp,
    version: integer,                  # Version history for auditing
    visibility: "private|team|org"     # Sharing/permission model
  },
  relationships: {
    parentIds: [...],                  # Hierarchical relationships
    referencedByIds: [...],            # Reverse relationships for queries
    linkedEntities: {...}              # Explicit relationships
  }
}
```

**Why this matters for SaaS**:
1. Single data model across all functionality
2. Extensibility through entity types (no schema migrations)
3. Multi-tenant by design (tenantId everywhere)
4. Version history built-in (audit trail)
5. Permission model integrated (visibility + ownership)

---

## Part 2: The Seven Pillars Extended for SaaS

### 1. CAPTURE (Multiple Input Channels)

**Personal System**:
- Terminal CLI (PowerShell)
- VS Code command palette

**SaaS Enhancement**:
- Web UI text input
- Mobile app voice capture
- Email forwarding (email→capture)
- Slack integration (mention bot)
- Browser extension (clip web content)
- API webhook (external systems)
- Zapier/IFTTT integration

**Entity Model**:
```
{
  id: "entity:capture:abc123",
  entityTypeId: "itemType:capture",
  tenantId: "tenant:acme",
  ownerId: "user:john",
  attributes: {
    content: "meeting notes about Q2 planning",
    source: "mobile_app|web|slack|email|api",
    sourceMetadata: {
      slackChannelId: "C123",
      slackMessageId: "1234567890.123456"
    },
    processingState: "raw|organizing|organized"
  }
}
```

**Monetization**:
- **Free tier**: Web capture + terminal (basic)
- **Pro tier**: +Mobile app + Email forwarding + Slack
- **Enterprise**: +API webhooks + Zapier + Custom integrations

**Community Plugin Hook**:
- Users can create capture sources as plugins
- Example: "Jira ticket capture plugin" - auto-capture assigned tickets
- Plugin creates custom `captureSource:{pluginName}` entities

---

### 2. ORGANIZE (Collection Management & Categorization)

**Personal System**:
- Config-driven collection types
- Manual categorization

**SaaS Enhancement**:
- Shared collections (team organizing)
- Workspace hierarchies
- Templates (collection blueprints)
- Auto-categorization suggestions (ML)
- Nested collections (parent-child)
- Collection permissions (read/write/admin)
- Collection sharing across teams

**Entity Model**:
```
{
  id: "entity:collection:projects_2026",
  entityTypeId: "collectionType:project",
  tenantId: "tenant:acme",
  ownerId: "user:jane",
  attributes: {
    metadata: {
      name: "2026 Projects",
      description: "All 2026 strategic projects",
      tags: ["strategic", "2026"],
      owner: "jane@acme.com"
    },
    references: {
      parentCollectionId: "collection:strategic_work",
      teamId: "team:engineering"
    }
  },
  itemIds: [...],                    # Items in this collection
  relationships: {
    members: [                        # Team access
      {userId: "user:john", role: "contributor"},
      {userId: "user:jane", role: "admin"}
    ]
  }
}
```

**Monetization**:
- **Free**: Up to 10 collections per user, private only
- **Pro**: Unlimited collections, team collections (up to 5 members)
- **Enterprise**: Unlimited teams, workspace management, collection templates

**Community Plugin Hook**:
- Template library (community creates collection templates)
- Smart organizer plugins (custom categorization logic)
- Example: "Auto-organize by client plugin" - watches new items, assigns collection

---

### 3. INFER (AI-Powered Understanding)

**Personal System**:
- Local inference rules
- Simple heuristics
- Learning from corrections

**SaaS Enhancement**:
- Hosted ML models (NLP, classification, recommendation)
- Multiple inference engines (plug-and-play)
- Confidence scoring + user feedback loop
- Batch inference (async)
- Custom model training per tenant
- Inference caching (faster repeated operations)

**Entity Model**:
```
{
  id: "entity:inferenceRule:type_classifier_v2",
  entityTypeId: "entityType:inferenceRule",
  tenantId: "tenant:acme",
  ownerId: "user:system",
  attributes: {
    metadata: {
      name: "Type Classifier v2",
      description: "Classifies items into types",
      modelProvider: "internal|azure|openai|huggingface",
      modelId: "bert-classification-v2",
      accuracy: 0.94,
      lastTrained: "2026-01-10"
    },
    logic: {
      triggersOn: ["itemType:capture"],
      outputs: ["metadata:inferredType"],
      confidence_threshold: 0.75,
      fallback_type: "itemType:note"
    }
  },
  references: {
    trainingDataCollectionId: "collection:training_data_v2"
  }
}
```

**Monetization**:
- **Free**: Basic inference (rule-based, no ML)
- **Pro**: ML-powered inference (1M API calls/month)
- **Enterprise**: Custom model training, priority inference, higher API limits

**Community Plugin Hook**:
- Inference rules as plugins (community shares rules)
- Custom inference models (users upload models)
- Example: "Legal document classifier" plugin for law firm users

---

### 4. REVIEW (Quality Gates & Human Validation)

**Personal System**:
- Review queue
- 3 items per session
- Manual confirmation

**SaaS Enhancement**:
- Collaborative review (team discussions)
- Approval workflows
- SLA tracking (how long in review?)
- Review assignments (assign to specific people)
- Commenting/annotation
- Review history & audit trails
- Automated review suggestions (based on confidence)

**Entity Model**:
```
{
  id: "entity:reviewSession:2026_01_14_morning",
  entityTypeId: "reflectionType:review_session",
  tenantId: "tenant:acme",
  ownerId: "user:john",
  attributes: {
    metadata: {
      status: "in_progress|completed|paused",
      startedAt: timestamp,
      completedAt: null,
      itemsReviewedCount: 3
    },
    references: {
      itemsUnderReview: [
        {
          itemId: "item:capture:xyz",
          assignedTo: "user:john",
          status: "pending|approved|rejected|needs_revision",
          approvalRequired: true,
          approvalsReceived: ["user:jane"],
          comments: [
            {userId: "user:jane", text: "Needs more detail", createdAt: timestamp}
          ]
        }
      ]
    }
  }
}
```

**Monetization**:
- **Free**: Single-user review queue
- **Pro**: Team review, approval workflows, comments
- **Enterprise**: Advanced workflows, SLA tracking, integrations

**Community Plugin Hook**:
- Review workflow plugins (users define custom workflows)
- Auto-assignment rules plugins
- Example: "Daily standup review" plugin - auto-creates review sessions

---

### 5. MACHINE LEARNING (Intelligence & Adaptation)

**Personal System**:
- Local training on corrections
- Simple learning indexes

**SaaS Enhancement**:
- Hosted model training (GPU-backed)
- Model versioning & A/B testing
- Explainability (why did model predict this?)
- Transfer learning (learn from community models)
- Privacy-preserving model training (federated learning?)
- Real-time model performance monitoring
- Model marketplace (sell/buy trained models)

**Entity Model**:
```
{
  id: "entity:mlModel:priority_predictor_v3",
  entityTypeId: "entityType:mlModel",
  tenantId: "tenant:acme",
  ownerId: "user:system",
  attributes: {
    metadata: {
      modelName: "Priority Predictor",
      version: 3,
      modelType: "gradient_boosting|neural_network|ensemble",
      framework: "xgboost|tensorflow|custom",
      trainingDate: timestamp,
      metrics: {
        accuracy: 0.89,
        precision: 0.92,
        recall: 0.85,
        f1: 0.88
      },
      performance: {
        inferenceTimeMs: 45,
        throughputPerSecond: 1000
      }
    },
    references: {
      trainingDataCollectionId: "collection:training_data_priority",
      inputAttributes: ["metadata:title", "metadata:estimate", "metadata:dueDate"],
      outputAttribute: "metadata:priority"
    }
  },
  relationships: {
    versions: [
      {modelId: "mlModel:priority_predictor_v2", createdDate: timestamp},
      {modelId: "mlModel:priority_predictor_v1", createdDate: timestamp}
    ],
    abtests: [
      {
        experimentId: "experiment:priority_v3_test",
        variantA: "mlModel:priority_predictor_v2",
        variantB: "mlModel:priority_predictor_v3",
        status: "running",
        startDate: timestamp
      }
    ]
  }
}
```

**Monetization**:
- **Free**: Community models only
- **Pro**: 1 custom model, 100K monthly predictions
- **Enterprise**: Unlimited models, GPU training credits, model marketplace access

**Community Plugin Hook**:
- Model marketplace (publish trained models)
- Feature engineering plugins (create new predictive features)
- Example: "Burnout detection model" - predicts user burnout from activity

---

### 6. ACTION/DASHBOARD (Execution & Focus)

**Personal System**:
- Terminal dashboard
- Next action display
- Quick wins section

**SaaS Enhancement**:
- Web/mobile dashboards
- Team dashboards (shared context)
- Real-time updates (websockets)
- Customizable widgets
- Dashboard templates
- Data visualization (charts, graphs)
- Integration widgets (Slack status, calendar, etc.)
- Dashboards as shareable artifacts

**Entity Model**:
```
{
  id: "entity:dashboard:adhd_focus_main",
  entityTypeId: "dashboardType:adhd_focus",
  tenantId: "tenant:acme",
  ownerId: "user:john",
  attributes: {
    metadata: {
      name: "My Focus Dashboard",
      description: "Daily dashboard for ADHD management",
      isDefault: true,
      refreshIntervalSeconds: 30
    },
    sections: [
      {
        sectionId: "overdue",
        title: "Overdue Items",
        query: {
          entityTypeId: "itemType:action",
          filters: [
            {attribute: "metadata:dueDate", operator: "lt", value: "now()"},
            {attribute: "metadata:status", operator: "ne", value: "completed"}
          ],
          sort: [{attribute: "metadata:dueDate", order: "asc"}],
          limit: 10
        },
        display: {
          showAttributes: ["metadata:title", "metadata:dueDate", "metadata:priority"],
          actions: ["complete", "defer", "escalate"],
          layout: "list|card|table"
        }
      },
      {
        sectionId: "next_action",
        title: "Next Action",
        query: {...},
        display: {...}
      },
      {
        sectionId: "integrations",
        title: "Slack Status",
        widgetType: "slack_status",
        config: {
          slackWorkspaceId: "workspace:acme",
          statusUpdateOn: "itemType:action",
          statusMapping: {
            "metadata:priority": "high" → "🔥 in focused work"
          }
        }
      }
    ]
  },
  relationships: {
    sharedWith: [
      {userId: "user:jane", accessLevel: "view"},
      {teamId: "team:engineering", accessLevel: "view"}
    ]
  }
}
```

**Monetization**:
- **Free**: 1 personal dashboard
- **Pro**: Unlimited dashboards, team dashboards, integrations
- **Enterprise**: Advanced visualizations, real-time updates, API access

**Community Plugin Hook**:
- Dashboard widgets as plugins
- Integration connectors (Slack, Jira, Asana, etc.)
- Example: "Pomodoro timer widget" - helps with time management

---

### 7. REFLECT (Periodic Insight & Growth)

**Personal System**:
- Daily/weekly/monthly reflection prompts
- Structured review

**SaaS Enhancement**:
- Guided reflection workflows
- Peer reflection groups (accountability partners)
- Reflection insights (patterns detected)
- Goal tracking & progress visualization
- Journaling features (rich text, attachments)
- Reflection prompts library (curated + community)
- Annual review workflows

**Entity Model**:
```
{
  id: "entity:reflection:2026_week_3",
  entityTypeId: "reflectionType:weekly",
  tenantId: "tenant:acme",
  ownerId: "user:john",
  attributes: {
    metadata: {
      weekStart: "2026-01-13",
      weekEnd: "2026-01-19",
      completedAt: timestamp,
      mood: "energized|neutral|overwhelmed",
      insights: [
        "Focused work on Tuesday was most productive",
        "Skipped lunch 3 days - need to prioritize breaks"
      ]
    },
    completedItems: {
      total: 42,
      byType: {
        "itemType:action": 28,
        "itemType:meeting_note": 14
      },
      byPriority: {
        "high": 15,
        "medium": 22,
        "low": 5
      }
    },
    references: {
      goals: [
        {goalId: "goal:q1_focus", progressPercent: 65}
      ],
      teamInsights: [                  # Team-level patterns
        {teamId: "team:engineering", avgItemsPerMember: 38}
      ]
    }
  },
  relationships: {
    sharedWith: [
      {userId: "user:manager", accessLevel: "view", purpose: "1-on-1"}
    ],
    reflectionGroup: {
      groupId: "group:accountability_partners",
      members: ["user:jane", "user:alice", "user:john"],
      sharedReflections: true
    }
  }
}
```

**Monetization**:
- **Free**: Personal reflections only
- **Pro**: Reflection groups, insights, goal tracking
- **Enterprise**: Team analytics, goal alignment across org

**Community Plugin Hook**:
- Reflection prompt libraries
- Insight analysis plugins (detect patterns)
- Example: "Burnout risk detector" - analyzes reflection text for warning signs

---

## Part 2.5: Object Storage & Media Handling

**Critical Capability**: Users can create collections of images, videos, documents, or any file type. The system must efficiently store and manage these without bloating the JSON data store.

### Storage Architecture

**Multi-Layer Approach**:

#### 1. Entity References (MongoDB)
Entities store **references** to objects, not the objects themselves:

```json
{
  "id": "entity:item:abc123",
  "entityTypeId": "itemType:image_capture",
  "tenantId": "tenant:acme",
  "attributes": {
    "metadata": {
      "title": "Screenshot from meeting",
      "description": "Key diagram"
    },
    "fileReferences": {
      "primaryImageId": "file:img_abc123",
      "thumbnailId": "file:thumb_abc123",
      "originalSize": 2458624,  // bytes
      "mimeType": "image/png"
    },
    "references": {
      "collectionId": "collection:2026_meetings"
    }
  }
}
```

#### 2. File Storage Backend
Three deployment options:

**Option A: Cloud Object Storage (Recommended for SaaS)**
- **AWS S3** - Production standard
  - Bucket structure: `s3://grind-{environment}/{tenantId}/{entityId}/{fileId}`
  - Automatic versioning + lifecycle policies
  - CloudFront CDN for delivery
  - Cost: ~$0.023 per GB/month + transfer fees
  
- **Azure Blob Storage** - Microsoft stack integration
  - Container structure: `grind/{tenantId}/{entityId}/{fileId}`
  - Automatic replication
  - Conditional access policies
  - Similar pricing to S3

- **Google Cloud Storage** - Multi-region support
  - Bucket structure: `gs://grind-{environment}-{region}/{tenantId}/{entityId}/{fileId}`

**Option B: Self-Hosted Object Storage**
- **MinIO** - S3-compatible, open source
  - Deploy on Kubernetes
  - Replication across nodes
  - Good for on-premises enterprise deployments

**Option C: 3rd Party Integrations (Plugin-Based)**
- **Dropbox Integration** - User's own Dropbox account
- **Google Drive Integration** - User's own Google Drive
- **OneDrive Integration** - User's own Microsoft 365
- **Box.com Integration** - Enterprise file management
- **AWS S3 (User Account)** - Customer's own AWS bucket

### File Entity Type

New entity type for file storage:

```json
{
  "id": "entity:file:img_abc123",
  "entityTypeId": "entityType:fileObject",
  "tenantId": "tenant:acme",
  "ownerId": "user:john",
  "attributes": {
    "metadata": {
      "originalFilename": "screenshot.png",
      "mimeType": "image/png",
      "sizeBytes": 2458624,
      "uploadedDate": "2026-01-14T10:30:00Z",
      "uploadSource": "web|mobile|api|integration:dropbox"
    },
    "storage": {
      "provider": "s3|azure|gcs|minio|dropbox",
      "bucketName": "grind-prod",
      "objectPath": "tenant_acme/entity_item_abc123/file_img_abc123.png",
      "externalUrl": "https://s3.amazonaws.com/grind-prod/...",
      "accessLevel": "private|public|shared"
    },
    "processing": {
      "status": "uploaded|processing|ready|error",
      "variants": [
        {
          "type": "thumbnail",
          "size": "200x200",
          "fileId": "file:thumb_abc123"
        },
        {
          "type": "preview",
          "size": "800x600",
          "fileId": "file:preview_abc123"
        }
      ],
      "scanStatus": "pending|safe|quarantined",  // Virus/malware scan
      "ocrText": "..."  // Extracted text from document
    },
    "references": {
      "parentEntityId": "entity:item:abc123",
      "collectionId": "collection:2026_meetings"
    }
  },
  "visibility": "private",
  "relationships": {
    "tags": ["screenshot", "meeting", "important"]
  }
}
```

### API Design for File Operations

```
# Upload file
POST /api/v1/files/upload
  body: multipart/form-data (file + entityTypeId + attributes)
  returns: {fileId, uploadUrl, externalUrl}

# Download file
GET /api/v1/files/{fileId}/download
  returns: file stream

# Get signed URL (for direct access)
GET /api/v1/files/{fileId}/signed-url
  query: ?expireIn=3600
  returns: {url, expiresAt}

# List files in collection
GET /api/v1/entities/{collectionId}/files
  returns: [fileEntity, ...]

# Delete file
DELETE /api/v1/files/{fileId}
  returns: {status: "deleted"}

# Get file metadata
GET /api/v1/files/{fileId}/metadata
  returns: {fileEntity}

# Update file attributes
PUT /api/v1/files/{fileId}
  body: {metadata: {...}}
  returns: {fileEntity}

# Process file (thumbnails, OCR, etc.)
POST /api/v1/files/{fileId}/process
  body: {operations: ["thumbnail", "ocr"]}
  returns: {jobId, status}
```

### Integration with Entity System

**Attribute Kind for Files**:
```json
{
  "id": "file:reference",
  "calculated": false,
  "userPrompt": true,
  "format": "file",
  "acceptedMimeTypes": ["image/*", "video/*", "application/pdf"],
  "maxSizeBytes": 104857600,  // 100MB default
  "description": "File upload field"
}
```

**Entity Type Example: Image Collection**:
```json
{
  "id": "collectionType:image_gallery",
  "attributes": [
    {
      "name": "metadata:title",
      "kind": "string",
      "required": true
    },
    {
      "name": "metadata:description",
      "kind": "string",
      "required": false
    },
    {
      "name": "files",
      "kind": "array:fileReference",
      "required": true,
      "description": "Images in this collection"
    },
    {
      "name": "metadata:totalStorageBytes",
      "kind": "timestamp:calculated",  // Custom calculated
      "value": "sum(files.*.sizeBytes)"
    }
  ]
}
```

### Storage Quotas & Billing

**Tier-Based Storage Limits**:

| Tier | Storage | File Size | # Files | Cost/Extra |
|------|---------|-----------|---------|-----------|
| Free | 1GB | 10MB max | 100 | N/A |
| Pro | 100GB | 500MB max | 10K | $0.10/GB |
| Enterprise | Unlimited | Unlimited | Unlimited | Custom |

**Overage Billing**:
- Enterprise: $0.023/GB/month (market rate)
- Can include CDN delivery charges
- Bandwidth overage: $0.085/GB after included quota

### Virus/Malware Scanning

**Security Pipeline**:
1. User uploads file
2. System queues file for scan
3. ClamAV or VirusTotal API scans
4. Safe: File marked "safe", available for download
5. Quarantined: File marked "quarantined", not downloadable, user notified
6. Admin can review quarantined files

```json
{
  "id": "entity:file:abc123",
  "processing": {
    "scanStatus": "pending|safe|quarantined|error",
    "lastScanDate": "2026-01-14T10:35:00Z",
    "scanProvider": "clamav|virustotal",
    "scanResults": {
      "detectionCount": 1,
      "detections": [
        {
          "provider": "virustotal",
          "engineName": "Avast",
          "category": "Trojan"
        }
      ]
    }
  }
}
```

### 3rd Party Integration (Plugin)

**Example: Dropbox Integration Plugin**:

```typescript
interface DropboxIntegrationPlugin {
  id: "integration:dropbox",
  type: "storage_provider",
  
  // User connects their Dropbox
  async setupAuth(credentials): Promise<{status: "authorized"}>,
  
  // Link file from user's Dropbox to entity
  async linkFile(entityId, dropboxPath): Promise<{
    fileId: "file:dropbox_abc123",
    externalUrl: "https://dl.dropboxusercontent.com/...",
    syncEnabled: true
  }>,
  
  // Sync changes (if user updates file in Dropbox)
  async syncFiles(entityId): Promise<{updated: number}>,
  
  // User can still view/edit in Dropbox, system reflects changes
  webhooks: [
    {
      event: "file_modified",
      handler: "syncFileFromDropbox"
    }
  ]
}
```

**Benefits**:
- User keeps files in their preferred storage
- No storage costs for platform (user pays Dropbox)
- Works with existing user workflows
- Integrations as plugins → easy to add new providers

### Implementation Timeline

**Phase 1 (SaaS MVP+8 weeks)**:
- File entity type definition
- AWS S3 integration
- Basic upload/download API
- Virus scanning
- Storage quotas per tier

**Phase 2 (SaaS MVP+16 weeks)**:
- Azure Blob + GCS support
- Image thumbnails + processing
- Document OCR
- Signed URLs for secure sharing

**Phase 3 (SaaS MVP+24 weeks)**:
- Dropbox/Drive integration plugins
- User's own cloud bucket (customer-managed)
- Advanced versioning
- Lifecycle management (archive old files)

---

## Part 3: Multi-Tenant Architecture

### Database Schema (MongoDB)

**Collections**:

#### 1. Entities Collection
```json
{
  _id: ObjectId,
  id: "entity:capture:abc123",
  entityTypeId: "itemType:capture",
  tenantId: "tenant:acme",
  ownerId: "user:john",
  createdDate: ISODate,
  updatedDate: ISODate,
  version: 1,
  attributes: {...},
  relationships: {...},
  indexes: [
    {tenantId: 1, entityTypeId: 1, createdDate: -1},
    {tenantId: 1, ownerId: 1, entityTypeId: 1},
    {tenantId: 1, "attributes.metadata.status": 1},
    {tenantId: 1, "relationships.parentIds": 1}
  ]
}
```

#### 2. Entity Types Collection
```json
{
  _id: ObjectId,
  id: "entityType:itemType:capture",
  parentTypeId: "entityType:item",
  tenantId: "tenant:system",  # System entity types, shared
  attributes: [
    {
      name: "content",
      kind: "string",
      required: true,
      description: "Captured text"
    },
    {...}
  ]
}
```

#### 3. Tenants Collection
```json
{
  _id: ObjectId,
  tenantId: "tenant:acme",
  name: "Acme Corp",
  plan: "enterprise",
  subscription: {
    tier: "enterprise",
    status: "active",
    startDate: ISODate,
    renewalDate: ISODate,
    monthlyBudget: 5000,
    currentSpend: 2340
  },
  quotas: {
    activeUsers: {limit: 1000, current: 350},
    storage: {limit: 1000000, current: 450000},  # MB
    monthlyApiCalls: {limit: 100000000, current: 45000000}
  },
  features: {
    teamCollaborations: true,
    mlModels: true,
    pluginMarketplace: true,
    advancedAnalytics: true
  }
}
```

#### 4. Users Collection
```json
{
  _id: ObjectId,
  userId: "user:john",
  tenantId: "tenant:acme",
  email: "john@acme.com",
  role: "admin|member|viewer",
  permissions: [
    "capture:create",
    "collection:manage",
    "review:approve",
    "mlModel:train"
  ],
  subscription: {
    personalPlan: "pro",  # Different from tenant plan
    credits: {
      apiCalls: {monthly: 1000000, used: 450000},
      storage: {monthly: 100000, used: 45000},  # MB
      modelTraining: {monthly: 10, used: 3}     # Number of models
    }
  }
}
```

#### 5. Plugins Collection
```json
{
  _id: ObjectId,
  pluginId: "plugin:slack_integration",
  tenantId: "tenant:acme",  # null for community plugins
  author: "user:jane",
  name: "Slack Integration",
  version: "1.2.0",
  type: "capture_source|inference_rule|dashboard_widget|workflow",
  status: "published|draft|deprecated",
  code: "...",  # Containerized/sandboxed
  permissions: [
    "read:entities",
    "write:entities",
    "write:inference_results"
  ],
  ratings: {
    avgRating: 4.7,
    totalRatings: 234,
    installs: 1250
  },
  pricing: {
    tier: "free|paid",
    monthlyPrice: 5.00
  }
}
```

### API Design (.NET Web API)

**Base Endpoint**: `/api/v1`

**Pattern**: RESTful + Entity-centric

```
# Entities CRUD
POST   /api/v1/entities
GET    /api/v1/entities/{id}
PUT    /api/v1/entities/{id}
DELETE /api/v1/entities/{id}
GET    /api/v1/entities?query=...

# Entity Types
GET    /api/v1/entity-types
GET    /api/v1/entity-types/{id}
POST   /api/v1/entity-types (admin only)

# Queries (search/filter)
POST   /api/v1/query  # Complex queries with filters, sorts, joins
GET    /api/v1/query/saved/{queryId}

# Inference
POST   /api/v1/infer  # Run inference on entity
GET    /api/v1/inference-rules

# ML Models
GET    /api/v1/ml-models
POST   /api/v1/ml-models/{id}/train
POST   /api/v1/ml-models/{id}/predict

# Plugins
GET    /api/v1/plugins
GET    /api/v1/plugins/marketplace
POST   /api/v1/plugins/{id}/install
POST   /api/v1/plugins/upload (user plugins)

# Webhooks
POST   /api/v1/webhooks
GET    /api/v1/webhooks/{id}
POST   /api/v1/webhooks/{id}/test

# Audit & Compliance
GET    /api/v1/audit-log
GET    /api/v1/entities/{id}/history

# Analytics
GET    /api/v1/analytics/dashboard
GET    /api/v1/analytics/entity-usage
GET    /api/v1/analytics/feature-usage
```

### Frontend Architecture (React)

**Component Structure**:
```
/components
  /entity
    EntityCard.tsx         # Display single entity
    EntityList.tsx         # List with filters, sorting
    EntityForm.tsx         # Create/edit entity
    EntityViewer.tsx       # Read-only detailed view
    EntityRelationships.tsx # Show connections

  /dashboard
    DashboardBuilder.tsx   # Create/edit dashboard config
    DashboardViewer.tsx    # Display dashboard with sections
    DashboardWidget.tsx    # Individual widget renderer

  /review
    ReviewQueue.tsx        # Display review items
    ReviewBatch.tsx        # Review 3 items
    ApprovalWorkflow.tsx   # Approval UI

  /reflection
    ReflectionForm.tsx     # Fill reflection
    ReflectionInsights.tsx # Show patterns/graphs
    GoalTracker.tsx        # Goal progress

  /ml
    ModelTrainer.tsx       # Train custom model
    ModelTester.tsx        # Test model predictions
    ModelMarketplace.tsx   # Browse community models

  /plugins
    PluginMarketplace.tsx  # Browse/install plugins
    PluginUploader.tsx     # Upload custom plugin
    PluginSettings.tsx     # Configure installed plugin

/hooks
  useEntity.ts            # CRUD operations
  useQuery.ts             # Search/filter
  useInference.ts         # Run inference
  useTenant.ts            # Tenant context
  useAuth.ts              # Auth + permissions

/services
  entityService.ts
  inferenceService.ts
  pluginService.ts
  analyticsService.ts
```

---

## Part 4: Pricing Tiers & Monetization

### Tier Structure

#### Free Tier
- **Price**: $0/month
- **Users**: 1 personal workspace
- **Features**:
  - ✅ Capture (web only, basic)
  - ✅ Organize (up to 10 collections)
  - ✅ Inference (rule-based only)
  - ✅ Review (personal queue)
  - ✅ Dashboard (1 dashboard)
  - ✅ Reflection (basic)
  - ❌ Team collaboration
  - ❌ ML model training
  - ❌ Advanced plugins
- **Quotas**:
  - 10K entities/month
  - 100MB storage
  - Community models only

#### Pro Tier
- **Price**: $15/month/user
- **Users**: Up to 50 per team
- **Features**:
  - ✅ Everything from Free
  - ✅ Capture (web + mobile + email + Slack)
  - ✅ Organize (unlimited collections, team collections)
  - ✅ Inference (1 ML model, 1M monthly predictions)
  - ✅ Review (team reviews, approval workflows)
  - ✅ Dashboard (unlimited, team dashboards)
  - ✅ Reflection (reflection groups)
  - ✅ Plugin marketplace (install up to 10 plugins)
  - ✅ API access (100K calls/month)
- **Quotas**:
  - 1M entities/month
  - 10GB storage
  - 1 custom ML model

#### Enterprise Tier
- **Price**: Custom (typically $50-200+/user/month)
- **Users**: Unlimited
- **Features**:
  - ✅ Everything from Pro
  - ✅ Advanced capture (custom sources)
  - ✅ Organize (nested workspaces, advanced permissions)
  - ✅ Inference (unlimited ML models, 100M monthly predictions)
  - ✅ Review (advanced workflows, SLA tracking)
  - ✅ Dashboard (advanced analytics, real-time)
  - ✅ Reflection (team insights, goal alignment)
  - ✅ Plugin marketplace (unlimited plugins + private plugins)
  - ✅ API access (unlimited)
  - ✅ Webhooks + Zapier
  - ✅ SSO + advanced auth
  - ✅ Compliance (HIPAA, SOC2, etc.)
  - ✅ Dedicated support
  - ✅ Custom training
- **Quotas**: Unlimited everything

### Usage-Based Add-ons

**API Calls Beyond Tier**:
- $0.10 per 10K calls (overage)

**ML Model Training**:
- Free: 1 model
- $50 per additional model per month
- GPU training credits: $10 per hour of GPU time

**Storage Beyond Tier**:
- $5 per 100GB/month

**Advanced Analytics**:
- $100/month for team analytics dashboard
- Custom reports: $500 setup + $200/month

**Premium Plugins**:
- Varies by plugin author
- Typical: $5-50/month per plugin

### Plugin Marketplace Monetization

**For Users**:
- Browse 1000+ plugins (free + paid)
- Install plugins with 1-click
- Use in-app credits to purchase plugins

**For Developers**:
- Create and publish plugins
- Free plugins: Build community credibility
- Paid plugins: Set your own price
- Revenue share: 70% developer, 30% platform
- Featured placement: $100/month

**Top Plugin Categories** (estimated demand):
1. **Integrations** (Slack, Jira, Asana, Salesforce) - high demand
2. **Inference Rules** (domain-specific classifiers) - high demand
3. **Dashboard Widgets** (charts, timers, notifications) - medium demand
4. **Workflow Templates** (industry-specific) - high demand
5. **Analytics** (burnout detection, productivity trends) - medium demand
6. **AI/ML** (custom models, predictions) - growing

---

## Part 5: Plugin Architecture & Extensibility

### Plugin System Design

**Plugin Types**:

#### 1. Capture Source Plugins
```typescript
interface CaptureSourcePlugin {
  id: "captureSource:plugin_name",
  type: "capture_source",
  
  // Setup: User authorizes (OAuth, API key, etc.)
  async setupAuth(credentials): Promise<{status: "authorized"}>,
  
  // Capture: Fetch from external system
  async capture(options): Promise<{
    entities: [{
      entityTypeId: "itemType:capture",
      attributes: {content: "..."}
    }]
  }>,
  
  // Polling: Schedule regular captures
  pollInterval?: "hourly|daily|weekly",
  
  // User config UI
  configSchema: {
    fields: [
      {name: "apiKey", type: "secret", label: "API Key"},
      {name: "workspace", type: "select", options: [...]}
    ]
  }
}
```

**Examples**:
- Jira ticket capture (get assigned tickets daily)
- Twitter/X bookmark capture (save liked tweets)
- Email capture (forward to capture service)
- Calendar capture (record meetings)
- Slack message capture (react with emoji)

#### 2. Inference Rule Plugins
```typescript
interface InferenceRulePlugin {
  id: "inferenceRule:plugin_name",
  type: "inference_rule",
  
  // Define what this rule does
  metadata: {
    triggersOn: ["itemType:capture"],
    outputs: ["metadata:suggestedType", "metadata:suggestedCollection"],
    version: "1.0.0"
  },
  
  // Execution
  async infer(entity, context): Promise<{
    predictions: [{
      attribute: "metadata:suggestedType",
      value: "itemType:action",
      confidence: 0.92,
      reasoning: "Keywords: 'task', 'deadline'"
    }]
  }>,
  
  // Learning from corrections
  async recordFeedback(entity, correction, context): Promise<void>,
  
  // Performance monitoring
  getMetrics(): {
    totalInferences: number,
    accuracy: number,
    avgConfidence: number
  }
}
```

**Examples**:
- Legal document classifier (for law firms)
- Medical coding (for healthcare)
- Bug severity predictor (for dev teams)
- Sales opportunity scorer (for sales teams)
- Content categorizer (for publishers)

#### 3. Dashboard Widget Plugins
```typescript
interface DashboardWidgetPlugin {
  id: "dashboardWidget:plugin_name",
  type: "dashboard_widget",
  
  // Widget metadata
  metadata: {
    displayName: "Plugin Widget",
    description: "Does something useful",
    minHeight: 200,
    minWidth: 300
  },
  
  // Query data
  async fetchData(config, filters, dateRange): Promise<any>,
  
  // Render widget (React component)
  Component: React.FC<{data: any, config: any, onAction: (action) => void}>,
  
  // Actions widget can trigger
  actions: [
    {id: "viewDetails", label: "View Details"}
  ]
}
```

**Examples**:
- Pomodoro timer widget
- Team standout widget (summarize team's day)
- Goal progress widget
- Burnout risk widget (visual indicator)
- Integration status widget (Slack, Jira, etc.)

#### 4. Workflow Plugins
```typescript
interface WorkflowPlugin {
  id: "workflow:plugin_name",
  type: "workflow",
  
  // Define workflow steps
  steps: [
    {
      id: "step1",
      type: "plugin_action",
      action: "customLogic",
      config: {...}
    },
    {
      id: "step2",
      type: "entity_create",
      config: {entityTypeId: "itemType:action"}
    }
  ],
  
  // Execution
  async execute(input, context): Promise<{output: any}>,
  
  // Rollback capability
  async rollback(state): Promise<void>
}
```

**Examples**:
- Weekly standup flow (auto-create reflection)
- Sprint planning flow (organize items into sprint)
- Incident response flow (create tickets, notify team)
- Client onboarding flow (create collections, set permissions)

### Plugin Sandbox & Security

**Execution Model**:
- **Isolation**: Run in WebWorker (frontend) or separate process (.NET)
- **Permissions**: Explicit request (like mobile app permissions)
- **Rate limiting**: Per-plugin quotas
- **Audit**: All plugin actions logged
- **Rollback**: Transactions, can undo plugin changes

**User Controls**:
- ✅ Approve/deny plugin permissions
- ✅ Monitor plugin API usage
- ✅ Uninstall plugins
- ✅ Rate and review plugins
- ✅ Report malicious plugins

**Developer Controls**:
- ✅ Request specific permissions (min viable)
- ✅ Transparency (code review possible)
- ✅ Rate limiting (graceful degradation)
- ✅ Analytics (how many users, usage patterns)
- ✅ Versioning (update plugins safely)

### Plugin Marketplace Features

**Discovery**:
- ✅ Browse by category
- ✅ Search with filters
- ✅ Top rated/trending
- ✅ Recommended for user (based on plan + usage)
- ✅ Reviews and ratings
- ✅ Detailed documentation + examples

**Installation**:
- ✅ 1-click install
- ✅ Automatic updates (with user consent)
- ✅ Dependency management
- ✅ Conflict detection (two plugins doing same thing)
- ✅ Version pinning (lock to specific version)

**Management**:
- ✅ Enable/disable plugins
- ✅ Configure plugin settings
- ✅ View plugin usage statistics
- ✅ Uninstall with cleanup
- ✅ Report plugins

---

## Part 6: Community & Growth Strategy

### Developer Community

**Incentives for Plugin Development**:
1. **Revenue share**: 70% of paid plugin revenue
2. **Featured placement**: Top 10 plugins get promotional space
3. **Badges & recognition**: Top developers featured
4. **Early access**: Beta features available to plugin developers
5. **Grants program**: $1,000-10,000 grants for promising plugins

**Community Programs**:
- **Developer forums**: Share knowledge, feedback
- **Plugin hackathons**: Quarterly competitions ($10K prizes)
- **Case studies**: Feature success stories
- **Blog**: Plugin spotlights, best practices
- **Documentation**: Comprehensive plugin guide + examples

### User Community

**Incentives for Sharing**:
1. **Template sharing**: Share collection/workflow templates
2. **Plugin reviews**: Get credits for writing reviews
3. **Best practices**: Share tips, get recognized
4. **User groups**: Regional/topic-based communities
5. **Referral program**: $25 credit for each referred user

**Community Content**:
- **Template library**: 500+ templates curated by community
- **Workflow marketplace**: Share workflows
- **Plugin recommendations**: User ratings/reviews
- **Case studies**: How teams use the system
- **Blog**: Tips, tricks, success stories

### Enterprise Community

**For large organizations**:
- **Advisory board**: Input on roadmap
- **User conference**: Annual gathering
- **Partner program**: Reseller/integration partners
- **Custom solutions**: Build with the platform team
- **Thought leadership**: Speaking opportunities

---

## Part 7: Feature Roadmap (Post-MVP)

### Q1 2026 (MVP Launch)
- Core entity system
- Basic 7 pillars
- Web UI
- Free/Pro/Enterprise tiers
- Plugin system (beta)
- Basic marketplace

### Q2 2026 (Community)
- Plugin marketplace (launch)
- Community templates
- Developer grant program
- Improved API
- Advanced analytics

### Q3 2026 (Intelligence)
- Advanced ML models
- Model marketplace
- Federated learning
- Explainability (why predictions)
- A/B testing framework

### Q4 2026 (Enterprise)
- Advanced workflows
- Compliance (HIPAA, SOC2)
- Single sign-on
- Team analytics
- Custom integrations

### 2027+ (Specialized Solutions)
- Vertical-specific offerings (healthcare, legal, finance)
- AI agents (autonomous systems)
- Voice/conversational interface
- Mobile-first redesign
- International expansion

---

## Part 8: Data Privacy & Multi-Tenancy

### Tenant Isolation

**Database Level**:
- All queries include `tenantId` filter
- Indexes on tenantId
- Separate backups per tenant (enterprise)
- Encryption at rest per tenant

**Application Level**:
- Tenant context from auth token
- Permission checks: user must belong to tenant
- Audit logging: all access includes tenantId

**Network Level**:
- API endpoints scoped to tenant
- No cross-tenant visibility (except team members)
- Webhooks scoped to tenant

### User Privacy

**Data Minimization**:
- Only collect necessary data
- Inference not shared by default
- User can opt-in to community learning

**Data Ownership**:
- Users own their entities
- Users can export data (JSON)
- Users can delete all data (including models)
- GDPR/CCPA compliance

**Transparency**:
- Clear privacy policy
- Data usage notifications
- Opt-in for analytics
- Breach notification plan

---

## Part 9: Comparison: PowerShell MVP vs. SaaS

| Aspect | PowerShell MVP | SaaS |
|--------|---|---|
| **Users** | Single user | Multi-tenant, unlimited users |
| **Capture** | Terminal CLI | Web, mobile, integrations |
| **Storage** | Local JSON files | MongoDB, distributed |
| **Inference** | Local rules, learning indexes | Hosted ML models, GPU training |
| **Review** | Personal queue | Team collaboration, workflows |
| **Collaboration** | Manual sharing | Built-in permissions, sharing |
| **Extensibility** | Hardcoded types | Plugin marketplace |
| **Analytics** | Manual queries | Real-time dashboards, ML insights |
| **Cost** | Free (self-hosted) | Freemium + paid tiers |
| **Maintenance** | User responsible | Platform responsible |
| **Scale** | Laptop/PC | Cloud (AWS, Azure, GCP) |

**Key Insight**: The unified entity architecture makes both possible. The PowerShell MVP proves the concept; the SaaS scales it globally with plugins and community.

---

## Part 10: Implementation Roadmap (Node/React/.NET)

### Phase 1: Core (8 weeks)
- [ ] Entity CRUD API (.NET)
- [ ] Multi-tenancy database design (MongoDB)
- [ ] Auth + permissions (.NET)
- [ ] Basic React UI (dashboard, entity viewer)
- [ ] Simple entity types (collection, item, capture)

### Phase 2: Pillars (12 weeks)
- [ ] Capture sources (web, mobile)
- [ ] Inference engine (.NET)
- [ ] Review queue UI
- [ ] Dashboard builder
- [ ] Reflection forms

### Phase 3: Intelligence (10 weeks)
- [ ] ML model training (.NET)
- [ ] GPU integration (modal, AWS SageMaker)
- [ ] Model versioning & A/B testing
- [ ] Inference caching

### Phase 4: Extensibility (8 weeks)
- [ ] Plugin sandbox (.NET)
- [ ] Plugin API
- [ ] Marketplace UI
- [ ] Developer tools

### Phase 5: Launch (4 weeks)
- [ ] Load testing & scaling
- [ ] Documentation
- [ ] Public launch
- [ ] Marketing

**Total**: 42 weeks (~10 months) to MVP+ with full plugin system

---

## Conclusion

The unified entity architecture is the key innovation that enables:

1. **Simplicity**: One data model, one processor
2. **Extensibility**: New types without code changes
3. **Multi-tenancy**: Built in from the start
4. **Monetization**: Flexible tier + plugin revenue
5. **Community**: Plugin marketplace creates ecosystem
6. **Growth**: Each pillar can evolve independently

This same architecture works for:
- ✅ PowerShell MVP (single-user, local)
- ✅ Node/React/.NET SaaS (multi-user, cloud)
- ✅ Mobile app (native interfaces, same backend)
- ✅ CLI tools (same API)
- ✅ Integrations (webhooks, plugins)

**The platform is the entity model. Everything else is UI.**

---

## Next Steps for Your Implementation

### For PowerShell MVP:
1. Complete Phase 1-3 of refactor plan (entity processor, item types, unified storage)
2. Add reflection pillar as proof-of-concept
3. Build plugin concept for inference rules

### For Node/React/.NET SaaS:
1. Start with Part 1-3 of this doc (foundational architecture, multi-tenancy, database)
2. Implement Phase 1 (core API, auth, entities)
3. Build Phase 2 pillars incrementally (capture, review, dashboard)
4. Add plugin system (Phase 4) before launch

Both systems can share:
- Entity type definitions (JSON format)
- Attribute kind specifications
- Inference rule definitions
- Plugin interfaces

This doc can serve as both a selling document (for potential users/investors) and an implementation guide (for development).

---

## Current Implementation Status (Divergent Flow MVP)

**Last Updated**: 2026-01-15

### What's Already Built

The current Divergent Flow MVP has laid the foundation for this unified entity system:

#### ✅ **Core Entity Model** (Partial)
- **Item entity**: The base entity type exists (`type: "capture"`)
- **MongoDB persistence**: System of record in place
- **Relationships**: Basic parent-child via `collectionId`
- **Type inference**: Pattern-based inference with confidence scoring
- **Background processing**: Async inference queue

#### ✅ **Clean Architecture** (Foundation)
- Layered architecture (API → Application → Domain → Infrastructure)
- MediatR command/query separation
- Repository pattern
- Dependency injection throughout

#### ✅ **ADHD-Optimized UX** (Pillar 1)
- Quick capture UI (web)
- Low-friction input
- Calm, focused design
- Framer Motion animations (200-300ms)

#### 🚧 **Review Queue** (Pillar 4 - In Progress)
- Priority-based queue
- 3 items per session
- Property validation (pending)

### What's Coming Next

Based on [ARCHITECTURE.md](./ARCHITECTURE.md) migration roadmap:

#### 📅 **Phase 3.5: Entity Abstraction** (8-12 weeks)
- Dynamic entity types (not hardcoded)
- EntityType definitions
- Version tracking
- Multi-tenant preparation

#### 📅 **Phase 4: Plugin System** (12-16 weeks)
- Plugin-based inference (this doc's Pillar 3)
- Plugin registry and marketplace
- Sandbox execution
- Plugin SDK

#### 📅 **Phase 5: Multi-Tenancy** (8-12 weeks)
- Tenant isolation (this doc's Part 3)
- Subscription tiers (this doc's Part 4)
- Team workspaces

#### 📅 **Phase 6: Collaboration** (12-16 weeks)
- Shared collections (this doc's Pillar 2)
- Team review workflows (this doc's Pillar 4)
- Permissions and visibility

#### 📅 **Phase 7: ML Training** (16-20 weeks)
- Custom model training (this doc's Pillar 5)
- Model marketplace
- GPU-backed inference

#### 📅 **Phase 8: Full Platform** (Ongoing)
- File storage (this doc's Part 2.5)
- Advanced dashboards (this doc's Pillar 6)
- Reflection analytics (this doc's Pillar 7)
- All remaining features from this document

### How MVP Maps to Unified System

| Unified System Concept | MVP Implementation |
|------------------------|-------------------|
| `Entity` base model | `Item` class (partial) |
| `entityTypeId` | `type` field (hardcoded values) |
| `tenantId` | `userId` field (single-user mode) |
| Dynamic attributes | Fixed C# properties |
| Relationship graph | `collectionId` (parent only) |
| Version history | Not yet implemented |
| Plugin system | Hardcoded inference rules |
| Multi-tenancy | Single tenant implied |
| Permissions | Not yet implemented |

### Key Architectural Alignment

The MVP was intentionally designed with this unified system in mind:

✅ **Entity-centric**: `Item` is already the foundational unit
✅ **Clean boundaries**: Domain/Application/Infrastructure layers match this doc's design
✅ **MongoDB**: Document model ready for flexible entity schemas
✅ **Background processing**: Async inference queue ready for plugin system
✅ **REST API**: Clear HTTP endpoints ready for `/api/v1/entities` evolution

### Migration Philosophy

**Incremental, not rewrite**. The MVP isn't being thrown away—it's the first iteration of this unified system. Each phase adds capabilities while maintaining backward compatibility.

**No disruption to users**. The migration happens behind the scenes. Users keep using Divergent Flow without interruption while we progressively unlock the full unified entity system.

**See [ARCHITECTURE.md](./ARCHITECTURE.md)** for the complete migration roadmap, architecture decision records, and current vs. target comparisons.

---

**🚀 Ready to contribute?** See [../CONTRIBUTING.md](../CONTRIBUTING.md) for guidelines.