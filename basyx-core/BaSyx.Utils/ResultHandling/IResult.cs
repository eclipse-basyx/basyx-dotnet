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
using BaSyx.Utils.JsonHandling;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace BaSyx.Utils.ResultHandling
{
    public interface IResult
    {
        [DataMember(Name = "entityType", EmitDefaultValue = false, IsRequired = false)]
        Type EntityType { get; }
        [DataMember(Name = "entity", EmitDefaultValue = false, IsRequired = false)]
        object Entity { get; }
        [DataMember(Name = "success", IsRequired = true)]
        bool Success { get; }
        [DataMember(Name = "isException", EmitDefaultValue = false, IsRequired = false)]
        bool? IsException { get; }
        [DataMember(Name = "messages", EmitDefaultValue = false, IsRequired = false)]
        MessageCollection Messages { get; }

        T GetEntity<T>();
    }

    public interface IResult<out TEntity> : IResult
    {
        [DataMember(Name = "entity", EmitDefaultValue = false, IsRequired = false)]
        [JsonConverter(typeof(CustomTypeSerializer))]
        new TEntity Entity { get; }
    }
}
