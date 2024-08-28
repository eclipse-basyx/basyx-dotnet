/*******************************************************************************
* Copyright (c) 2024 Bosch Rexroth AG
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
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class EventElement_V3_0 : SubmodelElementType_V3_0
    {
        [JsonProperty("modelType")]
        [XmlIgnore]
        public override ModelType ModelType => ModelType.Event;

        public EventElement_V3_0() { }
        public EventElement_V3_0(SubmodelElementType_V3_0 submodelElementType) : base(submodelElementType) { }
    }
}
