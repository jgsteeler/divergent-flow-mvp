using DivergentFlow.Domain.Entities;
using System.Text.Json;
using Xunit;

namespace DivergentFlow.Application.Tests;

/// <summary>
/// Tests for the unified entity system domain models.
/// Based on the unified entity architecture described in docs/unified.md
/// </summary>
public sealed class UnifiedEntitySystemTests
{
    [Fact]
    public void Entity_CanBeCreated_WithRequiredProperties()
    {
        // Arrange & Act
        var entity = new Entity
        {
            Id = "entity:capture:abc123",
            EntityTypeId = "entityType:itemType:capture",
            TenantId = "tenant:default",
            OwnerId = "user:test",
            Metadata = new EntityMetadata
            {
                CreatedDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                UpdatedDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            }
        };

        // Assert
        Assert.NotNull(entity);
        Assert.Equal("entity:capture:abc123", entity.Id);
        Assert.Equal("entityType:itemType:capture", entity.EntityTypeId);
        Assert.Equal("tenant:default", entity.TenantId);
        Assert.Equal("user:test", entity.OwnerId);
        Assert.NotNull(entity.Metadata);
        Assert.NotNull(entity.Attributes);
        Assert.NotNull(entity.Relationships);
    }

    [Fact]
    public void Entity_CanStore_FlexibleAttributes()
    {
        // Arrange
        var entity = new Entity
        {
            Id = "entity:capture:test",
            EntityTypeId = "entityType:itemType:capture",
            TenantId = "tenant:default",
            OwnerId = "user:test",
            Metadata = new EntityMetadata
            {
                CreatedDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                UpdatedDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            }
        };

        // Act - Add various attribute types
        entity.Attributes["content"] = JsonSerializer.SerializeToElement("Buy groceries");
        entity.Attributes["metadata:priority"] = JsonSerializer.SerializeToElement("high");
        entity.Attributes["metadata:dueDate"] = JsonSerializer.SerializeToElement(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        entity.Attributes["metadata:completed"] = JsonSerializer.SerializeToElement(false);

        // Assert
        Assert.Equal(4, entity.Attributes.Count);
        Assert.Equal("Buy groceries", entity.Attributes["content"].GetString());
        Assert.Equal("high", entity.Attributes["metadata:priority"].GetString());
        Assert.False(entity.Attributes["metadata:completed"].GetBoolean());
    }

    [Fact]
    public void EntityMetadata_DefaultsToPrivateVisibility()
    {
        // Arrange & Act
        var metadata = new EntityMetadata
        {
            CreatedDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            UpdatedDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        // Assert
        Assert.Equal(EntityVisibility.Private, metadata.Visibility);
        Assert.Equal(1, metadata.Version);
    }

    [Fact]
    public void EntityMetadata_CanSetVisibility()
    {
        // Arrange
        var metadata = new EntityMetadata
        {
            CreatedDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            UpdatedDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Visibility = EntityVisibility.Team
        };

        // Assert
        Assert.Equal(EntityVisibility.Team, metadata.Visibility);
    }

    [Fact]
    public void EntityRelationships_CanStore_HierarchicalAndExplicitLinks()
    {
        // Arrange
        var relationships = new EntityRelationships();

        // Act
        relationships.ParentIds.Add("entity:collection:project1");
        relationships.ReferencedByIds.Add("entity:item:dependent1");
        relationships.LinkedEntities["assignedTo"] = "user:john";
        relationships.LinkedEntities["dependsOn"] = "entity:item:prerequisite";

        // Assert
        Assert.Single(relationships.ParentIds);
        Assert.Single(relationships.ReferencedByIds);
        Assert.Equal(2, relationships.LinkedEntities.Count);
        Assert.Equal("user:john", relationships.LinkedEntities["assignedTo"]);
        Assert.Equal("entity:item:prerequisite", relationships.LinkedEntities["dependsOn"]);
    }

    [Fact]
    public void EntityType_CanBeCreated_WithRequiredProperties()
    {
        // Arrange & Act
        var entityType = new EntityType
        {
            Id = "entityType:itemType:capture",
            Name = "Capture",
            TenantId = "tenant:system",
            CreatedDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            UpdatedDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        // Assert
        Assert.NotNull(entityType);
        Assert.Equal("entityType:itemType:capture", entityType.Id);
        Assert.Equal("Capture", entityType.Name);
        Assert.Equal("tenant:system", entityType.TenantId);
        Assert.True(entityType.IsActive);
        Assert.NotNull(entityType.Attributes);
    }

    [Fact]
    public void EntityType_CanDefine_AttributeSchema()
    {
        // Arrange
        var entityType = new EntityType
        {
            Id = "entityType:itemType:action",
            Name = "Action",
            TenantId = "tenant:system",
            CreatedDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            UpdatedDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        // Act - Define attributes
        entityType.Attributes.Add(new AttributeDefinition
        {
            Name = "metadata:title",
            Kind = "string",
            Required = true,
            Description = "Title of the action item"
        });

        entityType.Attributes.Add(new AttributeDefinition
        {
            Name = "metadata:dueDate",
            Kind = "timestamp",
            Required = false,
            Description = "When the action is due"
        });

        entityType.Attributes.Add(new AttributeDefinition
        {
            Name = "metadata:priority",
            Kind = "string",
            Required = false,
            DefaultValue = "medium",
            ValidationRules = new Dictionary<string, object>
            {
                { "enum", new[] { "low", "medium", "high" } }
            }
        });

        // Assert
        Assert.Equal(3, entityType.Attributes.Count);
        
        var titleAttr = entityType.Attributes[0];
        Assert.Equal("metadata:title", titleAttr.Name);
        Assert.True(titleAttr.Required);
        Assert.Equal("string", titleAttr.Kind);

        var dueDateAttr = entityType.Attributes[1];
        Assert.Equal("metadata:dueDate", dueDateAttr.Name);
        Assert.False(dueDateAttr.Required);
        Assert.Equal("timestamp", dueDateAttr.Kind);

        var priorityAttr = entityType.Attributes[2];
        Assert.Equal("metadata:priority", priorityAttr.Name);
        Assert.Equal("medium", priorityAttr.DefaultValue);
        Assert.NotNull(priorityAttr.ValidationRules);
    }

    [Fact]
    public void AttributeDefinition_CanDefine_CalculatedAttribute()
    {
        // Arrange & Act
        var attr = new AttributeDefinition
        {
            Name = "metadata:totalStorageBytes",
            Kind = "number:calculated",
            Calculated = true,
            Description = "Total storage used by files"
        };

        // Assert
        Assert.True(attr.Calculated);
        Assert.Equal("number:calculated", attr.Kind);
    }

    [Fact]
    public void EntityType_CanHave_ParentType()
    {
        // Arrange & Act
        var baseType = new EntityType
        {
            Id = "entityType:item",
            Name = "Item",
            TenantId = "tenant:system",
            CreatedDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            UpdatedDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        var derivedType = new EntityType
        {
            Id = "entityType:itemType:capture",
            Name = "Capture",
            ParentTypeId = "entityType:item",
            TenantId = "tenant:system",
            CreatedDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            UpdatedDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        // Assert
        Assert.Null(baseType.ParentTypeId);
        Assert.Equal("entityType:item", derivedType.ParentTypeId);
    }

    [Fact]
    public void Entity_IdFormat_FollowsConvention()
    {
        // Arrange
        var entityId = "entity:capture:abc123";
        var entityTypeId = "entityType:itemType:capture";
        var tenantId = "tenant:acme";
        var ownerId = "user:john";

        // Act
        var entity = new Entity
        {
            Id = entityId,
            EntityTypeId = entityTypeId,
            TenantId = tenantId,
            OwnerId = ownerId,
            Metadata = new EntityMetadata
            {
                CreatedDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                UpdatedDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            }
        };

        // Assert - Verify ID format conventions
        Assert.StartsWith("entity:", entity.Id);
        Assert.StartsWith("entityType:", entity.EntityTypeId);
        Assert.StartsWith("tenant:", entity.TenantId);
        Assert.StartsWith("user:", entity.OwnerId);
    }
}
