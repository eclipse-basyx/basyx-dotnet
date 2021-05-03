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
using BaSyx.Models.Core.Common;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class RelationshipElement_V2_0 : SubmodelElementType_V2_0
    {
        [JsonProperty("first")]
        [XmlElement("first")]
        public EnvironmentReference_V2_0 First { get; set; }

        [JsonProperty("second")]
        [XmlElement("second")]
        public EnvironmentReference_V2_0 Second { get; set; }

        [JsonProperty("modelType")]
        [XmlIgnore]
        public override ModelType ModelType => ModelType.RelationshipElement;

        public RelationshipElement_V2_0() { }
        public RelationshipElement_V2_0(SubmodelElementType_V2_0 submodelElementType) : base(submodelElementType) { }
    }
}
