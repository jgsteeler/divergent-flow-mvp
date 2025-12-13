# Divergent Flow - Spark to AI Agent Transition Guide

## Executive Summary

Based on analysis of your current progress and PRD, **NOW is the ideal time to transition from Spark to an AI coding agent**.

### Measurable Criteria Met ✅
1. ✅ **Foundation Complete**: Phase 1-2 are functional (captures work, inference runs, data persists)
2. ✅ **Clear Requirements**: Your PRD is exceptionally detailed with specific acceptance criteria
3. ✅ **Complexity Inflection**: Phase 3+ requires logic beyond UI prototyping (LLM APIs, backend separation)
4. ✅ **Pattern Recognition**: 2+ commits show consistent patterns AI agents can learn from
5. ✅ **Architecture Evolution**: You're ready to start thinking about backend separation
6. ✅ **Learning Data Accumulation**: The system is generating data patterns that need more intelligence

**Quantitative Indicator**: When UI prototyping represents <30% of remaining work, switch to AI agents. Your project: ~20% UI, 80% logic/integration remaining.

---

## Current State Analysis

### What Spark Built Successfully ✨
- **Phase 1**: Quick capture interface with persistent storage (KV store)
- **Phase 2**: Basic type inference engine with pattern matching
- **Phase 3 (Partial)**: Review queue with priority algorithm
- **UI Components**: Beautiful Radix UI components with thoughtful styling
- **Type System**: Well-defined TypeScript interfaces

### What's Missing for MVP Completion 🎯
1. **Phase 3 Completion**: Property validation for items
2. **Phase 4**: LLM-powered inference for collections/priority/context
3. **Natural Language Date/Time Parsing**: Handle 20+ date patterns
4. **Edge Cases**: Duplicate detection, offline queuing, error handling
5. **Dashboard Features**: Next Action, Quick Wins, completion tracking
6. **Testing**: No test infrastructure exists yet
7. **Performance**: May need optimization as data grows

---

## Transition Timing Recommendation

### 🟢 STOP USING SPARK NOW - Here's Why:

#### 1. **LLM Integration Required** (Phase 4)
Spark is optimized for UI prototyping, but Phase 4 requires:
- LLM API integration (OpenAI, Anthropic, etc.)
- Prompt engineering and testing
- Response parsing and validation
- Error handling and retry logic
- Cost optimization

**AI Agent Advantage**: Better at implementing API integrations, error handling, and complex async patterns.

#### 2. **Backend Architecture Needed**
Your current KV storage won't scale for:
- Multi-device sync
- User authentication
- Data analytics
- LLM API key management
- Advanced queries

**AI Agent Advantage**: Can architect proper backend separation and set up .NET infrastructure.

#### 3. **Complex State Management**
As features grow, you'll need:
- React Query for server state
- Optimistic updates
- Conflict resolution
- Migration strategies

**AI Agent Advantage**: Experienced with state management patterns and can implement them consistently.

#### 4. **Testing & Quality**
MVP needs:
- Unit tests for inference logic
- Integration tests for flows
- E2E tests for critical paths

**AI Agent Advantage**: Can set up proper testing infrastructure and write comprehensive tests.

---

## AI Coding Agent Setup Instructions

### Option 1: GitHub Copilot Workspace (Recommended for your setup)

Since you're already in a GitHub repository, this is the most seamless option.

#### Setup Steps:

1. **Enable GitHub Copilot for your account**
   - Go to github.com/settings/copilot
   - Subscribe if not already (free for students/open source maintainers)

2. **Enable Copilot Workspace (Beta)**
   - Visit github.com/features/preview/copilot-workspace
   - Request access if not available
   - Once enabled, you can use natural language to describe tasks

