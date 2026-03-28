using BlockchainHistoryService.Domain.Entities;

namespace BlockchainHistoryService.Domain.Interfaces.Repositories;

public interface ITodoItemRepository
{
    Task<TodoItem?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<IEnumerable<TodoItem>> GetByListIdAsync(string todoListId, CancellationToken ct = default);
    Task<TodoItem> CreateAsync(TodoItem todoItem, CancellationToken ct = default);
    Task<TodoItem?> UpdateAsync(TodoItem todoItem, CancellationToken ct = default);
    Task<bool> DeleteAsync(string id, CancellationToken ct = default);
}
