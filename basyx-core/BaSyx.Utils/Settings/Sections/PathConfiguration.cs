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
using System.Xml;
using System.Xml.Serialization;

namespace BaSyx.Utils.Settings.Sections
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
