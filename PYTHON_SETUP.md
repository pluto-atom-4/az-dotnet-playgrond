# Python Environment Setup

## Overview
This project uses Python 3.13 with `uv` package manager for Azure CLI tooling in a .NET monorepo.

## Quick Start

### Activate the Virtual Environment
```bash
source .venv/bin/activate
```

### Verify Installation
```bash
az --version
python3 --version
```

## Configuration

- **Python Version**: 3.13.5
- **Package Manager**: uv (fast, reliable Python package manager)
- **Configuration File**: `pyproject.toml`
- **Virtual Environment Location**: `.venv/` (local, git-ignored)

### pyproject.toml
```toml
[project]
name = "az-dotnet-playground"
version = "0.1.0"
description = "Azure .NET SDK playground monorepo"
requires-python = ">=3.13"
dependencies = [
    "azure-cli",
]
```

## Usage

### Azure CLI Commands
Once the venv is activated, use Azure CLI normally:

```bash
source .venv/bin/activate

# Login to Azure
az login

# List subscriptions
az account list

# Check resources in a resource group
az resource list --resource-group <group-name>
```

### Adding New Python Dependencies
To add additional packages while preserving the environment:

```bash
source .venv/bin/activate
uv pip install <package-name>
```

## Troubleshooting

### Virtual Environment Not Activated
If `az` command is not found, ensure the venv is activated:
```bash
source .venv/bin/activate
```

### Python Version Mismatch
Verify Python 3.13 is being used:
```bash
python3 --version
```

## Integration with .NET Project

The Python environment is separate from the .NET stack:
- **Python**: Azure CLI, automation scripts, DevOps tools
- **.NET**: Core applications (Functions, App Service, etc.)

Always activate the venv when working with Azure CLI tools.
