using BlockchainHistoryService.Domain.Interfaces;
using MongoDB.Driver;

namespace BlockchainHistoryService.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork, IAsyncDisposable
{
    private readonly IMongoClient _client;
    private IClientSessionHandle? _session;

    public UnitOfWork(MongoDbContext context)
    {
        _client = context.Client;
    }

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        _session = await _client.StartSessionAsync(cancellationToken: ct);
        _session.StartTransaction();
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_session is { IsInTransaction: true })
            await _session.CommitTransactionAsync(ct);
    }

    public async ValueTask DisposeAsync()
    {
        if (_session is not null)
        {
            if (_session.IsInTransaction)
                await _session.AbortTransactionAsync();
            _session.Dispose();
        }
    }
}
