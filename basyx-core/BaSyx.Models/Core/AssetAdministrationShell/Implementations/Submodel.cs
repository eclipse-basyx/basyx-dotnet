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

using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Constraints;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BaSyx.Models.Core.AssetAdministrationShell.Semantics;
using BaSyx.Models.Core.Common;
using System.Linq;

namespace BaSyx.Models.Core.AssetAdministrationShell.Implementations
{
    [DataContract]
    public class Submodel : Identifiable, ISubmodel
    {
        public ModelingKind Kind { get; set; }
        public IReference SemanticId { get; set; }
        public IElementContainer<ISubmodelElement> SubmodelElements { get; set; }
        public ModelType ModelType => ModelType.Submodel;
        public IEnumerable<IEmbeddedDataSpecification> EmbeddedDataSpecifications { get; set; }
        public IConceptDescription ConceptDescription { get; set; }
        public IEnumerable<IConstraint> Constraints { get; set; }

        [JsonConstructor]
        public Submodel(string idShort, Identifier identification) : base(idShort, identification)
        {
            SubmodelElements = new ElementContainer<ISubmodelElement>(this);
            MetaData = new Dictionary<string, string>();
            EmbeddedDataSpecifications = new List<IEmbeddedDataSpecification>();
            Constraints = new List<IConstraint>();
        }
        public bool ShouldSerializeEmbeddedDataSpecifications()
        {
            if (EmbeddedDataSpecifications?.Count() > 0)
                return true;
            else
                return false;
        }

        public bool ShouldSerializeConstraints()
        {
            if (Constraints?.Count() > 0)
                return true;
            else
                return false;
        }
    }
}
