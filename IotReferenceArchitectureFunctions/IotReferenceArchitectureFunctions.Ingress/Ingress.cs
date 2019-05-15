using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventHubs;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Newtonsoft.Json.Linq;

namespace IotReferenceArchitectureFunctions.Ingress
{
    public static class Ingress
    {
        private static readonly Random Random = new Random();
        private static readonly string InstrumentationKey = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");
        private static readonly TelemetryClient GlobalTelemetryClient = new TelemetryClient(new TelemetryConfiguration(InstrumentationKey));

        [FunctionName("OnMessage")]
        public static async Task OnMessage(
            [EventHubTrigger("messages/events", Connection = "ref-hub-connection")]EventData[] messages,
            [Table("devicemetadata", Connection = "reftelemetry-connection-string")] CloudTable deviceMetaData,
            [Table("telemetry", Connection = "reftelemetry-connection-string")] CloudTable telemetry,
            ILogger log)
        {
            var entities = new List<DeviceTelemetryEntity>();

            foreach (var message in messages)
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

                JObject properties = new JObject();
                foreach (var property in message.SystemProperties)
                {
                    properties[property.Key] = property.Value?.ToString();
                }

                foreach (var property in message.Properties)
                {
                    properties[property.Key] = property.Value?.ToString();
                }

                entity.Properties = properties.ToString();
                entities.Add(entity);

                try
                {
                    // Result: can handle ~ 100 - 200 messages / second
                    if (message.SystemProperties.TryGetValue("iothub-creation-time-utc", out var enq) ||
                        message.Properties.TryGetValue("iothub-creation-time-utc", out enq))
                    {
                        var enquedAt = DateTime.ParseExact(enq.ToString(), "yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture).ToUniversalTime();
                        var nowUtc = DateTime.UtcNow;
                        var queuedFor = (nowUtc - enquedAt).TotalMilliseconds;
                        GlobalTelemetryClient.TrackMetric("MessageStuckInQueue", queuedFor);
                    }
                }
                catch
                {
                }
            }

            var tasks = new List<Task>();
            foreach (var entityGroup in entities.GroupBy(e => e.PartitionKey))
            {
                var batch = new TableBatchOperation();
                foreach (var entity in entityGroup)
                {
                    batch.Add(TableOperation.Insert(entity));
                }
                tasks.Add(telemetry.ExecuteBatchAsync(batch));
            }

            await Task.WhenAll(tasks);
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