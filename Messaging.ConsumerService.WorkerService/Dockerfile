#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Messaging.ConsumerService.WorkerService/Messaging.ConsumerService.WorkerService.csproj", "Messaging.ConsumerService.WorkerService/"]
COPY ["Messaging.ConsumerService.Application/Messaging.ConsumerService.Application.csproj", "Messaging.ConsumerService.Application/"]
COPY ["Messaging.ConsumerService.Domain/Messaging.ConsumerService.Domain.csproj", "Messaging.ConsumerService.Domain/"]
COPY ["Messaging.Core.Domain.Abstractions/Messaging.Core.Domain.Abstractions.csproj", "Messaging.Core.Domain.Abstractions/"]
COPY ["Messaging.Core.Application.Abstractions/Messaging.Core.Application.Abstractions.csproj", "Messaging.Core.Application.Abstractions/"]
COPY ["Messaging.Common/Messaging.Common.csproj", "Messaging.Common/"]
COPY ["Messaging.Infrastructure/Messaging.Infrastructure.csproj", "Messaging.Infrastructure/"]
RUN dotnet restore "Messaging.ConsumerService.WorkerService/Messaging.ConsumerService.WorkerService.csproj"
COPY . .
WORKDIR "/src/Messaging.ConsumerService.WorkerService"
RUN dotnet build "Messaging.ConsumerService.WorkerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Messaging.ConsumerService.WorkerService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Messaging.ConsumerService.WorkerService.dll"]