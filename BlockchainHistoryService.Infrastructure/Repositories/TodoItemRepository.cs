using BlockchainHistoryService.Domain.Entities;
using BlockchainHistoryService.Domain.Interfaces.Repositories;
using BlockchainHistoryService.Infrastructure.Persistence;
using MongoDB.Driver;

namespace BlockchainHistoryService.Infrastructure.Repositories;

public class TodoItemRepository(MongoDbContext context) : ITodoItemRepository
{
    public async Task<TodoItem?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        return await context.TodoItems
            .Find(t => t.Id == id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IEnumerable<TodoItem>> GetByListIdAsync(string todoListId, CancellationToken ct = default)
    {
        return await context.TodoItems
            .Find(t => t.TodoListId == todoListId)
            .SortByDescending(t => t.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<TodoItem> CreateAsync(TodoItem todoItem, CancellationToken ct = default)
    {
        await context.TodoItems.InsertOneAsync(todoItem, cancellationToken: ct);
        return todoItem;
    }

    public async Task<TodoItem?> UpdateAsync(TodoItem todoItem, CancellationToken ct = default)
    {
        var result = await context.TodoItems.ReplaceOneAsync(
            t => t.Id == todoItem.Id,
            todoItem,
            cancellationToken: ct);

        return result.ModifiedCount > 0 ? todoItem : null;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken ct = default)
    {
        var result = await context.TodoItems.DeleteOneAsync(t => t.Id == id, ct);
        return result.DeletedCount > 0;
    }
}
