using BlockchainHistoryService.Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace BlockchainHistoryService.Infrastructure.Persistence;

public class MongoDbContext
{
    private static bool _conventionsRegistered;
    private static readonly object _lock = new();

    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        RegisterConventions();

        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);

        ConfigureCollections(settings.Value);
    }

    public IMongoCollection<TodoList> TodoLists => _database.GetCollection<TodoList>("todo_lists");
    public IMongoCollection<TodoItem> TodoItems => _database.GetCollection<TodoItem>("todo_items");

    private static void RegisterConventions()
    {
        lock (_lock)
        {
            if (_conventionsRegistered) return;

            BsonClassMap.RegisterClassMap<TodoList>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(t => t.Id)
                  .SetIdGenerator(StringObjectIdGenerator.Instance)
                  .SetSerializer(new StringSerializer(BsonType.ObjectId));
            });

            BsonClassMap.RegisterClassMap<TodoItem>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(t => t.Id)
                  .SetIdGenerator(StringObjectIdGenerator.Instance)
                  .SetSerializer(new StringSerializer(BsonType.ObjectId));
            });

            var pack = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("CamelCase", pack, _ => true);

            _conventionsRegistered = true;
        }
    }

    private void ConfigureCollections(MongoDbSettings settings)
    {
        // Indexes for TodoLists
        var todoListIndexKeys = Builders<TodoList>.IndexKeys.Ascending(t => t.CreatedAt);
        TodoLists.Indexes.CreateOne(new CreateIndexModel<TodoList>(todoListIndexKeys));

        // Indexes for TodoItems
        var todoItemListIdIndex = Builders<TodoItem>.IndexKeys.Ascending(t => t.TodoListId);
        TodoItems.Indexes.CreateOne(new CreateIndexModel<TodoItem>(todoItemListIdIndex));
    }
}
