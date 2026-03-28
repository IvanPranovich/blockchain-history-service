using BlockchainHistoryService.Domain.Entities;
using BlockchainHistoryService.Domain.Interfaces.Repositories;
using BlockchainHistoryService.Infrastructure.Persistence;
using MongoDB.Driver;

namespace BlockchainHistoryService.Infrastructure.Repositories;

public class TodoListRepository(MongoDbContext context) : ITodoListRepository
{
    public async Task<IEnumerable<TodoList>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.TodoLists
            .Find(_ => true)
            .SortByDescending(t => t.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<TodoList?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        return await context.TodoLists
            .Find(t => t.Id == id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<TodoList> CreateAsync(TodoList todoList, CancellationToken ct = default)
    {
        await context.TodoLists.InsertOneAsync(todoList, cancellationToken: ct);
        return todoList;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken ct = default)
    {
        var result = await context.TodoLists.DeleteOneAsync(t => t.Id == id, ct);
        return result.DeletedCount > 0;
    }
}
