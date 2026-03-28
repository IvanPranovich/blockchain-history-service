using BlockchainHistoryService.Application.TodoItems.DTOs;
using BlockchainHistoryService.Domain.Entities;

namespace BlockchainHistoryService.Application.TodoItems.Mappings;

public static class TodoItemMappings
{
    public static TodoItemDto ToDto(this TodoItem todoItem) =>
        new(
            todoItem.Id,
            todoItem.TodoListId,
            todoItem.Title,
            todoItem.IsCompleted,
            todoItem.CreatedAt,
            todoItem.UpdatedAt
        );
}
