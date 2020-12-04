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
    public class BasicEvent_V2_0 : Event_V2_0
    {
        [JsonProperty("observed")]
        [XmlElement("observed")]
        public EnvironmentReference_V2_0 Observed { get; set; }

        [JsonProperty("modelType")]
        [XmlIgnore]
        public override ModelType ModelType => ModelType.BasicEvent;

        public BasicEvent_V2_0() { }
        public BasicEvent_V2_0(SubmodelElementType_V2_0 submodelElementType) : base(submodelElementType) { }
    }
}
