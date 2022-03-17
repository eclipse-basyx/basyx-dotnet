using BaSyx.AAS.Server.Http;
using BaSyx.API.ServiceProvider;
using BaSyx.Models.AdminShell;
using BaSyx.Utils.Settings;
using SimpleAssetAdministrationShell;
using System.Collections.Generic;

namespace AdminShellClientServerTests
{
    class Server
    {
        public static string ServerUrl = "http://localhost:5080";
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
            AssetAdministrationShellHttpServer aasServer = new AssetAdministrationShellHttpServer(settings);
            AssetAdministrationShell testShell = TestAssetAdministrationShell.GetAssetAdministrationShell();
            Submodel mainSubmodel = TestSubmodel.GetSubmodel("MainSubmodel");
            testShell.Submodels.Add(mainSubmodel);
            IAssetAdministrationShellServiceProvider aasServiceProvider = testShell.CreateServiceProvider(true);
            aasServer.SetServiceProvider(aasServiceProvider);
            aasServiceProvider.UseAutoEndpointRegistration(settings.ServerConfig);
            _ = aasServer.RunAsync();
        }
    }
}