3. **Create Task Templates**
   Create a `.github/copilot-instructions.md` file:
   ```markdown
   # Divergent Flow Development Guidelines
   
   ## Project Context
   This is an ADHD-friendly brain management tool. See PRD.md for full requirements.
   
   ## Code Style
   - TypeScript strict mode
   - React 19 with hooks
   - Tailwind CSS with OKLCH colors
   - Radix UI components
   - Framer Motion for animations
   
   ## Testing Requirements
   - Write tests for all new logic
   - Use Vitest for unit tests
   - Use Playwright for E2E tests
   
   ## Architecture Principles
   - Keep components small and focused
   - Separate business logic from UI
   - Use React Query for server state
   - Optimize for ADHD users (low friction, calm UI)
   ```

4. **Using Copilot to Complete MVP**
   ```
   Example prompts:
   
   - "Implement Phase 4 LLM-powered inference using OpenAI API. 
     Extract collections, priority, and context from item text. 
     Follow the patterns in src/lib/typeInference.ts"
   
   - "Add natural language date parsing supporting all patterns 
     listed in PRD.md section 'Date Parsing Failures'"
   
   - "Set up Vitest and write unit tests for typeInference.ts 
     and reviewPriority.ts"
   
   - "Implement duplicate detection that flags similar recent captures"
   
   - "Create Next Action and Quick Wins dashboard components 
     following the design in PRD.md"
   ```

### Option 2: Cursor IDE

Cursor is built specifically for AI-assisted development.

#### Setup Steps:

1. **Install Cursor**
   - Download from cursor.sh
   - Import your VS Code settings

2. **Clone Your Repo**
   ```bash
   git clone https://github.com/jgsteeler/divergent-flow-mvp.git
   cd divergent-flow-mvp
   ```

3. **Configure Cursor for Your Project**
   - Open `.cursorrules` or create it:
   ```
   # Divergent Flow Project Rules
   
   This is an ADHD productivity tool MVP. Reference PRD.md for all requirements.
   
   Tech Stack:
   - React 19 + TypeScript
   - Vite build tool
   - Radix UI + Tailwind CSS
   - Framer Motion animations
   - Currently using Spark KV store (will migrate to backend)
   
   Key Principles:
   - Low friction: minimize clicks and decision points
   - Calm UI: generous whitespace, soothing colors
   - Smart defaults: infer user intent, don't ask unless needed
   - ADHD-optimized: focus on capture speed and review simplicity
   ```

4. **Use Cursor's AI Features**
   - `Cmd+K`: Ask to generate/edit code
   - `Cmd+L`: Chat about the codebase
   - Use Composer for multi-file changes

### Option 3: Aider (CLI-based)

Best for command-line lovers and precise control.

#### Setup Steps:

1. **Install Aider**
   ```bash
   pip install aider-chat
   ```

2. **Set Up API Key**
   ```bash
   export OPENAI_API_KEY=your_key_here
   # or use Anthropic
   export ANTHROPIC_API_KEY=your_key_here
   ```

3. **Start Aider in Your Repo**
   ```bash
   cd divergent-flow-mvp
   aider --model gpt-4-turbo
   ```

4. **Example Workflow**
   ```bash
   # Add relevant files to context
   /add src/lib/typeInference.ts
   /add PRD.md
   
   # Give instructions
   Implement Phase 4 LLM-powered inference. Read the PRD.md 
   requirements and create a new inferenceV2.ts that uses OpenAI 
   to extract collections, priority, and context from item text.
   
   # Review and accept changes
   /accept
   ```

---

## MVP Completion Roadmap with AI Agent

### Phase 3: Complete Review Queue (1-2 weeks)
**What to delegate to AI:**
```
✓ Property validation for actions (priority check)
✓ Property validation for reminders (due date check)
✓ Enhanced review queue to show missing properties
✓ Unit tests for reviewPriority.ts
✓ Integration tests for review flow
```

**Prompts to use:**
- "Implement property validation for items based on their type per PRD.md Phase 3"
- "Add visual indicators in ReviewQueue.tsx for items missing required properties"
- "Write Vitest unit tests for reviewPriority.ts covering all priority scenarios"

