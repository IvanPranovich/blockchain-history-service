using BlockchainHistoryService.Domain.Interfaces;
using BlockchainHistoryService.Domain.Interfaces.Repositories;
using BlockchainHistoryService.Infrastructure.Persistence;
using BlockchainHistoryService.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlockchainHistoryService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDb"));
        services.AddSingleton<MongoDbContext>();

        services.AddScoped<ITodoListRepository, TodoListRepository>();
        services.AddScoped<ITodoItemRepository, TodoItemRepository>();
        services.AddScoped<IBlockchainSnapshotRepository, BlockchainSnapshotRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
