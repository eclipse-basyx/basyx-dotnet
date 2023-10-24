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
using BaSyx.Utils.Settings;
using System.Xml.Serialization;

namespace BaSyx.Registry.Client.Http
{
    public class RegistryClientSettings : Settings<RegistryClientSettings>
    {        
        public RegistryConfiguration RegistryConfig { get; set; } = new RegistryConfiguration();      
    }

    public class RegistryConfiguration
    {
        [XmlElement]
        public string RegistryUrl { get; set; }
        [XmlElement]
        public string RegisterAtAAS { get; set; }
    }
}
