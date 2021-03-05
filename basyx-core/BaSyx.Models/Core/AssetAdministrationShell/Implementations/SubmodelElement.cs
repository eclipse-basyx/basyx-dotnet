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

using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Constraints;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using BaSyx.Models.Core.AssetAdministrationShell.Semantics;
using BaSyx.Models.Core.Common;

namespace BaSyx.Models.Core.AssetAdministrationShell.Implementations
{
    [DataContract]
    public abstract class SubmodelElement : Referable, ISubmodelElement
    {
        public IReference SemanticId { get; set; }

        public IEnumerable<IConstraint> Constraints { get; set; }

        public ModelingKind Kind { get; set; }

        public abstract ModelType ModelType { get; }

        [IgnoreDataMember]
        public virtual GetValueHandler Get { get; set; }
        [IgnoreDataMember]
        public virtual SetValueHandler Set { get; set; }

        public IEnumerable<IEmbeddedDataSpecification> EmbeddedDataSpecifications { get; set; }

        private IConceptDescription conceptDescription;
        public IConceptDescription ConceptDescription
        {
            get => conceptDescription;
            set
            {
                if(value != null && value.EmbeddedDataSpecifications?.Count() > 0)
                {
                    conceptDescription = value;

                    (EmbeddedDataSpecifications as List<IEmbeddedDataSpecification>)?.AddRange(value.EmbeddedDataSpecifications);
                    if (value.Identification?.Id != null && SemanticId == null)
                        SemanticId = new Reference(new Key(KeyElements.ConceptDescription, value.Identification.IdType, value.Identification.Id, true));
                }
            }
        }


        [JsonConstructor]
        protected SubmodelElement(string idShort) : base(idShort)
        {            
            Constraints = new List<IConstraint>();
            MetaData = new Dictionary<string, string>();
            EmbeddedDataSpecifications = new List<IEmbeddedDataSpecification>();
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
