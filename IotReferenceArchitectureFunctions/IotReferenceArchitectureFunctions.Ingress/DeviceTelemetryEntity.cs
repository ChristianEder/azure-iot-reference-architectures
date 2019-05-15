using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace IotReferenceArchitectureFunctions.Ingress
{
    public class DeviceTelemetryEntity : TableEntity
    {
        public string TenantId { get; set; }
        public string DeviceId { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
    }
}