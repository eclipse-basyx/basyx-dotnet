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
using BaSyx.Models.Connectivity;
using BaSyx.Utils.Client;
using BaSyx.Utils.ResultHandling;
using BaSyx.Utils.ResultHandling.ResultTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaSyx.API.ServiceProvider
{
    public class SubmodelClientServiceProvider : ISubmodelServiceProvider
    {
        private ISubmodelClient _submodelClient;

        private ISubmodelDescriptor _serviceDescriptor;

        public ISubmodelDescriptor ServiceDescriptor
        {
            get
            {
                return _serviceDescriptor;
            }
            private set
            {
                _serviceDescriptor = value;
            }
        }

        public SubmodelClientServiceProvider(ISubmodelClient submodelClient)
        {
            _submodelClient = submodelClient;
            var submodel_retrieved = _submodelClient.RetrieveSubmodel();
            if (!submodel_retrieved.Success)
                throw new Exception("Could not retrieve submodel to create service provider: " + submodel_retrieved.Messages?.ToString());
            _serviceDescriptor = new SubmodelDescriptor(submodel_retrieved.Entity, new List<IEndpoint>() { _submodelClient.Endpoint });
        }

        public void BindTo(ISubmodel element)
        { }

        public IResult<ISubmodelElement> CreateSubmodelElement(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            return _submodelClient.CreateSubmodelElement(rootIdShortPath, submodelElement);
        }

        public IResult DeleteSubmodelElement(string idShortPath)
        {
            return _submodelClient.DeleteSubmodelElement(idShortPath);
        }

        public ISubmodel GetBinding()
        {
            ISubmodel submodel = _submodelClient.RetrieveSubmodel().Entity;
            return submodel;
        }

        public IResult<InvocationResponse> GetInvocationResult(string idShortPath, string requestId)
        {
            return _submodelClient.GetInvocationResult(idShortPath, requestId);
        }

        public IResult<InvocationResponse> InvokeOperation(string idShortPath, InvocationRequest invocationRequest, bool async = false)
        {
            return _submodelClient.InvokeOperation(idShortPath, invocationRequest, async);
        }

        public IResult<ISubmodel> RetrieveSubmodel(RequestLevel level = RequestLevel.Deep, RequestExtent extent = RequestExtent.WithoutBlobValue)
        {
            return _submodelClient.RetrieveSubmodel(level, extent);
        }

        public IResult<ISubmodelElement> RetrieveSubmodelElement(string idShortPath)
        {
            return _submodelClient.RetrieveSubmodelElement(idShortPath);
        }

        public IResult<PagedResult<IElementContainer<ISubmodelElement>>> RetrieveSubmodelElements(int limit = 100, string cursor = "")
        {
            return _submodelClient.RetrieveSubmodelElements(limit, cursor);
        }

        public IResult<ValueScope> RetrieveSubmodelElementValue(string idShortPath)
        {
            return _submodelClient.RetrieveSubmodelElementValue(idShortPath);
        }   

        public IResult UpdateSubmodel(ISubmodel submodel)
        {
            return _submodelClient.UpdateSubmodel(submodel);
        }

        public IResult UpdateSubmodelElement(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            return _submodelClient.UpdateSubmodelElement(rootIdShortPath, submodelElement);
        }

        public IResult UpdateSubmodelElementValue(string idShortPath, ValueScope value)
        {
            return _submodelClient.UpdateSubmodelElementValue(idShortPath, value);
        }

        public SubmodelElementHandler RetrieveSubmodelElementHandler(string pathToElement)
        {
            return new SubmodelElementHandler(
                prop =>
                {
                    var result = _submodelClient.RetrieveSubmodelElementValue(pathToElement);
                    return Task.FromResult<ValueScope>(result.Entity);
                },
                (prop, value) =>
                {
                    var result = _submodelClient.UpdateSubmodelElementValue(pathToElement, value);
                    return Task.FromResult<IResult>(result);
                });
        }

        public MethodCalledHandler RetrieveMethodCalledHandler(string pathToOperation)
        {
            return new MethodCalledHandler((op, inArgs, inoutArgs, outArgs, ct) =>
            {
                var invoked = _submodelClient.InvokeOperation(pathToOperation, new InvocationRequest(Guid.NewGuid().ToString())
                {
                    InputArguments = inArgs,
                    InOutputArguments = inoutArgs
                });

                if (invoked.Success)
                {
                    outArgs = invoked.Entity.OutputArguments;
                    inoutArgs = invoked.Entity.InOutputArguments;
                }
                return new OperationResult(invoked.Entity.Success, invoked.Entity.Messages);
            });
        }

        #region Not required to implement

        public void ConfigureEventHandler(IMessageClient messageClient)
        {
            throw new NotImplementedException();
        }

        public void SubscribeUpdates(string pathToSubmodelElement, Action<IValue> updateFunction)
        {
            throw new NotImplementedException();
        }  

        public IResult PublishEvent(IEventPayload eventMessage)
        {
            throw new NotImplementedException();
        }

        public void PublishUpdate(string pathToSubmodelElement, IValue value)
        {
            throw new NotImplementedException();
        }

        public void RegisterEventDelegate(string pathToEvent, EventDelegate eventDelegate)
        {
            throw new NotImplementedException();
        }

        public void RegisterMethodCalledHandler(string pathToOperation, MethodCalledHandler methodCalledHandler)
        {
            throw new NotImplementedException();
        }

        public void RegisterSubmodelElementHandler(string pathToElement, SubmodelElementHandler elementHandler)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
