using BlockchainHistoryService.Application.Common.Models;
using BlockchainHistoryService.Application.TodoItems.DTOs;
using BlockchainHistoryService.Application.TodoItems.Mappings;
using BlockchainHistoryService.Domain.Interfaces.Repositories;

namespace BlockchainHistoryService.Application.TodoItems.Commands.ToggleTodoItem;

public class ToggleTodoItemHandler(ITodoItemRepository repository)
{
    public async Task<Result<TodoItemDto>> HandleAsync(ToggleTodoItemCommand command, CancellationToken ct = default)
    {
        var existing = await repository.GetByIdAsync(command.Id, ct);
        if (existing is null)
            return Result<TodoItemDto>.Failure($"TodoItem with id '{command.Id}' was not found.");

        existing.IsCompleted = !existing.IsCompleted;
        existing.UpdatedAt = DateTime.UtcNow;

        var updated = await repository.UpdateAsync(existing, ct);
        return Result<TodoItemDto>.Success(updated!.ToDto());
    }
}
