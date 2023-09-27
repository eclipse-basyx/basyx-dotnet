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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BaSyx.Models.Connectivity
{
    public class ProtocolInformation
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "endpointAddress")]
        public string EndpointAddress { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "endpointProtocol")]
        public string EndpointProtocol { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "endpointProtocolVersion")]
        public string EndpointProtocolVersion { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "subprotocol")]
        public string Subprotocol { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "subprotocolBody")]
        public string SubprotocolBody { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "subprotocolBodyEncoding")]
        public string SubprotocolBodyEncoding { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "securityAttributes")]
        public IEnumerable<SecurityAttribute> SecurityAttributes { get; set; }

        [IgnoreDataMember]
        public Uri Uri { get; }

        [JsonConstructor]
        public ProtocolInformation(string endpointAddress)
        {
            EndpointAddress = endpointAddress ?? throw new ArgumentNullException(nameof(endpointAddress));
            SecurityAttributes = new List<SecurityAttribute>();

            if (Uri.TryCreate(endpointAddress, UriKind.Absolute, out Uri result))
            {
                Uri = result;
                EndpointProtocol = Uri.Scheme;
            }
        }

        public ProtocolInformation(Uri uri) : this(uri?.ToString())
        { }
    }
}
