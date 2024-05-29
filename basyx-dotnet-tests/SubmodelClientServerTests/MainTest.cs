/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using BaSyx.API.Clients;
using BaSyx.Models.AdminShell;
using BaSyx.Models.Extensions;
using BaSyx.Clients.AdminShell.Http;
using BaSyx.Utils.ResultHandling;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleAssetAdministrationShell;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BaSyx.Models.Connectivity;
using BaSyx.Utils.DependencyInjection;
using System.Text.Json;
using BaSyx.Utils.ResultHandling.ResultTypes;

namespace SubmodelClientServerTests
{
    [TestClass]
    public class MainTest : ISubmodelClient
    {
        private static Submodel Submodel;

        private static SubmodelHttpClient Client;

        public IEndpoint Endpoint => ((IClient)Client).Endpoint;

        static MainTest()
        {
            Server.Run();
            Submodel = TestSubmodel.GetSubmodel("TestSubmodel");
            Client = new SubmodelHttpClient(new Uri(Server.ServerUrl));
        }

        [TestMethod]
        public void Test100_UpdateSubmodel()
        {
            LangStringSet newDescription = new LangStringSet()
            {
                new LangString("de", "Meine neue Beschreibung"),
                new LangString("en", "My new description")
            };
            Submodel.Description = newDescription;
            var updated = UpdateSubmodel(Submodel);
            updated.Success.Should().BeTrue();
        }

        [TestMethod]
        public void Test101_RetrieveSubmodel()
        {
            var result = RetrieveSubmodel();

            result.Success.Should().BeTrue();
            result.Entity.IdShort.Should().BeEquivalentTo(Submodel.IdShort);
            result.Entity.Id.Should().BeEquivalentTo(Submodel.Id);
            result.Entity.Description.Should().BeEquivalentTo(Submodel.Description);
            result.Entity.DisplayName.Should().BeEquivalentTo(Submodel.DisplayName);
            result.Entity.SemanticId.Should().BeEquivalentTo(Submodel.SemanticId);
            result.Entity.Kind.Should().Be(Submodel.Kind);
        }

        [TestMethod]
        public void Test102_CreateSubmodelElement()
        {
            Property<string> property = new Property<string>("MyTestProperty", "MyTestValue");

            var result = CreateSubmodelElement(".", property);

            result.Success.Should().BeTrue();
            result.Entity.Should().BeEquivalentTo(property, options =>
            {
                options
                .Excluding(p => p.EmbeddedDataSpecifications)
                .Excluding(p => p.Parent)
                .Excluding(p => p.Get)
                .Excluding(p => p.Set);
                return options;
            });
        }

        [TestMethod]
        public void Test103_CreateSubmodelElementHierarchy()
        {
            SubmodelElementCollection coll = new SubmodelElementCollection("MyCollection")
            {
               Value =
                {
                    Value =
                    {
                        new Property<string>("MySubString", "MySubStringValue"),
                        new Property<int>("MySubInt", 5),
                        new Property<double>("MySubDouble", 4.5d),
                        new Property<float>("MySubFloat", 2.3f),
                        new SubmodelElementCollection("MySubCollection")
                        {
                            Value =
                            {
                                Value =
                                {
                                    new Property<string>("MySubSubString", "MySubSubStringValue"),
                                    new Property<int>("MySubSubInt", 6),
                                    new Property<double>("MySubSubDouble", 5.5d),
                                    new Property<float>("MySubSubFloat", 3.3f),
                                }
                            }
                        }
                    }
                }
            };
            Submodel.SubmodelElements.Add(coll);
            var created = CreateSubmodelElement(".", coll);
            created.Success.Should().BeTrue();
        }

        [TestMethod]
        public void Test104_RetrieveSubmodelElements()
        {
            var retrieved = RetrieveSubmodelElements();
            retrieved.Entity.Result.Should().ContainEquivalentOf(Submodel.SubmodelElements["MyCollection"],
                options =>
                {
                    options
                      .Excluding(p => p.EmbeddedDataSpecifications)
                      .Excluding(p => p.Parent)
                      .Excluding(p => p.Get)
                      .Excluding(p => p.Set);
                    return options;
                });
        }

        [TestMethod]
        public void Test105_RetrieveSubmodelElement()
        {
            var result = RetrieveSubmodelElement("MyCollection.MySubCollection.MySubSubFloat");
            result.Entity.GetValue<float>().Should().Be(3.3f);
        }

