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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BaSyx.Models.Connectivity
{
    public interface IDescriptor
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "endpoints")]
        IEnumerable<IEndpoint> Endpoints { get; }

        void AddEndpoints(IEnumerable<IEndpoint> endpoints);
        void SetEndpoints(IEnumerable<IEndpoint> endpoints);
    }
}
