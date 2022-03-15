using BaSyx.API.Clients;
using BaSyx.Models.AdminShell;
using BaSyx.Models.Connectivity;
using BaSyx.Registry.Client.Http;
using BaSyx.Utils.ResultHandling;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleAssetAdministrationShell;
using System;
using System.Collections.Generic;

namespace RegistryClientServerTests
{
    [TestClass]
    public class MainTest : IAssetAdministrationShellRegistryClient
    {
        private static RegistryClientSettings Settings;
        private static RegistryHttpClient Client;

        private static AssetAdministrationShell Shell;
        private static AssetAdministrationShellDescriptor ShellDescriptor;

        private static Submodel Submodel;
        private static SubmodelDescriptor SubmodelDescriptor;
        static MainTest()
        {
            Server.Run();
            Settings = new RegistryClientSettings()
            {
                RegistryConfig = new RegistryConfiguration
                {
                    RegistryUrl = Server.ServerUrl
                }
            };
            Client = new RegistryHttpClient(Settings);

            Shell = TestAssetAdministrationShell.GetAssetAdministrationShell();
            ShellDescriptor = new AssetAdministrationShellDescriptor(Shell, new List<IEndpoint>() { new HttpEndpoint("http://localhost:5080/aas") });

            Submodel = TestSubmodel.GetSubmodel("TestSubmodel");
            SubmodelDescriptor = new SubmodelDescriptor(Submodel, new List<IEndpoint>() { new HttpEndpoint("http://localhost:5040/submodel") });

        }

        [TestMethod]
        public void Test100_CreateAssetAdministrationShellRegistration()
        {
            CreateAssetAdministrationShellRegistration(ShellDescriptor);
        }

        [TestMethod]
        public void Test101_RetrieveAssetAdministrationShellRegistration()
        {
            RetrieveAssetAdministrationShellRegistration(Shell.Identification.Id);
        }

        [TestMethod]
        public void Test102_RetrieveAllAssetAdministrationShellRegistrations()
        {
            RetrieveAllAssetAdministrationShellRegistrations();
        }

        [TestMethod]
        public void Test103_RetrieveAllAssetAdministrationShellRegistrations()
        {
            RetrieveAllAssetAdministrationShellRegistrations(p => p.IdShort == ShellDescriptor.IdShort);
        }

        [TestMethod]
        public void Test104_UpdateAssetAdministrationShellRegistration()
        {
            LangStringSet newDescription = new LangStringSet()
            {
                new LangString("de", "Meine neue Beschreibung"),
                new LangString("en", "My new description")
            };
            ShellDescriptor.Description = newDescription;
            UpdateAssetAdministrationShellRegistration(ShellDescriptor.Identification.Id, ShellDescriptor);
        }

        [TestMethod]
        public void Test105_CreateSubmodelRegistration()
        {
            CreateSubmodelRegistration(Shell.Identification.Id, SubmodelDescriptor);
        }

        [TestMethod]
        public void Test106_RetrieveSubmodelRegistration()
        {
            RetrieveSubmodelRegistration(Shell.Identification.Id, SubmodelDescriptor.Identification.Id);
        }

        [TestMethod]
        public void Test107_RetrieveAllSubmodelRegistrations()
        {
            RetrieveAllSubmodelRegistrations(ShellDescriptor.Identification.Id);
        }

        [TestMethod]
        public void Test108_RetrieveAllSubmodelRegistrations()
        {
            RetrieveAllSubmodelRegistrations(ShellDescriptor.Identification.Id, p => p.IdShort == SubmodelDescriptor.IdShort);
        }

        [TestMethod]
        public void Test109_UpdateSubmodelRegistration()
        {
            LangStringSet newDescription = new LangStringSet()
            {
                new LangString("de", "Meine neue Beschreibung"),
                new LangString("en", "My new description")
            };
            SubmodelDescriptor.Description = newDescription;
            UpdateSubmodelRegistration(ShellDescriptor.Identification.Id, Submodel.Identification.Id, SubmodelDescriptor);
        }

        [TestMethod]
        public void Test110_DeleteSubmodelRegistration()
        {
            DeleteSubmodelRegistration(ShellDescriptor.Identification.Id, SubmodelDescriptor.Identification.Id);
        }

        [TestMethod]
        public void Test111_DeleteAssetAdministrationShellRegistration()
        {
            DeleteAssetAdministrationShellRegistration(ShellDescriptor.Identification.Id);
            var result = Client.RetrieveAllAssetAdministrationShellRegistrations();
            result.Success.Should().BeTrue();
            result.Entity.Should().HaveCount(0);
        }

