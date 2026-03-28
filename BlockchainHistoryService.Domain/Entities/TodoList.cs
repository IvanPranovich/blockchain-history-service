namespace BlockchainHistoryService.Domain.Entities;

public class TodoList
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<TodoItem> Items { get; set; } = [];
}
