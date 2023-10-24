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

using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaSyx.Utils.Settings
{
    public class ServerSettings : Settings<ServerSettings>
    {
        public ControllerConfiguration ControllerConfig { get; set; } = new ControllerConfiguration();
        public UserInterfaceConfiguration UIConfig { get; set; } = new UserInterfaceConfiguration();
    }

    public class ControllerConfiguration
    {
        [XmlArray("Controllers")]
        [XmlArrayItem("Controller")]
        public List<string> Controllers { get; set; }

        public ControllerConfiguration()
        {
            Controllers = new List<string>();
        }
    }

    public class UserInterfaceConfiguration
    {
        public bool? BlazorSupportEnabled { get; set; }
        public string BrandLogo { get; set; }
        public string BrandStyle { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }

        [XmlArray("Links")]
        [XmlArrayItem("Link")]
        public List<Link> Links { get; set; } = new List<Link>();
    }

    public class Link
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("href")]
        public string Url { get; set; }
    }
}
