/*******************************************************************************
* Copyright (c) 2020 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using BaSyx.Utils.Config;
using BaSyx.Utils.Security;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;

namespace BaSyx.Utils.Client.Mqtt
{
    public class MqttConfig : IEventHandlerConfig
    {
        [XmlElement]
        public string ClientId { get; set; }
        [XmlElement]
        public string BrokerEndpoint { get; set; }
        [XmlIgnore]
        public ICredentials Credentials { get; set; }
        [XmlElement]
        public bool SecureConnection { get; set; } = false;
        [XmlElement]
        public int PublishTimeout { get; set; } = 5000;
        [XmlElement]
        public int ReceiveTimeout { get; set; } = 5000;
        [XmlIgnore]
        public ISecurity Security { get; set; }
        [XmlElement]
        public MqttConnectConfig MqttConnectConfig { get; set; }

        internal MqttConfig() { }

        public MqttConfig(string clientId, string brokerEndpoint)
        {
            ClientId = clientId;
            BrokerEndpoint = brokerEndpoint;
            MqttConnectConfig = new MqttConnectConfig();
        }
        public MqttConfig(string clientId, string brokerEndpoint, MqttCredentials credentials) : this(clientId, brokerEndpoint)
        {
            Credentials = credentials;
        }

        public MqttConfig(string clientId, string brokerEndpoint, MqttCredentials credentials, MqttSecurity security) : this(clientId, brokerEndpoint, credentials)
        {
            Security = security;
        }
    }

    public class MqttConnectConfig
    {
        [XmlElement]
        public bool WillRetain { get; set; } = false;
        [XmlElement]
        public byte WillQosLevel { get; set; } = 0;
        [XmlElement]
        public bool WillFlag { get; set; } = false;
        [XmlElement]
        public string WillTopic { get; set; } = null;
        [XmlElement]
        public string WillMessage { get; set; } = null;
        [XmlElement]
        public bool CleanSession { get; set; } = true;
        [XmlElement]
        public ushort KeepAlivePeriod { get; set; } = 60;
    }

    public class MqttCredentials : ICredentials
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        internal MqttCredentials()
        { }
        
        public MqttCredentials(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }

    public class MqttSecurity : ISecurity
    {
        public X509Certificate CaCert { get; }
        public X509Certificate ClientCert { get; }

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
