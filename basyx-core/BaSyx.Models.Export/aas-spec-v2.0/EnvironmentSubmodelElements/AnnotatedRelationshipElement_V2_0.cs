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
    public class AnnotatedRelationshipElement_V2_0 : RelationshipElement_V2_0
    {
        [JsonProperty("annotation")]
        [XmlElement("annotation")]
        public EnvironmentReference_V2_0 Annotation { get; set; }

        [JsonProperty("modelType")]
        [XmlIgnore]
        public override ModelType ModelType => ModelType.AnnotatedRelationshipElement;

        public AnnotatedRelationshipElement_V2_0() { }
        public AnnotatedRelationshipElement_V2_0(SubmodelElementType_V2_0 submodelElementType) : base(submodelElementType) { }
    }
}
