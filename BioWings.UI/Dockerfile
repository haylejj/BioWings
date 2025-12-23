FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BioWings.UI/BioWings.UI.csproj", "BioWings.UI/"]
COPY ["BioWings.Application/BioWings.Application.csproj", "BioWings.Application/"]
COPY ["BioWings.Domain/BioWings.Domain.csproj", "BioWings.Domain/"]
COPY ["BioWings.Infrastructure/BioWings.Infrastructure.csproj", "BioWings.Infrastructure/"]
COPY ["BioWings.Persistence/BioWings.Persistence.csproj", "BioWings.Persistence/"]
RUN dotnet restore "./BioWings.UI/BioWings.UI.csproj"
COPY . .
WORKDIR "/src/BioWings.UI"
RUN dotnet build "./BioWings.UI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BioWings.UI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BioWings.UI.dll"]
