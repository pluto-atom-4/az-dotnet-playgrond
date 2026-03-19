# Azure .NET SDK Playground

A monorepo for exploring and experimenting with Azure services using .NET and C#.

## Project Structure

```
├── src/
│   ├── functions/           # Azure Functions
│   ├── appservice/          # App Service applications
│   ├── servicebus/          # Service Bus integrations
│   ├── storage/             # Azure Storage/Blob operations
│   ├── cosmosdb/            # CosmosDB operations
│   ├── eventgrid/           # Event Grid integrations
│   └── keyvault/            # Key Vault integrations
├── tests/                   # Unit and integration tests
├── infra/                   # Infrastructure as Code (Bicep/ARM templates)
└── docs/                    # Documentation
```

## Prerequisites

### Required Tools

- **.NET SDK 8.0+** - Install from [dotnet.microsoft.com](https://dotnet.microsoft.com/download)
- **Azure CLI (AZ)** - Install via pip without sudo:
  ```bash
  python3 -m venv venv
  source venv/bin/activate
  pip install azure-cli --user
  ```
- **Azure Developer CLI (AZD)** - Download from [Azure/azure-dev releases](https://github.com/Azure/azure-dev/releases)
  ```bash
  wget https://github.com/Azure/azure-dev/releases/download/azure-dev-cli_<version>_amd64.deb
  sudo dpkg -i azure-dev-cli_<version>_amd64.deb
  ```

### Optional Tools

- **Azure Functions Core Tools** - For local function development
- **Visual Studio Code** - Recommended editor with C# extension
- **JetBrains Rider** - Alternative IDE for .NET development

## Getting Started

1. Clone the repository
2. Install dependencies:
   ```bash
   dotnet restore
   ```
3. Build the solution:
   ```bash
   dotnet build
   ```
4. Run tests:
   ```bash
   dotnet test
   ```

## Local Emulators

For local development and testing without connecting to Azure, use the following emulators:

### Azurite (Storage Emulator)

Azurite emulates Azure Blob, Queue, and Table storage services locally.

**Setup:**
```bash
# Use Node.js LTS (nvm reads from .nvmrc)
nvm use --lts

# Install dependencies
pnpm install

# Start Azurite
pnpm run azurite
# Or use the shell script
./scripts/start-azurite.sh
```

**Endpoints:**
- Blob Storage: `http://localhost:10000`
- Queue Storage: `http://localhost:10001`
- Table Storage: `http://localhost:10002`

**Connection String (default development account):**
```
UseDevelopmentStorage=true
```

Or use the full connection string:
```
DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://localhost:10000/devstoreaccount1;QueueEndpoint=http://localhost:10001/devstoreaccount1;TableEndpoint=http://localhost:10002/devstoreaccount1;
```

Data is persisted in `.azurite/` directory.

### CosmosDB Emulator (Linux)

CosmosDB NoSQL (SQL API) emulator runs in Docker. **Note:** Table, Cassandra, and Gremlin APIs are not supported on the Linux Docker emulator.

**Prerequisites:**
- Docker with rootless mode (see [docker-rootless-debian13.md](docs/docker-rootless-debian13.md))

**Setup:**
```bash
# Start CosmosDB emulator
docker-compose up cosmosdb

# To run in background
docker-compose up -d cosmosdb
```

**Endpoints:**
- API Endpoint: `https://localhost:8081`
- Data Explorer: `http://localhost:1234`

**Connection String (static, well-known key):**
```
AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==;
```

**Important:** When using the CosmosDB emulator from .NET code, set `ConnectionMode` to `Gateway` and disable certificate validation for local dev:
```csharp
var client = new CosmosClient(
    connectionString,
    new CosmosClientOptions
    {
        ConnectionMode = ConnectionMode.Gateway,
        // For local emulator only: disable cert validation
        HttpClientFactory = () => new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        })
    }
);
```

Data is persisted in `.azurite/cosmosdb/` directory.

**Stop the emulator:**
```bash
docker-compose down
```

## Target Azure Services

This playground covers implementations of:

- **Azure Functions** - Serverless compute
- **App Service** - Web hosting and APIs
- **Service Bus** - Messaging and event processing
- **Azure Storage** - Blob, Queue, and Table storage
- **CosmosDB** - NoSQL database
- **Event Grid** - Event routing and delivery
- **Azure Key Vault** - Secrets management

## Contributing

See individual project READMEs in `src/` subdirectories for service-specific details.

## License

MIT
