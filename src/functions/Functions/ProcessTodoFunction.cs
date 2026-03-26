using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using AzureFunctions.Models;
using Azure.Data.Tables;
using System.Text.Json;
using Azure.Storage.Queues.Models;

namespace AzureFunctions.Functions
{
    /// <summary>
    /// Queue-triggered function that processes TODO items and stores them in Table Storage
    /// Listens to messages from the 'todo-queue' and inserts them into 'todo-table'
    /// </summary>
    public class ProcessTodoFunction
    {
        private readonly TableClient _tableClient;
        private readonly ILogger<ProcessTodoFunction> _logger;

        public ProcessTodoFunction(ILogger<ProcessTodoFunction> logger)
        {
            _logger = logger;

            // Initialize table client for Azure Storage (Azurite in local development)
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                ?? "UseDevelopmentStorage=true";
            var tableServiceClient = new TableServiceClient(connectionString);
            _tableClient = tableServiceClient.GetTableClient("todotable");
        }

        [Function("ProcessTodo")]
        public async Task Run(
            [QueueTrigger("todo-queue")] QueueMessage message,
            FunctionContext executionContext)
        {
            try
            {
                // Get the message body
                var messageBody = message.Body.ToString();
                if (string.IsNullOrEmpty(messageBody))
                {
                    _logger.LogWarning("Received empty queue message");
                    return;
                }

                // Deserialize the TODO item from the queue message
                var todoItem = JsonSerializer.Deserialize<TodoItem>(messageBody);
                if (todoItem == null || string.IsNullOrEmpty(todoItem.Id))
                {
                    _logger.LogWarning("Received invalid TODO item from queue");
                    return;
                }

                // Log the processing
                _logger.LogInformation($"Processing TODO item from queue: Id={todoItem.Id}, Title={todoItem.Title}, Completed={todoItem.Completed}");

                // Create table entity for storage
                var tableEntity = new TodoTableEntity
                {
                    PartitionKey = "TODO",
                    RowKey = todoItem.Id,
                    Title = todoItem.Title,
                    Completed = todoItem.Completed
                };

                // Ensure table exists
                try
                {
                    await _tableClient.CreateAsync();
                }
                catch (Azure.RequestFailedException ex) when (ex.Status == 409)
                {
                    // Table already exists, continue
                }

                // Insert the entity into table storage
                await _tableClient.AddEntityAsync(tableEntity);
                _logger.LogInformation($"Successfully stored TODO item in table storage: Id={todoItem.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing TODO item: {ex.Message}");
                throw;
            }
        }
    }

    /// <summary>
    /// Table Storage entity for TODO items
    /// Maps TODO items to table storage with PartitionKey=TODO and RowKey=id
    /// </summary>
    public class TodoTableEntity : ITableEntity
    {
        /// <summary>
        /// Partition key for organizing TODO items (all use 'TODO')
        /// </summary>
        public string PartitionKey { get; set; } = string.Empty;

        /// <summary>
        /// Row key for uniquely identifying each TODO item (uses the ID)
        /// </summary>
        public string RowKey { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp of when the entity was created/modified (managed by Table Storage)
        /// </summary>
        public DateTimeOffset? Timestamp { get; set; }

        /// <summary>
        /// ETag for optimistic concurrency control (managed by Table Storage)
        /// </summary>
        public Azure.ETag ETag { get; set; }

        /// <summary>
        /// Title of the TODO item
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Completion status of the TODO item
        /// </summary>
        public bool Completed { get; set; }
    }
}
