using System.ComponentModel.DataAnnotations;

namespace DivergentFlow.Application.Configuration;

/// <summary>
/// Configuration settings for MongoDB connection.
/// </summary>
public sealed class MongoDbSettings
{
    /// <summary>
    /// The configuration section name for MongoDB settings.
    /// </summary>
    public const string SectionName = "MongoDB";

    /// <summary>
    /// Gets or sets the MongoDB connection string.
    /// </summary>
    [Required]
    public required string ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the name of the database to use.
    /// </summary>
    [Required]
    public required string DatabaseName { get; set; }

    /// <summary>
    /// Gets or sets the name of the items collection.
    /// Default: "items"
    /// </summary>
    public string ItemsCollectionName { get; set; } = "items";

    /// <summary>
    /// Gets or sets the name of the collections collection.
    /// Default: "collections"
    /// </summary>
    public string CollectionsCollectionName { get; set; } = "collections";
}
