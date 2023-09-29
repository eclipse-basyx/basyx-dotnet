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
    [DataContract]
    public class BasicEventElement : SubmodelElement, IBasicEventElement
    {
        public override ModelType ModelType => ModelType.EventElement;
        public IReference Observed { get; set; }

        public IReference ObservableReference { get; set; }

        public EventDirection Direction { get; set; }

        public EventState State { get; set; }

        public string MessageTopic { get; set; }

        public IReference MessageBroker { get; set; }

        public IElementContainer<ISubmodelElement> MessageBrokerElements { get; set; }

        public string LastUpdate { get; set; }

        public string MinInterval { get; set; }

        public string MaxInterval { get; set; }
        
        public BasicEventElement(string idShort) : base(idShort)
        {
            MessageBrokerElements = new ElementContainer<ISubmodelElement>();
        }
    }
}
