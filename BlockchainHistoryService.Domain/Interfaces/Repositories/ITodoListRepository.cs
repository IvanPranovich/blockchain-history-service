using BlockchainHistoryService.Domain.Entities;

namespace BlockchainHistoryService.Domain.Interfaces.Repositories;

public interface ITodoListRepository
{
    Task<IEnumerable<TodoList>> GetAllAsync(CancellationToken ct = default);
    Task<TodoList?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<TodoList> CreateAsync(TodoList todoList, CancellationToken ct = default);
    Task<bool> DeleteAsync(string id, CancellationToken ct = default);
}
