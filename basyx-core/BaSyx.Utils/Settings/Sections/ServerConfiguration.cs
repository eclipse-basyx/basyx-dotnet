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
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace BaSyx.Utils.Settings.Sections
{
    
    public class ServerConfiguration
    {
        [XmlElement]
        public string ServerId { get; set; }
        [XmlElement]
        public HostingConfiguration Hosting { get; set; }
        [XmlElement]
        public string DefaultRoute { get; set; }      

        public ServerConfiguration()
        {
            Hosting = new HostingConfiguration();
        }
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
