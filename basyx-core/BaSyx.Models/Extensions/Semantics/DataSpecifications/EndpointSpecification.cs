/*******************************************************************************
* Copyright (c) 2020 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using BaSyx.Models.Connectivity;
using BaSyx.Models.Core.Attributes;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BaSyx.Models.Core.AssetAdministrationShell.Semantics;

namespace BaSyx.Models.Extensions.Semantics.DataSpecifications
{
    [DataContract, DataSpecification("urn:basys:org.eclipse.basyx:dataSpecifications:EndpointSpecification:1.0.0")]
    public class EndpointSpecification : IEmbeddedDataSpecification
    {
        public IReference HasDataSpecification => new Reference(
        new GlobalKey(KeyElements.GlobalReference, KeyType.IRI, "urn:basys:org.eclipse.basyx:dataSpecifications:EndpointSpecification:1.0.0"));
        [DataSpecificationContent(typeof(EndpointSpecificationContent), "BASYS")]
        public IDataSpecificationContent DataSpecificationContent { get; set; }

        public EndpointSpecification(EndpointSpecificationContent content)
        {
            DataSpecificationContent = content;
        }
    }

    [DataContract]
    public class EndpointSpecificationContent : IDataSpecificationContent
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "endpoints")]
        public List<IEndpoint> Endpoints { get; set; }
    }
}
