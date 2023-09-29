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
using BaSyx.Clients.AdminShell.Http;
using BaSyx.API.Clients;
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
using BaSyx.Models.Connectivity;

namespace AdminShellClientServerTests
{
    [TestClass]
    public class MainTest : IAssetAdministrationShellClient
    {
        private static Submodel Submodel;
        private static AssetAdministrationShell AdminShell;

        private static AssetAdministrationShellHttpClient Client;

        public IEndpoint Endpoint => ((IClient)Client).Endpoint;

        static MainTest()
        {
            Server.Run();   
            AdminShell = TestAssetAdministrationShell.GetAssetAdministrationShell("MainAdminShell");
            var mainSubmodel = TestSubmodel.GetSubmodel("MainSubmodel");
            AdminShell.Submodels.Add(mainSubmodel);
            Submodel = TestSubmodel.GetSubmodel("TestSubmodel");
            Client = new AssetAdministrationShellHttpClient(new Uri(Server.ServerUrl));
        }

        [TestMethod]
        public void Test000_RetrieveAssetAdministrationShell()
        {
            var result = RetrieveAssetAdministrationShell(default);
            result.Success.Should().BeTrue();
            result.Entity.Should().BeEquivalentTo(AdminShell, options =>
            {
                options
                .Excluding(p => p.Submodels);
                return options;
            });
        }

        [TestMethod]
        public void Test001_UpdateAssetAdministrationShell()
        {
            LangStringSet newDescription = new LangStringSet()
            {
                new LangString("de", "Meine neue Beschreibung"),
                new LangString("en", "My new description")
            };
            AdminShell.Description = newDescription;
            var updated = UpdateAssetAdministrationShell(AdminShell);
            updated.Success.Should().BeTrue();

            var result = RetrieveAssetAdministrationShell(default);
            result.Success.Should().BeTrue();
            result.Entity.Should().BeEquivalentTo(AdminShell, options =>
            {
                options
                .Excluding(p => p.Submodels);
                return options;
            });
        }

        [TestMethod]
        public void Test002_UpdateAssetInformation()
        {
            AssetInformation assetInfo = new AssetInformation()
            {
                AssetKind = AssetKind.Instance,
                DefaultThumbnail = new Resource() { ContentType = "image/jpg", Path = "/test/path/to/thumbnail.jpg"},
                GlobalAssetId = AdminShell.AssetInformation.GlobalAssetId,
                SpecificAssetIds = new List<SpecificAssetId>()
                {
                    new SpecificAssetId() { Name = "MyInventoryNumber", Value = "123" }
                }
            };
            AdminShell.AssetInformation = assetInfo;
            var updated = UpdateAssetInformation(assetInfo);
            updated.Success.Should().BeTrue();

            var result = RetrieveAssetAdministrationShell(default);
            result.Success.Should().BeTrue();
            result.Entity.Should().BeEquivalentTo(AdminShell, options =>
            {
                options
                .Excluding(p => p.Submodels);
                return options;
            });
        }

        [TestMethod]
        public void Test003_RetrieveAssetInformation()
        {
            var result = RetrieveAssetInformation();
            result.Success.Should().BeTrue();
            result.Entity.Should().BeEquivalentTo(AdminShell.AssetInformation, options =>
            {
                return options;
            });
        }

        [TestMethod]
        public void Test010_CreateSubmodelReference()
        {
            var reference = Submodel.CreateReference();
            var result = CreateSubmodelReference(reference);
            result.Success.Should().BeTrue();
            result.Entity.Should().BeEquivalentTo(reference);
        }

        [TestMethod]
        public void Test011_RetrieveAllSubmodelReferences()
        {
            var reference = Submodel.CreateReference();
            var result = RetrieveAllSubmodelReferences();
            result.Success.Should().BeTrue();
            result.Entity.Should().HaveCount(2);
            result.Entity.Should().ContainEquivalentOf(reference);
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
                   new Property<string>("MySubString", "MySubStringValue"),
                   new Property<int>("MySubInt", 5),
                   new Property<double>("MySubDouble", 4.5d),
                   new Property<float>("MySubFloat", 2.3f),
                   new SubmodelElementCollection("MySubCollection")
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
            };
            Submodel.SubmodelElements.Add(coll);
            var created = CreateSubmodelElement(".", coll);
            created.Success.Should().BeTrue();
        }

