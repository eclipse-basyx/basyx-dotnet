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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System;
using System.Threading.Tasks;

namespace BaSyx.Models.AdminShell
{
    [DataContract]
    public abstract class SubmodelElement : Referable, ISubmodelElement
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "semanticId")]
        public IReference SemanticId { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "supplementalSemanticIds")]
        public IEnumerable<IReference> SupplementalSemanticIds { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "qualifiers")]
        public IEnumerable<IQualifier> Qualifiers { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "kind")]
        public ModelingKind Kind { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "modelType")]
        public abstract ModelType ModelType { get; }

        [IgnoreDataMember]
        public virtual GetValueHandler Get { get; set; }
        [IgnoreDataMember]
        public virtual SetValueHandler Set { get; set; }
        internal object Value { get; set; }

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
                    if (value.Id?.Id != null && SemanticId == null)
                        SemanticId = new Reference(new Key(KeyType.ConceptDescription, value.Id));
                }
            }
        }

        public event EventHandler<ValueChangedArgs> ValueChanged;

        [JsonConstructor]
        protected SubmodelElement(string idShort) : base(idShort)
        {            
            Qualifiers = new List<IQualifier>();
            MetaData = new Dictionary<string, string>();
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

        public bool ShouldSerializeConstraints()
        {
            if (Qualifiers?.Count() > 0)
                return true;
            else
                return false;
        }

        protected virtual void OnValueChanged(ValueChangedArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

        public virtual Task<IValue> GetValue()
        {
            return Get?.Invoke(this);
        }
        public virtual async Task SetValue(IValue value)
        {
            await Set?.Invoke(this, value);
        }
    }
}
