using DivergentFlow.Services.Models;

namespace DivergentFlow.Services.Repositories;

/// <summary>
/// In-memory implementation of ICaptureRepository for testing
/// </summary>
public class InMemoryCaptureRepository : ICaptureRepository
{
    private readonly Dictionary<string, CaptureDto> _captures = new();
    private readonly object _lock = new();

    public Task<IEnumerable<CaptureDto>> GetAllAsync()
    {
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<CaptureDto>>(_captures.Values.ToList());
        }
    }

    public Task<CaptureDto?> GetByIdAsync(string id)
    {
        lock (_lock)
        {
            _captures.TryGetValue(id, out var capture);
            return Task.FromResult(capture);
        }
    }

    public Task<CaptureDto> SaveAsync(CaptureDto capture)
    {
        lock (_lock)
        {
            _captures[capture.Id] = capture;
            return Task.FromResult(capture);
        }
    }

    public Task<CaptureDto?> UpdateAsync(CaptureDto capture)
    {
        lock (_lock)
        {
            if (!_captures.ContainsKey(capture.Id))
            {
                return Task.FromResult<CaptureDto?>(null);
            }

            _captures[capture.Id] = capture;
            return Task.FromResult<CaptureDto?>(capture);
        }
    }

    public Task<bool> DeleteAsync(string id)
    {
        lock (_lock)
        {
            return Task.FromResult(_captures.Remove(id));
        }
    }
}