### Phase 4: Intelligent Processing (2-3 weeks)
**What to delegate to AI:**
```
✓ OpenAI/Anthropic API integration
✓ Prompt engineering for collection inference
✓ Prompt engineering for priority inference
✓ Context and tag extraction
✓ Natural language date/time parser (20+ patterns)
✓ Error handling and retry logic
✓ Cost tracking and optimization
```

**Prompts to use:**
- "Set up OpenAI API integration with proper error handling and retry logic"
- "Create LLM prompts to infer collections from item text based on user patterns"
- "Implement comprehensive date parser handling all patterns in PRD.md"
- "Add usage tracking for LLM calls and display cost estimates to user"

### Phase 5: ADHD Dashboard (1-2 weeks)
**What to delegate to AI:**
```
✓ Next Action algorithm and component
✓ Quick Wins detection and display
✓ Adaptive algorithms that learn patterns
✓ Dashboard layout with animations
✓ Responsive mobile design
```

**Prompts to use:**
- "Create Next Action algorithm that selects single best focus item for user"
- "Implement Quick Wins component showing easy completions (< 5 min actions)"
- "Build adaptive dashboard that learns user work patterns over time"

### Phase 6: Item Completion (1 week)
**What to delegate to AI:**
```
✓ Mark items complete functionality
✓ Satisfying completion animations
✓ Completion history storage
✓ Learning from completion patterns
```

**Prompts to use:**
- "Add item completion with satisfying animations per PRD.md design guidelines"
- "Track completion patterns to improve future inference accuracy"

### Testing & Polish (1-2 weeks)
**What to delegate to AI:**
```
✓ E2E test suite with Playwright
✓ Accessibility audit and fixes
✓ Performance optimization
✓ Error boundary improvements
✓ Loading state refinements
```

**Prompts to use:**
- "Set up Playwright E2E tests covering critical user flows"
- "Audit app for WCAG AA accessibility and fix issues"
- "Optimize performance for 1000+ items in storage"

---

## Scaling to Frontend + .NET Backend

### Architecture Overview

```
┌─────────────────────────────────────────────────────┐
│                 CURRENT (MVP)                        │
│  ┌────────────────────────────────────────────┐    │
│  │  React App (Vite)                          │    │
│  │  • UI Components                            │    │
│  │  • Client-side inference                    │    │
│  │  • Spark KV Storage (browser)              │    │
│  └────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────┘

                         ↓

┌─────────────────────────────────────────────────────┐
│              SCALED ARCHITECTURE                     │
│                                                      │
│  ┌────────────────────────────────────────────┐    │
│  │  React Frontend (Vite/Next.js)            │    │
│  │  • UI Components (reuse existing)          │    │
│  │  • React Query for API calls               │    │
│  │  • Offline-first with sync                 │    │
│  └─────────────────┬──────────────────────────┘    │
│                    │                                 │
│              HTTPS REST/GraphQL                     │
│                    │                                 │
│  ┌─────────────────▼──────────────────────────┐    │
│  │  .NET Backend (ASP.NET Core)              │    │
│  │  • RESTful API / GraphQL                   │    │
│  │  • Authentication (Identity)               │    │
│  │  • LLM Integration Service                 │    │
│  │  • Background Jobs (Hangfire)              │    │
│  │  • Real-time (SignalR)                     │    │
│  └─────────────────┬──────────────────────────┘    │
│                    │                                 │
│  ┌─────────────────▼──────────────────────────┐    │
│  │  PostgreSQL Database                       │    │
│  │  • User data                                │    │
│  │  • Items & learning patterns               │    │
│  │  • Audit logs                               │    │
│  └────────────────────────────────────────────┘    │
│                                                      │
│  ┌────────────────────────────────────────────┐    │
│  │  Redis Cache                               │    │
│  │  • Session data                             │    │
│  │  • Rate limiting                            │    │
│  │  • LLM response cache                       │    │
│  └────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────┘
```

