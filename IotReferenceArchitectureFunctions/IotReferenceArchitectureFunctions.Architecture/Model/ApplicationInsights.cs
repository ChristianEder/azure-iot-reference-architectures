using Structurizr.InfrastructureAsCode;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace IotReferenceArchitectureFunctions.Architecture.Model
{
    public class ApplicationInsights : ContainerWithInfrastructure<Structurizr.InfrastructureAsCode.Azure.Model.ApplicationInsights>
    {

        public ApplicationInsights(CloudBackend cloudBackend, 
            IInfrastructureEnvironment environment)
        {

            Container = cloudBackend.System.AddContainer(
                name: "Application Insights",
                description: "Serves as a logging target and monitoring tool",
                technology: "Azure Application Insights");

            Infrastructure = new Structurizr.InfrastructureAsCode.Azure.Model.ApplicationInsights
            {
                Name = "ref-ai-" + environment.Name
            };
        }


    }
}
