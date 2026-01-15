# Unified Entity System

## Overview

The Unified Entity System is the foundation for Divergent Flow's evolution into a flexible, extensible SaaS platform. This document describes the implementation of the entity/attribute data model as outlined in [docs/unified.md](../../docs/unified.md).

## Status

🚧 **Under Development** - Currently behind feature flag `FeatureFlags:UnifiedEntitySystem` (default: `false`)

## Core Concepts

### Entity-Centric Everything

Everything in the system (items, collections, captures, dashboards, etc.) is fundamentally represented as an `Entity`. This provides:

1. **Single data model** across all functionality
2. **Extensibility** through entity types (no schema migrations needed)
3. **Multi-tenancy** by design (tenantId on every entity)
4. **Version history** built-in (audit trail)
5. **Permission model** integrated (visibility + ownership)

### Key Classes

#### Entity
The universal entity model. All entities have:
- `Id`: Unique identifier (format: `entity:{type}:{uniqueId}`)
- `EntityTypeId`: Reference to the entity type that defines the schema
- `TenantId`: Multi-tenant isolation (format: `tenant:{orgId}`)
- `OwnerId`: Owner user identifier (format: `user:{userId}`)
- `Attributes`: Flexible key-value properties (JSON)
- `Metadata`: Version tracking, timestamps, visibility
- `Relationships`: Links to other entities

```csharp
var entity = new Entity
{
    Id = "entity:capture:abc123",
    EntityTypeId = "entityType:itemType:capture",
    TenantId = "tenant:default",
    OwnerId = "user:test",
    Metadata = new EntityMetadata
    {
        CreatedDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
        UpdatedDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
        Visibility = EntityVisibility.Private
    },
    Attributes = new Dictionary<string, JsonElement>
    {
        ["content"] = JsonSerializer.SerializeToElement("Buy groceries"),
        ["metadata:priority"] = JsonSerializer.SerializeToElement("high")
    }
};
```

#### EntityType
Defines a type of entity with its schema and behavior. Entity types act as templates:
- `Id`: Unique identifier (format: `entityType:{typename}`)
- `Name`: Display name
- `TenantId`: System types use `tenant:system`, custom types use organization's tenant ID
- `ParentTypeId`: Optional parent for inheritance
- `Attributes`: List of attribute definitions (schema)

```csharp
var entityType = new EntityType
{
    Id = "entityType:itemType:action",
    Name = "Action",
    TenantId = "tenant:system",
    Attributes = new List<AttributeDefinition>
    {
        new AttributeDefinition
        {
            Name = "metadata:title",
            Kind = "string",
            Required = true,
            Description = "Title of the action item"
        },
        new AttributeDefinition
        {
            Name = "metadata:dueDate",
            Kind = "timestamp",
            Required = false,
            Description = "When the action is due"
        }
    }
};
```

#### AttributeDefinition
Defines a single attribute for an entity type:
- `Name`: Attribute name (e.g., `metadata:title`, `content`)
- `Kind`: Data type (e.g., `string`, `number`, `timestamp`, `boolean`)
- `Required`: Whether the attribute must be present
- `Calculated`: Whether the attribute is computed from other attributes
- `ValidationRules`: JSON object with validation rules

#### EntityMetadata
Metadata for version tracking and permissions:
- `CreatedDate`: When entity was created (Unix timestamp)
- `UpdatedDate`: When entity was last updated (Unix timestamp)
- `Version`: Version number for optimistic concurrency
- `Visibility`: Privacy level (`Private`, `Team`, `Organization`)

#### EntityRelationships
Relationships between entities:
- `ParentIds`: List of parent entity IDs (hierarchical)
- `ReferencedByIds`: List of entities that reference this one
- `LinkedEntities`: Named relationships (e.g., `assignedTo`, `dependsOn`)

## Feature Flag

The unified entity system is behind a feature flag to maintain production stability while development continues.

### Configuration

In `appsettings.json`:
```json
{
  "FeatureFlags": {
    "UnifiedEntitySystem": false
  }
}
```

### Checking the Flag

