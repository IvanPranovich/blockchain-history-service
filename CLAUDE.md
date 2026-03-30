# BlockchainHistoryService — Claude Guidelines

## Project Overview

A .NET 10 REST API that fetches blockchain data from external APIs, persists it with a `CreatedAt` timestamp, and exposes history endpoints ordered by ingestion time (descending). Built on **Clean Architecture** with CQRS, Repository, and Unit of Work patterns.

---

## Architecture

```
BlockchainHistoryService/          ← ASP.NET Core Web API (Presentation)
BlockchainHistoryService.Application/  ← CQRS commands/queries, DTOs, interfaces
BlockchainHistoryService.Domain/       ← Entities, value objects, domain interfaces
BlockchainHistoryService.Infrastructure/   ← MongoDB repos, external HTTP clients, UoW
BlockchainHistoryService.Tests.Unit/       ← xUnit unit tests
BlockchainHistoryService.Tests.Integration/ ← Integration tests (real DB, TestContainers)
BlockchainHistoryService.Tests.Functional/ ← End-to-end API tests (WebApplicationFactory)
```

**Dependency rule:** inner layers never reference outer layers. Infrastructure and API reference Application; Application references Domain; Domain references nothing.

---

## Technology Stack

| Concern | Choice |
|---|---|
| Framework | .NET 10 (ASP.NET Core) |
| Database | MongoDB 8 via `MongoDB.Driver` |
| CQRS mediator | MediatR |
| Validation | FluentValidation + MediatR pipeline behaviour |
| Mapping | Mapster (preferred) |
| Serialization | `System.Text.Json` with camelCase + `JsonStringEnumConverter` |
| Logging | `Microsoft.Extensions.Logging` + Serilog (structured, JSON sink for prod) |
| Health checks | `Microsoft.AspNetCore.Diagnostics.HealthChecks` + `AspNetCore.HealthChecks.MongoDb` |
| OpenAPI | `Microsoft.AspNetCore.OpenApi` + Scalar UI (or Swashbuckle) |
| Testing | xUnit, Moq, FluentAssertions, Testcontainers (MongoDB) |
| Containerisation | Docker (Linux) + Docker Compose |

---

## Domain Model

### `BlockchainSnapshot` (core entity)

```csharp
public sealed class BlockchainSnapshot
{
    public string Id { get; init; }          // MongoDB ObjectId as string
    public string Chain { get; init; }       // e.g. "bitcoin", "ethereum"
    public JsonDocument RawData { get; init; } // full JSON from upstream API
    public DateTime CreatedAt { get; init; } // UTC timestamp set on ingest
}
```

- `CreatedAt` is **always set server-side** at ingestion time; never accept it from callers.
- `RawData` stores the upstream response verbatim to satisfy "main data stored as provided in the API JSON responses."
- History queries **must** sort by `CreatedAt` descending; expose `createdAt` in all response DTOs.

---

## API Endpoints

All routes are prefixed `/api/v1`.

| Method | Route | Description |
|---|---|---|
| `POST` | `/blockchain/{chain}/fetch` | Fetch latest data from upstream and persist |
| `GET` | `/blockchain/{chain}/history` | Return all snapshots for a chain (CreatedAt desc) |
| `GET` | `/blockchain/{chain}/history/{id}` | Return one snapshot by ID |
| `GET` | `/health` | Health check (liveness + MongoDB probe) |
| `GET` | `/` or `/swagger` | Scalar/Swagger UI |

- Every endpoint must appear in Swagger/OpenAPI.
- Responses use consistent envelope: `{ "data": ..., "createdAt": "..." }` for single items; `{ "items": [...], "total": N }` for collections.

---

## Design Patterns

### CQRS (MediatR)
- **Commands** live in `Application/{Feature}/Commands/` — mutate state, return `Result<T>`.
- **Queries** live in `Application/{Feature}/Queries/` — read-only, return `Result<T>`.
- One handler per command/query class.
- Validation pipeline behaviour runs before every handler; log pipeline behaviour runs after.

### Repository + Unit of Work
- `IBlockchainSnapshotRepository` in Domain: `AddAsync`, `GetByChainAsync`, `GetByIdAsync`.
- `IUnitOfWork` in Domain: `CommitAsync()` — wraps MongoDB client sessions for multi-document consistency.
- Concrete implementations in Infrastructure; never exposed to Application directly (only via interfaces).

