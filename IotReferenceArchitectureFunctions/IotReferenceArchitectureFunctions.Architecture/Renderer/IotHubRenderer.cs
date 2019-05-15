using Newtonsoft.Json.Linq;

namespace IotReferenceArchitectureFunctions.Architecture.Renderer
{
    public class IoTHubRenderer : Structurizr.InfrastructureAsCode.Azure.ARM.IoTHubRenderer
    {
        protected override JObject PostProcess(JObject template)
        {
            template = base.PostProcess(template);
            template["sku"]["name"] = "S1";
            return template;
        }
    }
}