```csharp
public class MyService
{
    private readonly IFeatureFlags _featureFlags;

    public MyService(IFeatureFlags featureFlags)
    {
        _featureFlags = featureFlags;
    }

    public async Task DoSomething()
    {
        if (_featureFlags.IsUnifiedEntitySystemEnabled)
        {
            // Use unified entity system
            await UseNewEntitySystem();
        }
        else
        {
            // Use existing Item/Collection/Capture entities
            await UseLegacySystem();
        }
    }
}
```

## Repository Interfaces

### IEntityRepository
CRUD operations for entities:
- `CreateAsync(Entity)`: Create a new entity
- `GetByIdAsync(string)`: Get entity by ID
- `GetByTenantAsync(string)`: Get all entities for a tenant
- `GetByEntityTypeAsync(string, string)`: Get entities of a specific type
- `GetByOwnerAsync(string, string)`: Get entities owned by a user
- `UpdateAsync(Entity)`: Update an entity
- `DeleteAsync(string)`: Delete an entity

### IEntityTypeRepository
CRUD operations for entity types:
- `CreateAsync(EntityType)`: Create a new entity type
- `GetByIdAsync(string)`: Get entity type by ID
- `GetAllAsync(string)`: Get all entity types for a tenant (including system types)
- `GetActiveAsync(string)`: Get active entity types
- `UpdateAsync(EntityType)`: Update an entity type
- `DeleteAsync(string)`: Delete an entity type (only if no entities of this type exist)

## ID Conventions

All IDs follow consistent naming conventions:

| Type | Format | Example |
|------|--------|---------|
| Entity | `entity:{type}:{uniqueId}` | `entity:capture:abc123` |
| Entity Type | `entityType:{typename}` | `entityType:itemType:capture` |
| Tenant | `tenant:{orgId}` | `tenant:acme` or `tenant:default` |
| User | `user:{userId}` | `user:john` |

## Backward Compatibility

The existing `Item`, `Collection`, and `Capture` entities continue to work unchanged. The unified entity system is additive, not a replacement (yet).

In the future, these legacy entities will be migrated to the unified system:
- `Item` → `Entity` with `entityTypeId = "entityType:itemType:*"`
- `Collection` → `Entity` with `entityTypeId = "entityType:collectionType:*"`
- `Capture` → `Entity` with `entityTypeId = "entityType:itemType:capture"`

## Testing

Comprehensive unit tests are in `DivergentFlow.Application.Tests`:
- `UnifiedEntitySystemTests.cs`: Tests for entity models
- `FeatureFlagsTests.cs`: Tests for feature flag service

Run tests:
```bash
cd backend
dotnet test --filter "FullyQualifiedName~UnifiedEntitySystemTests"
dotnet test --filter "FullyQualifiedName~FeatureFlagsTests"
```

## Next Steps

### Phase 1: Repository Implementation (Current)
- [ ] Implement `MongoEntityRepository`
- [ ] Implement `MongoEntityTypeRepository`
- [ ] Add MongoDB collection setup
- [ ] Add indexes for performance
- [ ] Integration tests

### Phase 2: Seed Data
- [ ] Create system entity types (capture, action, note, etc.)
- [ ] Migration script to populate entity types
- [ ] Documentation for adding custom entity types

### Phase 3: API Endpoints
- [ ] `POST /api/v1/entities` - Create entity
- [ ] `GET /api/v1/entities/{id}` - Get entity
- [ ] `PUT /api/v1/entities/{id}` - Update entity
- [ ] `DELETE /api/v1/entities/{id}` - Delete entity
- [ ] `GET /api/v1/entity-types` - List entity types

### Phase 4: Migration Path
- [ ] Dual-write pattern (write to both old and new systems)
- [ ] Background migration job
- [ ] Validation that migrated data is correct
- [ ] Cutover plan

### Phase 5: Plugin System
- [ ] Plugin architecture based on entity types
- [ ] Plugin marketplace foundation
- [ ] Community plugin support

## References

- [docs/unified.md](../../docs/unified.md) - Complete unified entity system specification
- [docs/ARCHITECTURE.md](../../docs/ARCHITECTURE.md) - Architecture overview and migration roadmap
- [CONTRIBUTING.md](../../CONTRIBUTING.md) - Contribution guidelines

## Questions?

For questions or discussions about the unified entity system, please:
1. Check the [unified.md](../../docs/unified.md) document
2. Review existing issues tagged with `unified-entity-system`
3. Open a new issue or discussion on GitHub
