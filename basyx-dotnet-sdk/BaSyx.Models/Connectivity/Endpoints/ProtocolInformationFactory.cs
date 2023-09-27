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

namespace BaSyx.Models.Connectivity
{
    public static partial class ProtocolInformationFactory
    {
        public static ProtocolInformation CreateProtocolInformation(Uri uri)
        {
            switch (uri.Scheme)
            {
                case ProtocolType.HTTPS:
                case ProtocolType.HTTP:
                    HttpProtocol httpEndpoint = new HttpProtocol(uri);
                    return httpEndpoint;

                case ProtocolType.MQTTS:
                case ProtocolType.MQTT:
                    MqttProtocol mqttEndpoint = new MqttProtocol(uri);
                    return mqttEndpoint;

                case ProtocolType.OPC_TCP: 
                    OpcUaProtocol opcUaEndpoint = new OpcUaProtocol(uri);
                    return opcUaEndpoint;

                default:
                    return null;
            }
        }
        public static ProtocolInformation CreateProtocolInformation(string endpointAddress)
        {
            if (Uri.TryCreate(endpointAddress, UriKind.Absolute, out Uri result))
            {
                return CreateProtocolInformation(result);
            }
            return null;
        }
    }
}
