using Newtonsoft.Json;

namespace IotReferenceArchitectureFunctions.Ingress
{
    public class DeviceTelemetryMessage
    {
        [JsonProperty("temperature")]
        public double Temperature { get; set; }
        [JsonProperty("humidity")]
        public double Humidity { get; set; }
    }
}