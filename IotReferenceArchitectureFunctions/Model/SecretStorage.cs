using Structurizr.InfrastructureAsCode;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace IotReferenceArchitectureFunctions.Model
{
    public class SecretStorage : ContainerWithInfrastructure<KeyVault>
    {
        public SecretStorage(CloudBackend cloudBackend,
            IInfrastructureEnvironment environment)
        {
            Container = cloudBackend.System.AddContainer(
                "Secret storage",
                "Stores secrets that other services require to access each other",
                "Azure Key Vault");

            Infrastructure = new KeyVault
            {
                Name = "ref-keyvault-" + environment.Name
            };
        }
    }
}