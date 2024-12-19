FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["LegendarySocialNetwork.WebApi/LegendarySocialNetwork.WebApi.csproj", "LegendarySocialNetwork.WebApi/"]
COPY ["LegendarySocialNetwork.Application/LegendarySocialNetwork.Application.csproj", "LegendarySocialNetwork.Application/"]
COPY ["LegendarySocialNetwork.Infrastructure/LegendarySocialNetwork.Infrastructure.csproj", "LegendarySocialNetwork.Infrastructure/"]
COPY ["LegendarySocialNetwork.Domain/LegendarySocialNetwork.Domain.csproj", "LegendarySocialNetwork.Domain/"]

RUN dotnet restore "LegendarySocialNetwork.WebApi/LegendarySocialNetwork.WebApi.csproj"

COPY . .

RUN dotnet build "LegendarySocialNetwork.WebApi/LegendarySocialNetwork.WebApi.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "LegendarySocialNetwork.WebApi/LegendarySocialNetwork.WebApi.csproj" -c Debug -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "LegendarySocialNetwork.WebApi.dll"]