using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventHubs;
using System.Text;
using Microsoft.Extensions.Logging;

namespace IotReferenceArchitectureFunctions.Ingress
{
    public static class Ingress
    {
        [FunctionName("OnMessage")]
        public static void OnMessage([EventHubTrigger("messages/events", Connection = "sdfsdf")]EventData message, ILogger log)
        {
            log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.Array)}");
        }
    }
}