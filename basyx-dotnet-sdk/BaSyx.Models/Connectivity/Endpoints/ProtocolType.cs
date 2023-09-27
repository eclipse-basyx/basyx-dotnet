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
namespace BaSyx.Models.Connectivity
{
    public static class ProtocolType
    {
        public const string HTTP = "http";
        public const string HTTPS = "https";
        public const string TCP = "tcp";
        public const string MQTT = "mqtt";
        public const string MQTTS = "mqtts";
        public const string OPC_TCP = "opc.tcp";
        public const string COAP = "coap";
        public const string COAPS = "coaps";
        public const string WEBSOCKET = "ws";
        public const string WEBSOCKETS = "wss";
    }
}

