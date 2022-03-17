using BaSyx.API.Clients;
using BaSyx.Models.AdminShell;
using BaSyx.Models.Extensions;
using BaSyx.Submodel.Client.Http;
using BaSyx.Utils.ResultHandling;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleAssetAdministrationShell;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SubmodelClientServerTests
{
    [TestClass]
    public class MainTest : ISubmodelClient
    {
        private static Submodel Submodel;

        private static SubmodelHttpClient Client;
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

        public IResult<ISubmodel> RetrieveSubmodel(RequestLevel level = RequestLevel.Deep, RequestContent content = RequestContent.Normal, RequestExtent extent = RequestExtent.WithoutBlobValue)
        {
            var result = Client.RetrieveSubmodel(level, content, extent);

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
            var result = Client.UpdateSubmodel(submodel);

            result.Success.Should().BeTrue();

            return result;
        }

        public IResult<ISubmodelElement> CreateSubmodelElement(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            var result = Client.CreateSubmodelElement(rootIdShortPath, submodelElement);

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
            var result = Client.DeleteSubmodelElement(idShortPath);

            result.Success.Should().BeTrue();

            return result;
        }

        public IResult<InvocationResponse> GetInvocationResult(string idShortPath, string requestId)
        {
            var result = Client.GetInvocationResult(idShortPath, requestId);

            result.Success.Should().BeTrue();

            return result;
        }

        public IResult<InvocationResponse> InvokeOperation(string idShortPath, InvocationRequest invocationRequest, bool async)
        {
            var result = Client.InvokeOperation(idShortPath, invocationRequest, async);

            result.Success.Should().BeTrue();

            return result;
        }

        public IResult<ISubmodelElement> RetrieveSubmodelElement(string idShortPath)
        {
            var result = Client.RetrieveSubmodelElement(idShortPath);

            result.Success.Should().BeTrue();

            return result;
        }

        public IResult<IElementContainer<ISubmodelElement>> RetrieveSubmodelElements()
        {
            var result = Client.RetrieveSubmodelElements();

            result.Success.Should().BeTrue();

            return result;
        }

        public IResult<IValue> RetrieveSubmodelElementValue(string idShortPath)
        {
            var result = Client.RetrieveSubmodelElementValue(idShortPath);

            result.Success.Should().BeTrue();

            return result;
        }

        public IResult<ISubmodelElement> UpdateSubmodelElement(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            var result = Client.UpdateSubmodelElement(rootIdShortPath, submodelElement);

            result.Success.Should().BeTrue();

            return result;
        }

        public IResult UpdateSubmodelElementValue(string idShortPath, IValue value)
        {
            var result = Client.UpdateSubmodelElementValue(idShortPath, value);

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
    }
}
