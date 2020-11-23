#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Messaging.PublishService.Api/Messaging.PublishService.Api.csproj", "Messaging.PublishService.Api/"]
COPY ["Messaging.PublishService.Application/Messaging.PublishService.Application.csproj", "Messaging.PublishService.Application/"]
COPY ["Messaging.Common/Messaging.Common.csproj", "Messaging.Common/"]
COPY ["Messaging.PublishService.Domain/Messaging.PublishService.Domain.csproj", "Messaging.PublishService.Domain/"]
COPY ["Messaging.Core.Domain.Abstractions/Messaging.Core.Domain.Abstractions.csproj", "Messaging.Core.Domain.Abstractions/"]
COPY ["Messaging.Core.Application.Abstractions/Messaging.Core.Application.Abstractions.csproj", "Messaging.Core.Application.Abstractions/"]
COPY ["Messaging.Infrastructure/Messaging.Infrastructure.csproj", "Messaging.Infrastructure/"]
COPY ["Messaging.ConsumerService.Domain/Messaging.ConsumerService.Domain.csproj", "Messaging.ConsumerService.Domain/"]
RUN dotnet restore "Messaging.PublishService.Api/Messaging.PublishService.Api.csproj"
COPY . .
WORKDIR "/src/Messaging.PublishService.Api"
RUN dotnet build "Messaging.PublishService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Messaging.PublishService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Messaging.PublishService.Api.dll"]