### Step-by-Step Migration Plan

#### Step 1: Set Up .NET Backend (Week 1-2)

**Project Structure:**
```
DivergentFlow.Backend/
├── DivergentFlow.API/              # ASP.NET Core Web API
│   ├── Controllers/
│   │   ├── ItemsController.cs
│   │   ├── InferenceController.cs
│   │   └── UsersController.cs
│   ├── Program.cs
│   └── appsettings.json
├── DivergentFlow.Core/             # Business Logic
│   ├── Models/
│   │   ├── Item.cs
│   │   ├── TypeLearningData.cs
│   │   └── User.cs
│   ├── Services/
│   │   ├── ITypeInferenceService.cs
│   │   ├── TypeInferenceService.cs
│   │   ├── ILLMService.cs
│   │   └── LLMService.cs
│   └── Interfaces/
├── DivergentFlow.Infrastructure/   # Data Access
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   └── Repositories/
│   └── Migrations/
└── DivergentFlow.Tests/            # Unit & Integration Tests
```

**AI Agent Prompts:**
```
1. "Create new ASP.NET Core 8 Web API project named DivergentFlow.API 
   with clean architecture (API, Core, Infrastructure layers)"

2. "Set up Entity Framework Core with PostgreSQL and create models 
   for Item, User, and TypeLearningData based on TypeScript types 
   in src/lib/types.ts"

3. "Implement ItemsController with CRUD endpoints following RESTful 
   best practices and include Swagger documentation"

4. "Create TypeInferenceService that ports the logic from 
   src/lib/typeInference.ts to C#"

5. "Set up ASP.NET Core Identity for authentication with JWT tokens"
```

#### Step 2: API Design & Implementation (Week 2-3)

**Core Endpoints:**

```csharp
// Items API
GET    /api/items                    // List all items (paginated)
GET    /api/items/{id}               // Get single item
POST   /api/items                    // Create item
PUT    /api/items/{id}               // Update item
DELETE /api/items/{id}               // Delete item
POST   /api/items/{id}/confirm-type  // Confirm item type

// Inference API
POST   /api/inference/type           // Infer item type
POST   /api/inference/collection     // Infer collection (Phase 4)
POST   /api/inference/priority       // Infer priority (Phase 4)
POST   /api/inference/date           // Parse natural language date

// Review API
GET    /api/review/queue             // Get review queue items
POST   /api/review/{id}/complete     // Mark review complete

// User API
POST   /api/auth/register            // Create account
POST   /api/auth/login               // Login
GET    /api/user/settings            // Get user settings
PUT    /api/user/settings            // Update settings
```

**AI Agent Prompts:**
```
1. "Implement all Items API endpoints with proper validation, 
   error handling, and authorization"

2. "Create InferenceController that wraps TypeInferenceService 
   and handles rate limiting for LLM calls"

3. "Add background job using Hangfire to process items async 
   after capture without blocking API response"

4. "Implement comprehensive API error handling with proper 
   HTTP status codes and problem details"
```

#### Step 3: Database Schema (Week 3)

**AI Agent Prompts:**
```
1. "Create EF Core migrations for Users, Items, TypeLearning tables 
   with proper indexes and relationships"

2. "Add audit fields (CreatedAt, UpdatedAt, CreatedBy) to all entities"

3. "Implement soft delete pattern for Items (IsDeleted flag)"

4. "Create database seed data for testing with sample items"
```

