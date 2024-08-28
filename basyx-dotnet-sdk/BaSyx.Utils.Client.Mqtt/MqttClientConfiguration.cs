/*******************************************************************************
* Copyright (c) 2024 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using System.Xml.Serialization;

namespace BaSyx.Utils.Client.Mqtt
{
    public class MqttClientConfiguration
    {
        [XmlElement]
        public bool Activated { get; set; }
        [XmlElement]
        public string ClientId { get; set; }
        [XmlElement]
        public string BrokerEndpoint { get; set; }
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
        [XmlElement]
        public bool Reconnect { get; set; } = false;
        [XmlElement]
        public int ReconnectDelay { get; set; } = 5000;

        [XmlElement]
        public MqttCredentials Credentials { get; set; }
        [XmlElement]
        public MqttSecurity Security { get; set; }

        public MqttClientConfiguration() { }

        public MqttClientConfiguration(string clientId, string brokerEndpoint)
        {
            ClientId = clientId;
            BrokerEndpoint = brokerEndpoint;
        }
        public MqttClientConfiguration(string clientId, string brokerEndpoint, MqttCredentials credentials) : this(clientId, brokerEndpoint)
        {
            Credentials = credentials;
        }
    }
}
