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
using BaSyx.Models.AdminShell;
using BaSyx.Utils.Client;
using BaSyx.Utils.ResultHandling;
using System;
using BaSyx.Models.Connectivity;

namespace BaSyx.API.ServiceProvider
{
    public class DistributedSubmodelServiceProvider : ISubmodelServiceProvider
    {
        public ISubmodel Submodel => GetBinding();

        public ISubmodelDescriptor ServiceDescriptor { get; }

        private readonly ISubmodelClient submodelClient;

        public DistributedSubmodelServiceProvider(ISubmodelClientFactory submodelClientFactory, ISubmodelDescriptor serviceDescriptor)
        {
            ServiceDescriptor = serviceDescriptor;
            submodelClient = submodelClientFactory.CreateSubmodelClient(serviceDescriptor);            
        }

        public void SubscribeUpdates(string propertyId, Action<ValueScope> updateFunction)
        {
            throw new NotImplementedException();
        }

        public void PublishUpdate(string propertyId, IValue value)
        {
            throw new NotImplementedException();
        }

        public SubmodelElementHandler RetrieveSubmodelElementHandler(string submodelElementIdShort)
        {
            throw new NotImplementedException();
        }

        public void RegisterSubmodelElementHandler(string submodelElementIdShort, SubmodelElementHandler handler)
        {
            throw new NotImplementedException();
        }

        public MethodCalledHandler RetrieveMethodCalledHandler(string pathToOperation)
        {
            throw new NotImplementedException();
        }

        public void RegisterMethodCalledHandler(string pathToOperation, MethodCalledHandler methodCalledHandler)
        {
            throw new NotImplementedException();
        }

        public void RegisterEventHandler(IMessageClient messageClient)
        {
            throw new NotImplementedException();
        }

        public void BindTo(ISubmodel element)
        {
            throw new NotImplementedException();
        }

        public ISubmodel GetBinding()
        {
            var submodel = RetrieveSubmodel();
            if (submodel.Success && submodel.Entity != null)
                return submodel.Entity;
            return null;
        }
        public IResult<ISubmodel> RetrieveSubmodel()
        {
            return submodelClient.RetrieveSubmodel();
        }
        
        public IResult<InvocationResponse> InvokeOperation(string operationId, InvocationRequest invocationRequest, bool async)
        {
            return submodelClient.InvokeOperation(operationId, invocationRequest);
        }
        

        public IResult PublishEvent(IEventPayload eventMessage)
        {
            throw new NotImplementedException();
        }

        public void RegisterEventHandler(string eventId, Delegate handler)
        {
            throw new NotImplementedException();
        }

        public void ConfigureEventHandler(IMessageClient messageClient)
        {
            throw new NotImplementedException();
        }

        public void RegisterEventDelegate(string eventId, EventDelegate handler)
        {
            throw new NotImplementedException();
        }

        public IResult<ISubmodelElement> UpdateSubmodelElement(string rootSubmodelElementPath, ISubmodelElement submodelElement)
        {
            return submodelClient.UpdateSubmodelElement(rootSubmodelElementPath, submodelElement);
        }

        public IResult<IElementContainer<ISubmodelElement>> RetrieveSubmodelElements()
        {
            return submodelClient.RetrieveSubmodelElements();
        }

        public IResult<ISubmodelElement> RetrieveSubmodelElement(string submodelElementId)
        {
            return submodelClient.RetrieveSubmodelElement(submodelElementId);
        }

        public IResult<ValueScope> RetrieveSubmodelElementValue(string submodelElementId)
        {
            return submodelClient.RetrieveSubmodelElementValue(submodelElementId);
        }

        public IResult DeleteSubmodelElement(string submodelElementId)
        {
            return submodelClient.DeleteSubmodelElement(submodelElementId);
        }

        public IResult<InvocationResponse> GetInvocationResult(string operationId, string requestId)
        {
            return submodelClient.GetInvocationResult(operationId, requestId);
        }

        public IResult UpdateSubmodelElementValue(string submodelElementId, ValueScope value)
        {
            return submodelClient.UpdateSubmodelElementValue(submodelElementId, value);
        }

        public IResult<ISubmodel> RetrieveSubmodel(RequestLevel level, RequestExtent extent)
        {
            throw new NotImplementedException();
        }

        public IResult UpdateSubmodel(ISubmodel submodel)
        {
            throw new NotImplementedException();
        }

        public IResult<ISubmodelElement> CreateSubmodelElement(string rootIdShortPath, ISubmodelElement submodelElement)
        {
            throw new NotImplementedException();
        }
    }
}
