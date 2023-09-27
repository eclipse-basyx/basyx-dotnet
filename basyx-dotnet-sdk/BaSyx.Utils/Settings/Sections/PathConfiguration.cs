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
    
    public class PathConfiguration
    {
        [XmlElement]
        public string Host { get; set; }
        [XmlElement]
        public string BasePath { get; set; }
        [XmlElement]
        public string ServicePath { get; set; }
        [XmlElement]
        public string AggregatePath { get; set; }
        [XmlElement]
        public string EntityPath { get; set; }
        [XmlElement]
        public string EntityId { get; set; }
    }
}
