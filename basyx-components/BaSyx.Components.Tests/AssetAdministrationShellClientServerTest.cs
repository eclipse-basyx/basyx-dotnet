using BaSyx.AAS.Client.Http;
using BaSyx.AAS.Server.Http;
using BaSyx.API.AssetAdministrationShell.Extensions;
using BaSyx.API.Clients;
using BaSyx.API.Components;
using BaSyx.Models.Communication;
using BaSyx.Models.Connectivity;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Extensions;
using BaSyx.Registry.Client.Http;
using BaSyx.Registry.ReferenceImpl.FileBased;
using BaSyx.Registry.Server.Http;
using BaSyx.Utils.ResultHandling;
using BaSyx.Utils.Settings.Types;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace BaSyx.Components.Tests
{
    [TestClass]
    public class AssetAdministrationShellClientServerTest : IAssetAdministrationShellClient, IAssetAdministrationShellSubmodelClient
    {
        private static AssetAdministrationShellHttpServer server;
        private static AssetAdministrationShellHttpClient client;

        private static IAssetAdministrationShell aas;
        private static ISubmodel submodel;
        private static IAssetAdministrationShellDescriptor aasDescriptor;

        static AssetAdministrationShellClientServerTest()
        {
            ServerSettings aasServerSettings = ServerSettings.CreateSettings();
            aasServerSettings.ServerConfig.Hosting.ContentPath = "Content";
            aasServerSettings.ServerConfig.Hosting.Environment = "Development";
            aasServerSettings.ServerConfig.Hosting.Urls.Add("http://localhost:5080");
            aasServerSettings.ServerConfig.Hosting.Urls.Add("https://localhost:5443");

            aas = TestAssetAdministrationShell.GetAssetAdministrationShell();
            submodel = TestAssetAdministrationShell.GetTestSubmodel();
            var serviceProvider = aas.CreateServiceProvider(true);
            serviceProvider.UseAutoEndpointRegistration(aasServerSettings.ServerConfig);
            aasDescriptor = serviceProvider.ServiceDescriptor;

            server = new AssetAdministrationShellHttpServer(aasServerSettings);
            server.SetServiceProvider(serviceProvider);
            _ = server.RunAsync();

            client = new AssetAdministrationShellHttpClient(aasDescriptor);
        }

        [TestMethod]
        public void Test1_RetrieveAssetAdministrationShell()
        {
            var retrieved = RetrieveAssetAdministrationShell();
            retrieved.Success.Should().BeTrue();
        }

        public IResult<IAssetAdministrationShell> RetrieveAssetAdministrationShell()
        {
            return client.RetrieveAssetAdministrationShell();
        }

        [TestMethod]
        public void Test2_RetrieveSubmodel()
        {
            var retrieved = RetrieveSubmodel(submodel.IdShort);
            retrieved.Success.Should().BeTrue();
        }
        public IResult<ISubmodel> RetrieveSubmodel(string submodelId)
        {
            return client.RetrieveSubmodel(submodelId);
        }

        [TestMethod]
        public void Test31_CreateOrUpdateSubmodelElement_FirstLevelHierarchy()
        {
            Property testProperty = new Property("TestIntegerProperty", typeof(int), 8);
            var created = CreateOrUpdateSubmodelElement(submodel.IdShort, testProperty.IdShort, testProperty);
            created.Success.Should().BeTrue();
            created.Entity.Should().BeEquivalentTo(testProperty, opts => opts
            .Excluding(p => p.Get)
            .Excluding(p => p.Set)
            .Excluding(p => p.EmbeddedDataSpecifications)
            .Excluding(p => p.Parent));

            submodel.SubmodelElements.Add(testProperty);
        }
        public IResult<ISubmodelElement> CreateOrUpdateSubmodelElement(string submodelId, string rootSeIdShortPath, ISubmodelElement submodelElement)
        {
            return client.CreateOrUpdateSubmodelElement(submodelId, rootSeIdShortPath, submodelElement);
        }

        [TestMethod]
        public void Test41_RetrieveSubmodelElements()
        {
            var retrieved = RetrieveSubmodelElements(submodel.IdShort);
            retrieved.Success.Should().BeTrue();
            retrieved.Entity.Count.Should().Be(submodel.SubmodelElements.Count);
        }
        public IResult<IElementContainer<ISubmodelElement>> RetrieveSubmodelElements(string submodelId)
        {
            return client.RetrieveSubmodelElements(submodelId);
        }

        [TestMethod]
        public void Test51_RetrieveSubmodelElement_FirstLevelHierarchy()
        {
            var retrieved = RetrieveSubmodelElement(submodel.IdShort, "TestIntegerProperty");
            retrieved.Success.Should().BeTrue();
            retrieved.Entity.Should().BeEquivalentTo(submodel.SubmodelElements["TestIntegerProperty"].Cast<Property>(), opts => opts
            .Excluding(p => p.Get)
            .Excluding(p => p.Set)
            .Excluding(p => p.EmbeddedDataSpecifications)
            .Excluding(p => p.Parent));
        }
        public IResult<ISubmodelElement> RetrieveSubmodelElement(string submodelId, string seIdShortPath)
        {
            return client.RetrieveSubmodelElement(submodelId, seIdShortPath);
        }

        [TestMethod]
        public void Test61_RetrieveSubmodelElementValue_FirstLevelHierarchy()
        {
            var retrieved = RetrieveSubmodelElementValue(submodel.IdShort, "TestIntegerProperty");
            retrieved.Success.Should().BeTrue();
            retrieved.Entity.Value.Should().BeEquivalentTo(8);
        }
        public IResult<IValue> RetrieveSubmodelElementValue(string submodelId, string seIdShortPath)
        {
            return client.RetrieveSubmodelElementValue(submodelId, seIdShortPath);
        }

        [TestMethod]
        public void Test71_UpdateSubmodelElementValue_FirstLevelHierarchy()
        {
            var updated = UpdateSubmodelElementValue(submodel.IdShort, "TestIntegerProperty", new ElementValue(5, typeof(int)));
            updated.Success.Should().BeTrue();
        }
        [TestMethod]
        public void Test72_RetrieveSubmodelElementValue_FirstLevelHierarchy_AfterUpdate()
        {
            var retrieved = RetrieveSubmodelElementValue(submodel.IdShort, "TestIntegerProperty");
            retrieved.Success.Should().BeTrue();
            retrieved.Entity.Value.Should().BeEquivalentTo(5);
        }

        public IResult UpdateSubmodelElementValue(string submodelId, string seIdShortPath, IValue value)
        {
            return client.UpdateSubmodelElementValue(submodelId, seIdShortPath, value);
        }

        [TestMethod]
        public void Test81_DeleteSubmodelElement_FirstLevelHierarchy()
        {
            var deleted = DeleteSubmodelElement(submodel.IdShort, "TestIntegerProperty");
            deleted.Success.Should().BeTrue();
        }

        public IResult DeleteSubmodelElement(string submodelId, string seIdShortPath)
        {
            return client.DeleteSubmodelElement(submodelId, seIdShortPath);
        }

        [TestMethod]
        public void Test91_InvokeOperation()
        {
            string requestId = Guid.NewGuid().ToString();
            var invocationResponse = InvokeOperation(submodel.IdShort, "Calculate", new InvocationRequest(requestId)
            {
                InputArguments =
                {
                    new Property<string>("Expression", "6*(8+5)"),
                    new Property<int>("ComputingTime", 1000)
                },
                InOutputArguments =
                {
                     new Property<string>("ThroughputVariable", "Test for InOutputArguments"),
                },
                Timeout = 5000
            });
            invocationResponse.Success.Should().BeTrue();
            invocationResponse.Entity.Should().NotBeNull();
            invocationResponse.Entity.RequestId.Should().BeEquivalentTo(requestId);
            
            invocationResponse.Entity.OperationResult.Success.Should().BeTrue();
            invocationResponse.Entity.ExecutionState.Should().BeEquivalentTo(ExecutionState.Completed);
            invocationResponse.Entity.InOutputArguments.Get("ThroughputVariable").Cast<IProperty>().GetValue<string>().Should().Contain("Test for InOutputArguments");
            invocationResponse.Entity.OutputArguments.Get("Result").Cast<IProperty>().GetValue<double>().Should().Be(78.0);            
        }

        [TestMethod]
        public void Test91_InvokeOperation_Timeout()
        {
            string requestId = Guid.NewGuid().ToString();
            var invocationResponse = InvokeOperation(submodel.IdShort, "Calculate", new InvocationRequest(requestId)
            {
                InputArguments =
                {
                    new Property<string>("Expression", "6*(8+5)"),
                    new Property<int>("ComputingTime", 5000)
                },
                InOutputArguments =
                {
                     new Property<string>("ThroughputVariable", "Test for InOutputArguments"),
                },
                Timeout = 2000
            });
            invocationResponse.Success.Should().BeTrue();
            invocationResponse.Entity.Should().NotBeNull();
            invocationResponse.Entity.RequestId.Should().BeEquivalentTo(requestId);

            invocationResponse.Entity.OperationResult.Success.Should().BeFalse();
            invocationResponse.Entity.InOutputArguments.Get("ThroughputVariable").Cast<IProperty>().GetValue<string>().Should().Contain("Test for InOutputArguments");
            invocationResponse.Entity.ExecutionState.Should().BeEquivalentTo(ExecutionState.Timeout);
        }

        public IResult<InvocationResponse> InvokeOperation(string submodelId, string operationIdShortPath, InvocationRequest invocationRequest)
        {
            return client.InvokeOperation(submodelId, operationIdShortPath, invocationRequest);
        }

        [TestMethod]
        public void Test92_InvokeOperationAsync()
        {
            string requestId = Guid.NewGuid().ToString();
            var callbackResponse = InvokeOperationAsync(submodel.IdShort, "Calculate", new InvocationRequest(requestId)
            {
                InputArguments =
                {
                    new Property<string>("Expression", "6*(8+5)"),
                    new Property<int>("ComputingTime", 5000)
                },
                InOutputArguments =
                {
                     new Property<string>("ThroughputVariable", "Test for InOutputArguments"),
                },
                Timeout = 15000
            });
            callbackResponse.Success.Should().BeTrue();
            callbackResponse.Entity.Should().NotBeNull();
            callbackResponse.Entity.RequestId.Should().BeEquivalentTo(requestId);
            callbackResponse.Entity.CallbackUrl.Should().NotBeNull();

            InvocationResponse response = null;

            do
            {
                var invocationResponse = GetInvocationResult(submodel.IdShort, "Calculate", requestId);
                invocationResponse.Success.Should().BeTrue();
                invocationResponse.Entity.Should().NotBeNull();

                response = invocationResponse.Entity;
                response.RequestId.Should().BeEquivalentTo(requestId);

            } while (response.ExecutionState == ExecutionState.Running);          

            response.OperationResult.Success.Should().BeTrue();
            response.ExecutionState.Should().BeEquivalentTo(ExecutionState.Completed);
            response.InOutputArguments.Get("ThroughputVariable").Cast<IProperty>().GetValue<string>().Should().Contain("Test for InOutputArguments");
            response.OutputArguments.Get("Result").Cast<IProperty>().GetValue<double>().Should().Be(78.0);
        }

        [TestMethod]
        public void Test92_InvokeOperationAsync_Timeout()
        {
            string requestId = Guid.NewGuid().ToString();
            var callbackResponse = InvokeOperationAsync(submodel.IdShort, "Calculate", new InvocationRequest(requestId)
            {
                InputArguments =
                {
                    new Property<string>("Expression", "6*(8+5)"),
                    new Property<int>("ComputingTime", 10000)
                },
                InOutputArguments =
                {
                     new Property<string>("ThroughputVariable", "Test for InOutputArguments"),
                },
                Timeout = 3000
            });
            callbackResponse.Success.Should().BeTrue();
            callbackResponse.Entity.Should().NotBeNull();
            callbackResponse.Entity.RequestId.Should().BeEquivalentTo(requestId);
            callbackResponse.Entity.CallbackUrl.Should().NotBeNull();

            InvocationResponse response = null;

            do
            {
                var invocationResponse = GetInvocationResult(submodel.IdShort, "Calculate", requestId);
                invocationResponse.Success.Should().BeTrue();
                invocationResponse.Entity.Should().NotBeNull();

                response = invocationResponse.Entity;
                response.RequestId.Should().BeEquivalentTo(requestId);

            } while (response.ExecutionState == ExecutionState.Running);

            response.OperationResult.Success.Should().BeFalse();
            response.InOutputArguments.Get("ThroughputVariable").Cast<IProperty>().GetValue<string>().Should().Contain("Test for InOutputArguments");
            response.ExecutionState.Should().BeEquivalentTo(ExecutionState.Timeout);
        }


        public IResult<CallbackResponse> InvokeOperationAsync(string submodelId, string operationIdShortPath, InvocationRequest invocationRequest)
        {
            return client.InvokeOperationAsync(submodelId, operationIdShortPath, invocationRequest);
        }

        public IResult<InvocationResponse> GetInvocationResult(string submodelId, string operationIdShortPath, string requestId)
        {
            return client.GetInvocationResult(submodelId, operationIdShortPath, requestId);
        }
    }
}
