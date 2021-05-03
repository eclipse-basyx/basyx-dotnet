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
using System.Runtime.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Implementations
{
    [DataContract]
    public class EventElement : Event, IEventElement
    {
        public override ModelType ModelType => ModelType.EventElement;

        public IReference ObservableReference { get; set; }

        public EventDirection Direction { get; set; }

        public EventState State { get; set; }

        public string MessageTopic { get; set; }

        public IReference MessageBroker { get; set; }

        public IElementContainer<ISubmodelElement> MessageBrokerElements { get; set; }

        public string LastUpdate { get; set; }

        public string MinInterval { get; set; }

        public string MaxInterval { get; set; }

        public IElementContainer<ISubmodelElement> SubmodelElements { get; set; }

        public EventElement(string idShort) : base(idShort) 
        {
            MessageBrokerElements = new ElementContainer<ISubmodelElement>();
            SubmodelElements = new ElementContainer<ISubmodelElement>();

            Get = element => 
            {
                string eventElements = SubmodelElements?.MinimizeSubmodelElements()?.ToString();
                return new ElementValue(eventElements, new DataType(DataObjectType.String)); 
            };
            Set = (element, value) =>  { };
        }
    }
}
