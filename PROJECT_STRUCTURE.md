# Project Structure

## Solution Overview

**Solution File:** `AzPlayground.sln`

This is a .NET 8 monorepo containing modular Azure services with clear separation of concerns.

## Projects

### Source Projects (`src/`)

#### Main Applications

1. **AzureFunctions** (`src/functions/AzureFunctions.csproj`)
   - Target Framework: .NET 8
   - Type: Executable (Azure Functions Worker)
   - Dependencies: All utility libraries
   - Packages:
     - Microsoft.Azure.Functions.Worker
     - Microsoft.Azure.WebJobs.Extensions.*
     - Azure.* SDK packages

2. **AppService** (`src/appservice/AppService.csproj`)
   - Target Framework: .NET 8 (ASP.NET Core)
   - Type: Executable (Web Application)
   - Dependencies: All utility libraries
   - Packages:
     - Azure.Storage, Azure.Messaging, Azure.Security packages
     - Swashbuckle.AspNetCore (Swagger)

#### Utility Libraries

3. **AzureStorage** (`src/storage/AzureStorage.csproj`)
   - Blob, Queue, and File Share operations
   - Packages: Azure.Storage.* libraries

4. **ServiceBus** (`src/servicebus/ServiceBus.csproj`)
   - Queue and Topic messaging
   - Packages: Azure.Messaging.ServiceBus

5. **CosmosDB** (`src/cosmosdb/CosmosDB.csproj`)
   - NoSQL database operations
   - Packages: Microsoft.Azure.Cosmos

6. **KeyVault** (`src/keyvault/KeyVault.csproj`)
   - Secrets and keys management
   - Packages: Azure.Security.KeyVault.*

7. **EventGrid** (`src/eventgrid/EventGrid.csproj`)
   - Event publishing and handling
   - Packages: Azure.Messaging.EventGrid

## Build Instructions

```bash
# Restore packages and build all projects
dotnet build AzPlayground.sln

# Build specific project
dotnet build src/storage/AzureStorage.csproj

# Clean
dotnet clean AzPlayground.sln
```

## Project Dependencies

```
AzureFunctions
├── AzureStorage
├── ServiceBus
├── CosmosDB
├── KeyVault
└── EventGrid

AppService
├── AzureStorage
├── ServiceBus
├── CosmosDB
├── KeyVault
└── EventGrid
```

All utility libraries are independent class libraries that can be consumed separately.

## Key Features

- **Target Framework:** .NET 8 (LTS)
- **Language Features:** Nullable reference types enabled, Implicit usings enabled
- **Package Manager:** NuGet (dotnet CLI)
- **Azure SDK:** Latest stable versions (as of March 2026)

## Directory Layout

```
/
├── AzPlayground.sln                 # Main solution file
├── src/
│   ├── functions/                   # Azure Functions application
│   │   ├── AzureFunctions.csproj
│   │   └── Program.cs
│   ├── appservice/                  # Web API application
│   │   ├── AppService.csproj
│   │   └── Program.cs
│   ├── storage/                     # Storage library
│   │   └── AzureStorage.csproj
│   ├── servicebus/                  # Service Bus library
│   │   └── ServiceBus.csproj
│   ├── cosmosdb/                    # Cosmos DB library
│   │   └── CosmosDB.csproj
│   ├── keyvault/                    # Key Vault library
│   │   └── KeyVault.csproj
│   └── eventgrid/                   # Event Grid library
│       └── EventGrid.csproj
└── tests/                           # (Planned) Test projects
```

## Next Steps

1. Implement service-specific handlers and utilities in each library
2. Add integration tests in `tests/` directory
3. Create example implementations in Functions and AppService
4. Add Bicep/ARM templates in `infra/` directory
5. Configure CI/CD pipelines
