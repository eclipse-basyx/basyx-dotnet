using BaSyx.API.ServiceProvider;
using BaSyx.Common.UI.Swagger;
using BaSyx.Common.UI;
using BaSyx.Models.AdminShell;
using BaSyx.Servers.AdminShell.Http;
using NLog.Web;
using BaSyx.Utils.Settings;

namespace SubmodelRepository
{
    public class Program
    {
        static void Main(string[] args)
        {
            ServerSettings smRepositorySettings = ServerSettings.CreateSettings();
            smRepositorySettings.ServerConfig.Hosting.ContentPath = "Content";
            smRepositorySettings.ServerConfig.Hosting.Urls.Add("http://+:5080");
            smRepositorySettings.ServerConfig.Hosting.Urls.Add("https://+:5443");

            SubmodelRepositoryHttpServer server = new SubmodelRepositoryHttpServer(smRepositorySettings);
            server.WebHostBuilder.UseNLog();


            SubmodelRepositoryServiceProvider repositoryService = new SubmodelRepositoryServiceProvider();

            for (int i = 0; i < 3; i++)
            {
                Submodel submodel = new Submodel("MultiSubmodel_" + i, new BaSyxSubmodelIdentifier("MultiSubmodel_" + i, "1.0.0"))
                {
                    Description = new LangStringSet()
                    {
                       new LangString("de", i + ". Teilmodell"),
                       new LangString("en", i + ". Submodel")
                    },
                    Administration = new AdministrativeInformation()
                    {
                        Version = "1.0",
                        Revision = "120"
                    },
                    SubmodelElements = new ElementContainer<ISubmodelElement>()
                    {
                        new Property<string>("Property_" + i, "TestValue_" + i),
                        new SubmodelElementCollection("Coll_" + i)
                        {
                            Value =
                            {
                                Value =
                                {
                                    new Property<string>("SubProperty_" + i, "TestSubValue_" + i)
                                }
                            }
                        }
                    }
                };

                var submodelServiceProvider = submodel.CreateServiceProvider();
                repositoryService.RegisterSubmodelServiceProvider(submodel.Id, submodelServiceProvider);
            }

            repositoryService.UseAutoEndpointRegistration(server.Settings.ServerConfig);

            server.SetServiceProvider(repositoryService);

            server.AddBaSyxUI(PageNames.SubmodelRepositoryServer);

            server.AddSwagger(Interface.SubmodelRepository);

            server.Run();
        }

    }
}