        [TestMethod]
        public void Test106_UpdateSubmodelElement()
        {
            var mySubFloat = Submodel.SubmodelElements["MyCollection.MySubCollection.MySubSubFloat"].Cast<Property<float>>();
            mySubFloat.Description = new LangStringSet()
            {
                new LangString("de", "Meine float Property Beschreibung"),
                new LangString("en", "My float Property description")
            };
            var updated = UpdateSubmodelElement("MyCollection.MySubCollection.MySubSubFloat", mySubFloat);
            updated.Success.Should().BeTrue();
            var retrieved = RetrieveSubmodelElement("MyCollection.MySubCollection.MySubSubFloat");
            retrieved.Success.Should().BeTrue();
            retrieved.Entity.Description.Should().BeEquivalentTo(mySubFloat.Description);
        }

        [TestMethod]
        public void Test107_RetrieveSubmodelElementHierarchy()
        {
            var result = RetrieveSubmodelElement("MyCollection.MySubCollection");
            result.Success.Should().BeTrue();
            result.Entity.Cast<ISubmodelElementCollection>().Value.Value["MySubSubInt"].GetValue<int>().Should().Be(6);
        }

        [TestMethod]
        public void Test108_UpdateSubmodelElementValue()
        {
            var result = UpdateSubmodelElementValue("MyCollection.MySubCollection.MySubSubDouble", new PropertyValue(new ElementValue<double>(1.8d)));
            result.Success.Should().BeTrue();
        }

        [TestMethod]
        public void Test109_RetrieveSubmodelElementValue()
        {
            var result = RetrieveSubmodelElementValue("MyCollection.MySubCollection.MySubSubDouble");
            result.Success.Should().BeTrue();
            result.Entity.GetValue<double>().Should().Be(1.8d);
        }

        [TestMethod]
        public void Test110_InvokeOperation()
        {
            InvocationRequest request = new InvocationRequest(Guid.NewGuid().ToString())
            {
                InputArguments =
                {
                    new Property<string>("Expression", "3*8"),
                    new Property<int>("ComputingTime", 100)
                }
            };

            var result = InvokeOperation("Calculate", request, false);
            result.Success.Should().BeTrue();
            result.Entity.OutputArguments["Result"].GetValue<double>().Should().Be(24);

        }

        [TestMethod]
        public void Test111_InvokeOperationAsync()
        {
            InvocationRequest request = new InvocationRequest(Guid.NewGuid().ToString())
            {
                InputArguments =
                {
                    new Property<string>("Expression", "3*8"),
                    new Property<int>("ComputingTime", 2000)
                }
            };

            var result = InvokeOperation("Calculate", request, true);
            result.Success.Should().BeTrue();
            result.Entity.ExecutionState.Should().Be(ExecutionState.Initiated);

            Thread.Sleep(2500);

            var handleResult = GetInvocationResult("Calculate", request.RequestId);
            handleResult.Success.Should().BeTrue();
            handleResult.Entity.OutputArguments["Result"].GetValue<double>().Should().Be(24);
        }

        [TestMethod]
        public void Test112_DeleteSubmodelElement()
        {
            var deleted = DeleteSubmodelElement("MyCollection");
            deleted.Success.Should().BeTrue();

            var retrieved = RetrieveSubmodelElements();
            retrieved.Success.Should().BeTrue();
            retrieved.Entity.Result.Should().NotContainEquivalentOf(Submodel.SubmodelElements["MyCollection"]);
        }

        [TestMethod]
        public void Test113_ToFromValueOnly()
        {
            var property = Submodel.SubmodelElements["TestProperty1"].Cast<IProperty>();
            PropertyValue oldPropertyValue = property.GetValueScope<PropertyValue>();            
            string json = oldPropertyValue.ToJsonValueOnly();
            PropertyValue newPropertyValue = json.FromJsonValueOnly<PropertyValue>();
            oldPropertyValue.Value.Value.Should().Be(newPropertyValue.Value.Value);
        }

        public IResult<ISubmodel> RetrieveSubmodel(RequestLevel level = RequestLevel.Deep, RequestExtent extent = RequestExtent.WithoutBlobValue)
        {
            return ((ISubmodelClient)Client).RetrieveSubmodel(level, extent);
        } 

