/*******************************************************************************
* Copyright (c) 2024 Bosch Rexroth AG
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
    [DataContract]
    public class BasicEventElement : SubmodelElement<BasicEventElementValue>, IBasicEventElement
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "modelType")]
        public override ModelType ModelType => ModelType.BasicEventElement;

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "observableReference")]
        public IReference ObservableReference { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "direction")]
        public EventDirection Direction { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "state")]
        public EventState State { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "messageTopic")]
        public string MessageTopic { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "messageBroker")]
        public IReference MessageBroker { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "lastUpdate")]
        public string LastUpdate { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "minInterval")]
        public string MinInterval { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "maxInterval")]
        public string MaxInterval { get; set; }
        
        public BasicEventElement(string idShort) : base(idShort)
        {

        }
    }
}
