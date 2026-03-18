# Start from here for Azure Dot Net SDK playground

## Install the CLI tools

### Azure CLI (az)
Python 3.13 virtual environment has been configured with `uv` and `pyproject.toml`:

```bash
# Activate the virtual environment
source .venv/bin/activate

# Verify Azure CLI installation
az --version

# To use Azure CLI, always activate the venv first
source .venv/bin/activate
az login
```

The environment is configured with:
- **Python version**: 3.13.5
- **Package manager**: uv
- **Dependencies**: `azure-cli` (latest stable)
- **Virtual environment**: `.venv/` (excluded from git)

### AZD (Azure Developer CLI)
✅ **Already installed!** Azure Developer CLI v1.23.10 is installed at `~/.local/bin/azd`

To verify:
```bash
azd version
```

To use Azure Developer CLI for project initialization and deployment:
```bash
azd init          # Initialize a new project
azd auth login    # Authenticate with Azure
azd up            # Deploy infrastructure and app
```

For more commands and options:
```bash
azd help
```

### Function App
(To be documented)

## Organize C# and .NET projects using the monorepo styles
- The target applications and services are: Azure Function, App Service, Service Bus, Azure Account Storage, CosmosDB, EventGrid, Azure Key Vault.


## Add a README.md and intilize Claude Code

## Initialize Git repository and .gitignore which exclude the Jetbrains or VS Cocde specific configuration.

 

