using BaSyx.Registry.ReferenceImpl.FileBased;
using BaSyx.Registry.Server.Http;
using BaSyx.Utils.Settings;
using System.Collections.Generic;

namespace RegistryClientServerTests
{
    class Server
    {
        public static string ServerUrl = "http://localhost:4999";
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
            RegistryHttpServer registryServer = new RegistryHttpServer(settings);
            FileBasedRegistry fileBasedRegistry = new FileBasedRegistry();
            registryServer.SetRegistryProvider(fileBasedRegistry);
            _ = registryServer.RunAsync();
        }
    }
}
