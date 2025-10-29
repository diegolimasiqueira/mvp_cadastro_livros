# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY src/BookStore.Domain/BookStore.Domain.csproj src/BookStore.Domain/
COPY src/BookStore.Application/BookStore.Application.csproj src/BookStore.Application/
COPY src/BookStore.Infrastructure/BookStore.Infrastructure.csproj src/BookStore.Infrastructure/
COPY src/BookStore.API/BookStore.API.csproj src/BookStore.API/

# Restore dependencies
RUN dotnet restore src/BookStore.API/BookStore.API.csproj

# Copy source code
COPY src/ src/

# Build the application
WORKDIR /src/src/BookStore.API
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "BookStore.API.dll"]



