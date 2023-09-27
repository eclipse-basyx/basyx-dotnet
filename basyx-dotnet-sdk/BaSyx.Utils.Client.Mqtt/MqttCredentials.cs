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
using System.Xml.Serialization;

namespace BaSyx.Utils.Client.Mqtt
{
    public class MqttCredentials : IMqttCredentials
    {
        [XmlElement]
        public string Username { get; set; }

        [XmlElement]
        public string Password { get; set; }

        public MqttCredentials() { }

        public MqttCredentials(string username, string password)
        {
            Username = username;
            Password = password;
        }       
    }
}
