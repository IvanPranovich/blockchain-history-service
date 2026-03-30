namespace BlockchainHistoryService.Infrastructure.Persistence.Documents;

internal sealed class BlockchainSnapshotDocument
{
    public string Id { get; set; } = string.Empty;
    public string Chain { get; set; } = string.Empty;
    public string RawData { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
