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
using System.Runtime.Serialization;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.Common;

namespace BaSyx.Models.Core.AssetAdministrationShell.Generics
{
    public interface IEventMessage : IModelElement
    {
        /// <summary>
        /// Reference to the source EventElement, including identification of AAS, Submodel, SubmodelElements.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "source")]
        IReference Source { get; }

        /// <summary>
        /// IdShort of the source EventElement
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "sourceIdShort")]
        string SourceIdShort { get; set; }
        
        /// <summary>
        /// semanticId of the source EventElement, if available
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "sourceSemanticId")]
        IReference SourceSemanticId { get; }
       
        /// <summary>
        /// Reference to the Referable, which defines the scope of the event. Can be AAS, Submodel, SubmodelElementCollection or SubmodelElement.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "observableReference")]
        IReference ObservableReference { get; }

        /// <summary>
        /// semanticId of the Referable, which defines the scope of the event, if available. See above.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "observableSemanticId")]
        IReference ObservableSemanticId { get; }

        /// <summary>
        /// Information for the outer message infrastructure for scheduling the event to the respective communication channel.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "topic")]
        string Topic { get; }
        
        /// <summary>
        /// ABAC-Subject, who/ which initiated the creation
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "subject")]
        string Subject { get; }
        
        /// <summary>
        /// Timestamp in UTC, when this event was triggered.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "timestamp")]
        string Timestamp { get; }
        
        /// <summary>
        /// Event specific payload. Detailed in annex.
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
