using System.Text.Json;
using BlockchainHistoryService.Domain.Entities;
using BlockchainHistoryService.Infrastructure.Persistence;
using BlockchainHistoryService.Infrastructure.Repositories;
using DotNet.Testcontainers.Builders;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Testcontainers.MongoDb;
using Xunit;

namespace BlockchainHistoryService.Tests.Integration.Repositories;

public sealed class BlockchainSnapshotRepositoryTests : IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer = new MongoDbBuilder()
        .WithImage("mongo:8")
        .Build();

    public Task InitializeAsync() => _mongoContainer.StartAsync();

    public Task DisposeAsync() => _mongoContainer.DisposeAsync().AsTask();

    private BlockchainSnapshotRepository CreateRepository()
    {
        var settings = Options.Create(new MongoDbSettings
        {
            ConnectionString = _mongoContainer.GetConnectionString(),
            DatabaseName = $"test_{Guid.NewGuid():N}"
        });
        var context = new MongoDbContext(settings);
        return new BlockchainSnapshotRepository(context);
    }

    [Fact]
    public async Task AddAsync_WhenSnapshotIsValid_ShouldPersistAndReturnWithGeneratedId()
    {
        var repository = CreateRepository();
        var snapshot = new BlockchainSnapshot
        {
            Chain = "bitcoin",
            RawData = JsonDocument.Parse("""{"name":"BTC.main","height":360060}"""),
            CreatedAt = DateTime.UtcNow
        };

        var result = await repository.AddAsync(snapshot);

        result.Id.Should().NotBeNullOrEmpty();
        result.Chain.Should().Be("bitcoin");
        result.CreatedAt.Should().Be(snapshot.CreatedAt);
    }

    [Fact]
    public async Task GetByChainAsync_WhenSnapshotsExist_ShouldReturnSortedByCreatedAtDescending()
    {
        var repository = CreateRepository();
        var now = DateTime.UtcNow;

        await repository.AddAsync(new BlockchainSnapshot
        {
            Chain = "ethereum",
            RawData = JsonDocument.Parse("""{"name":"ETH.main","height":1}"""),
            CreatedAt = now.AddMinutes(-10)
        });
        await repository.AddAsync(new BlockchainSnapshot
        {
            Chain = "ethereum",
            RawData = JsonDocument.Parse("""{"name":"ETH.main","height":2}"""),
            CreatedAt = now.AddMinutes(-5)
        });
        await repository.AddAsync(new BlockchainSnapshot
        {
            Chain = "ethereum",
            RawData = JsonDocument.Parse("""{"name":"ETH.main","height":3}"""),
            CreatedAt = now
        });

        var results = (await repository.GetByChainAsync("ethereum")).ToList();

        results.Should().HaveCount(3);
        results[0].CreatedAt.Should().BeAfter(results[1].CreatedAt);
        results[1].CreatedAt.Should().BeAfter(results[2].CreatedAt);
    }

    [Fact]
    public async Task GetByChainAsync_WhenChainHasNoSnapshots_ShouldReturnEmpty()
    {
        var repository = CreateRepository();

        var results = await repository.GetByChainAsync("litecoin");

        results.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_WhenSnapshotExists_ShouldReturnSnapshot()
    {
        var repository = CreateRepository();
        var inserted = await repository.AddAsync(new BlockchainSnapshot
        {
            Chain = "bitcoin",
            RawData = JsonDocument.Parse("""{"name":"BTC.main","height":360060}"""),
            CreatedAt = DateTime.UtcNow
        });

        var result = await repository.GetByIdAsync(inserted.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(inserted.Id);
        result.Chain.Should().Be("bitcoin");
    }

    [Fact]
    public async Task GetByIdAsync_WhenSnapshotDoesNotExist_ShouldReturnNull()
    {
        var repository = CreateRepository();

        var result = await repository.GetByIdAsync("000000000000000000000000");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByChainAsync_ShouldNotReturnSnapshotsFromOtherChains()
    {
        var repository = CreateRepository();

        await repository.AddAsync(new BlockchainSnapshot
        {
            Chain = "bitcoin",
            RawData = JsonDocument.Parse("""{"name":"BTC.main"}"""),
            CreatedAt = DateTime.UtcNow
        });

        var results = await repository.GetByChainAsync("ethereum");

        results.Should().BeEmpty();
    }
}
