using System.Collections.Concurrent;
using LendingPlatform.Api.Domain;

namespace LendingPlatform.Api.Infrastructure;

public interface IApplicationRepository
{
    void Add(ApplicationRecord application);
    IReadOnlyList<ApplicationRecord> GetAll();
}

public sealed class InMemoryApplicationRepository : IApplicationRepository
{
    private readonly ConcurrentQueue<ApplicationRecord> _applications = new();

    public void Add(ApplicationRecord application) => _applications.Enqueue(application);
    public IReadOnlyList<ApplicationRecord> GetAll() => _applications.ToArray();
}
