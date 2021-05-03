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