**Schema Example:**
```sql
-- Users Table
CREATE TABLE Users (
    Id UUID PRIMARY KEY,
    Email VARCHAR(255) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    CreatedAt TIMESTAMP NOT NULL,
    UpdatedAt TIMESTAMP
);

-- Items Table
CREATE TABLE Items (
    Id UUID PRIMARY KEY,
    UserId UUID NOT NULL REFERENCES Users(Id),
    Text TEXT NOT NULL,
    InferredType VARCHAR(50),
    TypeConfidence INT,
    ConfidenceReasoning TEXT,
    LastReviewedAt TIMESTAMP,
    CreatedAt TIMESTAMP NOT NULL,
    UpdatedAt TIMESTAMP,
    IsDeleted BOOLEAN DEFAULT FALSE,
    
    INDEX idx_user_created (UserId, CreatedAt),
    INDEX idx_type_confidence (InferredType, TypeConfidence)
);

-- TypeLearning Table
CREATE TABLE TypeLearning (
    Id UUID PRIMARY KEY,
    UserId UUID NOT NULL REFERENCES Users(Id),
    Pattern TEXT NOT NULL,
    Type VARCHAR(50) NOT NULL,
    Confidence INT NOT NULL,
    WasCorrect BOOLEAN,
    CreatedAt TIMESTAMP NOT NULL,
    
    INDEX idx_user_pattern (UserId, Pattern)
);
```

#### Step 4: Frontend Migration (Week 4-5)

**Option A: Keep Vite React (Simpler migration)**
```
1. Replace Spark KV hooks with React Query + API calls
2. Add authentication flow (login/register)
3. Implement offline sync with service worker
4. Keep existing components (they're great!)
```

**Option B: Migrate to Next.js (Better for SEO/performance)**
```
1. Create Next.js 14 app with App Router
2. Port React components from Vite (mostly copy-paste)
3. Use Server Components for initial loads
4. Add SSR for faster first paint
```

**AI Agent Prompts for Option A (Recommended):**
```
1. "Replace useKV hook with React Query hooks that call API endpoints. 
   Create custom hooks like useItems, useTypeInference"

2. "Implement JWT authentication with token refresh. Store tokens 
   securely and add auth interceptor to API calls"

3. "Add optimistic updates for item creation and type confirmation 
   using React Query mutations"

4. "Implement offline support with service worker. Queue mutations 
   when offline and sync when back online"

5. "Set up environment-based API URLs (dev: localhost:5000, 
   prod: api.divergentflow.com)"
```

**Example Hook Migration:**
```typescript
// BEFORE (Spark):
const [items, setItems] = useKV<Item[]>('items', [])

// AFTER (React Query):
const { data: items, isLoading } = useItems()
const createItemMutation = useCreateItem()

// Usage:
createItemMutation.mutate({ text: 'my item' })
```

#### Step 5: Deployment (Week 5-6)

**Frontend Deployment (Vercel):**
```
1. Connect GitHub repo to Vercel
2. Set environment variables (VITE_API_URL)
3. Configure build settings
4. Deploy on every push to main
```

**Backend Deployment (Azure):**
```
1. Create Azure App Service (Linux, .NET 8)
2. Create Azure Database for PostgreSQL
3. Set up Azure Redis Cache
4. Configure CI/CD with GitHub Actions
5. Add App Insights for monitoring
```

**AI Agent Prompts:**
```
1. "Create Dockerfile for .NET backend with multi-stage build"

2. "Create docker-compose.yml for local development with 
   PostgreSQL and Redis"

3. "Create GitHub Actions workflow for backend CI/CD to Azure"

4. "Create GitHub Actions workflow for frontend deployment to Vercel"

5. "Add health check endpoints and configure Azure monitoring"
```

### Key Technologies for Scaled Version

**Frontend:**
- React 19 (keep current)
- React Query v5 (server state)
- React Router v7 (navigation)
- Zustand (client state if needed)
- Service Worker (offline)

**Backend:**
- ASP.NET Core 8 (Web API)
- Entity Framework Core 8 (ORM)
- PostgreSQL 16 (database)
- Redis (caching)
- Hangfire (background jobs)
- SignalR (real-time updates)
- Serilog (logging)

