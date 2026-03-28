using BlockchainHistoryService.Application.Common.Models;
using BlockchainHistoryService.Domain.Interfaces.Repositories;

namespace BlockchainHistoryService.Application.TodoLists.Commands.DeleteTodoList;

public class DeleteTodoListHandler(ITodoListRepository repository)
{
    public async Task<Result> HandleAsync(DeleteTodoListCommand command, CancellationToken ct = default)
    {
        var deleted = await repository.DeleteAsync(command.Id, ct);

        return deleted
            ? Result.Success()
            : Result.Failure($"TodoList with id '{command.Id}' was not found.");
    }
}
