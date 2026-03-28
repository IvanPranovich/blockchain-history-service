FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["BlockchainHistoryService/BlockchainHistoryService.csproj", "BlockchainHistoryService/"]
COPY ["BlockchainHistoryService.Application/BlockchainHistoryService.Application.csproj", "BlockchainHistoryService.Application/"]
COPY ["BlockchainHistoryService.Domain/BlockchainHistoryService.Domain.csproj", "BlockchainHistoryService.Domain/"]
COPY ["BlockchainHistoryService.Infrastructure/BlockchainHistoryService.Infrastructure.csproj", "BlockchainHistoryService.Infrastructure/"]

RUN dotnet restore "BlockchainHistoryService/BlockchainHistoryService.csproj"

COPY . .

RUN dotnet publish "BlockchainHistoryService/BlockchainHistoryService.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BlockchainHistoryService.dll"]
