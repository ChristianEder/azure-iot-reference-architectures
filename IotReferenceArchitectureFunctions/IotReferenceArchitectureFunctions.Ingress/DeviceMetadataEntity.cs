using Microsoft.WindowsAzure.Storage.Table;

namespace IotReferenceArchitectureFunctions.Ingress
{
    public class DeviceMetadataEntity : TableEntity
    {
        public string TenantId { get; set; }
    }
}