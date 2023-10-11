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
using BaSyx.Models.Extensions;

using System;
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    [DataContract]
    public class EventPayload : IEventPayload
    {
        public ModelType ModelType => ModelType.EventMessage;
        public string MessageId { get; set; }
        public string Payload { get; set; }
        public string Timestamp { get; set; }
        public IReference SubjectId { get; set; }
        public IReference Source { get; set; }
        public IReference SourceSemanticId { get; set; }
        public IReference ObservableReference { get; set; }
        public IReference ObservableSemanticId { get; set; }
        public string Topic { get; set; }

        public EventPayload(IReference source, IReference observableReference)
        {
            MessageId = Guid.NewGuid().ToString();
            Timestamp = DateTime.UtcNow.ToString();

            Source = source;
            ObservableReference = observableReference;
        }

        public EventPayload(IBasicEventElement eventElement, IReference observableReference) 
            : this(eventElement.CreateReference(), observableReference)
        {
            SourceSemanticId = eventElement.SemanticId;     
            Topic = eventElement.MessageTopic;
        }

    }
}
