using AutoMapper;
using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.Items.Queries;
using MediatR;

namespace DivergentFlow.Application.Features.Items.Handlers;

/// <summary>
/// Handler for getting dashboard data
/// </summary>
public sealed class GetDashboardDataHandler : IRequestHandler<GetDashboardDataQuery, DashboardDataDto>
{
    private readonly IItemRepository _repository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetDashboardDataHandler(IItemRepository repository, IMapper mapper, IUserContext userContext)
    {
        _repository = repository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<DashboardDataDto> Handle(GetDashboardDataQuery request, CancellationToken cancellationToken)
    {
        // Get all items for the user
        var allItems = await _repository.GetAllAsync(_userContext.UserId, cancellationToken);
        
        // Get current timestamp
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var todayStart = new DateTimeOffset(DateTimeOffset.UtcNow.Date, TimeSpan.Zero).ToUnixTimeMilliseconds();
        var todayEnd = todayStart + (24 * 60 * 60 * 1000); // milliseconds in a day
        
        // Filter items that are action items (have inferredType = "action" or similar)
        var actionItems = allItems
            .Where(item => IsActionItem(item.InferredType))
            .ToList();
        
        // Calculate metrics
        var metrics = new DashboardMetrics
        {
            TotalItems = allItems.Count,
            PendingReview = allItems.Count(item => !item.LastReviewedAt.HasValue),
            ActionItems = actionItems.Count,
            CompletedToday = allItems.Count(item => 
                item.LastReviewedAt.HasValue && 
                item.LastReviewedAt.Value >= todayStart &&
                item.LastReviewedAt.Value < todayEnd)
        };
        
        // Get today's tasks (created today, not yet reviewed)
        var todayTasks = actionItems
            .Where(item => 
                item.CreatedAt >= todayStart && 
                item.CreatedAt < todayEnd &&
                !item.LastReviewedAt.HasValue)
            .OrderBy(item => item.CreatedAt)
            .Select(item => MapToTaskItemDto(item))
            .ToList();
        
        // Get overdue tasks (older than today, not yet reviewed)
        var overdueTasks = actionItems
            .Where(item => 
                item.CreatedAt < todayStart &&
                !item.LastReviewedAt.HasValue)
            .OrderBy(item => item.CreatedAt)
            .Select(item => MapToTaskItemDto(item))
            .ToList();
        
        // Get upcoming tasks (created today or recently, already reviewed but still relevant)
        var upcomingTasks = actionItems
            .Where(item => 
                item.CreatedAt >= todayStart - (7 * 24 * 60 * 60 * 1000) && // last 7 days
                item.LastReviewedAt.HasValue)
            .OrderBy(item => item.CreatedAt)
            .Take(10)
            .Select(item => MapToTaskItemDto(item))
            .ToList();
        
        return new DashboardDataDto
        {
            Metrics = metrics,
            TodayTasks = todayTasks,
            OverdueTasks = overdueTasks,
            UpcomingTasks = upcomingTasks
        };
    }
    
    private static bool IsActionItem(string? inferredType)
    {
        if (string.IsNullOrEmpty(inferredType))
            return false;
        
        var actionTypes = new[] { "action", "task", "todo", "reminder" };
        return actionTypes.Contains(inferredType.ToLowerInvariant());
    }
    
    private static TaskItemDto MapToTaskItemDto(DivergentFlow.Domain.Entities.Item item)
    {
        return new TaskItemDto
        {
            Id = item.Id,
            Text = item.Text,
            CreatedAt = item.CreatedAt,
            InferredType = item.InferredType,
            TypeConfidence = item.TypeConfidence,
            LastReviewedAt = item.LastReviewedAt
        };
    }
}
