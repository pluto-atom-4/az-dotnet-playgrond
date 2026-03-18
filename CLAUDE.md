# Claude Code Configuration

This is an Azure .NET SDK playground monorepo project.

## Project Overview

- **Type**: .NET/C# monorepo
- **Purpose**: Explore and experiment with Azure services
- **Structure**: Modular services in `src/` directory
- **Tech Stack**: .NET 8+, C#, Azure SDK

## Key Areas

### Services
- Azure Functions
- App Service
- Service Bus
- Azure Storage
- CosmosDB
- Event Grid
- Azure Key Vault

### Development
- Use `dotnet` CLI for building/testing
- Follow C# naming conventions (PascalCase for classes/methods)
- Maintain clear separation between services
- Each service has its own project file

## Useful Commands

```bash
# Build all projects
dotnet build

# Run specific project
dotnet run --project src/functions/MyFunction.csproj

# Run tests
dotnet test

# Add NuGet package
dotnet add package <PackageName>

# Azure authentication
az login
```

## Notes

- Each service directory should contain a local README with service-specific details
- Infrastructure code goes in `infra/` (Bicep or ARM templates)
- Tests are organized in `tests/` mirror the source structure
