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
