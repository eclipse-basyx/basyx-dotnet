/*******************************************************************************
* Copyright (c) 2022 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using BaSyx.API.Clients;
using BaSyx.API.Interfaces;
using BaSyx.Models.AdminShell;
using BaSyx.Models.Connectivity;
using BaSyx.Registry.Client.Http;
using BaSyx.Utils.ResultHandling;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleAssetAdministrationShell;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public IEndpoint Endpoint => ((IClient)Client).Endpoint;

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

            Shell = TestAssetAdministrationShell.GetAssetAdministrationShell("MainAdminShell");
            ShellDescriptor = new AssetAdministrationShellDescriptor(Shell, new List<IEndpoint>() { new Endpoint("http://localhost:5080/aas", InterfaceName.AssetAdministrationShellInterface) });

            Submodel = TestSubmodel.GetSubmodel("TestSubmodel");
            SubmodelDescriptor = new SubmodelDescriptor(Submodel, new List<IEndpoint>() { new Endpoint("http://localhost:5040/submodel", InterfaceName.SubmodelInterface) });

        }

        [TestMethod]
        public void Test100_CreateAssetAdministrationShellRegistration()
        {
            CreateAssetAdministrationShellRegistration(ShellDescriptor);
        }

        [TestMethod]
        public void Test101_RetrieveAssetAdministrationShellRegistration()
        {
            RetrieveAssetAdministrationShellRegistration(Shell.Id);
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
            CreateSubmodelRegistration(Shell.Id, SubmodelDescriptor);
        }

        [TestMethod]
        public void Test106_RetrieveSubmodelRegistration()
        {
            RetrieveSubmodelRegistration(Shell.Id, SubmodelDescriptor.Identification.Id);
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
            UpdateSubmodelRegistration(ShellDescriptor.Identification.Id, Submodel.Id, SubmodelDescriptor);
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

        public IResult<IEnumerable<ISubmodelDescriptor>> RetrieveAllSubmodelRegistrations(string aasIdentifier)
        {
            var result = Client.RetrieveAllSubmodelRegistrations(aasIdentifier);
            result.Success.Should().BeTrue();
            result.Entity.Should().ContainEquivalentOf(SubmodelDescriptor);
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


        public IResult<IEnumerable<IAssetAdministrationShellDescriptor>> RetrieveAllAssetAdministrationShellRegistrations()
        {
            var result = Client.RetrieveAllAssetAdministrationShellRegistrations();
            result.Success.Should().BeTrue();
            result.Entity.Should().ContainEquivalentOf(ShellDescriptor);
            return result;
        }

        public IResult<IEnumerable<IAssetAdministrationShellDescriptor>> RetrieveAllAssetAdministrationShellRegistrations(Predicate<IAssetAdministrationShellDescriptor> predicate)
        {
            var result = Client.RetrieveAllAssetAdministrationShellRegistrations(predicate);
            result.Success.Should().BeTrue();
            result.Entity.Should().ContainEquivalentOf(ShellDescriptor);
            return result;
        }

        public IResult<IEnumerable<ISubmodelDescriptor>> RetrieveAllSubmodelRegistrations(string aasIdentifier, Predicate<ISubmodelDescriptor> predicate)
        {
            var result = Client.RetrieveAllSubmodelRegistrations(aasIdentifier, predicate);
            result.Success.Should().BeTrue();
            result.Entity.Should().ContainEquivalentOf(SubmodelDescriptor);
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

        public Task<IResult<IAssetAdministrationShellDescriptor>> CreateAssetAdministrationShellRegistrationAsync(IAssetAdministrationShellDescriptor aasDescriptor)
        {
            return ((IAssetAdministrationShellRegistryClient)Client).CreateAssetAdministrationShellRegistrationAsync(aasDescriptor);
        }

        public Task<IResult<IAssetAdministrationShellDescriptor>> UpdateAssetAdministrationShellRegistrationAsync(string aasIdentifier, IAssetAdministrationShellDescriptor aasDescriptor)
        {
            return ((IAssetAdministrationShellRegistryClient)Client).UpdateAssetAdministrationShellRegistrationAsync(aasIdentifier, aasDescriptor);
        }

        public Task<IResult<IAssetAdministrationShellDescriptor>> RetrieveAssetAdministrationShellRegistrationAsync(string aasIdentifier)
        {
            return ((IAssetAdministrationShellRegistryClient)Client).RetrieveAssetAdministrationShellRegistrationAsync(aasIdentifier);
        }

        public Task<IResult<IEnumerable<IAssetAdministrationShellDescriptor>>> RetrieveAllAssetAdministrationShellRegistrationsAsync()
        {
            return ((IAssetAdministrationShellRegistryClient)Client).RetrieveAllAssetAdministrationShellRegistrationsAsync();
        }

        public Task<IResult<IEnumerable<IAssetAdministrationShellDescriptor>>> RetrieveAllAssetAdministrationShellRegistrationsAsync(Predicate<IAssetAdministrationShellDescriptor> predicate)
        {
            return ((IAssetAdministrationShellRegistryClient)Client).RetrieveAllAssetAdministrationShellRegistrationsAsync(predicate);
        }

        public Task<IResult> DeleteAssetAdministrationShellRegistrationAsync(string aasIdentifier)
        {
            return ((IAssetAdministrationShellRegistryClient)Client).DeleteAssetAdministrationShellRegistrationAsync(aasIdentifier);
        }

        public Task<IResult<ISubmodelDescriptor>> CreateSubmodelRegistrationAsync(string aasIdentifier, ISubmodelDescriptor submodelDescriptor)
        {
            return ((IAssetAdministrationShellRegistryClient)Client).CreateSubmodelRegistrationAsync(aasIdentifier, submodelDescriptor);
        }

        public Task<IResult<ISubmodelDescriptor>> UpdateSubmodelRegistrationAsync(string aasIdentifier, string submodelIdentifier, ISubmodelDescriptor submodelDescriptor)
        {
            return ((IAssetAdministrationShellRegistryClient)Client).UpdateSubmodelRegistrationAsync(aasIdentifier, submodelIdentifier, submodelDescriptor);
        }

        public Task<IResult<IEnumerable<ISubmodelDescriptor>>> RetrieveAllSubmodelRegistrationsAsync(string aasIdentifier)
        {
            return ((IAssetAdministrationShellRegistryClient)Client).RetrieveAllSubmodelRegistrationsAsync(aasIdentifier);
        }

        public Task<IResult<IEnumerable<ISubmodelDescriptor>>> RetrieveAllSubmodelRegistrationsAsync(string aasIdentifier, Predicate<ISubmodelDescriptor> predicate)
        {
            return ((IAssetAdministrationShellRegistryClient)Client).RetrieveAllSubmodelRegistrationsAsync(aasIdentifier, predicate);
        }

        public Task<IResult<ISubmodelDescriptor>> RetrieveSubmodelRegistrationAsync(string aasIdentifier, string submodelIdentifier)
        {
            return ((IAssetAdministrationShellRegistryClient)Client).RetrieveSubmodelRegistrationAsync(aasIdentifier, submodelIdentifier);
        }

        public Task<IResult> DeleteSubmodelRegistrationAsync(string aasIdentifier, string submodelIdentifier)
        {
            return ((IAssetAdministrationShellRegistryClient)Client).DeleteSubmodelRegistrationAsync(aasIdentifier, submodelIdentifier);
        }
    }
}
