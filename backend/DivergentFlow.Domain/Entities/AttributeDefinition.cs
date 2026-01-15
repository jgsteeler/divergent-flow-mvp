namespace DivergentFlow.Domain.Entities;

/// <summary>
/// Defines an attribute for an entity type.
/// Attributes define the schema and validation rules for entity properties.
/// Part of the unified entity system described in docs/unified.md
/// </summary>
public sealed class AttributeDefinition
{
    /// <summary>
    /// Gets or sets the name of the attribute (e.g., "metadata:title", "content", "metadata:dueDate").
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the kind/type of the attribute.
    /// Examples: "string", "number", "boolean", "timestamp", "array:string", "file:reference"
    /// </summary>
    public required string Kind { get; set; }
    
    /// <summary>
    /// Gets or sets whether this attribute is required.
    /// </summary>
    public bool Required { get; set; }
    
    /// <summary>
    /// Gets or sets whether this attribute is calculated/computed.
    /// Calculated attributes are derived from other attributes and not stored directly.
    /// </summary>
    public bool Calculated { get; set; }
    
    /// <summary>
    /// Gets or sets whether users should be prompted to provide this attribute.
    /// </summary>
    public bool UserPrompt { get; set; }
    
    /// <summary>
    /// Gets or sets the description of this attribute (for UI hints and documentation).
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the default value for this attribute (JSON encoded).
    /// </summary>
    public string? DefaultValue { get; set; }
    
    /// <summary>
    /// Gets or sets validation rules for this attribute (JSON encoded).
    /// Examples: {"minLength": 1, "maxLength": 500}, {"min": 0, "max": 100}
    /// </summary>
    public Dictionary<string, object>? ValidationRules { get; set; }
}
