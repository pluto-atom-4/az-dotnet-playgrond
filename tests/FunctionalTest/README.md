# Azure Functions TODO App - Functional Test

End-to-end functional test for the Azure Functions TODO application that verifies both `PostTodoFunction` (HTTP trigger) and `ProcessTodoFunction` (Queue trigger) work correctly.

## Overview

This test performs a complete integration test of the TODO app pipeline:
1. **HTTP Input** - Simulates POST requests to `/api/todo`
2. **Queue Processing** - Sends and receives messages from Queue Storage
3. **Table Storage** - Creates and retrieves entities from Table Storage
4. **Data Integrity** - Verifies round-trip data consistency

## Prerequisites

- .NET 8.0 or later
- Azurite (Azure Storage emulator) running
- Azure Functions TODO App compiled

### Start Azurite

```bash
azurite
```

## Running the Test

```bash
# From this directory (tests/FunctionalTest)
dotnet run

# Or from project root
dotnet run --project tests/FunctionalTest/FunctionalTest.csproj
```

## Test Coverage

The test includes 7 test scenarios:

1. **PostTodoFunction Logic** - JSON serialization and queue operations
2. **Queue Storage Verification** - Message queueing and retrieval
3. **ProcessTodoFunction Logic** - Message deserialization and processing
4. **Table Storage Operations** - Entity creation and insertion
5. **Data Retrieval & Verification** - Data integrity round-trip
6. **Complete Pipeline Verification** - End-to-end flow validation
7. **Multiple Items Processing** - Batch processing verification

## Expected Output

All tests pass with green checkmarks (✓) and confirm:
- ✅ PostTodoFunction (HTTP Trigger) - WORKING
- ✅ ProcessTodoFunction (Queue Trigger) - WORKING
- ✅ Queue Storage Integration - WORKING
- ✅ Table Storage Integration - WORKING
- ✅ Complete Data Pipeline - WORKING

## Storage Configuration

The test uses Azurite development storage:
- **Connection String**: `UseDevelopmentStorage=true`
- **Queue Name**: `todo-queue`
- **Table Name**: `todotable`

## Notes

- The test uses `UpsertEntityAsync` (insert or update) to handle repeated test runs
- Queue and table are created automatically if they don't exist
- Tests are idempotent and can be run multiple times

## Files

- `Program.cs` - Main test entry point
- `FunctionalTest.csproj` - Project configuration

## References

- [Azure Functions TODO App Documentation](../../src/functions/README.md)
- [Function App Verification Report](../../FUNCTION_APP_VERIFICATION.md)
- [Test Results](../TEST_RESULTS.md)
