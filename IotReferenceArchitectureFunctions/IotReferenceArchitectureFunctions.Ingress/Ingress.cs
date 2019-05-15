using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventHubs;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace IotReferenceArchitectureFunctions.Ingress
{
    public static class Ingress
    {
        private static readonly Random Random = new Random();

        [FunctionName("OnMessage")]
        [return: Table("telemetry", Connection = "reftelemetry-connection-string")]
        public static async Task<DeviceTelemetryEntity> OnMessage(
            [EventHubTrigger("messages/events", Connection = "ref-hub-connection")]EventData message,
            [Table("device-metadata", Connection = "reftelemetry-connection-string")] CloudTable deviceMetaData,
            ILogger log)
        {
            var dtm = JsonConvert.DeserializeObject<DeviceTelemetryMessage>(Encoding.UTF8.GetString(message.Body.ToArray()));
            var deviceId = message.SystemProperties["iothub-connection-device-id"].ToString();

            var tenantId = await GetTenantId(deviceId, deviceMetaData);
            var entity = new DeviceTelemetryEntity
            {
                TenantId = tenantId,
                DeviceId = deviceId,
                Humidity = dtm.Humidity,
                Temperature = dtm.Temperature
            };
            entity.PartitionKey = entity.TenantId + "-" + entity.DeviceId;
            entity.RowKey = (long.MaxValue - DateTime.UtcNow.Ticks).ToString();

            return entity;
        }

        private static async Task<string> GetTenantId(string deviceId, CloudTable deviceMetaData)
        {
            var retrieve = await deviceMetaData.ExecuteAsync(TableOperation.Retrieve<DeviceMetadataEntity>(deviceId, deviceId));

            if (retrieve.Result is DeviceMetadataEntity metadata)
            {
                return metadata.TenantId;
            }

            var allTenants = new[] { "tenantA", "tenantB", "tenantC", "tenantD", "tenantE" };
            metadata = new DeviceMetadataEntity
            {
                PartitionKey = deviceId,
                RowKey = deviceId,
                TenantId = allTenants[Random.Next(allTenants.Length)]
            };

            await deviceMetaData.ExecuteAsync(TableOperation.InsertOrReplace(metadata));
            return metadata.TenantId;
        }

    }
}