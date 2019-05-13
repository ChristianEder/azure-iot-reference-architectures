using Structurizr.InfrastructureAsCode;
using Structurizr.InfrastructureAsCode.Azure.Model;
using Structurizr.InfrastructureAsCode.InfrastructureRendering;

namespace IotReferenceArchitectureFunctions.Architecture.Model
{
    public class MasterDataStorage : ContainerWithInfrastructure<SqlDatabase>
    {
        public MasterDataStorage(CloudBackend cloudBackend, SqlServer sqlServer, 
            IInfrastructureEnvironment environment)
        {
            Container = cloudBackend.System.AddContainer(
                name: "Master Data Storage",
                description: "Stores master data",
                technology: "Azure SQL Database");

            Infrastructure = new SqlDatabase (sqlServer.Infrastructure)
            {
                Name = "masterdata" + environment.Name
            };
        }
    }
}
