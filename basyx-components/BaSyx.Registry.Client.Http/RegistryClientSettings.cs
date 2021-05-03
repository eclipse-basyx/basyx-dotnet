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
using BaSyx.Utils.Settings;
using System.Xml.Serialization;

namespace BaSyx.Registry.Client.Http
{
    public class RegistryClientSettings : Settings<RegistryClientSettings>
    {        
        public RegistryConfiguration RegistryConfig { get; set; } = new RegistryConfiguration();
     
        public class RegistryConfiguration 
        {
            [XmlElement]
            public string RegistryUrl { get; set; }
            [XmlElement]
            public string RegisterAtAAS { get; set; }
        }
       
    }
}
