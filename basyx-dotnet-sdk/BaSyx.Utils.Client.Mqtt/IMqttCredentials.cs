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
using BaSyx.Security.Abstractions;
using System.Xml.Serialization;

namespace BaSyx.Utils.Client.Mqtt
{
    [XmlInclude(typeof(MqttCredentials))]
    public interface IMqttCredentials : ICredentials
    {
        string Username { get; }
        string Password { get; }
    }
}
