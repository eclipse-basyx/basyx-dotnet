/*******************************************************************************
* Copyright (c) 2020 Robert Bosch GmbH
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

namespace BaSyx.Models.Core.AssetAdministrationShell.Implementations
{
    public class PublishableEvent : IPublishableEvent
    {
        public string Timestamp { get; set; }

        public string Message { get; set; }

        public string Originator { get; set; }

        public IReference<IEvent> EventReference { get; set; }

        public string Name { get; set; }

        public string MessageId { get; set; }
    }
}
