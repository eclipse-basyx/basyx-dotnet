using BaSyx.API.AssetAdministrationShell.Extensions;
using BaSyx.API.Clients;
using BaSyx.API.Components;
using BaSyx.Models.Communication;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Extensions;
using BaSyx.Submodel.Client.Http;
using BaSyx.Submodel.Server.Http;
using BaSyx.Utils.ResultHandling;
using BaSyx.Utils.Settings.Types;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BaSyx.Components.Tests
{
    [TestClass]
    public class SubmodelClientServerTest : ISubmodelClient
    {
        private static SubmodelHttpServer server;
        private static SubmodelHttpClient client;

        private static ISubmodel submodel;
        private static ISubmodelDescriptor  submodelDescriptor;

        static SubmodelClientServerTest()
        {
            ServerSettings submodelServerSettings = ServerSettings.CreateSettings();
            submodelServerSettings.ServerConfig.Hosting.ContentPath = "Content";
            submodelServerSettings.ServerConfig.Hosting.Environment = "Development";
            submodelServerSettings.ServerConfig.Hosting.Urls.Add("http://localhost:5085");
            submodelServerSettings.ServerConfig.Hosting.Urls.Add("https://localhost:5448");

            submodel = TestAssetAdministrationShell.GetTestSubmodel();
            var serviceProvider = submodel.CreateServiceProvider();
            serviceProvider.UseAutoEndpointRegistration(submodelServerSettings.ServerConfig);
            submodelDescriptor = serviceProvider.ServiceDescriptor;

            server = new SubmodelHttpServer(submodelServerSettings);
            server.SetServiceProvider(serviceProvider);
            _ = server.RunAsync();

            client = new SubmodelHttpClient(submodelDescriptor);
        }


        [TestMethod]
        public void Test1_RetrieveSubmodel()
        {
            var retrieved = RetrieveSubmodel();
            retrieved.Success.Should().BeTrue();
        }
        public IResult<ISubmodel> RetrieveSubmodel()
        {
            return client.RetrieveSubmodel();
        }

        [TestMethod]
        public void Test21_CreateOrUpdateSubmodelElement_FirstLevelHierarchy()
        {
            Property testProperty = new Property("TestIntegerProperty", typeof(int), 8);
            var created = CreateOrUpdateSubmodelElement(testProperty.IdShort, testProperty);
            created.Success.Should().BeTrue();
            created.Entity.Should().BeEquivalentTo(testProperty, opts => opts
            .Excluding(p => p.Get)
            .Excluding(p => p.Set)
            .Excluding(p => p.EmbeddedDataSpecifications)
            .Excluding(p => p.Parent));

            submodel.SubmodelElements.Add(testProperty);
        }
        public IResult<ISubmodelElement> CreateOrUpdateSubmodelElement(string rootSeIdShortPath, ISubmodelElement submodelElement)
        {
            return client.CreateOrUpdateSubmodelElement(rootSeIdShortPath, submodelElement);
        }

        [TestMethod]
        public void Test31_RetrieveSubmodelElements()
        {
            var retrieved = RetrieveSubmodelElements();
            retrieved.Success.Should().BeTrue();
            retrieved.Entity.Count.Should().Be(submodel.SubmodelElements.Count);
        }
        public IResult<IElementContainer<ISubmodelElement>> RetrieveSubmodelElements()
        {
            return client.RetrieveSubmodelElements();
        }

        [TestMethod]
        public void Test41_RetrieveSubmodelElement_FirstLevelHierarchy()
        {
            var retrieved = RetrieveSubmodelElement("TestIntegerProperty");
            retrieved.Success.Should().BeTrue();
            retrieved.Entity.Should().BeEquivalentTo(submodel.SubmodelElements["TestIntegerProperty"].Cast<Property>(), opts => opts
            .Excluding(p => p.Get)
            .Excluding(p => p.Set)
            .Excluding(p => p.EmbeddedDataSpecifications)
            .Excluding(p => p.Parent));
        }
        public IResult<ISubmodelElement> RetrieveSubmodelElement(string seIdShortPath)
        {
            return client.RetrieveSubmodelElement(seIdShortPath);
        }

        [TestMethod]
        public void Test51_RetrieveSubmodelElementValue_FirstLevelHierarchy()
        {
            var retrieved = RetrieveSubmodelElementValue("TestIntegerProperty");
            retrieved.Success.Should().BeTrue();
            retrieved.Entity.Value.Should().BeEquivalentTo(8);
        }
        public IResult<IValue> RetrieveSubmodelElementValue(string seIdShortPath)
        {
            return client.RetrieveSubmodelElementValue(seIdShortPath);
        }

        [TestMethod]
        public void Test61_UpdateSubmodelElementValue_FirstLevelHierarchy()
        {
            var updated = UpdateSubmodelElementValue("TestIntegerProperty", new ElementValue(5, typeof(int)));
            updated.Success.Should().BeTrue();
        }
        [TestMethod]
        public void Test62_RetrieveSubmodelElementValue_FirstLevelHierarchy_AfterUpdate()
        {
            var retrieved = RetrieveSubmodelElementValue("TestIntegerProperty");
            retrieved.Success.Should().BeTrue();
            retrieved.Entity.Value.Should().BeEquivalentTo(5);
        }

        public IResult UpdateSubmodelElementValue(string seIdShortPath, IValue value)
        {
            return client.UpdateSubmodelElementValue(seIdShortPath, value);
        }

        [TestMethod]
        public void Test71_DeleteSubmodelElement_FirstLevelHierarchy()
        {
            var deleted = DeleteSubmodelElement("TestIntegerProperty");
            deleted.Success.Should().BeTrue();
        }

        public IResult DeleteSubmodelElement(string seIdShortPath)
        {
            return client.DeleteSubmodelElement(seIdShortPath);
        }

        [TestMethod]
        public void Test81_InvokeOperation()
        {
            string requestId = Guid.NewGuid().ToString();
            var invocationResponse = InvokeOperation("Calculate", new InvocationRequest(requestId)
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
        public void Test82_InvokeOperation_Timeout()
        {
            string requestId = Guid.NewGuid().ToString();
            var invocationResponse = InvokeOperation("Calculate", new InvocationRequest(requestId)
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

        public IResult<InvocationResponse> InvokeOperation(string operationIdShortPath, InvocationRequest invocationRequest)
        {
            return client.InvokeOperation(operationIdShortPath, invocationRequest);
        }

        [TestMethod]
        public void Test91_InvokeOperationAsync()
        {
            string requestId = Guid.NewGuid().ToString();
            var callbackResponse = InvokeOperationAsync("Calculate", new InvocationRequest(requestId)
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
                var invocationResponse = GetInvocationResult("Calculate", requestId);
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
            var callbackResponse = InvokeOperationAsync("Calculate", new InvocationRequest(requestId)
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
                var invocationResponse = GetInvocationResult("Calculate", requestId);
                invocationResponse.Success.Should().BeTrue();
                invocationResponse.Entity.Should().NotBeNull();

                response = invocationResponse.Entity;
                response.RequestId.Should().BeEquivalentTo(requestId);

            } while (response.ExecutionState == ExecutionState.Running);

            response.OperationResult.Success.Should().BeFalse();
            response.InOutputArguments.Get("ThroughputVariable").Cast<IProperty>().GetValue<string>().Should().Contain("Test for InOutputArguments");
            response.ExecutionState.Should().BeEquivalentTo(ExecutionState.Timeout);
        }


        public IResult<CallbackResponse> InvokeOperationAsync(string operationIdShortPath, InvocationRequest invocationRequest)
        {
            return client.InvokeOperationAsync(operationIdShortPath, invocationRequest);
        }

        public IResult<InvocationResponse> GetInvocationResult(string operationIdShortPath, string requestId)
        {
            return client.GetInvocationResult(operationIdShortPath, requestId);
        }
    }
}
