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
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BaSyx.Models.AdminShell
{
    public interface IBasicEventElement : IEventElement, IHasSemantics
    {
        /// <summary>
        /// Reference to the data or other elements that are being observed.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "observed")]
        IReference Observed { get; set; }

        /// <summary>
        /// Can be { Input, Output }.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "direction")]
        EventDirection Direction { get; }

        /// <summary>
        /// Can be { On, Off }.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "state")]
        EventState State { get; }

        /// <summary>
        /// Information for the outer message infrastructure for scheduling the event to the respective communication channel.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "messageTopic")]
        string MessageTopic { get; }

        /// <summary>
        /// Information, which outer message infrastructure shall handle messages for the EventElement. Refers to a Submodel, SubmodelElementCollection, which contains DataElements describing the proprietary specification for the message broker.
        /// Note: for different message infrastructure, e.g.OPC UA or MQTT or AMQP, these proprietary specification could be standardized by having respective Submodels.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "messageBroker")]
        IReference MessageBroker { get; }

        [IgnoreDataMember]
        IElementContainer<ISubmodelElement> MessageBrokerElements { get; }

        /// <summary>
        /// Timestamp in UTC, when the last event was received (input direction) or sent (output direction).
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "lastUpdate")]
        string LastUpdate { get; }

        /// <summary>
        /// For input direction, reports on the maximum frequency, the software entity behind the respective Referable can handle input events.
        /// For output events, specifies the maximum frequency of outputting this event to an outer infrastructure.
        /// Might be not specified, that is, there is no minimum interval.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "minInterval")]
        string MinInterval { get; }

        /// <summary>
        /// For input direction: not applicable.
        /// For output direction: maximum interval in time, the respective Referable shall send an update of the status of the event, even if not other trigger condition for the event was not met.
        /// Might be not specified, that is, there is no maximum interval.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "maxInterval")]
        string MaxInterval { get; }        
    }

    [DataContract]
    public enum EventDirection : int
    {
        [XmlEnum("None")]
        None = 0,
        [XmlEnum("Input")]
        Input = 1,
        [XmlEnum("Output")]
        Output = 2
    }

    [DataContract]
    public enum EventState : int
    {
        [XmlEnum("None")]
        None = 0,
        [XmlEnum("On")]
        On,
        [XmlEnum("Off")]
        Off
    }
}
