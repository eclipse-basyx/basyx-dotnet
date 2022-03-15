using BaSyx.API.ServiceProvider;
using BaSyx.Models.AdminShell;
using BaSyx.Submodel.Server.Http;
using BaSyx.Utils.Settings;
using SimpleAssetAdministrationShell;
using System;
using System.Collections.Generic;
using System.Text;

namespace SubmodelClientServerTests
{
    class Server
    {
        public static string ServerUrl = "http://localhost:5070";
        public static void Run()
        {
            ServerSettings settings = new ServerSettings()
            {
               ServerConfig = new ServerConfiguration()
               {
                   Hosting = new HostingConfiguration()
                   {
                       Urls = new List<string>() { ServerUrl }
                   }
               }
            };
            SubmodelHttpServer submodelServer = new SubmodelHttpServer(settings);
            Submodel testSubmodel = TestSubmodel.GetSubmodel("FirstSubmodel");
            ISubmodelServiceProvider submodelServiceProvider = testSubmodel.CreateServiceProvider();
            submodelServer.SetServiceProvider(submodelServiceProvider);
            submodelServiceProvider.UseAutoEndpointRegistration(settings.ServerConfig);
            _ = submodelServer.RunAsync();
        }
    }
}
