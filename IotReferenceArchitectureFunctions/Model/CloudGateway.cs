using Structurizr.InfrastructureAsCode;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace IotReferenceArchitectureFunctions.Model
{
    public class CloudGateway : ContainerWithInfrastructure<IoTHub>
    {
        public CloudGateway(CloudBackend cloudBackend, IInfrastructureEnvironment environment)
        {
            Container = cloudBackend.System.AddContainer(
                name: "Cloud Gateway",
                description: "Receives incoming messages from the device",
                technology: "Azure IoT Hub");

            Infrastructure = new IoTHub
            {
                Name = "ref-hub-" + environment.Name,
                EnvironmentInvariantName = "ref-hub"
            };
        }
    }
}
