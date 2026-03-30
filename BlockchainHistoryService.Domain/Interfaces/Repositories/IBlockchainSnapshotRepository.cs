using BlockchainHistoryService.Domain.Entities;

namespace BlockchainHistoryService.Domain.Interfaces.Repositories;

public interface IBlockchainSnapshotRepository
{
    Task<BlockchainSnapshot> AddAsync(BlockchainSnapshot snapshot, CancellationToken ct = default);
    Task<IEnumerable<BlockchainSnapshot>> GetByChainAsync(string chain, CancellationToken ct = default);
    Task<BlockchainSnapshot?> GetByIdAsync(string id, CancellationToken ct = default);
}
