using BaSyx.AAS.Client.Http;
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

namespace AdminShellClientServerTests
{
    [TestClass]
    public class MainTest : IAssetAdministrationShellClient
    {
        private static Submodel Submodel;
        private static AssetAdministrationShell AdminShell;

        private static AssetAdministrationShellHttpClient Client;
        static MainTest()
        {
            Server.Run();
            Submodel = TestSubmodel.GetSubmodel("TestSubmodel");
            AdminShell = TestAssetAdministrationShell.GetAssetAdministrationShell();
            var mainSubmodel = TestSubmodel.GetSubmodel("MainSubmodel");
            AdminShell.Submodels.Add(mainSubmodel);
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
                Kind = AssetKind.Instance,
                BillOfMaterial = Submodel.CreateReference(),
                DefaultThumbnail = new FileElement("MyThumbnail") { Value = "/test/path/to/thumbnail.jpg"},
                GlobalAssetId = new Reference(new Key(KeyElements.GlobalReference, KeyType.IRI, AdminShell.Asset.Identification.Id, false)),
                SpecificAssetIds = new List<IdentifierKeyValuePair>()
                {
                    new IdentifierKeyValuePair() { Key = "MyInventoryNumber", Value = "123" }
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
                .Excluding(p => p.Submodels)
                .Excluding(p => p.AssetInformation.DefaultThumbnail.EmbeddedDataSpecifications)
                .Excluding(p => p.AssetInformation.DefaultThumbnail.Parent)
                .Excluding(p => p.AssetInformation.DefaultThumbnail.Get)
                .Excluding(p => p.AssetInformation.DefaultThumbnail.Set);
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
                options
                .Excluding(p => p.DefaultThumbnail.EmbeddedDataSpecifications)
                .Excluding(p => p.DefaultThumbnail.Parent)
                .Excluding(p => p.DefaultThumbnail.Get)
                .Excluding(p => p.DefaultThumbnail.Set);
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
            UpdateSubmodel(Submodel);
        }

        [TestMethod]
        public void Test101_RetrieveSubmodel()
        {
            RetrieveSubmodel();
        }

        [TestMethod]
        public void Test102_CreateSubmodelElement()
        {
            Property<string> property = new Property<string>("MyTestProperty", "MyTestValue");

            CreateSubmodelElement(".", property);
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
            CreateSubmodelElement(".", coll);
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
            var retrieved = RetrieveSubmodelElement("MyCollection.MySubCollection.MySubSubFloat").Entity.Description.Should().BeEquivalentTo(mySubFloat.Description);
        }

        [TestMethod]
        public void Test107_RetrieveSubmodelElementHierarchy()
        {
            var result = RetrieveSubmodelElement("MyCollection.MySubCollection");
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
            DeleteSubmodelElement("MyCollection");
            var retrieved = RetrieveSubmodelElements().Entity.Should().NotContainEquivalentOf(Submodel.SubmodelElements["MyCollection"]);
        }

        [TestMethod]
        public void Test112_DeleteSubmodelReference()
        {
            var result = DeleteSubmodelReference(Submodel.Identification.Id);
            result.Success.Should().BeTrue();
        }

        public IResult<ISubmodel> RetrieveSubmodel(RequestLevel level = RequestLevel.Deep, RequestContent content = RequestContent.Normal, RequestExtent extent = RequestExtent.WithoutBlobValue)
        {
            var result = Client.RetrieveSubmodel(Submodel.Identification.Id, level, content, extent);

            result.Success.Should().BeTrue();
            result.Entity.IdShort.Should().BeEquivalentTo(Submodel.IdShort);
            result.Entity.Identification.Should().BeEquivalentTo(Submodel.Identification);
            result.Entity.Description.Should().BeEquivalentTo(Submodel.Description);
            result.Entity.DisplayName.Should().BeEquivalentTo(Submodel.DisplayName);
            result.Entity.SemanticId.Should().BeEquivalentTo(Submodel.SemanticId);
            result.Entity.Kind.Should().Be(Submodel.Kind);
            
            return result;
        } 

        public IResult UpdateSubmodel(ISubmodel submodel)
        {
            var result = Client.UpdateSubmodel(Submodel.Identification.Id, submodel);

            result.Success.Should().BeTrue();

            return result;
        }

        public IResult<ISubmodelElement> CreateSubmodelElement(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            var result = Client.CreateSubmodelElement(Submodel.Identification.Id, rootIdShortPath, submodelElement);

            result.Success.Should().BeTrue();
            result.Entity.Should().BeEquivalentTo(submodelElement, options =>
            {
                options
                .Excluding(p => p.EmbeddedDataSpecifications)
                .Excluding(p => p.Parent)
                .Excluding(p => p.Get)
                .Excluding(p => p.Set);
                return options;
            });

            return result;
        }

        public IResult DeleteSubmodelElement(string idShortPath)
        {
            var result = Client.DeleteSubmodelElement(Submodel.Identification.Id, idShortPath);

            result.Success.Should().BeTrue();

            return result;
        }

        public IResult<InvocationResponse> GetInvocationResult(string idShortPath, string requestId)
        {
            string mainSubmodelId = AdminShell.Submodels["MainSubmodel"].Identification.Id;
            var result = Client.GetInvocationResult(mainSubmodelId, idShortPath, requestId);

            result.Success.Should().BeTrue();

            return result;
        }

        public IResult<InvocationResponse> InvokeOperation(string idShortPath, InvocationRequest invocationRequest, bool async)
        {
            string mainSubmodelId = AdminShell.Submodels["MainSubmodel"].Identification.Id;
            var result = Client.InvokeOperation(mainSubmodelId, idShortPath, invocationRequest, async);

            result.Success.Should().BeTrue();

            return result;
        }

        public IResult<ISubmodelElement> RetrieveSubmodelElement(string idShortPath)
        {
            var result = Client.RetrieveSubmodelElement(Submodel.Identification.Id, idShortPath);

            result.Success.Should().BeTrue();

            return result;
        }

        public IResult<IElementContainer<ISubmodelElement>> RetrieveSubmodelElements()
        {
            var result = Client.RetrieveSubmodelElements(Submodel.Identification.Id);

            result.Success.Should().BeTrue();

            return result;
        }

        public IResult<IValue> RetrieveSubmodelElementValue(string idShortPath)
        {
            var result = Client.RetrieveSubmodelElementValue(Submodel.Identification.Id, idShortPath);

            result.Success.Should().BeTrue();

            return result;
        }

        public IResult<ISubmodelElement> UpdateSubmodelElement(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            var result = Client.UpdateSubmodelElement(Submodel.Identification.Id, rootIdShortPath, submodelElement);

            result.Success.Should().BeTrue();

            return result;
        }

        public IResult UpdateSubmodelElementValue(string idShortPath, IValue value)
        {
            var result = Client.UpdateSubmodelElementValue(Submodel.Identification.Id, idShortPath, value);

            result.Success.Should().BeTrue();

            return result;
        }

        public Task<IResult<ISubmodel>> RetrieveSubmodelAsync(RequestLevel level = RequestLevel.Deep, RequestContent content = RequestContent.Normal, RequestExtent extent = RequestExtent.WithoutBlobValue)
        {
            return ((ISubmodelClient)Client).RetrieveSubmodelAsync(level, content, extent);
        }

        public Task<IResult> UpdateSubmodelAsync(ISubmodel submodel)
        {
            return ((ISubmodelClient)Client).UpdateSubmodelAsync(submodel);
        }

        public Task<IResult<ISubmodelElement>> CreateSubmodelElementAsync(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            return ((ISubmodelClient)Client).CreateSubmodelElementAsync(rootIdShortPath, submodelElement);
        }

        public Task<IResult<ISubmodelElement>> UpdateSubmodelElementAsync(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            return ((ISubmodelClient)Client).UpdateSubmodelElementAsync(rootIdShortPath, submodelElement);
        }

        public Task<IResult<IElementContainer<ISubmodelElement>>> RetrieveSubmodelElementsAsync()
        {
            return ((ISubmodelClient)Client).RetrieveSubmodelElementsAsync();
        }

        public Task<IResult<ISubmodelElement>> RetrieveSubmodelElementAsync(string idShortPath)
        {
            return ((ISubmodelClient)Client).RetrieveSubmodelElementAsync(idShortPath);
        }

        public Task<IResult<IValue>> RetrieveSubmodelElementValueAsync(string idShortPath)
        {
            return ((ISubmodelClient)Client).RetrieveSubmodelElementValueAsync(idShortPath);
        }

        public Task<IResult> UpdateSubmodelElementValueAsync(string idShortPath, IValue value)
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
    }
}
