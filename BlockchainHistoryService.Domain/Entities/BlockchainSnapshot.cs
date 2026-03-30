using System.Text.Json;

namespace BlockchainHistoryService.Domain.Entities;

public sealed class BlockchainSnapshot
{
    public string Id { get; init; } = string.Empty;
    public string Chain { get; init; } = string.Empty;
    public JsonDocument RawData { get; init; } = JsonDocument.Parse("{}");
    public DateTime CreatedAt { get; init; }
}
