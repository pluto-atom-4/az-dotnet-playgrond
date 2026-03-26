# Azure Functions - TODO App

This is a serverless function app that demonstrates Azure Functions integration with Azure Storage (Queues and Tables). It implements a two-tier processing pipeline: HTTP input → Queue → Table Storage.

## Architecture

The application follows a producer-consumer pattern:
1. **PostTodo** (HTTP trigger) receives TODO items and queues them
2. **ProcessTodo** (queue trigger) processes queued items and stores them durably in table storage

## Functions

### 1. PostTodo (HTTP Trigger)
- **Route**: `POST /api/todo`
- **Purpose**: Accepts TODO items via HTTP and queues them for asynchronous processing
- **Request Body**:
  ```json
  {
    "id": "1",
    "title": "Buy groceries",
    "completed": false
  }
  ```
- **Logging**:
  ```
  [Information] Received HTTP request to POST /api/todo
  [Information] Received TODO item: Id=1, Title=Buy groceries, Completed=False
  [Information] TODO item queued for processing: 1
  ```
- **Responses**:
  | Status | Description |
  |--------|-------------|
  | **202 Accepted** | TODO item queued successfully |
  | **400 Bad Request** | Empty request body or invalid JSON |
  | **500 Internal Server Error** | Unexpected processing error |
- **Example Response** (202):
  ```json
  {
    "message": "TODO item queued for processing",
    "id": "1",
    "title": "Buy groceries"
  }
  ```

### 2. ProcessTodo (Queue Trigger)
- **Trigger**: Messages from `todo-queue`
- **Purpose**: Processes TODO items from the queue and durably stores them in Table Storage
- **Logging**:
  ```
  [Information] Processing TODO item from queue: Id=1, Title=Buy groceries, Completed=False
  [Information] Successfully stored TODO item in table storage: Id=1
  ```
- **Table Storage Destination**: `todo-table` with:
  - **PartitionKey**: `TODO` (groups all TODO items together)
  - **RowKey**: Item `id` (unique identifier within the partition)
  - **Properties**: `Title`, `Completed`, `Timestamp`, `ETag`
- **Error Handling**:
  - Empty or invalid messages are logged and skipped
  - Table creation is idempotent (safe to retry)
  - Exceptions are logged and propagated to Function host for retry policy

## Local Development with Azurite

### Prerequisites
- .NET 8.0 SDK
- Azure Functions Core Tools (v4+)
- Node.js (for running Azurite)
- Azurite emulator

### Setup & Execution

#### 1. Start Azurite Storage Emulator
```bash
# In the project root directory
azurite --silent --location .azurite
```
This starts:
- Blob Storage: `http://127.0.0.1:10000`
- Queue Storage: `http://127.0.0.1:10001`
- Table Storage: `http://127.0.0.1:10002`

#### 2. Build the Functions Project
```bash
cd src/functions
dotnet build
```

#### 3. Run the Functions Locally
```bash
func start
```
Expected output:
```
PostTodo: [POST] http://localhost:7071/api/todo
ProcessTodo: queueTrigger-todo-queue
```

#### 4. Test the HTTP Trigger

**Success Case - Valid TODO item**:
```bash
curl -X POST http://localhost:7071/api/todo \
  -H "Content-Type: application/json" \
  -d '{"id":"1","title":"Buy groceries","completed":false}'
```

**Expected Response** (202 Accepted):
```json
{
  "message": "TODO item queued for processing",
  "id": "1",
  "title": "Buy groceries"
}
```

**Error Case - Missing ID**:
```bash
curl -X POST http://localhost:7071/api/todo \
  -H "Content-Type: application/json" \
  -d '{"title":"Buy groceries","completed":false}'
```

**Expected Response** (400 Bad Request):
```json
{
  "error": "Invalid TODO item - must include id, title, and completed fields"
}
```

### Configuration