### Result Pattern
```csharp
public sealed record Result<T>(T? Value, string? Error, bool IsSuccess);
```
Controllers map `Result<T>` to HTTP status codes — never throw exceptions for expected failures.

---

## Coding Standards

- **SOLID** strictly. Each class has one reason to change.
- **Async all the way:** every I/O method is `Async` and returns `Task`/`ValueTask`. No `.Result` or `.Wait()`.
- **Parallel ingest:** when fetching multiple chains in a batch use `Task.WhenAll` or `Parallel.ForEachAsync`.
- **Nullable enabled** everywhere. Treat all warnings as errors in CI.
- **No magic strings.** Constants/enums for chain names, route prefixes, config keys.
- **Immutable DTOs:** use `record` types for all request/response objects.
- Do not add comments unless the logic is non-obvious. Code should be self-documenting.
- Do not add error handling for impossible states; trust the DI container and framework guarantees.

---

## Validation

- All command/query inputs validated with FluentValidation rules registered in `Application/DependencyInjection.cs`.
- MediatR `ValidationBehaviour<,>` runs before every handler and returns a 422 `Result` on failure — never throws.
- API models use `[ApiController]` automatic model-state validation as a first pass.

---

## Logging

- Use `ILogger<T>` injected via DI everywhere. Never use static loggers.
- Log at `Information` for successful ingestion, `Warning` for upstream API non-2xx, `Error` for unhandled exceptions.
- In production (Docker) emit structured JSON logs via Serilog `WriteTo.Console(formatter: new JsonFormatter())`.
- Do **not** log sensitive data (connection strings, API keys).

---

## CORS Policy

```csharp
builder.Services.AddCors(o => o.AddPolicy("Default", p =>
    p.WithOrigins(builder.Configuration.GetSection("Cors:Origins").Get<string[]>()!)
     .AllowAnyMethod()
     .AllowAnyHeader()));
```

- Origins configured per environment in `appsettings.{Environment}.json`.
- Development allows `*`; production locks to explicit origins.

---

## Health Checks

- Route: `GET /health`
- Checks: application liveness + MongoDB connectivity.
- Returns JSON with status `Healthy` / `Degraded` / `Unhealthy`.

---

## Configuration

All secrets (connection strings, external API keys) come from environment variables or Docker secrets — never committed to source. Use the Options pattern (`IOptions<T>`) for all config sections.

```
MongoDb__ConnectionString
MongoDb__DatabaseName
Blockchain__ApiBaseUrl
Cors__Origins__0
```

---

## Runtime Profiles

| Profile | How to run |
|---|---|
| Local (HTTP) | `dotnet run --project BlockchainHistoryService` (port 5258) |
| Local (HTTPS) | `dotnet run --launch-profile https` |
| Docker | `docker compose up --build` |
| Production | `ASPNETCORE_ENVIRONMENT=Production docker compose up -d` |

`launchSettings.json` defines `http`, `https`, and `Docker` profiles.

---

## Testing

### Unit Tests (`BlockchainHistoryService.Tests.Unit`)
- Test handlers, validators, and domain logic in isolation.
- Mock all I/O with Moq. No database, no HTTP.
- Name convention: `{Method}_When{Condition}_Should{Expectation}`.

### Integration Tests (`BlockchainHistoryService.Tests.Integration`)
- Spin up a real MongoDB instance via **Testcontainers**.
- Test repository implementations and UoW against a live database.
- Each test class gets a fresh collection; tear down after each run.

### Functional Tests (`BlockchainHistoryService.Tests.Functional`)
- Use `WebApplicationFactory<Program>` with `HttpClient`.
- Replace external blockchain HTTP client with a stub/mock via `IHttpClientFactory`.
- Assert full request → response round-trips including status codes and JSON shape.

---

## What NOT to Do

- Do not use `.Result` or `.GetAwaiter().GetResult()` on async code.
- Do not add `try/catch` in handlers for expected failures — use the `Result` pattern.
- Do not reference Infrastructure from Application or Domain.
- Do not hardcode connection strings or API keys.
- Do not create helpers or abstractions for one-off operations.
- Do not store anything other than UTC datetimes in `CreatedAt`.
- Do not skip OpenAPI attributes on new controllers or actions.
- Do not break the descending sort on history endpoints.
