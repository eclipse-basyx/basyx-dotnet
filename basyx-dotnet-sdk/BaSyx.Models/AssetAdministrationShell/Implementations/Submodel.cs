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

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace BaSyx.Models.AdminShell
{
    [DataContract]
    public class Submodel : Identifiable, ISubmodel
    {
        public ModelingKind Kind { get; set; }
        public IReference SemanticId { get; set; }
        public IEnumerable<IReference> SupplementalSemanticIds { get; set; }
        public IElementContainer<ISubmodelElement> SubmodelElements { get; set; }
        public ModelType ModelType => ModelType.Submodel;
        public IEnumerable<IEmbeddedDataSpecification> EmbeddedDataSpecifications { get; set; }
        public IConceptDescription ConceptDescription { get; set; }
        public IEnumerable<IQualifier> Qualifiers { get; set; }
        

        public Submodel(string idShort, Identifier id) : base(idShort, id)
        {
            SubmodelElements = new ElementContainer<ISubmodelElement>(this);
            MetaData = new Dictionary<string, string>();
            Qualifiers = new List<IQualifier>();
            EmbeddedDataSpecifications = new List<IEmbeddedDataSpecification>();
            SupplementalSemanticIds = new List<IReference>();
        }
        public bool ShouldSerializeEmbeddedDataSpecifications()
        {
            if (EmbeddedDataSpecifications?.Count() > 0)
                return true;
            else
                return false;
        }

        public bool ShouldSerializeQualifiers()
        {
            if (Qualifiers?.Count() > 0)
                return true;
            else
                return false;
        }

        public bool ShouldSerializeSupplementalSemanticIds()
        {
            if (SupplementalSemanticIds?.Count() > 0)
                return true;
            else
                return false;
        }
    }
}
