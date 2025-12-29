using DivergentFlow.Application.Abstractions;
using DivergentFlow.Domain.Entities;

namespace DivergentFlow.Infrastructure.Repositories;

public sealed class InMemoryCaptureRepository : ICaptureRepository
{
    private readonly List<Capture> _captures = new();
    private readonly object _lock = new();

    public Task<IReadOnlyList<Capture>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            IReadOnlyList<Capture> snapshot = _captures
                .Select(Clone)
                .ToList();

            return Task.FromResult(snapshot);
        }
    }

    public Task<Capture?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var capture = _captures.FirstOrDefault(c => c.Id == id);
            return Task.FromResult(capture is null ? null : Clone(capture));
        }
    }

    public Task<Capture> CreateAsync(Capture capture, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var toStore = Clone(capture);
            _captures.Add(toStore);
            return Task.FromResult(Clone(toStore));
        }
    }

    public Task<Capture?> UpdateAsync(string id, Capture updated, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var existing = _captures.FirstOrDefault(c => c.Id == id);
            if (existing is null)
            {
                return Task.FromResult<Capture?>(null);
            }

            existing.Text = updated.Text;
            existing.InferredType = updated.InferredType;
            existing.TypeConfidence = updated.TypeConfidence;

            return Task.FromResult<Capture?>(Clone(existing));
        }
    }

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var existing = _captures.FirstOrDefault(c => c.Id == id);
            if (existing is null)
            {
                return Task.FromResult(false);
            }

            _captures.Remove(existing);
            return Task.FromResult(true);
        }
    }

    private static Capture Clone(Capture capture) => new()
    {
        Id = capture.Id,
        Text = capture.Text,
        CreatedAt = capture.CreatedAt,
        InferredType = capture.InferredType,
        TypeConfidence = capture.TypeConfidence
    };
}
