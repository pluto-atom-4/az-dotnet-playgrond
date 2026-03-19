#!/bin/bash
set -e

# Source nvm and use LTS version of Node
export NVM_DIR="$HOME/.nvm"
[ -s "$NVM_DIR/nvm.sh" ] && \. "$NVM_DIR/nvm.sh"
nvm use --lts

# Create data directory if it doesn't exist
mkdir -p .azurite

# Start Azurite with all storage services
echo "Starting Azurite (Blob, Queue, Table storage emulator)..."
echo "Blob endpoint:  http://localhost:10000"
echo "Queue endpoint: http://localhost:10001"
echo "Table endpoint: http://localhost:10002"
echo ""

pnpm exec azurite --location .azurite --debug .azurite/azurite.log
