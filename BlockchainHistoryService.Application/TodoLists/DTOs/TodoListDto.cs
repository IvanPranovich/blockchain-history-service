using BlockchainHistoryService.Application.TodoItems.DTOs;

namespace BlockchainHistoryService.Application.TodoLists.DTOs;

public record TodoListDto(
    string Id,
    string Title,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<TodoItemDto> Items
);
