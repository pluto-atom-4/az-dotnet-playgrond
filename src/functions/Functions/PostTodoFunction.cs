using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using AzureFunctions.Models;
using System.Net;
using System.Text.Json;
using Azure.Storage.Queues;

namespace AzureFunctions.Functions
{
    /// <summary>
    /// HTTP-triggered function that receives TODO items and queues them for processing
    /// Accepts POST requests at /api/todo and sends TODO items to the todo-queue
    /// </summary>
    public static class PostTodoFunction
    {
        private static readonly QueueClient _queueClient;

        static PostTodoFunction()
        {
            // Initialize queue client for Azure Storage (Azurite in local development)
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                ?? "UseDevelopmentStorage=true";
            _queueClient = new QueueClient(connectionString, "todo-queue");
        }

        [Function("PostTodo")]
        public static async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequestData req,
            ILogger log)
        {
            log.LogInformation("Received HTTP request to POST /api/todo");

            try
            {
                // Read the request body
                var requestBody = await req.ReadAsStringAsync();
                if (string.IsNullOrEmpty(requestBody))
                {
                    log.LogWarning("Request body is empty");
                    var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badRequest.WriteAsJsonAsync(new { error = "Request body is empty" });
                    return badRequest;
                }

                // Deserialize the TODO item from the request
                var todoItem = JsonSerializer.Deserialize<TodoItem>(requestBody);

                if (todoItem == null || string.IsNullOrEmpty(todoItem.Id))
                {
                    log.LogWarning("Failed to deserialize TODO item from request body");
                    var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badRequest.WriteAsJsonAsync(new { error = "Invalid TODO item - must include id, title, and completed fields" });
                    return badRequest;
                }

                // Log the received TODO item
                log.LogInformation($"Received TODO item: Id={todoItem.Id}, Title={todoItem.Title}, Completed={todoItem.Completed}");

                // Serialize and queue the TODO item for processing
                var message = JsonSerializer.Serialize(todoItem);
                await _queueClient.SendMessageAsync(message);
                log.LogInformation($"TODO item queued for processing: {todoItem.Id}");

                // Return 202 Accepted response
                var response = req.CreateResponse(HttpStatusCode.Accepted);
                await response.WriteAsJsonAsync(new
                {
                    message = "TODO item queued for processing",
                    id = todoItem.Id,
                    title = todoItem.Title
                });
                return response;
            }
            catch (JsonException ex)
            {
                log.LogError("JSON deserialization error: {ExMessage}", ex.Message);
                var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await errorResponse.WriteAsJsonAsync(new { error = "Invalid JSON in request body" });
                return errorResponse;
            }
            catch (Exception ex)
            {
                log.LogError($"Unexpected error processing TODO item: {ex.Message}");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteAsJsonAsync(new { error = "Internal server error" });
                return errorResponse;
            }
        }
    }
}
