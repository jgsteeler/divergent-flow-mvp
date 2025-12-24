using DivergentFlow.Api.Models;

namespace DivergentFlow.Api.Services;

/// <summary>
/// Temporary in-memory implementation of ICaptureService
/// This will be replaced with a real database implementation later
/// </summary>
public class InMemoryCaptureService : ICaptureService
{
    private readonly List<CaptureDto> _captures = new();
    private readonly object _lock = new();

    public Task<IEnumerable<CaptureDto>> GetAllAsync()
    {
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<CaptureDto>>(_captures.ToList());
        }
    }

    public Task<CaptureDto?> GetByIdAsync(string id)
    {
        lock (_lock)
        {
            var capture = _captures.FirstOrDefault(c => c.Id == id);
            return Task.FromResult(capture);
        }
    }

    public Task<CaptureDto> CreateAsync(CreateCaptureRequest request)
    {
        lock (_lock)
        {
            var capture = new CaptureDto
            {
                Id = Guid.NewGuid().ToString(),
                Text = request.Text,
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            _captures.Add(capture);
            return Task.FromResult(capture);
        }
    }

    public Task<CaptureDto?> UpdateAsync(string id, UpdateCaptureRequest request)
    {
        lock (_lock)
        {
            var capture = _captures.FirstOrDefault(c => c.Id == id);
            if (capture == null)
            {
                return Task.FromResult<CaptureDto?>(null);
            }

            capture.Text = request.Text;
            return Task.FromResult<CaptureDto?>(capture);
        }
    }

    public Task<bool> DeleteAsync(string id)
    {
        lock (_lock)
        {
            var capture = _captures.FirstOrDefault(c => c.Id == id);
            if (capture == null)
            {
                return Task.FromResult(false);
            }

            _captures.Remove(capture);
            return Task.FromResult(true);
        }
    }
}