        public IResult UpdateSubmodel(ISubmodel submodel)
        {
            return ((ISubmodelClient)Client).UpdateSubmodel(submodel);
        }

        public IResult<ISubmodelElement> CreateSubmodelElement(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            return ((ISubmodelClient)Client).CreateSubmodelElement(rootIdShortPath, submodelElement);
        }

        public IResult DeleteSubmodelElement(string idShortPath)
        {
            return ((ISubmodelClient)Client).DeleteSubmodelElement(idShortPath);
        }

        public IResult<InvocationResponse> GetInvocationResult(string idShortPath, string requestId)
        {
            return ((ISubmodelClient)Client).GetInvocationResult(idShortPath, requestId);
        }

        public IResult<InvocationResponse> InvokeOperation(string idShortPath, InvocationRequest invocationRequest, bool async)
        {
            return ((ISubmodelClient)Client).InvokeOperation(idShortPath, invocationRequest, async);
        }

        public IResult<ISubmodelElement> RetrieveSubmodelElement(string idShortPath)
        {
            return ((ISubmodelClient)Client).RetrieveSubmodelElement(idShortPath);
        }

        public IResult<PagedResult<IElementContainer<ISubmodelElement>>> RetrieveSubmodelElements()
        {
            return ((ISubmodelClient)Client).RetrieveSubmodelElements();
        }

        public IResult<ValueScope> RetrieveSubmodelElementValue(string idShortPath)
        {
            return ((ISubmodelClient)Client).RetrieveSubmodelElementValue(idShortPath);
        }

        public IResult UpdateSubmodelElement(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            return ((ISubmodelClient)Client).UpdateSubmodelElement(rootIdShortPath, submodelElement);
        }

        public IResult UpdateSubmodelElementValue(string idShortPath, ValueScope value)
        {
            return ((ISubmodelClient)Client).UpdateSubmodelElementValue(idShortPath, value);
        }

        public Task<IResult<ISubmodel>> RetrieveSubmodelAsync(RequestLevel level = RequestLevel.Deep, RequestExtent extent = RequestExtent.WithoutBlobValue)
        {
            return ((ISubmodelClient)Client).RetrieveSubmodelAsync(level, extent);
        }

        public Task<IResult> UpdateSubmodelAsync(ISubmodel submodel)
        {
            return ((ISubmodelClient)Client).UpdateSubmodelAsync(submodel);
        }

        public Task<IResult<ISubmodelElement>> CreateSubmodelElementAsync(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            return ((ISubmodelClient)Client).CreateSubmodelElementAsync(rootIdShortPath, submodelElement);
        }

        public Task<IResult> UpdateSubmodelElementAsync(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            return ((ISubmodelClient)Client).UpdateSubmodelElementAsync(rootIdShortPath, submodelElement);
        }

        public Task<IResult<PagedResult<IElementContainer<ISubmodelElement>>>> RetrieveSubmodelElementsAsync()
        {
            return ((ISubmodelClient)Client).RetrieveSubmodelElementsAsync();
        }

        public Task<IResult<ISubmodelElement>> RetrieveSubmodelElementAsync(string idShortPath)
        {
            return ((ISubmodelClient)Client).RetrieveSubmodelElementAsync(idShortPath);
        }

        public Task<IResult<ValueScope>> RetrieveSubmodelElementValueAsync(string idShortPath)
        {
            return ((ISubmodelClient)Client).RetrieveSubmodelElementValueAsync(idShortPath);
        }

        public Task<IResult> UpdateSubmodelElementValueAsync(string idShortPath, ValueScope value)
        {
            return ((ISubmodelClient)Client).UpdateSubmodelElementValueAsync(idShortPath, value);
        }

        public Task<IResult> DeleteSubmodelElementAsync(string idShortPath)
        {
            return ((ISubmodelClient)Client).DeleteSubmodelElementAsync(idShortPath);
        }

        public Task<IResult<InvocationResponse>> InvokeOperationAsync(string idShortPath, InvocationRequest invocationRequest, bool async = false)
        {
            return ((ISubmodelClient)Client).InvokeOperationAsync(idShortPath, invocationRequest, async);
        }

        public Task<IResult<InvocationResponse>> GetInvocationResultAsync(string idShortPath, string requestId)
        {
            return ((ISubmodelClient)Client).GetInvocationResultAsync(idShortPath, requestId);
        }
    }
}
