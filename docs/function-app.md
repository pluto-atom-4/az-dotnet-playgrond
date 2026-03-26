# Function App

## Target Azure Services

This playground covers implementations of: 
- **Azure Functions** - Serverless compute
- **Azure Storage** - Blob, Queue, and Table storage

- Location: 
  - `src/FunctionApp`
  - `tests/FunctionApp.Tests` 

## Functions Features 
- `HttpTrigger` 
  - HTTP-triggered function that responds to POST requests at `/api/todo` and logs the request body.
  - Example request body:
    ```json
    {
      "id": "1",
      "title": "Buy groceries",
      "completed": false
    }
    ```
  - outputs a log message like:
    ```text
    [Information] Received TODO item: Id=1, Title=Buy groceries, Completed=False
    ```
  - binding to Storage Queue `todo-queue` with the same TODO item as a message

- `QueueTrigger` - 
  - Queue-triggered function that listens to `todo-queue` and logs the received message.
  - Example log message:
    ```
    [Information] Processing TODO item from queue: Id=1, Title=Buy groceries, Completed=False
    ```
  - Binding to Table Storage `todo-table` to insert the TODO item as an entity with `PartitionKey=TODO` and `RowKey=id`

## Azurite for Local Development
- Azurite is a lightweight emulator for Azure Storage services (Blob, Queue, Table)
- Allows you to develop and test Azure Functions locally without connecting to Azure
- Supports the same APIs as Azure Storage, so you can switch to the real service with minimal changes
- Data is persisted in the `.azurite/` directory, so you can stop
- Configure the storage table and queue for the functions to point to the local Azurite instance using the connection string:
  ```
  UseDevelopmentStorage=true
  ```