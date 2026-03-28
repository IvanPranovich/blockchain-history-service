using BlockchainHistoryService.Application.TodoItems.Commands.CreateTodoItem;
using BlockchainHistoryService.Application.TodoItems.Commands.DeleteTodoItem;
using BlockchainHistoryService.Application.TodoItems.Commands.ToggleTodoItem;
using BlockchainHistoryService.Application.TodoItems.Commands.UpdateTodoItem;
using BlockchainHistoryService.Application.TodoLists.Commands.CreateTodoList;
using BlockchainHistoryService.Application.TodoLists.Commands.DeleteTodoList;
using BlockchainHistoryService.Application.TodoLists.Queries.GetTodoListById;
using BlockchainHistoryService.Application.TodoLists.Queries.GetTodoLists;
using Microsoft.Extensions.DependencyInjection;

namespace BlockchainHistoryService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<GetTodoListsHandler>();
        services.AddScoped<GetTodoListByIdHandler>();
        services.AddScoped<CreateTodoListHandler>();
        services.AddScoped<DeleteTodoListHandler>();
        services.AddScoped<CreateTodoItemHandler>();
        services.AddScoped<UpdateTodoItemHandler>();
        services.AddScoped<DeleteTodoItemHandler>();
        services.AddScoped<ToggleTodoItemHandler>();

        return services;
    }
}
