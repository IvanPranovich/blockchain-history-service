namespace BlockchainHistoryService.Domain.Entities;

public class TodoItem
{
    public string Id { get; set; } = string.Empty;
    public string TodoListId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
