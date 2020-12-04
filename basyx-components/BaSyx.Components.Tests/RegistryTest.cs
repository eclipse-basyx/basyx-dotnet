using BaSyx.API.Components;
using BaSyx.Models.Connectivity;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using BaSyx.Registry.Client.Http;
using BaSyx.Registry.ReferenceImpl.FileBased;
using BaSyx.Registry.Server.Http;
using BaSyx.Utils.ResultHandling;
using BaSyx.Utils.Settings.Sections;
using BaSyx.Utils.Settings.Types;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace BaSyx.Components.Tests
{
    [TestClass]
    public class RegistryTest : IAssetAdministrationShellRegistry
    {
        private static RegistryHttpServer registryHttpServer;
        private static RegistryHttpClient registryHttpClient;

        static RegistryTest()
        {
            ServerSettings registrySettings = ServerSettings.CreateSettings();
            registrySettings.ServerConfig.Hosting = new HostingConfiguration()
            {
                Urls = new List<string>()
                {
                    "http://localhost:4999",
                    "https://localhost:4499"
                },
                Environment = "Development",
                ContentPath = "Content"
            };

            registryHttpServer = new RegistryHttpServer(registrySettings);
            registryHttpServer.SetRegistryProvider(new FileBasedRegistry());
            _ = registryHttpServer.RunAsync();

            RegistryClientSettings registryClientSettings = RegistryClientSettings.LoadSettings();
            registryHttpClient = new RegistryHttpClient(registryClientSettings);
        }

        

        private static IAssetAdministrationShell aas = new AssetAdministrationShell("MyTestAAS", new Identifier("https://www.basys40.de/shells/MyTestAAS", KeyType.IRI))
        {
            Asset = new Asset("MyTestAsset", new Identifier("https://www.basys40.de/assets/MyTestAsset", KeyType.IRI))
            {
                Kind = AssetKind.Instance
            },
            Administration = new AdministrativeInformation()
            {
                Version = "1.0.0",
                Revision = "0.0.1"
            },
            Description = new LangStringSet()
            {
                new LangString(new CultureInfo("de").TwoLetterISOLanguageName, "Meine Test-Verwaltungsschale"),
                new LangString(new CultureInfo("en").TwoLetterISOLanguageName, "My Test Asset Administration Shell"),
            },
            Submodels = new ElementContainer<ISubmodel>()
            {
                new BaSyx.Models.Core.AssetAdministrationShell.Implementations.Submodel("MyTestSubmodel", new Identifier("https://www.basys40.de/submodels/MyTestSubmodel", KeyType.IRI))
                {
                    SemanticId = new Reference(new Key(KeyElements.GlobalReference, KeyType.IRI, "urn:basys:org.eclipse.basyx:submodels:MyTestSubmodel:1.0.0", false))
                }
            }
        };

        private static ISubmodel submodel = new BaSyx.Models.Core.AssetAdministrationShell.Implementations.Submodel("MyAdditionalTestSubmodel", new Identifier("https://www.basys40.de/submodels/MyAdditionalTestSubmodel", KeyType.IRI))
        {
            SemanticId = new Reference(new Key(KeyElements.GlobalReference, KeyType.IRI, "urn:basys:org.eclipse.basyx:submodels:MyAdditionalTestSubmodel:1.0.0", false))
        };

        private static IAssetAdministrationShellDescriptor aasDescriptor = new AssetAdministrationShellDescriptor(aas, new List<IEndpoint>()
        {
            new HttpEndpoint("http://localhost:5080/aas")
        });

        private static ISubmodelDescriptor submodelDescriptor = new SubmodelDescriptor(submodel, new List<IEndpoint>()
        {
            new HttpEndpoint("http://localhost:5111/submodel")
        });

        [TestMethod]
        public void Test11_CreateBlankAssetAdministrationShellRegistration()
        {
            var result = CreateOrUpdateAssetAdministrationShellRegistration(aas.Identification.Id, aasDescriptor);
            result.Entity.Should().BeEquivalentTo(aasDescriptor, opts => opts.IgnoringCyclicReferences());
        }

        [TestMethod]
        public void Test12_UpdateAssetAdministrationShellRegistration()
        {
            aasDescriptor.SubmodelDescriptors.Add(new SubmodelDescriptor(aas.Submodels["MyTestSubmodel"], new List<IEndpoint>()
            {
                new HttpEndpoint("http://localhost:5080/aas/submodels/MyTestSubmodel/submodel")
            }));

            var updatedResult = CreateOrUpdateAssetAdministrationShellRegistration(aas.Identification.Id, aasDescriptor);
            updatedResult.Entity.Should().BeEquivalentTo(aasDescriptor, opts => opts.IgnoringCyclicReferences());
        }

        public IResult<IAssetAdministrationShellDescriptor> CreateOrUpdateAssetAdministrationShellRegistration(string aasId, IAssetAdministrationShellDescriptor aasDescriptor)
        {
            return registryHttpClient.CreateOrUpdateAssetAdministrationShellRegistration(aasId, aasDescriptor);
        }

        [TestMethod]
        public void Test2_CreateOrUpdateSubmodelRegistration()
        {
            var result = CreateOrUpdateSubmodelRegistration(aas.Identification.Id, submodel.Identification.Id, submodelDescriptor);
            result.Entity.Should().BeEquivalentTo(submodelDescriptor, opts => opts.IgnoringCyclicReferences());
            aasDescriptor.SubmodelDescriptors.Add(submodelDescriptor);
        }

        public IResult<ISubmodelDescriptor> CreateOrUpdateSubmodelRegistration(string aasId, string submodelId, ISubmodelDescriptor submodelDescriptor)
        {
            return registryHttpClient.CreateOrUpdateSubmodelRegistration(aasId, submodelId, submodelDescriptor);
        }

        [TestMethod]
        public void Test3_RetrieveAssetAdministrationShellRegistration()
        {
            var result = RetrieveAssetAdministrationShellRegistration(aas.Identification.Id);
            result.Entity.Should().BeEquivalentTo(aasDescriptor, opts => opts.IgnoringCyclicReferences());
        }

        public IResult<IAssetAdministrationShellDescriptor> RetrieveAssetAdministrationShellRegistration(string aasId)
        {
            return registryHttpClient.RetrieveAssetAdministrationShellRegistration(aasId);
        }

        [TestMethod]
        public void Test41_RetrieveAllAssetAdministrationShellRegistrations()
        {
            var result = RetrieveAllAssetAdministrationShellRegistrations();
            result.Entity.Should().ContainEquivalentOf(aasDescriptor, opts => opts.IgnoringCyclicReferences());
        }

        public IResult<IQueryableElementContainer<IAssetAdministrationShellDescriptor>> RetrieveAllAssetAdministrationShellRegistrations()
        {
            return registryHttpClient.RetrieveAllAssetAdministrationShellRegistrations();
        }

        [TestMethod]
        public void Test42_RetrieveAllAssetAdministrationShellRegistrations()
        {
            var result = RetrieveAllAssetAdministrationShellRegistrations(p => p.Identification.Id == aas.Identification.Id);
            result.Entity.Should().ContainEquivalentOf(aasDescriptor, opts => opts.IgnoringCyclicReferences());
        }

        public IResult<IQueryableElementContainer<IAssetAdministrationShellDescriptor>> RetrieveAllAssetAdministrationShellRegistrations(Predicate<IAssetAdministrationShellDescriptor> predicate)
        {
            return registryHttpClient.RetrieveAllAssetAdministrationShellRegistrations(predicate);
        }

        [TestMethod]
        public void Test5_RetrieveSubmodelRegistration()
        {
            var result = RetrieveSubmodelRegistration(aas.Identification.Id, submodel.Identification.Id);
            result.Entity.Should().BeEquivalentTo(submodelDescriptor, opts => opts.IgnoringCyclicReferences().Excluding(o => o.Parent));
        }


        public IResult<ISubmodelDescriptor> RetrieveSubmodelRegistration(string aasId, string submodelId)
        {
            return registryHttpClient.RetrieveSubmodelRegistration(aasId, submodelId);
        }

        [TestMethod]
        public void Test61_RetrieveAllSubmodelRegistrations()
        {
            var result = RetrieveAllSubmodelRegistrations(aas.Identification.Id);
            result.Entity.Should().ContainEquivalentOf(submodelDescriptor, opts => opts.IgnoringCyclicReferences().Excluding(o => o.Parent));
        }

        public IResult<IQueryableElementContainer<ISubmodelDescriptor>> RetrieveAllSubmodelRegistrations(string aasId)
        {
            return registryHttpClient.RetrieveAllSubmodelRegistrations(aasId);
        }

        [TestMethod]
        public void Test62_RetrieveAllSubmodelRegistrations()
        {
            var result = RetrieveAllSubmodelRegistrations(aas.Identification.Id, p => p.Identification.Id == submodel.Identification.Id);
            result.Entity.Should().ContainEquivalentOf(submodelDescriptor, opts => opts.IgnoringCyclicReferences().Excluding(o => o.Parent));
        }

        public IResult<IQueryableElementContainer<ISubmodelDescriptor>> RetrieveAllSubmodelRegistrations(string aasId, Predicate<ISubmodelDescriptor> predicate)
        {
            return registryHttpClient.RetrieveAllSubmodelRegistrations(aasId, predicate);
        }

        [TestMethod]
        public void Test7_DeleteSubmodelRegistration()
        {
            var deleted = DeleteSubmodelRegistration(aas.Identification.Id, submodel.Identification.Id);
            deleted.Success.Should().BeTrue();

            var retrieved = RetrieveAllSubmodelRegistrations(aas.Identification.Id);
            retrieved.Entity.Should().NotContain(submodelDescriptor);

        }

        public IResult DeleteSubmodelRegistration(string aasId, string submodelId)
        {
            return registryHttpClient.DeleteSubmodelRegistration(aasId, submodelId);
        }

        [TestMethod]
        public void Test8_DeleteAssetAdministrationShellRegistration()
        {
            var deleted = DeleteAssetAdministrationShellRegistration(aas.Identification.Id);
            deleted.Success.Should().BeTrue();

            var retrieved = RetrieveAllAssetAdministrationShellRegistrations();
            retrieved.Entity.Should().NotContain(aasDescriptor);

        }

        public IResult DeleteAssetAdministrationShellRegistration(string aasId)
        {
            return registryHttpClient.DeleteAssetAdministrationShellRegistration(aasId);
        }

    }
}
