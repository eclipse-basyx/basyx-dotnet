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

using Newtonsoft.Json;
using System;

namespace BaSyx.Models.Connectivity
{
    public class Endpoint : IEndpoint
    {
        public ProtocolInformation ProtocolInformation { get; set; }
        public InterfaceName Interface { get; set; }

        [JsonConstructor]
        public Endpoint(ProtocolInformation protocolInformation, InterfaceName @interface)
        {
            ProtocolInformation = protocolInformation;
            Interface = @interface;
        }

        public Endpoint(Uri endpointUri, InterfaceName @interface) :
            this(ProtocolInformationFactory.CreateProtocolInformation(endpointUri), @interface)
        { }

        public Endpoint(string endpointAddress, InterfaceName @interface) : 
            this (ProtocolInformationFactory.CreateProtocolInformation(endpointAddress), @interface) { }
    }
}
