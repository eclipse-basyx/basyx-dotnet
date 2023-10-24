/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace BaSyx.Utils.ResultHandling
{
    public interface IResult
    {
        [JsonIgnore, IgnoreDataMember]
        Type EntityType { get; }
        [JsonIgnore, IgnoreDataMember]
        object Entity { get; }
        [DataMember(Name = "success", IsRequired = true)]
        bool Success { get; }
        [JsonIgnore, IgnoreDataMember]
        bool HasContent { get; }
        [JsonIgnore, IgnoreDataMember]
        bool SuccessAndContent { get; }
        [JsonIgnore, IgnoreDataMember]
        bool? IsException { get; }
        [DataMember(Name = "messages", EmitDefaultValue = false, IsRequired = false)]
        MessageCollection Messages { get; }

        T GetEntity<T>();
    }

    public interface IResult<out TEntity> : IResult
    {
        [JsonIgnore, IgnoreDataMember]
        new TEntity Entity { get; }
    }
}