**DevOps:**
- Docker (containerization)
- GitHub Actions (CI/CD)
- Azure App Service (backend hosting)
- Vercel (frontend hosting)
- Azure Database for PostgreSQL
- Azure Application Insights (monitoring)

---

## Cost Estimates

### MVP Phase (Current - 3 months)
- **GitHub Copilot**: $10-20/month (free for students)
- **LLM API calls**: $20-50/month for development
  - Assumes: 50-100 API calls/day for testing
  - GPT-4 Turbo: ~$0.01 per call (1K input tokens)
  - Monthly: 3,000 calls × $0.01 = $30
- **Total**: ~$30-70/month

### Scaled Production (6+ months)
- **Frontend Hosting (Vercel)**: $20/month (Pro plan)
- **Backend Hosting (Azure)**: $100-200/month (App Service + DB)
- **Redis Cache**: $20-50/month
- **LLM API calls**: $50-500/month (depends on users)
  - 100 users × 20 inferences/day × $0.005 = $100/month
  - Caching can reduce costs by 50-70%
- **Monitoring**: $10-30/month
- **Total**: ~$200-800/month for 100-1000 users

**Note**: AI coding agent API usage can be higher during intensive development sessions (200-500 calls/day when actively coding). Budget $50-100/month for heavy development periods.

---

## Risk Mitigation

### Data Migration Risk
**Problem**: Users will have data in Spark KV store
**Solution**: 
1. Export feature before backend launch
2. Import endpoint on backend
3. Migration tool with clear instructions

**AI Agent Prompt:**
```
"Create data export feature that downloads all items and learning 
data as JSON. Then create backend import endpoint that accepts 
this JSON and creates user account with imported data."
```

### Offline Functionality Risk
**Problem**: Current app works offline, backend version might not
**Solution**:
1. Service worker with request queueing
2. IndexedDB for offline storage
3. Sync queue with conflict resolution

**AI Agent Prompt:**
```
"Implement offline-first architecture with service worker. Queue 
API calls when offline, sync when online, handle conflicts 
by showing user both versions to choose."
```

---

## Next Steps - Action Plan

### Immediate (This Week)
1. ✅ Read this guide thoroughly
2. ✅ Choose your AI agent tool (GitHub Copilot Workspace recommended)
3. ✅ Set up the tool and configure for your repo
4. ✅ Create `.github/copilot-instructions.md` with project context
5. ✅ Test with one small task: "Add unit tests for typeInference.ts"

### Short Term (Next 2-4 weeks)
1. Complete Phase 3 with AI agent assistance
2. Implement Phase 4 LLM integration
3. Add comprehensive testing
4. Polish UI and fix edge cases

### Medium Term (1-3 months)
1. Complete Phase 5 (Dashboard) and Phase 6 (Completion)
2. User testing with 10-20 beta users
3. Gather feedback and iterate
4. Prepare for backend migration

### Long Term (3-6 months)
1. Build .NET backend following architecture guide
2. Migrate frontend to use backend APIs
3. Deploy to production
4. Launch publicly

---

## Conclusion

You're at the perfect inflection point. Spark gave you a beautiful, functional MVP foundation. Now it's time to:

1. **Switch to AI coding agent** to handle the increasing complexity
2. **Complete MVP** (Phase 3-6) over next 2-3 months
3. **Scale to backend** when you have 50+ beta users or need multi-device sync

The PRD you've created is exceptional - clear, detailed, and implementable. Combined with an AI coding agent, you can move much faster than continuing with Spark alone.

**Recommended First AI Agent Task:**
```
"Based on PRD.md Phase 4, implement OpenAI integration for 
collection inference. Create new file src/lib/llmInference.ts 
that sends item text to GPT-4, extracts suggested collection 
based on learned patterns, and returns with confidence score. 
Include error handling and cost tracking."
```

Good luck! You're building something valuable for the ADHD community. 🚀