#### Connection Strings
The app reads the Azure Storage connection string from the `AzureWebJobsStorage` environment variable defined in `local.settings.json`:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
  }
}
```

#### Special Connection String: UseDevelopmentStorage
- `UseDevelopmentStorage=true` is a special connection string that automatically points to Azurite
- Default account name: `devstoreaccount1`
- Default account key: `Eby8vdM02xNHdbNBiYQQ+DhD4FkBzKM6NPvX6cBYNGI=`
- No authentication required for local development

#### Storage Resources Created Automatically
- **Queue**: `todo-queue` - Message buffer between HTTP and processing
- **Table**: `todo-table` - Persistent storage for processed TODO items

### Monitoring Execution

#### Azure Storage Explorer
You can view queued messages and table data using Azure Storage Explorer:
1. Open Azure Storage Explorer
2. Connect to Local Emulator
3. Browse `devstoreaccount1`:
   - Queues → `todo-queue` (messages waiting to be processed)
   - Tables → `todo-table` (processed TODO items)

## Azure Deployment

### Prerequisites
- Azure subscription
- Azure Storage account (or use an existing one)
- Azure Function App resource

### Deployment Steps

1. **Prepare Production Configuration**:
   ```bash
   # Update local.settings.json with your Azure Storage connection string
   # DO NOT commit secrets to version control
   ```

2. **Authenticate with Azure**:
   ```bash
   func azure login
   ```

3. **Publish the Function App**:
   ```bash
   func azure functionapp publish <FunctionAppName>
   ```

4. **Configure Environment in Azure Portal**:
   - Go to Function App → Configuration → Application settings
   - Update `AzureWebJobsStorage` with your production storage account connection string
   - Set `FUNCTIONS_WORKER_RUNTIME` to `dotnet-isolated`

### Production Considerations
- Use **Azure Key Vault** to store connection strings
- Enable **Application Insights** for monitoring
- Configure **CORS** if calling from web browsers
- Set appropriate **Function App tier** based on expected load
- Enable **Managed Identity** for secure Azure Storage access

## Project Structure

```
src/functions/
├── Functions/
│   ├── PostTodoFunction.cs        - HTTP trigger (receives TODO items)
│   └── ProcessTodoFunction.cs     - Queue trigger (processes and stores items)
├── Models/
│   └── TodoItem.cs                - Shared TODO data model
├── Program.cs                     - Function host initialization
├── local.settings.json            - Local development configuration
├── host.json                      - Function host configuration
├── .env                           - Environment variables (local only)
├── AzureFunctions.csproj          - Project configuration
└── README.md                      - This file
```

## Dependencies

### Azure Functions
- `Microsoft.Azure.Functions.Worker` (v1.21.0) - Azure Functions Worker SDK
- `Microsoft.Azure.Functions.Worker.Extensions.Http` (v3.3.0) - HTTP trigger support
- `Microsoft.Azure.Functions.Worker.Extensions.Storage` (v6.0.0) - Storage bindings

### Azure Storage
- `Azure.Storage.Queues` (v12.18.0) - Queue Storage operations
- `Azure.Data.Tables` (v12.8.0) - Table Storage operations

### Other
- `Microsoft.Extensions.Logging.Abstractions` - Structured logging

## Error Handling & Resilience

### HTTP Trigger (PostTodo)
- **Validates** JSON format before queueing
- **Returns 400** for invalid input (fail-fast)
- **Catches JsonException** separately for clarity
- **Generic exception handler** for unexpected errors

### Queue Trigger (ProcessTodo)
- **Skips invalid messages** without failing
- **Idempotent table creation** (safe to retry)
- **Logs all operations** for debugging
- **Propagates exceptions** to Function host for automatic retry

### Best Practices Implemented
✓ Structured logging with `ILogger<T>`
✓ Specific exception handling
✓ Dependency injection for services
✓ Configuration via environment variables
✓ Async/await throughout
✓ XML documentation on public types
