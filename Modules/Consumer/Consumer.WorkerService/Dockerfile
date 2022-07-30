#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Modules/Consumer/Consumer.WorkerService/Consumer.WorkerService.csproj", "Modules/Consumer/Consumer.WorkerService/"]
COPY ["Common/Common.Infrastructure/Common.Infrastructure.csproj", "Common/Common.Infrastructure/"]
COPY ["Common/Common.Application/Common.Application.csproj", "Common/Common.Application/"]
COPY ["Modules/Consumer/Consumer.Application/Consumer.Application.csproj", "Modules/Consumer/Consumer.Application/"]
RUN dotnet restore "Modules/Consumer/Consumer.WorkerService/Consumer.WorkerService.csproj"
COPY . .
WORKDIR "/src/Modules/Consumer/Consumer.WorkerService"
RUN dotnet build "Consumer.WorkerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Consumer.WorkerService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Consumer.WorkerService.dll"]