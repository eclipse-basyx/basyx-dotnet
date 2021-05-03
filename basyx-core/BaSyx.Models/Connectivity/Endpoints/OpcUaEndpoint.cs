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
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace BaSyx.Models.Connectivity
{
    public class OpcUaEndpoint : IEndpoint
    {
        public string Address { get; }

        [IgnoreDataMember]
        public string BrowsePath { get; }
        [IgnoreDataMember]
        public string Authority { get; }
        public string Type => EndpointType.OPC_UA;

        public IEndpointSecurity Security { get; set; }

        [JsonConstructor]
        public OpcUaEndpoint(string address)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
            Uri uri = new Uri(address);
            BrowsePath = uri.AbsolutePath;
            Authority = uri.Authority;
        }

        public OpcUaEndpoint(Uri uri) : this(uri?.ToString())
        { }

    }
}
