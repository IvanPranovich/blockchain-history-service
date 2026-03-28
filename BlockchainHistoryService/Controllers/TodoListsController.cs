using BlockchainHistoryService.Application.TodoLists.Commands.CreateTodoList;
using BlockchainHistoryService.Application.TodoLists.Commands.DeleteTodoList;
using BlockchainHistoryService.Application.TodoLists.Queries.GetTodoListById;
using BlockchainHistoryService.Application.TodoLists.Queries.GetTodoLists;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainHistoryService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoListsController(
    GetTodoListsHandler getTodoListsHandler,
    GetTodoListByIdHandler getTodoListByIdHandler,
    CreateTodoListHandler createTodoListHandler,
    DeleteTodoListHandler deleteTodoListHandler) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await getTodoListsHandler.HandleAsync(new GetTodoListsQuery(), ct);
        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken ct)
    {
        var result = await getTodoListByIdHandler.HandleAsync(new GetTodoListByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTodoListRequest request, CancellationToken ct)
    {
        var result = await createTodoListHandler.HandleAsync(
            new CreateTodoListCommand(request.Title, request.Description), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        var result = await deleteTodoListHandler.HandleAsync(new DeleteTodoListCommand(id), ct);
        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }
}

public record CreateTodoListRequest(string Title, string? Description);