        public IResult<IQueryableElementContainer<ISubmodelDescriptor>> RetrieveAllSubmodelRegistrations(string aasIdentifier)
        {
            var result = Client.RetrieveAllSubmodelRegistrations(aasIdentifier);
            result.Success.Should().BeTrue();
            result.Entity.Should().Contain(SubmodelDescriptor);
            return result;
        }

        public IResult<IAssetAdministrationShellDescriptor> CreateAssetAdministrationShellRegistration(IAssetAdministrationShellDescriptor aasDescriptor)
        {
            var result = Client.CreateAssetAdministrationShellRegistration(aasDescriptor);
            result.Success.Should().BeTrue();
            result.Entity.Should().BeEquivalentTo(aasDescriptor);
            return result;
        }

        

        public IResult<ISubmodelDescriptor> CreateSubmodelRegistration(string aasIdentifier, ISubmodelDescriptor submodelDescriptor)
        {
            var result = Client.CreateSubmodelRegistration(aasIdentifier, submodelDescriptor);
            result.Success.Should().BeTrue();
            result.Entity.Should().BeEquivalentTo(submodelDescriptor);
            return result;
        }

        public IResult DeleteAssetAdministrationShellRegistration(string aasIdentifier)
        {
            var result = Client.DeleteAssetAdministrationShellRegistration(aasIdentifier);
            result.Success.Should().BeTrue();
            return result;
        }

        public IResult DeleteSubmodelRegistration(string aasIdentifier, string submodelIdentifier)
        {
            var result = Client.DeleteSubmodelRegistration(aasIdentifier, submodelIdentifier);
            result.Success.Should().BeTrue();
            return result;
        }


        public IResult<IQueryableElementContainer<IAssetAdministrationShellDescriptor>> RetrieveAllAssetAdministrationShellRegistrations()
        {
            var result = Client.RetrieveAllAssetAdministrationShellRegistrations();
            result.Success.Should().BeTrue();
            result.Entity.Should().Contain(ShellDescriptor);
            return result;
        }

        public IResult<IQueryableElementContainer<IAssetAdministrationShellDescriptor>> RetrieveAllAssetAdministrationShellRegistrations(Predicate<IAssetAdministrationShellDescriptor> predicate)
        {
            var result = Client.RetrieveAllAssetAdministrationShellRegistrations(predicate);
            result.Success.Should().BeTrue();
            result.Entity.Should().Contain(ShellDescriptor);
            return result;
        }

        public IResult<IQueryableElementContainer<ISubmodelDescriptor>> RetrieveAllSubmodelRegistrations(string aasIdentifier, Predicate<ISubmodelDescriptor> predicate)
        {
            var result = Client.RetrieveAllSubmodelRegistrations(aasIdentifier, predicate);
            result.Success.Should().BeTrue();
            result.Entity.Should().Contain(SubmodelDescriptor);
            return result;
        }

        public IResult<IAssetAdministrationShellDescriptor> RetrieveAssetAdministrationShellRegistration(string aasIdentifier)
        {
            var result = Client.RetrieveAssetAdministrationShellRegistration(aasIdentifier);
            result.Success.Should().BeTrue();
            result.Entity.Should().BeEquivalentTo(ShellDescriptor);
            return result;
        }

        public IResult<ISubmodelDescriptor> RetrieveSubmodelRegistration(string aasIdentifier, string submodelIdentifier)
        {
            var result = Client.RetrieveSubmodelRegistration(aasIdentifier, submodelIdentifier);
            result.Success.Should().BeTrue();
            result.Entity.Should().BeEquivalentTo(SubmodelDescriptor);
            return result;
        }

        public IResult<IAssetAdministrationShellDescriptor> UpdateAssetAdministrationShellRegistration(string aasIdentifier, IAssetAdministrationShellDescriptor aasDescriptor)
        {
            var result = Client.UpdateAssetAdministrationShellRegistration(aasIdentifier, aasDescriptor);
            result.Success.Should().BeTrue();
            result.Entity.Should().BeEquivalentTo(aasDescriptor);
            return result;
        }

        public IResult<ISubmodelDescriptor> UpdateSubmodelRegistration(string aasIdentifier, string submodelIdentifier, ISubmodelDescriptor submodelDescriptor)
        {
            var result = Client.UpdateSubmodelRegistration(aasIdentifier, submodelIdentifier, submodelDescriptor);
            result.Success.Should().BeTrue();
            result.Entity.Should().BeEquivalentTo(SubmodelDescriptor);
            return result;
        }
    }
}
