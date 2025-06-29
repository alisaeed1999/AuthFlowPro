# Use the official .NET 9 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5063
EXPOSE 7084

# Use the official .NET 9 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/AuthFlowPro.API/AuthFlowPro.API.csproj", "src/AuthFlowPro.API/"]
COPY ["src/AuthFlowPro.Application/AuthFlowPro.Application.csproj", "src/AuthFlowPro.Application/"]
COPY ["src/AuthFlowPro.Domain/AuthFlowPro.Domain.csproj", "src/AuthFlowPro.Domain/"]
COPY ["src/AuthFlowPro.Infrastructure/AuthFlowPro.Infrastructure.csproj", "src/AuthFlowPro.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/AuthFlowPro.API/AuthFlowPro.API.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/src/AuthFlowPro.API"
RUN dotnet build "AuthFlowPro.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "AuthFlowPro.API.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Install Entity Framework tools for migrations
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

ENTRYPOINT ["dotnet", "AuthFlowPro.API.dll"]