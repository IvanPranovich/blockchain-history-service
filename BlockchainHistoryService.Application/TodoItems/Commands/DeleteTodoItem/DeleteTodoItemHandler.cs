using BlockchainHistoryService.Application.Common.Models;
using BlockchainHistoryService.Domain.Interfaces.Repositories;

namespace BlockchainHistoryService.Application.TodoItems.Commands.DeleteTodoItem;

public class DeleteTodoItemHandler(ITodoItemRepository repository)
{
    public async Task<Result> HandleAsync(DeleteTodoItemCommand command, CancellationToken ct = default)
    {
        var deleted = await repository.DeleteAsync(command.Id, ct);

        return deleted
            ? Result.Success()
            : Result.Failure($"TodoItem with id '{command.Id}' was not found.");
    }
}