        [TestMethod]
        public void Test104_RetrieveSubmodelElements()
        {
            var result = RetrieveSubmodelElements();
            result.Entity.Should().ContainEquivalentOf(Submodel.SubmodelElements["MyCollection"],
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
            result.Entity.Cast<ISubmodelElementCollection>().Value["MySubSubInt"].GetValue<int>().Should().Be(6);
        }

        [TestMethod]
        public void Test108_UpdateSubmodelElementValue()
        {
            var result = UpdateSubmodelElementValue("MyCollection.MySubCollection.MySubSubDouble", new ElementValue(1.8d));
            result.Success.Should().BeTrue();
        }

        [TestMethod]
        public void Test109_RetrieveSubmodelElementValue()
        {
            var result = RetrieveSubmodelElementValue("MyCollection.MySubCollection.MySubSubDouble");
            result.Success.Should().BeTrue();
            result.Entity.ToObject<double>().Should().Be(1.8d);
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
            retrieved.Entity.Should().NotContainEquivalentOf(Submodel.SubmodelElements["MyCollection"]);
        }

        [TestMethod]
        public void Test112_DeleteSubmodelReference()
        {
            var result = DeleteSubmodelReference(Submodel.Id);
            result.Success.Should().BeTrue();
        }

        #region Submodel Client

        public IResult<ISubmodel> RetrieveSubmodel(RequestLevel level = RequestLevel.Deep, RequestContent content = RequestContent.Normal, RequestExtent extent = RequestExtent.WithoutBlobValue)
        {
            return Client.RetrieveSubmodel(Submodel.Id, level, content, extent);
        }

        public IResult UpdateSubmodel(ISubmodel submodel)
        {
            return Client.UpdateSubmodel(Submodel.Id, submodel);
        }

        public IResult<ISubmodelElement> CreateSubmodelElement(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            return Client.CreateSubmodelElement(Submodel.Id, rootIdShortPath, submodelElement);
        }

        public IResult DeleteSubmodelElement(string idShortPath)
        {
            return Client.DeleteSubmodelElement(Submodel.Id, idShortPath);
        }

        public IResult<InvocationResponse> GetInvocationResult(string idShortPath, string requestId)
        {
            return Client.GetInvocationResult(AdminShell.Submodels["MainSubmodel"].Id, idShortPath, requestId);
        }

        public IResult<InvocationResponse> InvokeOperation(string idShortPath, InvocationRequest invocationRequest, bool async)
        {
            return Client.InvokeOperation(AdminShell.Submodels["MainSubmodel"].Id, idShortPath, invocationRequest, async);
        }

        public IResult<ISubmodelElement> RetrieveSubmodelElement(string idShortPath)
        {
            return Client.RetrieveSubmodelElement(Submodel.Id, idShortPath);
        }

        public IResult<IElementContainer<ISubmodelElement>> RetrieveSubmodelElements()
        {
            return Client.RetrieveSubmodelElements(Submodel.Id);
        }

        public IResult<IValue> RetrieveSubmodelElementValue(string idShortPath)
        {
            return Client.RetrieveSubmodelElementValue(Submodel.Id, idShortPath);
        }

        public IResult<ISubmodelElement> UpdateSubmodelElement(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            return Client.UpdateSubmodelElement(Submodel.Id, rootIdShortPath, submodelElement);
        }

        public IResult UpdateSubmodelElementValue(string idShortPath, IValue value)
        {
            return Client.UpdateSubmodelElementValue(Submodel.Id, idShortPath, value);
        }

        public Task<IResult<ISubmodel>> RetrieveSubmodelAsync(RequestLevel level = RequestLevel.Deep, RequestContent content = RequestContent.Normal, RequestExtent extent = RequestExtent.WithoutBlobValue)
        {
            return Client.RetrieveSubmodelAsync(Submodel.Id, level, content, extent);
        }

        public Task<IResult> UpdateSubmodelAsync(ISubmodel submodel)
        {
            return Client.UpdateSubmodelAsync(Submodel.Id, submodel);
        }

        public Task<IResult<ISubmodelElement>> CreateSubmodelElementAsync(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            return Client.CreateSubmodelElementAsync(Submodel.Id, rootIdShortPath, submodelElement);
        }

        public Task<IResult<ISubmodelElement>> UpdateSubmodelElementAsync(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            return Client.UpdateSubmodelElementAsync(Submodel.Id, rootIdShortPath, submodelElement);
        }

        public Task<IResult<IElementContainer<ISubmodelElement>>> RetrieveSubmodelElementsAsync()
        {
            return Client.RetrieveSubmodelElementsAsync(Submodel.Id);
        }

        public Task<IResult<ISubmodelElement>> RetrieveSubmodelElementAsync(string idShortPath)
        {
            return Client.RetrieveSubmodelElementAsync(Submodel.Id, idShortPath);
        }

        public Task<IResult<IValue>> RetrieveSubmodelElementValueAsync(string idShortPath)
        {
            return Client.RetrieveSubmodelElementValueAsync(Submodel.Id, idShortPath);
        }

        public Task<IResult> UpdateSubmodelElementValueAsync(string idShortPath, IValue value)
        {
            return Client.UpdateSubmodelElementValueAsync(Submodel.Id, idShortPath, value);
        }

        public Task<IResult> DeleteSubmodelElementAsync(string idShortPath)
        {
            return Client.DeleteSubmodelElementAsync(Submodel.Id, idShortPath);
        }

        public Task<IResult<InvocationResponse>> InvokeOperationAsync(string idShortPath, InvocationRequest invocationRequest, bool async = false)
        {
            return Client.InvokeOperationAsync(AdminShell.Submodels["MainSubmodel"].Id, idShortPath, invocationRequest, async);
        }

        public Task<IResult<InvocationResponse>> GetInvocationResultAsync(string idShortPath, string requestId)
        {
            return Client.GetInvocationResultAsync(AdminShell.Submodels["MainSubmodel"].Id, idShortPath, requestId);
        }

        #endregion

        #region Asset Administration Shell Client

        public Task<IResult<IAssetAdministrationShell>> RetrieveAssetAdministrationShellAsync(RequestContent content)
        {
            return ((IAssetAdministrationShellClient)Client).RetrieveAssetAdministrationShellAsync(content);
        }

        public Task<IResult> UpdateAssetAdministrationShellAsync(IAssetAdministrationShell aas)
        {
            return ((IAssetAdministrationShellClient)Client).UpdateAssetAdministrationShellAsync(aas);
        }

        public Task<IResult<IAssetInformation>> RetrieveAssetInformationAsync()
        {
            return ((IAssetAdministrationShellClient)Client).RetrieveAssetInformationAsync();
        }

        public Task<IResult> UpdateAssetInformationAsync(IAssetInformation assetInformation)
        {
            return ((IAssetAdministrationShellClient)Client).UpdateAssetInformationAsync(assetInformation);
        }

        public Task<IResult<IEnumerable<IReference<ISubmodel>>>> RetrieveAllSubmodelReferencesAsync()
        {
            return ((IAssetAdministrationShellClient)Client).RetrieveAllSubmodelReferencesAsync();
        }

        public Task<IResult<IReference>> CreateSubmodelReferenceAsync(IReference submodelRef)
        {
            return ((IAssetAdministrationShellClient)Client).CreateSubmodelReferenceAsync(submodelRef);
        }

        public Task<IResult> DeleteSubmodelReferenceAsync(string submodelIdentifier)
        {
            return ((IAssetAdministrationShellClient)Client).DeleteSubmodelReferenceAsync(submodelIdentifier);
        }

        public IResult<IAssetAdministrationShell> RetrieveAssetAdministrationShell(RequestContent content)
        {
            return ((IAssetAdministrationShellInterface)Client).RetrieveAssetAdministrationShell(content);
        }

        public IResult UpdateAssetAdministrationShell(IAssetAdministrationShell aas)
        {
            return ((IAssetAdministrationShellInterface)Client).UpdateAssetAdministrationShell(aas);
        }

        public IResult<IAssetInformation> RetrieveAssetInformation()
        {
            return ((IAssetAdministrationShellInterface)Client).RetrieveAssetInformation();
        }

        public IResult UpdateAssetInformation(IAssetInformation assetInformation)
        {
            return ((IAssetAdministrationShellInterface)Client).UpdateAssetInformation(assetInformation);
        }

        public IResult<IEnumerable<IReference<ISubmodel>>> RetrieveAllSubmodelReferences()
        {
            return ((IAssetAdministrationShellInterface)Client).RetrieveAllSubmodelReferences();
        }

        public IResult<IReference> CreateSubmodelReference(IReference submodelRef)
        {
            return ((IAssetAdministrationShellInterface)Client).CreateSubmodelReference(submodelRef);
        }

        public IResult DeleteSubmodelReference(string submodelIdentifier)
        {
            return ((IAssetAdministrationShellInterface)Client).DeleteSubmodelReference(submodelIdentifier);
        }

        #endregion
    }
}
