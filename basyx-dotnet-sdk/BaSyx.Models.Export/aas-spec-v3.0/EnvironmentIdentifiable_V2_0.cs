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
using BaSyx.Models.AdminShell;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class EnvironmentIdentifiable_V2_0 : EnvironmentReferable_V2_0
    {
        private Identifier _identifier;
        [JsonProperty("identification", Order = -2)]
        [XmlElement("identification")]
        public Identifier Identification
        {
            get { return _identifier; }
            set
            {
                if (value.IdType == KeyType.URI)
                    _identifier = new Identifier(value.Id, KeyType.IRI);
                else
                    _identifier = new Identifier(value.Id, value.IdType);
            }
        }
        [JsonProperty("administration", Order = -1)]
        [XmlElement("administration")]
        public AdministrativeInformation Administration { get; set; }
    }
}
