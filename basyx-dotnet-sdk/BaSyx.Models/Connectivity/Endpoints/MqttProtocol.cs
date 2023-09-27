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
using System.Runtime.Serialization;

namespace BaSyx.Models.Connectivity
{
    public class MqttProtocol : ProtocolInformation
    {
        [IgnoreDataMember]
        public Uri BrokerUri { get; }

        [IgnoreDataMember]
        public string Topic { get; }

        public MqttProtocol(string endpointAddress) : base (endpointAddress)
        {
            Uri uri = new Uri(endpointAddress);
            BrokerUri = new Uri(uri.AbsoluteUri);
            Topic = uri.AbsolutePath;
        }

        public MqttProtocol(Uri uri) : this(uri?.ToString())
        { }
    }
}
