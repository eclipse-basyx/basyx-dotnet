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
using BaSyx.Utils.Client;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Utils.ResultHandling;
using System;
using BaSyx.API.AssetAdministrationShell;
using BaSyx.API.Clients;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.Common;

namespace BaSyx.API.Components
{
    /// <summary>
    /// Provides basic functions for a Submodel
    /// </summary>
    public interface ISubmodelServiceProvider : IServiceProvider<ISubmodel, ISubmodelDescriptor>, ISubmodelClient
    {
        /// <summary>
        /// Subscribe to value updates of a specific SubmodelElement
        /// </summary>
        /// <param name="pathToSubmodelElement">IdShort-Path to the SubmodelElement</param>
        /// <param name="updateFunction">Callback function</param>
        void SubscribeUpdates(string pathToSubmodelElement, Action<IValue> updateFunction);

        /// <summary>
        /// Publishes value updates to subscribers
        /// </summary>
        /// <param name="pathToSubmodelElement">IdShort-Path to the SubmodelElement</param>
        /// <param name="value">New value</param>
        void PublishUpdate(string pathToSubmodelElement, IValue value);

        /// <summary>
        /// Publishs an Event
        /// </summary>
        /// <param name="eventMessage">The event to publish</param>
        /// <param name="topic">Message-Topic (default=/)</param>
        /// <param name="MessagePublished">Message-Published-Callback (default=null)</param>
        /// <param name="qosLevel">QoS Level of the event to be sent (default=2)</param>
        /// <param name="retain">Shall the event be persisted at the Message Broker? (default=false)</param>
        /// <returns></returns>
        IResult PublishEvent(IEventMessage eventMessage, string topic, Action<IMessagePublishedEventArgs> MessagePublished, byte qosLevel, bool retain);

        /// <summary>
        /// Returns the SubmodelElementHandler for a specific SubmodelElement
        /// </summary>
        /// <param name="pathToElement">IdShort-Path to the SubmodelElement</param>
        /// <returns></returns>
        SubmodelElementHandler RetrieveSubmodelElementHandler(string pathToElement);

        /// <summary>
        /// Registers a new SubmodelElementHandler for a specific SubmodelElement
        /// </summary>
        /// <param name="pathToElement">IdShort-Path to the SubmodelElement</param>
        /// <param name="elementHandler">SubmodelElementHandler</param>
        void RegisterSubmodelElementHandler(string pathToElement, SubmodelElementHandler elementHandler);

        /// <summary>
        /// Returns the MethodCalledHandler for a specific Operation
        /// </summary>
        /// <param name="pathToOperation">IdShort-Path to the Operation</param>
        /// <returns></returns>
        MethodCalledHandler RetrieveMethodCalledHandler(string pathToOperation);

        /// <summary>
        /// Registers a new MethodCalledHandler for a specific Operation
        /// </summary>
        /// <param name="pathToOperation">IdShort-Path to the Operation</param>
        /// <param name="methodCalledHandler">MethodCalledHandler</param>
        void RegisterMethodCalledHandler(string pathToOperation, MethodCalledHandler methodCalledHandler);

        /// <summary>
        /// Registers a new EventDelegate for a specific Event
        /// </summary>
        /// <param name="pathToEvent">IdShort-Path to the Event</param>
        /// <param name="eventDelegate">EventDelegate</param>
        void RegisterEventDelegate(string pathToEvent, EventDelegate eventDelegate);

        /// <summary>
        /// Configures the SubmodelServiceProvider with a MessageClient for the Events
        /// </summary>
        /// <param name="messageClient">MessageClient</param>
        void ConfigureEventHandler(IMessageClient messageClient);
    }
}
