namespace BlockchainHistoryService.Application.TodoItems.Commands.CreateTodoItem;

public record CreateTodoItemCommand(string TodoListId, string Title);
