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
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;

namespace BaSyx.Utils.Client.Mqtt
{
    public class MqttSecurity : IMqttSecurity
    {
        [XmlElement]
        public bool UseTls { get; set; }
        [XmlElement]
        public string SslProtocols { get; set; }
        [XmlElement]
        public bool AllowUntrustedCertificates { get; set; }
        [XmlElement]
        public bool IgnoreCertificateChainErrors { get; set; }
        [XmlElement]
        public bool IgnoreCertificateRevocationErrors { get; set; }

        public X509Certificate CaCert { get; }
        public X509Certificate ClientCert { get; }

        public MqttSecurity() { }

        public MqttSecurity(X509Certificate caCert)
        {
            CaCert = caCert;
        }
        public MqttSecurity(X509Certificate caCert, X509Certificate clientCert)
        {
            CaCert = caCert;
            ClientCert = clientCert;
        }
    }
}
