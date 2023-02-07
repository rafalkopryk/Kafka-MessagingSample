#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Modules/Publisher/Publisher.Api/Publisher.Api.csproj", "Modules/Publisher/Publisher.Api/"]
COPY ["Common/Common.Infrastructure/Common.Infrastructure.csproj", "Common/Common.Infrastructure/"]
COPY ["Common/Common.Application/Common.Application.csproj", "Common/Common.Application/"]
COPY ["Modules/Publisher/Publisher.Application/Publisher.Application.csproj", "Modules/Publisher/Publisher.Application/"]
RUN dotnet restore "Modules/Publisher/Publisher.Api/Publisher.Api.csproj"
COPY . .
WORKDIR "/src/Modules/Publisher/Publisher.Api"
RUN dotnet build "Publisher.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Publisher.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Publisher.Api.dll"]