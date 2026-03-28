using BlockchainHistoryService.Application.Common.Models;
using BlockchainHistoryService.Application.TodoLists.DTOs;
using BlockchainHistoryService.Application.TodoLists.Mappings;
using BlockchainHistoryService.Domain.Entities;
using BlockchainHistoryService.Domain.Interfaces.Repositories;

namespace BlockchainHistoryService.Application.TodoLists.Commands.CreateTodoList;

public class CreateTodoListHandler(ITodoListRepository repository)
{
    public async Task<Result<TodoListDto>> HandleAsync(CreateTodoListCommand command, CancellationToken ct = default)
    {
        var todoList = new TodoList
        {
            Title = command.Title,
            Description = command.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await repository.CreateAsync(todoList, ct);

        return Result<TodoListDto>.Success(created.ToDto());
    }
}
