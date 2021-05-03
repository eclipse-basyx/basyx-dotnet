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
namespace BaSyx.Models.Connectivity
{
    public static class EndpointType
    {
        public const string HTTP = "http";
        public const string HTTPS = "https";
        public const string TCP = "tcp";
        public const string MQTT = "mqtt";
        public const string MQTTS = "mqtts";
        public const string OPC_UA = "opc-ua";
        public const string COAP = "coap";
        public const string WEBSOCKET = "websocket";
    }
}

