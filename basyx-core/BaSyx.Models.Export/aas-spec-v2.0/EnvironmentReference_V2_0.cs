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
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{

    public class EnvironmentReference_V2_0
    {
        [JsonProperty("keys")]
        [XmlArray("keys")] 
        [XmlArrayItem("key")]
        public List<EnvironmentKey_V2_0> Keys { get; set; }

        public bool ShouldSerializeKeys()
        {
            if (Keys == null || Keys.Count == 0)
                return false;
            else
                return true;
        }
    }
}
