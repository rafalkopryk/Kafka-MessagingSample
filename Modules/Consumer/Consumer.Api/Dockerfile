#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Modules/Consumer/Consumer.Api/Consumer.Api.csproj", "Modules/Consumer/Consumer.Api/"]
RUN dotnet restore "Modules/Consumer/Consumer.Api/Consumer.Api.csproj"
COPY . .
WORKDIR "/src/Modules/Consumer/Consumer.Api"
RUN dotnet build "Consumer.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Consumer.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Consumer.Api.dll"]