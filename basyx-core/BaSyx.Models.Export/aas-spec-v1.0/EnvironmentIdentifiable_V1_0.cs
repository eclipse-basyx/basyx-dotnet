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
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class EnvironmentIdentifiable_V1_0 : EnvironmentReferable_V1_0
    {
        [JsonProperty("identification", Order = -2)]
        [XmlElement("identification")]
        public Identifier Identification { get; set; }

        [JsonProperty("administration", Order = -1)]
        [XmlElement("administration")]
        public AdministrativeInformation Administration { get; set; }
    }
}
