﻿using Structurizr.InfrastructureAsCode;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace IotReferenceArchitectureFunctions.Architecture.Model
{
    public class TelemetryStorage : ContainerWithInfrastructure<StorageAccount>
    {
        public TelemetryStorage(CloudBackend cloudBackend,
            IInfrastructureEnvironment environment)
        {
            Container = cloudBackend.System.AddContainer(
                name: "Telemetry Storage",
                description: "Stores all telemetry data from the devices",
                technology: "Azure Table Storage");

            Infrastructure = new StorageAccount
            {
                Kind = StorageAccountKind.StorageV2,
                Name = "reftelemetry" + environment.Name,
                EnvironmentInvariantName = "reftelemetry"
            };
        }
    }
}
