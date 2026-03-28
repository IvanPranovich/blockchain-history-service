using BlockchainHistoryService.Application.Common.Models;
using BlockchainHistoryService.Application.TodoLists.DTOs;
using BlockchainHistoryService.Application.TodoLists.Mappings;
using BlockchainHistoryService.Domain.Interfaces.Repositories;

namespace BlockchainHistoryService.Application.TodoLists.Queries.GetTodoLists;

public class GetTodoListsHandler(ITodoListRepository repository)
{
    public async Task<Result<IEnumerable<TodoListDto>>> HandleAsync(GetTodoListsQuery query, CancellationToken ct = default)
    {
        var todoLists = await repository.GetAllAsync(ct);
        return Result<IEnumerable<TodoListDto>>.Success(todoLists.Select(t => t.ToDto()));
    }
}
