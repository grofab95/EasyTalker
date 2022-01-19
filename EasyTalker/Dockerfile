FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["EasyTalker/EasyTalker.csproj", "EasyTalker/"]
COPY ["EasyTalker.Api/EasyTalker.Api.csproj", "EasyTalker.Api/"]
COPY ["EasyTalker.Core/EasyTalker.Core.csproj", "EasyTalker.Core/"]
COPY ["EasyTalker.Infrastructure/EasyTalker.Infrastructure.csproj", "EasyTalker.Infrastructure/"]
COPY ["EasyTalker.Database/EasyTalker.Database.csproj", "EasyTalker.Database/"]
RUN dotnet restore "EasyTalker/EasyTalker.csproj"
COPY . .
WORKDIR "/src/EasyTalker"
RUN dotnet build "EasyTalker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EasyTalker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EasyTalker.dll"]
