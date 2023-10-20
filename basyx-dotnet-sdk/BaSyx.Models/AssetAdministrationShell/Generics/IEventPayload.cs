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

namespace BaSyx.Models.AdminShell
{
    public interface IEventPayload
    {
        /// <summary>
        /// Reference to the source event element.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "source")]
        IReference Source { get; }

        /// <summary>
        /// semanticId of the source event element, if available
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "sourceSemanticId")]
        IReference SourceSemanticId { get; }

        /// <summary>
        /// Reference to the referable, which defines the scope of the event.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "observableReference")]
        IReference ObservableReference { get; }

        /// <summary>
        /// semanticId of the referable, which defines the scope of the event, if available.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "observableSemanticId")]
        IReference ObservableSemanticId { get; }

        /// <summary>
        /// Information for the outer message infrastructure for scheduling the event to the respective communication channel.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "topic")]
        string Topic { get; }

        /// <summary>
        /// Subject, who/which initiated the creation.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "subject")]
        IReference SubjectId { get; }

        /// <summary>
        /// Timestamp in UTC, when this event was triggered.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "timestamp")]
        string Timestamp { get; }

        /// <summary>
        /// Event-specific payload.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "payload")]
        string Payload { get; set; }

        /// <summary>
        /// Temporary unique id to identify the event message (e.g. GUID)
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "messageId")]
        string MessageId { get; set; }       
    }
}
