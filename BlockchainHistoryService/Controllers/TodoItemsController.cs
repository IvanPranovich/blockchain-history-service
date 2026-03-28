using BlockchainHistoryService.Application.TodoItems.Commands.CreateTodoItem;
using BlockchainHistoryService.Application.TodoItems.Commands.DeleteTodoItem;
using BlockchainHistoryService.Application.TodoItems.Commands.ToggleTodoItem;
using BlockchainHistoryService.Application.TodoItems.Commands.UpdateTodoItem;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainHistoryService.Controllers;

[ApiController]
[Route("api/todo-lists/{todoListId}/items")]
public class TodoItemsController(
    CreateTodoItemHandler createTodoItemHandler,
    UpdateTodoItemHandler updateTodoItemHandler,
    DeleteTodoItemHandler deleteTodoItemHandler,
    ToggleTodoItemHandler toggleTodoItemHandler) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(string todoListId, [FromBody] CreateTodoItemRequest request, CancellationToken ct)
    {
        var result = await createTodoItemHandler.HandleAsync(
            new CreateTodoItemCommand(todoListId, request.Title), ct);

        return result.IsSuccess
            ? CreatedAtAction(null, new { id = result.Value!.Id }, result.Value)
            : NotFound(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateTodoItemRequest request, CancellationToken ct)
    {
        var result = await updateTodoItemHandler.HandleAsync(
            new UpdateTodoItemCommand(id, request.Title), ct);

        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPatch("{id}/toggle")]
    public async Task<IActionResult> Toggle(string id, CancellationToken ct)
    {
        var result = await toggleTodoItemHandler.HandleAsync(new ToggleTodoItemCommand(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        var result = await deleteTodoItemHandler.HandleAsync(new DeleteTodoItemCommand(id), ct);
        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }
}

public record CreateTodoItemRequest(string Title);
public record UpdateTodoItemRequest(string Title);
