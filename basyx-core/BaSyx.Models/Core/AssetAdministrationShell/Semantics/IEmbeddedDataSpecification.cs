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
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Extensions;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Semantics
{
    [JsonConverter(typeof(DataSpecificationConverter))]
    public interface IEmbeddedDataSpecification
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "hasDataSpecification")]
        IReference HasDataSpecification { get; }
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "dataSpecificationContent")]
        IDataSpecificationContent DataSpecificationContent { get; }
    }
}
