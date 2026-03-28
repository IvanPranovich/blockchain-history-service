using BlockchainHistoryService.Application.Common.Models;
using BlockchainHistoryService.Application.TodoLists.DTOs;
using BlockchainHistoryService.Application.TodoLists.Mappings;
using BlockchainHistoryService.Domain.Interfaces.Repositories;

namespace BlockchainHistoryService.Application.TodoLists.Queries.GetTodoListById;

public class GetTodoListByIdHandler(ITodoListRepository repository)
{
    public async Task<Result<TodoListDto>> HandleAsync(GetTodoListByIdQuery query, CancellationToken ct = default)
    {
        var todoList = await repository.GetByIdAsync(query.Id, ct);

        return todoList is null
            ? Result<TodoListDto>.Failure($"TodoList with id '{query.Id}' was not found.")
            : Result<TodoListDto>.Success(todoList.ToDto());
    }
}
