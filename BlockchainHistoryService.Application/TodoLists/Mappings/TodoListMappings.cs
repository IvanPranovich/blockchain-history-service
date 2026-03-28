using BlockchainHistoryService.Application.TodoItems.DTOs;
using BlockchainHistoryService.Application.TodoItems.Mappings;
using BlockchainHistoryService.Application.TodoLists.DTOs;
using BlockchainHistoryService.Domain.Entities;

namespace BlockchainHistoryService.Application.TodoLists.Mappings;

public static class TodoListMappings
{
    public static TodoListDto ToDto(this TodoList todoList) =>
        new(
            todoList.Id,
            todoList.Title,
            todoList.Description,
            todoList.CreatedAt,
            todoList.UpdatedAt,
            todoList.Items.Select(i => i.ToDto()).ToList()
        );
}
