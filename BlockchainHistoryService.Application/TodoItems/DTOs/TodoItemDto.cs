namespace BlockchainHistoryService.Application.TodoItems.DTOs;

public record TodoItemDto(
    string Id,
    string TodoListId,
    string Title,
    bool IsCompleted,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
