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
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class EnvironmentIdentifiable_V2_0 : EnvironmentReferable_V2_0
    {
        private EnvironmentIdentifier_V2_0 _identifier;
        [JsonProperty("identification", Order = -2)]
        [XmlElement("identification")]
        public EnvironmentIdentifier_V2_0 Identification
        {
            get { return _identifier; }
            set
            {
                if (value.IdType == KeyType_V2_0.URI)
                    _identifier = new EnvironmentIdentifier_V2_0(value.Id, KeyType_V2_0.IRI);
                else
                    _identifier = new EnvironmentIdentifier_V2_0(value.Id, value.IdType);
            }
        }
        [JsonProperty("administration", Order = -1)]
        [XmlElement("administration")]
        public EnvironmentAdministrativeInformation_V2_0 Administration { get; set; }
    }
}
