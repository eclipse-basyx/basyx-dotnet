using BaSyx.API.Clients;
using BaSyx.Models.AdminShell;
using BaSyx.Submodel.Client.Http;
using BaSyx.Utils.ResultHandling;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SubmodelClientServerTests
{
    [TestClass]
    public class MainTest : ISubmodelClient
    {
        static MainTest()
        {
            Server.Run();
        }


        [TestMethod]
        public void Test_RetrieveSubmodel()
        {
            RetrieveSubmodel();
        }

        public IResult<ISubmodel> RetrieveSubmodel(RequestLevel level = RequestLevel.Deep, RequestContent content = RequestContent.Normal, RequestExtent extent = RequestExtent.WithoutBlobValue)
        {
            SubmodelHttpClient client = new SubmodelHttpClient(new Uri(Server.ServerUrl));
            var result = client.RetrieveSubmodel(level, content, extent);

            result.Success.Should().BeTrue();
            return result;
        }

        [TestMethod]
        public void Test_UpdateSubmodel()
        {
            RetrieveSubmodel();
        }

        public IResult UpdateSubmodel(ISubmodel submodel)
        {
            throw new System.NotImplementedException();
        }

        public IResult<ISubmodelElement> CreateSubmodelElement(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            throw new System.NotImplementedException();
        }

        public IResult DeleteSubmodelElement(string idShortPath)
        {
            throw new System.NotImplementedException();
        }

        public IResult<InvocationResponse> GetInvocationResult(string idShortPath, string requestId)
        {
            throw new System.NotImplementedException();
        }

        public IResult<InvocationResponse> InvokeOperation(string idShortPath, InvocationRequest invocationRequest)
        {
            throw new System.NotImplementedException();
        }

        public IResult<CallbackResponse> InvokeOperationAsync(string idShortPath, InvocationRequest invocationRequest)
        {
            throw new System.NotImplementedException();
        }

        public IResult<ISubmodelElement> RetrieveSubmodelElement(string idShortPath)
        {
            throw new System.NotImplementedException();
        }

        public IResult<IElementContainer<ISubmodelElement>> RetrieveSubmodelElements()
        {
            throw new System.NotImplementedException();
        }

        public IResult<IValue> RetrieveSubmodelElementValue(string idShortPath)
        {
            throw new System.NotImplementedException();
        }

        public IResult<ISubmodelElement> UpdateSubmodelElement(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            throw new System.NotImplementedException();
        }

        public IResult UpdateSubmodelElementValue(string idShortPath, IValue value)
        {
            throw new System.NotImplementedException();
        }
    }
}
