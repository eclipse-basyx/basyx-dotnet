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
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Identification
{
    /// <summary>
    /// An element that has a globally unique identifier.  
    /// </summary>
    public interface IIdentifiable : IReferable
    {
        /// <summary>
        /// The globally unique identification of the element. 
        /// </summary>
        [JsonProperty(Order = -1), DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false, Name = "identification")]
        Identifier Identification { get; }

        /// <summary>
        /// Administrative information of an identifiable element.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "administration")]
        AdministrativeInformation Administration { get; }
    }
}
