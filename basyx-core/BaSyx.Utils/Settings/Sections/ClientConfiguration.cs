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
using System.Xml;
using System.Xml.Serialization;

namespace BaSyx.Utils.Settings.Sections
{

    public class ClientConfiguration
    {
        [XmlElement]
        public string ClientId { get; set; }
        [XmlElement]
        public string Endpoint { get; set; }
        [XmlElement]
        public RequestConfiguration RequestConfig { get; set; }

        public ClientConfiguration()
        {
            RequestConfig = new RequestConfiguration();
        }
    }

    public class RequestConfiguration
    {
        [XmlElement]
        public int? RequestTimeout { get; set; }
    }
}
