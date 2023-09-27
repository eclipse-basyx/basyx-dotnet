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
using System;
using System.Xml;
using System.Xml.Serialization;

namespace BaSyx.Utils.Settings
{
    
    public class ProxyConfiguration
    {
        [XmlElement]
        public bool UseProxy { get; set; }
        [XmlElement]
        public string ProxyAddress { get; set; }
        [XmlElement]
        public string Domain { get; set; }
        [XmlElement]
        public string UserName { get; set; }
        [XmlElement]
        public string Password { get; set; }
    }
}
