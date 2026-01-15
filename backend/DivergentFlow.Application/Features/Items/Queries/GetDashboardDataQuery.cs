using MediatR;

namespace DivergentFlow.Application.Features.Items.Queries;

/// <summary>
/// Query to get dashboard metrics and task data
/// </summary>
public sealed record GetDashboardDataQuery : IRequest<DashboardDataDto>;

/// <summary>
/// DTO for dashboard data including metrics and task lists
/// </summary>
public sealed class DashboardDataDto
{
    public DashboardMetrics Metrics { get; set; } = new();
    public IReadOnlyList<TaskItemDto> TodayTasks { get; set; } = Array.Empty<TaskItemDto>();
    public IReadOnlyList<TaskItemDto> OverdueTasks { get; set; } = Array.Empty<TaskItemDto>();
    public IReadOnlyList<TaskItemDto> UpcomingTasks { get; set; } = Array.Empty<TaskItemDto>();
}

/// <summary>
/// Dashboard metrics summary
/// </summary>
public sealed class DashboardMetrics
{
    public int TotalItems { get; set; }
    public int PendingReview { get; set; }
    public int ActionItems { get; set; }
    public int CompletedToday { get; set; }
}

/// <summary>
/// Simplified task item DTO for dashboard display
/// </summary>
public sealed class TaskItemDto
{
    public required string Id { get; set; }
    public required string Text { get; set; }
    public required long CreatedAt { get; set; }
    public string? InferredType { get; set; }
    public double? TypeConfidence { get; set; }
    public long? LastReviewedAt { get; set; }
}
