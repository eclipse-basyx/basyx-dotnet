/*******************************************************************************
* Copyright (c) 2020 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using BaSyx.API.AssetAdministrationShell;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Utils.ResultHandling;
using BaSyx.Utils.Client;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.Linq;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Communication;
using System.Threading.Tasks;
using NLog;
using System.Threading;

namespace BaSyx.API.Components
{
    /// <summary>
    /// Reference implementation of ISubmodelServiceProvider interface
    /// </summary>
    public class SubmodelServiceProvider : ISubmodelServiceProvider
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private ISubmodel _submodel;

        public const int DEFAULT_TIMEOUT = 30000;
        public ISubmodelDescriptor ServiceDescriptor { get; internal set; }

        private readonly Dictionary<string, MethodCalledHandler> methodCalledHandler;
        private readonly Dictionary<string, SubmodelElementHandler> submodelElementHandler;
        private readonly Dictionary<string, Action<IValue>> updateFunctions;
        private readonly Dictionary<string, EventDelegate> eventDelegates;
        private readonly Dictionary<string, InvocationResponse> invocationResults;
        
        private IMessageClient messageClient;

        /// <summary>
        /// Constructor for SubmodelServiceProvider
        /// </summary>
        public SubmodelServiceProvider()
        {
            methodCalledHandler = new Dictionary<string, MethodCalledHandler>();
            submodelElementHandler = new Dictionary<string, SubmodelElementHandler>();
            updateFunctions = new Dictionary<string, Action<IValue>>();
            eventDelegates = new Dictionary<string, EventDelegate>();
            invocationResults = new Dictionary<string, InvocationResponse>();
        }
        /// <summary>
        /// Contructor for SubmodelServiceProvider with a Submodel object to bind to
        /// </summary>
        /// <param name="submodel">Submodel object</param>
        public SubmodelServiceProvider(ISubmodel submodel) : this()
        {
            BindTo(submodel);
        }
        /// <summary>
        /// Contructor for SubmodelServiceProvider with a Submodel object to bind to and a SubmodelDescriptor as ServiceDescriptor
        /// </summary>
        /// <param name="submodel">Submodel object</param>
        /// <param name="submodelDescriptor">SubmodelDescriptor object</param>
        public SubmodelServiceProvider(ISubmodel submodel, ISubmodelDescriptor submodelDescriptor) : this()
        {
            _submodel = submodel;
            ServiceDescriptor = submodelDescriptor;
        }

        /// <summary>
        /// Bind this SubmodelServiceProvider to a specific Submodel
        /// </summary>
        /// <param name="submodel">Submodel object</param>
        public void BindTo(ISubmodel submodel)
        {
            _submodel = submodel;
            ServiceDescriptor = new SubmodelDescriptor(submodel, null);
        }

        /// <summary>
        /// Returns the model binding of this SubmodelServiceProvider
        /// </summary>
        /// <returns>Submodel object</returns>
        public ISubmodel GetBinding()
        {
            return _submodel;
        }

        public void UseInMemorySubmodelElementHandler()
        {
            UseInMemorySubmodelElementHandlerInternal(_submodel.SubmodelElements);
        }
        private void UseInMemorySubmodelElementHandlerInternal(IElementContainer<ISubmodelElement> submodelElements)
        {
            if (submodelElements.HasChildren())
            {
                foreach (var child in submodelElements.Children)
                {
                    UseInMemorySubmodelElementHandlerInternal(child);
                }
            }
            if(submodelElements.Value != null)
            {
                if (submodelElements.Value.ModelType != ModelType.Operation)
                    RegisterSubmodelElementHandler(submodelElements.Path, new SubmodelElementHandler(submodelElements.Value.Get, submodelElements.Value.Set));                
            }
        }

        /// <summary>
        /// Use as specific SubmodelElementHandler for all SubmodelElements
        /// </summary>
        /// <param name="elementHandler">SubmodelElementHandler</param>
        public void UseSubmodelElementHandler(SubmodelElementHandler elementHandler)
        {
            UseSubmodelElementHandlerInternal(_submodel.SubmodelElements, elementHandler, null);
        }
        /// <summary>
        /// Use a specific SubmodelElementHandler for all SubmodelElements of a specific ModelType (e.g. Property) except Operations
        /// </summary>
        /// <param name="elementHandler">SubmodelElementHandler</param>
        /// <param name="modelType">ModelType</param>
        public void UseSubmodelElementHandlerForModelType(SubmodelElementHandler elementHandler, ModelType modelType)
        {
            UseSubmodelElementHandlerInternal(_submodel.SubmodelElements, elementHandler, modelType);
        }

        private void UseSubmodelElementHandlerInternal(IElementContainer<ISubmodelElement> submodelElements, SubmodelElementHandler elementHandler, ModelType modelType = null)
        {
            if (submodelElements.HasChildren())
            {
                foreach (var child in submodelElements.Children)
                {
                    UseSubmodelElementHandlerInternal(child, elementHandler, modelType);
                }
            }
            if (submodelElements.Value != null)
            {
                if (modelType == null)
                    RegisterSubmodelElementHandler(submodelElements.Path, elementHandler);
                else if (submodelElements.Value.ModelType == modelType)
                    RegisterSubmodelElementHandler(submodelElements.Path, elementHandler);
                else
                    return;
            }
        }
        /// <summary>
        /// Use a specific MethodCalledHandler for all Operations
        /// </summary>
        /// <param name="methodCalledHandler"></param>
        public void UseOperationHandler(MethodCalledHandler methodCalledHandler)
        {
            UseOperationHandlerInternal(_submodel.SubmodelElements, methodCalledHandler);
        }

        private void UseOperationHandlerInternal(IElementContainer<ISubmodelElement> submodelElements, MethodCalledHandler methodCalledHandler)
        {
            if (submodelElements.HasChildren())
            {
                foreach (var child in submodelElements.Children)
                {
                    UseOperationHandlerInternal(child, methodCalledHandler);
                }
            }
            else
            {
                if (submodelElements.Value is IOperation)
                    RegisterMethodCalledHandler(submodelElements.Path, methodCalledHandler);
                else
                    return;
            }
        }
        
        public MethodCalledHandler RetrieveMethodCalledHandler(string pathToOperation)
        {
            if (methodCalledHandler.TryGetValue(pathToOperation, out MethodCalledHandler handler))
                return handler;
            else
                return null;
        }
       
        public SubmodelElementHandler RetrieveSubmodelElementHandler(string pathToElement)
        {
            if (submodelElementHandler.TryGetValue(pathToElement, out SubmodelElementHandler elementHandler))
                return elementHandler;
            else
                return null;
        }
      
        public void RegisterSubmodelElementHandler(string pathToElement, SubmodelElementHandler elementHandler)
        {
            if (!submodelElementHandler.ContainsKey(pathToElement))
                submodelElementHandler.Add(pathToElement, elementHandler);
            else
                submodelElementHandler[pathToElement] = elementHandler;
        }

        public void UnregisterSubmodelElementHandler(string pathToElement)
        {
            if (submodelElementHandler.ContainsKey(pathToElement))
                submodelElementHandler.Remove(pathToElement);
        }

        public void RegisterMethodCalledHandler(string pathToOperation, MethodCalledHandler handler)
        {
            if (!methodCalledHandler.ContainsKey(pathToOperation))
                methodCalledHandler.Add(pathToOperation, handler);
            else
                methodCalledHandler[pathToOperation] = handler;
        }

        public void RegisterEventDelegate(string pathToEvent, EventDelegate eventDelegate)
        {
            if (!eventDelegates.ContainsKey(pathToEvent))
                eventDelegates.Add(pathToEvent, eventDelegate);
            else
                eventDelegates[pathToEvent] = eventDelegate;
        }

        public IResult<InvocationResponse> InvokeOperation(string pathToOperation, InvocationRequest invocationRequest)
        {
            if (_submodel == null)
                return new Result<InvocationResponse>(false, new NotFoundMessage("Submodel"));

            var operation_Retrieved = _submodel.SubmodelElements.Retrieve<IOperation>(pathToOperation);
            if (operation_Retrieved.Success && operation_Retrieved.Entity != null)
            {
                MethodCalledHandler methodHandler;
                if (operation_Retrieved.Entity.OnMethodCalled != null)
                    methodHandler = operation_Retrieved.Entity.OnMethodCalled;
                else if (methodCalledHandler.TryGetValue(pathToOperation, out MethodCalledHandler handler))
                    methodHandler = handler;
                else
                    return new Result<InvocationResponse>(false, new NotFoundMessage($"MethodHandler for {pathToOperation}"));

                InvocationResponse invocationResponse = new InvocationResponse(invocationRequest.RequestId);
                invocationResponse.InOutputArguments = invocationRequest.InOutputArguments;

                int timeout = DEFAULT_TIMEOUT;
                if (invocationRequest.Timeout.HasValue)
                    timeout = invocationRequest.Timeout.Value;

                using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
                {
                    Task<OperationResult> runner = Task.Run(async () =>
                    {
                        try
                        {
                            invocationResponse.ExecutionState = ExecutionState.Running;
                            return await methodHandler.Invoke(operation_Retrieved.Entity, invocationRequest.InputArguments, invocationResponse.InOutputArguments, invocationResponse.OutputArguments, cancellationTokenSource.Token);                                                        
                        }
                        catch (Exception e)
                        {
                            invocationResponse.ExecutionState = ExecutionState.Failed;
                            return new OperationResult(e);
                        }

                    }, cancellationTokenSource.Token);

                    if (Task.WhenAny(runner, Task.Delay(timeout, cancellationTokenSource.Token)).Result == runner)
                    {
                        cancellationTokenSource.Cancel();

                        invocationResponse.OperationResult = runner.Result;
                        if (invocationResponse.ExecutionState != ExecutionState.Failed)
                            invocationResponse.ExecutionState = ExecutionState.Completed;

                        return new Result<InvocationResponse>(true, invocationResponse);
                    }
                    else
                    {
                        cancellationTokenSource.Cancel();
                        invocationResponse.OperationResult = new OperationResult(false, new TimeoutMessage());
                        invocationResponse.ExecutionState = ExecutionState.Timeout;
                        return new Result<InvocationResponse>(true, invocationResponse);
                    }
                }
            }
            return new Result<InvocationResponse>(operation_Retrieved);
        }

        public IResult<CallbackResponse> InvokeOperationAsync(string pathToOperation, InvocationRequest invocationRequest)
        {
            if (_submodel == null)
                return new Result<CallbackResponse>(false, new NotFoundMessage("Submodel"));
            if (invocationRequest == null)
                return new Result<CallbackResponse>(new ArgumentNullException(nameof(invocationRequest)));

            var operation_Retrieved = _submodel.SubmodelElements.Retrieve<IOperation>(pathToOperation);
            if (operation_Retrieved.Success && operation_Retrieved.Entity != null)
            {
                MethodCalledHandler methodHandler;
                if (operation_Retrieved.Entity.OnMethodCalled != null)
                    methodHandler = operation_Retrieved.Entity.OnMethodCalled;
                else if (methodCalledHandler.TryGetValue(pathToOperation, out MethodCalledHandler handler))
                    methodHandler = handler;
                else
                    return new Result<CallbackResponse>(false, new NotFoundMessage($"MethodHandler for {pathToOperation}"));
             
                Task invocationTask = Task.Run(async() =>
                {
                    InvocationResponse invocationResponse = new InvocationResponse(invocationRequest.RequestId);
                    invocationResponse.InOutputArguments = invocationRequest.InOutputArguments;
                    SetInvocationResult(pathToOperation, invocationRequest.RequestId, ref invocationResponse);

                    int timeout = DEFAULT_TIMEOUT;
                    if (invocationRequest.Timeout.HasValue)
                        timeout = invocationRequest.Timeout.Value;

                    using (var cancellationTokenSource = new CancellationTokenSource())
                    {
                        Task<OperationResult> runner = Task.Run(async () =>
                        {
                            try
                            {
                                invocationResponse.ExecutionState = ExecutionState.Running;
                                return await methodHandler.Invoke(operation_Retrieved.Entity, invocationRequest.InputArguments, invocationResponse.InOutputArguments, invocationResponse.OutputArguments, cancellationTokenSource.Token);                                                              
                            }
                            catch (Exception e)
                            {
                                invocationResponse.ExecutionState = ExecutionState.Failed;
                                return new OperationResult(e);
                            }
                        }, cancellationTokenSource.Token);

                        if (await Task.WhenAny(runner, Task.Delay(timeout, cancellationTokenSource.Token)) == runner)
                        {
                            cancellationTokenSource.Cancel();
                            invocationResponse.OperationResult = runner.Result;
                            if (invocationResponse.ExecutionState != ExecutionState.Failed)
                                invocationResponse.ExecutionState = ExecutionState.Completed;
                        }
                        else
                        {
                            cancellationTokenSource.Cancel();
                            invocationResponse.OperationResult = new OperationResult(false, new TimeoutMessage());
                            invocationResponse.ExecutionState = ExecutionState.Timeout;
                        }
                    }
                });
         
                string endpoint = ServiceDescriptor?.Endpoints?.FirstOrDefault()?.Address;
                CallbackResponse callbackResponse = new CallbackResponse(invocationRequest.RequestId);
                if (string.IsNullOrEmpty(endpoint))
                    callbackResponse.CallbackUrl = new Uri($"/submodelElements/{pathToOperation}/invocationList/{invocationRequest.RequestId}", UriKind.Relative);
                else
                    callbackResponse.CallbackUrl = new Uri($"{endpoint}/submodelElements/{pathToOperation}/invocationList/{invocationRequest.RequestId}", UriKind.Absolute);
                return new Result<CallbackResponse>(true, callbackResponse);
            }
            return new Result<CallbackResponse>(operation_Retrieved);
        }

        private void SetInvocationResult(string operationId, string requestId, ref InvocationResponse invocationResponse)
        {
            string key = string.Join("_", operationId, requestId);
            if (invocationResults.ContainsKey(key))
            {
                invocationResults[key] = invocationResponse;
            }
            else
            {
                invocationResults.Add(key, invocationResponse);
            }
        }
      
        public IResult<InvocationResponse> GetInvocationResult(string pathToOperation, string requestId)
        {
            string key = string.Join("_", pathToOperation, requestId);
            if (invocationResults.ContainsKey(key))
            {
                return new Result<InvocationResponse>(true, invocationResults[key]);
            }
            else
            {
               return new Result<InvocationResponse>(false, new NotFoundMessage($"Request with id {requestId}"));
            }
        }              
       
        public IResult PublishEvent(IEventMessage eventMessage, string topic = null, Action<IMessagePublishedEventArgs> MessagePublished = null, byte qosLevel = 2, bool retain = false)
        {
            if (messageClient == null || !messageClient.IsConnected)
                return new Result(false, new Message(MessageType.Warning, "MessageClient is not initialized or not connected"));

            if (eventMessage == null)
                return new Result(new ArgumentNullException(nameof(eventMessage)));

            if (eventDelegates.TryGetValue(eventMessage.SourceIdShort, out EventDelegate eventDelegate))
                eventDelegate.Invoke(this, eventMessage);

            try
            {
                string messageTopic = topic ?? eventMessage.Topic; 
                string message = JsonConvert.SerializeObject(eventMessage, Formatting.Indented);
                return messageClient.Publish(messageTopic, message, MessagePublished, qosLevel, retain);
            }
            catch (Exception e)
            {
                logger.Error(e, "Error sending event message");
                return new Result(e);
            }
        }

       
        public virtual void ConfigureEventHandler(IMessageClient messageClient)
        {
            this.messageClient = messageClient;
        }
       
        public virtual void SubscribeUpdates(string pathToSubmodelElement, Action<IValue> updateFunction)
        {
            if (!updateFunctions.ContainsKey(pathToSubmodelElement))
                updateFunctions.Add(pathToSubmodelElement, updateFunction);
            else
                updateFunctions[pathToSubmodelElement] = updateFunction;
        }
       
        public virtual void PublishUpdate(string pathToSubmodelElement, IValue value)
        {
            if (updateFunctions.TryGetValue(pathToSubmodelElement, out Action<IValue> updateFunction))
                updateFunction.Invoke(value);

        }

        public IResult<ISubmodel> RetrieveSubmodel()
        {
            return new Result<ISubmodel>(_submodel != null, _submodel);
        }

        public IResult<ISubmodelElement> CreateOrUpdateSubmodelElement(string pathToSubmodelElement, ISubmodelElement submodelElement)
        {
            if (_submodel == null)
                return new Result<ISubmodelElement>(false, new NotFoundMessage("Submodel"));

            var created = _submodel.SubmodelElements.CreateOrUpdate(pathToSubmodelElement, submodelElement);
            if(created.Success && created.Entity != null)
                RegisterSubmodelElementHandler(pathToSubmodelElement, new SubmodelElementHandler(submodelElement.Get, submodelElement.Set));
            return created;
        }

        public IResult<IElementContainer<ISubmodelElement>> RetrieveSubmodelElements()
        {
            if (_submodel == null)
                return new Result<ElementContainer<ISubmodelElement>>(false, new NotFoundMessage("Submodel"));

            if (_submodel.SubmodelElements == null)
                return new Result<ElementContainer<ISubmodelElement>>(false, new NotFoundMessage("SubmodelElements"));
            return _submodel.SubmodelElements.RetrieveAll();
        }

        public IResult<ISubmodelElement> RetrieveSubmodelElement(string submodelElementId)
        {
            if (_submodel == null)
                return new Result<ISubmodelElement>(false, new NotFoundMessage("Submodel"));

            if (_submodel.SubmodelElements == null)
                return new Result<ISubmodelElement>(false, new NotFoundMessage(submodelElementId));

            return _submodel.SubmodelElements.Retrieve(submodelElementId);
        }
        public IResult<IValue> RetrieveSubmodelElementValue(string submodelElementId)
        {
            if (_submodel == null)
                return new Result<IValue>(false, new NotFoundMessage("Submodel"));

            if (submodelElementHandler.TryGetValue(submodelElementId, out SubmodelElementHandler elementHandler) && elementHandler.GetValueHandler != null)
            {
                var submodelElement = _submodel.SubmodelElements.Retrieve<ISubmodelElement>(submodelElementId);
                if (submodelElement.Success && submodelElement.Entity != null)
                    return new Result<IValue>(true, elementHandler.GetValueHandler.Invoke(submodelElement.Entity));
                else
                    return new Result<IValue>(false, new Message(MessageType.Error, "SubmodelElement not found"));
            }            
            else
                return new Result<IValue>(false, new Message(MessageType.Error, "SubmodelElementHandler not found"));
        }

        public IResult UpdateSubmodelElementValue(string submodelElementId, IValue value)
        {
            if (_submodel == null)
                return new Result(false, new NotFoundMessage("Submodel"));

            if (submodelElementHandler.TryGetValue(submodelElementId, out SubmodelElementHandler elementHandler) && elementHandler.SetValueHandler != null)
            {
                var submodelElement = _submodel.SubmodelElements.Retrieve<ISubmodelElement>(submodelElementId);
                if (submodelElement.Success && submodelElement.Entity != null)
                {
                    elementHandler.SetValueHandler.Invoke(submodelElement.Entity, value);
                    return new Result(true);
                }
                else
                    return new Result<IValue>(false, new Message(MessageType.Error, "SubmodelElement not found"));
            }
            else
                return new Result<IValue>(false, new Message(MessageType.Error, "SubmodelElementHandler not found"));
        }

        public IResult DeleteSubmodelElement(string submodelElementId)
        {
            if (_submodel == null)
                return new Result(false, new NotFoundMessage("Submodel"));

            if (_submodel.SubmodelElements == null)
                return new Result(false, new NotFoundMessage(submodelElementId));

            var deleted = _submodel.SubmodelElements.Delete(submodelElementId);
            if (deleted.Success)
                UnregisterSubmodelElementHandler(submodelElementId);
            return deleted;
        }        
    }
}
