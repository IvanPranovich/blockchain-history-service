namespace BlockchainHistoryService.Infrastructure.Persistence;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string TodoListsCollectionName { get; set; } = "todo_lists";
    public string TodoItemsCollectionName { get; set; } = "todo_items";
    public string BlockchainSnapshotsCollectionName { get; set; } = "blockchain_snapshots";
}
