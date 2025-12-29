using DivergentFlow.Domain.Entities;

namespace DivergentFlow.Application.Abstractions;

public interface ICaptureRepository
{
    Task<IReadOnlyList<Capture>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Capture?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Capture> CreateAsync(Capture capture, CancellationToken cancellationToken = default);
    Task<Capture?> UpdateAsync(string id, Capture updated, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
