using BlockchainHistoryService.Application.Common.Models;
using BlockchainHistoryService.Application.TodoItems.DTOs;
using BlockchainHistoryService.Application.TodoItems.Mappings;
using BlockchainHistoryService.Domain.Interfaces.Repositories;

namespace BlockchainHistoryService.Application.TodoItems.Commands.UpdateTodoItem;

public class UpdateTodoItemHandler(ITodoItemRepository repository)
{
    public async Task<Result<TodoItemDto>> HandleAsync(UpdateTodoItemCommand command, CancellationToken ct = default)
    {
        var existing = await repository.GetByIdAsync(command.Id, ct);
        if (existing is null)
            return Result<TodoItemDto>.Failure($"TodoItem with id '{command.Id}' was not found.");

        existing.Title = command.Title;
        existing.UpdatedAt = DateTime.UtcNow;

        var updated = await repository.UpdateAsync(existing, ct);
        return Result<TodoItemDto>.Success(updated!.ToDto());
    }
}
