/*******************************************************************************
* Copyright (c) 2020, 2021 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using BaSyx.API.AssetAdministrationShell;
using BaSyx.API.Clients;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Utils.Client;
using BaSyx.Utils.ResultHandling;
using System;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Communication;

namespace BaSyx.API.Components
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

        public void SubscribeUpdates(string propertyId, Action<IValue> updateFunction)
        {
            throw new NotImplementedException();
        }

        public void PublishUpdate(string propertyId, IValue value)
        {
            throw new NotImplementedException();
        }

        public IResult PublishEvent(IEventMessage eventMessage, string topic, Action<IMessagePublishedEventArgs> MessagePublished, byte qosLevel)
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
        
        public IResult<InvocationResponse> InvokeOperation(string operationId, InvocationRequest invocationRequest)
        {
            return submodelClient.InvokeOperation(operationId, invocationRequest);
        }
        

        public IResult PublishEvent(IEventMessage eventMessage, string topic, Action<IMessagePublishedEventArgs> MessagePublished, byte qosLevel, bool retain)
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

        public IResult<ISubmodelElement> CreateOrUpdateSubmodelElement(string rootSubmodelElementPath, ISubmodelElement submodelElement)
        {
            return submodelClient.CreateOrUpdateSubmodelElement(rootSubmodelElementPath, submodelElement);
        }

        public IResult<IElementContainer<ISubmodelElement>> RetrieveSubmodelElements()
        {
            return submodelClient.RetrieveSubmodelElements();
        }

        public IResult<ISubmodelElement> RetrieveSubmodelElement(string submodelElementId)
        {
            return submodelClient.RetrieveSubmodelElement(submodelElementId);
        }

        public IResult<IValue> RetrieveSubmodelElementValue(string submodelElementId)
        {
            return submodelClient.RetrieveSubmodelElementValue(submodelElementId);
        }

        public IResult DeleteSubmodelElement(string submodelElementId)
        {
            return submodelClient.DeleteSubmodelElement(submodelElementId);
        }

        public IResult<CallbackResponse> InvokeOperationAsync(string operationId, InvocationRequest invocationRequest)
        {
            return submodelClient.InvokeOperationAsync(operationId, invocationRequest);
        }

        public IResult<InvocationResponse> GetInvocationResult(string operationId, string requestId)
        {
            return submodelClient.GetInvocationResult(operationId, requestId);
        }

        public IResult UpdateSubmodelElementValue(string submodelElementId, IValue value)
        {
            return submodelClient.UpdateSubmodelElementValue(submodelElementId, value);
        }
    }
}
