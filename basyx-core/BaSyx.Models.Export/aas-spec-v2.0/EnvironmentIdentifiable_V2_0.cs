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
