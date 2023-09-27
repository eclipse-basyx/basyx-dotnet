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
using Newtonsoft.Json;
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
        public string Subject { get; set; }
        public IReference Source { get; set; }
        public string SourceIdShort { get; set; }
        public IReference SourceSemanticId { get; set; }
        public IReference ObservableReference { get; set; }
        public IReference ObservableSemanticId { get; set; }
        public string Topic { get; set; }

        [JsonConstructor]
        public EventPayload(string sourceIdShort, string subject)
        {
            MessageId = Guid.NewGuid().ToString();
            Timestamp = DateTime.UtcNow.ToString();

            SourceIdShort = sourceIdShort;
            Subject = subject;
        }

        public EventPayload(IBasicEventElement eventElement, string subject) : this(eventElement.IdShort, subject)
        {
            Source = eventElement.CreateReference();
            SourceSemanticId = eventElement.SemanticId;
            ObservableReference = eventElement.ObservableReference;            
            Topic = eventElement.MessageTopic;
        }

    }
}
