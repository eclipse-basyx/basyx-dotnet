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
    public class OpcUaProtocol : ProtocolInformation
    {
        [IgnoreDataMember]
        public string BrowsePath { get; }

        [IgnoreDataMember]
        public string Authority { get; }

        public OpcUaProtocol(string endpointAddress) : base(endpointAddress)
        {
            Uri uri = new Uri(endpointAddress);
            BrowsePath = uri.AbsolutePath;
            Authority = uri.Authority;
        }

        public OpcUaProtocol(Uri uri) : this(uri?.ToString())
        { }

    }
}
