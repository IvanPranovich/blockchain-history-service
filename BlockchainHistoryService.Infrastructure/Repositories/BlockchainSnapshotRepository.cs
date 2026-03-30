using System.Text.Json;
using BlockchainHistoryService.Domain.Entities;
using BlockchainHistoryService.Domain.Interfaces.Repositories;
using BlockchainHistoryService.Infrastructure.Persistence;
using BlockchainHistoryService.Infrastructure.Persistence.Documents;
using MongoDB.Driver;

namespace BlockchainHistoryService.Infrastructure.Repositories;

public sealed class BlockchainSnapshotRepository : IBlockchainSnapshotRepository
{
    private readonly IMongoCollection<BlockchainSnapshotDocument> _collection;

    public BlockchainSnapshotRepository(MongoDbContext context)
    {
        _collection = context.BlockchainSnapshots;
    }

    public async Task<BlockchainSnapshot> AddAsync(BlockchainSnapshot snapshot, CancellationToken ct = default)
    {
        var document = new BlockchainSnapshotDocument
        {
            Chain = snapshot.Chain,
            RawData = snapshot.RawData.RootElement.GetRawText(),
            CreatedAt = snapshot.CreatedAt
        };

        await _collection.InsertOneAsync(document, cancellationToken: ct);

        return new BlockchainSnapshot
        {
            Id = document.Id,
            Chain = snapshot.Chain,
            RawData = snapshot.RawData,
            CreatedAt = snapshot.CreatedAt
        };
    }

    public async Task<IEnumerable<BlockchainSnapshot>> GetByChainAsync(string chain, CancellationToken ct = default)
    {
        var documents = await _collection
            .Find(d => d.Chain == chain)
            .SortByDescending(d => d.CreatedAt)
            .ToListAsync(ct);

        return documents.Select(MapToDomain);
    }

    public async Task<BlockchainSnapshot?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        var document = await _collection
            .Find(d => d.Id == id)
            .FirstOrDefaultAsync(ct);

        return document is null ? null : MapToDomain(document);
    }

    private static BlockchainSnapshot MapToDomain(BlockchainSnapshotDocument document) =>
        new()
        {
            Id = document.Id,
            Chain = document.Chain,
            RawData = JsonDocument.Parse(document.RawData),
            CreatedAt = document.CreatedAt
        };
}
