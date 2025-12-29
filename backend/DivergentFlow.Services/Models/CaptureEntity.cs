using Redis.OM.Modeling;

namespace DivergentFlow.Services.Models;

/// <summary>
/// Redis entity model for Capture items
/// Used by Redis.OM for object mapping to Redis
/// </summary>
[Document(StorageType = StorageType.Json, Prefixes = new[] { "Capture" })]
public class CaptureEntity
{
    /// <summary>
    /// Unique identifier for the capture (Redis.OM ID)
    /// </summary>
    [RedisIdField]
    [Indexed]
    public string? Id { get; set; }

    /// <summary>
    /// The captured text content
    /// </summary>
    [Indexed(Sortable = true)]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the capture was created (Unix milliseconds)
    /// </summary>
    [Indexed(Sortable = true)]
    public long CreatedAt { get; set; }

    /// <summary>
    /// The inferred type of the item (e.g., note, action, reminder)
    /// </summary>
    [Indexed]
    public string? InferredType { get; set; }

    /// <summary>
    /// Confidence score for the type inference (0-100)
    /// </summary>
    public double? TypeConfidence { get; set; }

    /// <summary>
    /// Convert to DTO
    /// </summary>
    public CaptureDto ToDto()
    {
        return new CaptureDto
        {
            Id = Id ?? string.Empty,
            Text = Text,
            CreatedAt = CreatedAt,
            InferredType = InferredType,
            TypeConfidence = TypeConfidence
        };
    }

    /// <summary>
    /// Create from DTO
    /// </summary>
    public static CaptureEntity FromDto(CaptureDto dto)
    {
        return new CaptureEntity
        {
            Id = string.IsNullOrEmpty(dto.Id) ? null : dto.Id,
            Text = dto.Text,
            CreatedAt = dto.CreatedAt,
            InferredType = dto.InferredType,
            TypeConfidence = dto.TypeConfidence
        };
    }
}
