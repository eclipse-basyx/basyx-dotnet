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
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Extensions;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Implementations
{
    [DataContract]
    public class EventMessage : IEventMessage
    {
        public ModelType ModelType => ModelType.EventMessage;
        public string MessageId { get; set; }
        public string Payload { get; set; }
        public string Timestamp { get; set; }
        public string Subject { get; set; }
        public IReference Source { get; set; }
        public string SourceIdShort { get; set; }
        public IReference SourceSemanticId { get; set; }
        public IReference ObservableReference { get; set; }
        public IReference ObservableSemanticId { get; set; }
        public string Topic { get; set; }

        [JsonConstructor]
        public EventMessage(string sourceIdShort, string subject)
        {
            MessageId = Guid.NewGuid().ToString();
            Timestamp = DateTime.UtcNow.ToString();

            SourceIdShort = sourceIdShort;
            Subject = subject;
        }

        public EventMessage(IEventElement eventElement, string subject) : this(eventElement.IdShort, subject)
        {
            Source = eventElement.CreateReference();
            SourceSemanticId = eventElement.SemanticId;
            ObservableReference = eventElement.ObservableReference;            
            Topic = eventElement.MessageTopic;
        }

    }
}
