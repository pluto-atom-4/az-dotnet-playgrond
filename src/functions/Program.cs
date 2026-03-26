using Microsoft.Extensions.Hosting;

namespace AzureFunctions
{
    /// <summary>
    /// Azure Functions Worker application
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .Build();

            host.Run();
        }
    }
}
