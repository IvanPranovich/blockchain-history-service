using BlockchainHistoryService.Application.Common.Models;
using BlockchainHistoryService.Application.TodoItems.DTOs;
using BlockchainHistoryService.Application.TodoItems.Mappings;
using BlockchainHistoryService.Domain.Entities;
using BlockchainHistoryService.Domain.Interfaces.Repositories;

namespace BlockchainHistoryService.Application.TodoItems.Commands.CreateTodoItem;

public class CreateTodoItemHandler(ITodoItemRepository repository, ITodoListRepository todoListRepository)
{
    public async Task<Result<TodoItemDto>> HandleAsync(CreateTodoItemCommand command, CancellationToken ct = default)
    {
        var todoList = await todoListRepository.GetByIdAsync(command.TodoListId, ct);
        if (todoList is null)
            return Result<TodoItemDto>.Failure($"TodoList with id '{command.TodoListId}' was not found.");

        var todoItem = new TodoItem
        {
            TodoListId = command.TodoListId,
            Title = command.Title,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await repository.CreateAsync(todoItem, ct);
        return Result<TodoItemDto>.Success(created.ToDto());
    }
}
