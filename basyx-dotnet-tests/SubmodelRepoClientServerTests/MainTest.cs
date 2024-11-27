/*******************************************************************************
* Copyright (c) 2024 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using BaSyx.API.Clients;
using BaSyx.API.Http;
using BaSyx.API.Interfaces;
using BaSyx.Models.AdminShell;
using BaSyx.Models.Extensions;
using BaSyx.Utils.ResultHandling;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleAssetAdministrationShell;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BaSyx.Utils.Extensions;
using BaSyx.Clients.AdminShell.Http;
using BaSyx.Models.Connectivity;
using BaSyx.Utils.ResultHandling.ResultTypes;

namespace SubmodelRepoClientServerTests
{
    [TestClass]
    public class MainTest : ISubmodelRepositoryClient
    {
        private static Submodel MainSubmodel;
        private static Submodel TestingSubmodel;

        private static SubmodelHttpClient Client;
        private static SubmodelRepositoryHttpClient RepoClient;

        public IEndpoint Endpoint => ((IClient)RepoClient).Endpoint;

        static MainTest()
        {
            Server.Run();
            MainSubmodel = TestSubmodel.GetSubmodel("MainSubmodel");
            TestingSubmodel = TestSubmodel.GetSubmodel("TestSubmodel");
            string basePath = SubmodelRepositoryRoutes.SUBMODEL_BYID.Replace("{submodelIdentifier}", StringOperations.Base64UrlEncode(MainSubmodel.Id));
            Client = new SubmodelHttpClient(new Uri(Server.ServerUrl + basePath), false);
            RepoClient = new SubmodelRepositoryHttpClient(new Uri(Server.ServerUrl));
        }


        [TestMethod]
        public void Test001_CreateSubmodel()
        {
            var result = CreateSubmodel(TestingSubmodel);
            result.Success.Should().BeTrue();
            result.Entity.IdShort.Should().BeEquivalentTo(TestingSubmodel.IdShort);
            result.Entity.Id.Should().BeEquivalentTo(TestingSubmodel.Id);
            result.Entity.Description.Should().BeEquivalentTo(TestingSubmodel.Description);
            result.Entity.DisplayName.Should().BeEquivalentTo(TestingSubmodel.DisplayName);
            result.Entity.SemanticId.Should().BeEquivalentTo(TestingSubmodel.SemanticId);
            result.Entity.Kind.Should().Be(TestingSubmodel.Kind);
        }

        [TestMethod]
        public void Test002_RetrieveAllSubmodels()
        {
            var result = RetrieveSubmodels();
            result.Success.Should().BeTrue();
            result.Entity.Result.Should().HaveCount(2);
            //result.Entity.Should().ContainEquivalentOf(MainSubmodel);
            //result.Entity.Should().ContainEquivalentOf(TestingSubmodel);
        }


        [TestMethod]
        public void Test003_UpdateSubmodel()
        {
            LangStringSet newDescription = new LangStringSet()
            {
                new LangString("de", "Meine neue Beschreibung"),
                new LangString("en", "My new description")
            };
            TestingSubmodel.Description = newDescription;
            var result = UpdateSubmodel(TestingSubmodel.Id, TestingSubmodel);
            result.Success.Should().BeTrue();

            var retrieved = RetrieveSubmodel(TestingSubmodel.Id);
            retrieved.Success.Should().BeTrue();
            retrieved.Entity.IdShort.Should().BeEquivalentTo(TestingSubmodel.IdShort);
            retrieved.Entity.Id.Should().BeEquivalentTo(TestingSubmodel.Id);
            retrieved.Entity.Description.Should().BeEquivalentTo(TestingSubmodel.Description);
            retrieved.Entity.DisplayName.Should().BeEquivalentTo(TestingSubmodel.DisplayName);
            retrieved.Entity.SemanticId.Should().BeEquivalentTo(TestingSubmodel.SemanticId);
            retrieved.Entity.Kind.Should().Be(TestingSubmodel.Kind);
        }

        [TestMethod]
        public void Test004_RetrieveSubmodel()
        {
            var retrievedMain = RetrieveSubmodel(MainSubmodel.Id);
            retrievedMain.Success.Should().BeTrue();
            retrievedMain.Entity.IdShort.Should().BeEquivalentTo(MainSubmodel.IdShort);
            retrievedMain.Entity.Id.Should().BeEquivalentTo(MainSubmodel.Id);
            retrievedMain.Entity.Description.Should().BeEquivalentTo(MainSubmodel.Description);
            retrievedMain.Entity.DisplayName.Should().BeEquivalentTo(MainSubmodel.DisplayName);
            retrievedMain.Entity.SemanticId.Should().BeEquivalentTo(MainSubmodel.SemanticId);
            retrievedMain.Entity.Kind.Should().Be(MainSubmodel.Kind);

            var retrievedTest = RetrieveSubmodel(TestingSubmodel.Id);
            retrievedTest.Success.Should().BeTrue();
            retrievedTest.Entity.IdShort.Should().BeEquivalentTo(TestingSubmodel.IdShort);
            retrievedTest.Entity.Id.Should().BeEquivalentTo(TestingSubmodel.Id);
            retrievedTest.Entity.Description.Should().BeEquivalentTo(TestingSubmodel.Description);
            retrievedTest.Entity.DisplayName.Should().BeEquivalentTo(TestingSubmodel.DisplayName);
            retrievedTest.Entity.SemanticId.Should().BeEquivalentTo(TestingSubmodel.SemanticId);
            retrievedTest.Entity.Kind.Should().Be(TestingSubmodel.Kind);
        }

        [TestMethod]
        public void Test005_DeleteSubmodel()
        {
            var deleted = DeleteSubmodel(TestingSubmodel.Id);
            deleted.Success.Should().BeTrue();

            var retrieved = RetrieveSubmodel(TestingSubmodel.Id);
            retrieved.Success.Should().BeFalse();
        }

        [TestMethod]
        public void Test102_CreateSubmodelElement()
        {
            Property<string> property = new Property<string>("MyTestProperty", "MyTestValue");

            var created = CreateSubmodelElement(".", property);
            created.Success.Should().BeTrue();

            created.Entity.Should().BeEquivalentTo(property,
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
            MainSubmodel.SubmodelElements.Add(coll);
            var created = CreateSubmodelElement(".", coll);
            created.Success.Should().BeTrue();
        }

        [TestMethod]
        public void Test104_RetrieveSubmodelElements()
        {
            var retrieved = RetrieveSubmodelElements();
            retrieved.Entity.Result.Should().ContainEquivalentOf(MainSubmodel.SubmodelElements["MyCollection"],
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
            var mySubFloat = MainSubmodel.SubmodelElements["MyCollection.MySubCollection.MySubSubFloat"].Cast<Property<float>>();
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
            var result = UpdateSubmodelElementValue("MyCollection.MySubCollection.MySubSubDouble", new PropertyValue(new ElementValue(1.8d)));
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
            retrieved.Entity.Result.Should().NotContainEquivalentOf(MainSubmodel.SubmodelElements["MyCollection"]);
        }

        #region Submodel Client

        public IResult<ISubmodel> RetrieveSubmodel()
        {
            return ((ISubmodelClient)Client).RetrieveSubmodel();
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

        #endregion

        #region Submodel Repository Client

        public Task<IResult<ISubmodel>> CreateSubmodelAsync(ISubmodel submodel)
        {
            return ((ISubmodelRepositoryClient)RepoClient).CreateSubmodelAsync(submodel);
        }

        public Task<IResult<ISubmodel>> RetrieveSubmodelAsync(Identifier id)
        {
            return ((ISubmodelRepositoryClient)RepoClient).RetrieveSubmodelAsync(id);
        }

        public Task<IResult<PagedResult<IElementContainer<ISubmodel>>>> RetrieveSubmodelsAsync(int limit = 100, string cursor = "", string semanticId = "", string idShort = "")
        {
            return ((ISubmodelRepositoryClient)RepoClient).RetrieveSubmodelsAsync(limit, cursor, semanticId, idShort);
        }

        public Task<IResult> UpdateSubmodelAsync(Identifier id, ISubmodel submodel)
        {
            return ((ISubmodelRepositoryClient)RepoClient).UpdateSubmodelAsync(id, submodel);
        }

        public Task<IResult> DeleteSubmodelAsync(Identifier id)
        {
            return ((ISubmodelRepositoryClient)RepoClient).DeleteSubmodelAsync(id);
        }

        public IResult<ISubmodel> CreateSubmodel(ISubmodel submodel)
        {
            return ((ISubmodelRepositoryInterface)RepoClient).CreateSubmodel(submodel);
        }

        public IResult<ISubmodel> RetrieveSubmodel(Identifier id)
        {
            return ((ISubmodelRepositoryInterface)RepoClient).RetrieveSubmodel(id);
        }

        public IResult<PagedResult<IElementContainer<ISubmodel>>> RetrieveSubmodels(int limit = 100, string cursor = "", string semanticId = "", string idShort = "")
        {
            return ((ISubmodelRepositoryInterface)RepoClient).RetrieveSubmodels(limit, cursor, semanticId, idShort);
        }

        public IResult UpdateSubmodel(Identifier id, ISubmodel submodel)
        {
            return ((ISubmodelRepositoryInterface)RepoClient).UpdateSubmodel(id, submodel);
        }

        public IResult DeleteSubmodel(Identifier id)
        {
            return ((ISubmodelRepositoryInterface)RepoClient).DeleteSubmodel(id);
        }

        #endregion
    }
}
