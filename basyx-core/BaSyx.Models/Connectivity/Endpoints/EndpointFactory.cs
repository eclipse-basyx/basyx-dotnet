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
using System;

namespace BaSyx.Models.Connectivity
{
    public static partial class EndpointFactory
    {
        public static IEndpoint CreateEndpoint(Uri uri, IEndpointSecurity security)
        {
            switch (uri.Scheme)
            {
                case EndpointType.HTTPS:
                case EndpointType.HTTP:
                    HttpEndpoint httpEndpoint = new HttpEndpoint(uri);
                    httpEndpoint.Security = security;
                    return httpEndpoint;

                case EndpointType.MQTTS:
                case EndpointType.MQTT:
                    MqttEndpoint mqttEndpoint = new MqttEndpoint(uri);
                    mqttEndpoint.Security = security;
                    return mqttEndpoint;

                case EndpointType.OPC_UA: 
                    OpcUaEndpoint opcUaEndpoint = new OpcUaEndpoint(uri);
                    opcUaEndpoint.Security = security;
                    return opcUaEndpoint;

                default:
                    return null;
            }
        }
        public static IEndpoint CreateEndpoint(string endpointType, string address, IEndpointSecurity security)
        {
            switch (endpointType.ToLower())
            {
                case EndpointType.HTTPS:
                case EndpointType.HTTP:
                    HttpEndpoint httpEndpoint = new HttpEndpoint(address);
                    httpEndpoint.Security = security;
                    return httpEndpoint;

                case EndpointType.MQTTS:
                case EndpointType.MQTT:
                    Uri uri = new Uri(address);
                    MqttEndpoint mqttEndpoint = new MqttEndpoint(address);
                    mqttEndpoint.Security = security;
                    return mqttEndpoint;

                case EndpointType.OPC_UA:
                    OpcUaEndpoint opcUaEndpoint = new OpcUaEndpoint(address);
                    opcUaEndpoint.Security = security;
                    return opcUaEndpoint;

                default:
                    return null;
            }
        }
    }
}
