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
using BaSyx.Models.AdminShell;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class BasicEventElement_V3_0 : EventElement_V3_0
    {
        [JsonProperty("observed")]
        [XmlElement("observed")]
        public EnvironmentReference_V3_0 Observed { get; set; }

        [JsonProperty("direction")]
        [XmlElement("direction")]
        public EventDirection Direction { get; set; }

        [JsonProperty("state")]
        [XmlElement("state")]
        public EventState State { get; set; }

        [JsonProperty("messageTopic")]
        [XmlElement("messageTopic")]
        public string MessageTopic { get; set; }

        [JsonProperty("messageBroker")]
        [XmlElement("messageBroker")]
        public EnvironmentReference_V3_0 MessageBroker { get; set; }

        [JsonProperty("lastUpdate")]
        [XmlElement("lastUpdate")]
        public string LastUpdate { get; set; }

        [JsonProperty("minInterval")]
        [XmlElement("minInterval")]
        public string MinInterval { get; set; }

        [JsonProperty("maxInterval")]
        [XmlElement("maxInterval")]
        public string MaxInterval { get; set; }


        [JsonProperty("modelType")]
        [XmlIgnore]
        public override ModelType ModelType => ModelType.BasicEventElement;

        public BasicEventElement_V3_0() { }
        public BasicEventElement_V3_0(SubmodelElementType_V3_0 submodelElementType) : base(submodelElementType) { }
    }
}
