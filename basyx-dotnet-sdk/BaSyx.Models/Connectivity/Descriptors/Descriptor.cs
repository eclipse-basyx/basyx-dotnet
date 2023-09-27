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
using BaSyx.Models.AdminShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BaSyx.Models.Connectivity
{
    [DataContract]
    public abstract class Descriptor : IServiceDescriptor
    {
        public string IdShort { get; set; }
        public Identifier Identification { get; set; }
        public AdministrativeInformation Administration { get; set; }
        public LangStringSet Description { get; set; }
        public LangStringSet DisplayName { get; set; }
        public abstract ModelType ModelType { get; }
        public IEnumerable<IEndpoint> Endpoints => _endpoints;

        private List<IEndpoint> _endpoints;

        protected Descriptor(IEnumerable<IEndpoint> endpoints)
        {
            _endpoints = endpoints?.ToList() ?? new List<IEndpoint>();
        }

        public void AddEndpoints(IEnumerable<IEndpoint> endpoints)
        {
            foreach (var endpoint in endpoints)
            {
                int index = _endpoints.FindIndex(e => e.ProtocolInformation.EndpointAddress == endpoint.ProtocolInformation.EndpointAddress);
                if (index == -1)
                    _endpoints.Add(endpoint);
            }
        }

        public void SetEndpoints(IEnumerable<IEndpoint> endpoints)
        {
            _endpoints = endpoints.ToList();
        }

        public void DeleteEndpoint(IEndpoint endpoint)
        {
            int index = _endpoints.FindIndex(e => e.ProtocolInformation.EndpointAddress == endpoint.ProtocolInformation.EndpointAddress);
            _endpoints.RemoveAt(index);
        }
    }
}
