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
using BaSyx.Models.Core.Common;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class ReferenceElement_V1_0 : SubmodelElementType_V1_0
    {
        [JsonProperty("value")]
        [XmlElement("value")]
        public EnvironmentReference_V1_0 Value { get; set; }

        [JsonProperty("modelType")]
        [XmlIgnore]
        public override ModelType ModelType => ModelType.ReferenceElement;

        public ReferenceElement_V1_0() { }
        public ReferenceElement_V1_0(SubmodelElementType_V1_0 submodelElementType) : base(submodelElementType) { }
    }
}