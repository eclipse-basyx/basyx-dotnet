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
using System.Runtime.Serialization;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;

namespace BaSyx.Models.Core.AssetAdministrationShell.Generics
{
    public interface IPublishableEvent
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "originator")]
        string Originator { get; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "eventReference")]
        IReference<IEvent> EventReference { get; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "timestamp")]
        string Timestamp { get; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "message")]
        string Message { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "messageId")]
        string MessageId { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "name")]
        string Name { get; set; }

    }
}
