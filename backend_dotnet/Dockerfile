FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ViberLounge.API/ViberLounge.API.csproj", "ViberLounge.API/"]
COPY ["ViberLounge.Application/ViberLounge.Application.csproj", "ViberLounge.Application/"]
COPY ["ViberLounge.Domain/ViberLounge.Domain.csproj", "ViberLounge.Domain/"]
COPY ["ViberLounge.Infrastructure/ViberLounge.Infrastructure.csproj", "ViberLounge.Infrastructure/"]

RUN dotnet restore "ViberLounge.API/ViberLounge.API.csproj"

COPY . .
WORKDIR "/src/ViberLounge.API"
RUN dotnet build "ViberLounge.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ViberLounge.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ViberLounge.API.dll"]
