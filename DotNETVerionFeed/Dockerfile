#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DotNETVerionFeed/DotNETVersionFeed.csproj", "DotNETVerionFeed/"]
COPY ["VersionsFeedService/VersionsFeedService.csproj", "VersionsFeedService/"]
RUN dotnet restore "DotNETVerionFeed/DotNETVersionFeed.csproj"
COPY . .
WORKDIR "/src/DotNETVerionFeed"
RUN dotnet build "DotNETVersionFeed.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DotNETVersionFeed.csproj" -c Release -o /app/publish
RUN  dotnet restore
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DotNETVersionFeed.dll"]