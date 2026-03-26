namespace AzureFunctions.Models
{
    /// <summary>
    /// Represents a TODO item
    /// </summary>
    public class TodoItem
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public bool Completed { get; set; }
    }
}
