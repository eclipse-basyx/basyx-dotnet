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
using System.Xml;
using System.Xml.Serialization;

namespace BaSyx.Utils.Settings
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
