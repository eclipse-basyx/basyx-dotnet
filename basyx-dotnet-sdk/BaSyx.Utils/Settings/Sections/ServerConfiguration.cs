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
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace BaSyx.Utils.Settings
{
    
    public class ServerConfiguration
    {
        [XmlElement]
        public string ServerId { get; set; }
        [XmlElement]
        public HostingConfiguration Hosting { get; set; }
        [XmlElement]
        public string DefaultRoute { get; set; }
        [XmlElement]
        public string PathBase { get; set; }
        [XmlElement]
        public SecurityConfiguration Security { get; set; }

        public ServerConfiguration()
        {
            Hosting = new HostingConfiguration();
            Security = new SecurityConfiguration();
        }
    }

    public class SecurityConfiguration
    {
        [XmlElement]
        public string ServerCertificatePath { get; set; }
        [XmlElement]
        public string ServerCertificatePassword { get; set; }
    }

    
    public class HostingConfiguration
    {
        [XmlElement]
        public string Environment { get; set; }
        [XmlArrayItem("Url")]
        public List<string> Urls { get; set; }
        [XmlElement]
        public string ContentPath { get; set; }
        [XmlElement]
        public bool? EnableIPv6 { get; set; }

        public HostingConfiguration()
        {
            Urls = new List<string>();
        }
    }
}
