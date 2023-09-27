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

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    /// <summary>
    /// An element with a kind is an element that can either represent a template (type) or an instance. Default for an element is that it is representing an instance
    /// </summary>
    public interface IHasKind
    {
        /// <summary>
        ///Kind of the element: either type or instance (Default Value = Instance )
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "kind")]
        [JsonConverter(typeof(StringEnumConverter))]
        ModelingKind Kind { get; }
    }
}
