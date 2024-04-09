/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

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

        private GetValueScopeHandler _get;
        private SetValueScopeHandler _set;
        internal ValueScope _valueScope;

        [JsonIgnore, IgnoreDataMember]
        public virtual GetValueScopeHandler Get
        {
            get
            {
                if (_get == null)
                    _get = new GetValueScopeHandler(element => Task.FromResult(_valueScope));
                return _get;
            }
            set
            {
                _get = value;
            }
        }
        [JsonIgnore, IgnoreDataMember]
        public virtual SetValueScopeHandler Set
        {
            get
            {
                if (_set == null)
                    _set = new SetValueScopeHandler((element, valueScope) => 
                    { 
                        _valueScope = valueScope;
                        OnValueChanged(new ValueChangedArgs(IdShort, valueScope));
                        return Task.CompletedTask; 
                    });
                return _set;
            }
            set
            {
                _set = value;
            }
        }

        [JsonIgnore, IgnoreDataMember]
        public ValueScope Value { get => Get(this).Result; set => Set(this, value); }

        public IEnumerable<IEmbeddedDataSpecification> EmbeddedDataSpecifications { get; set; }

        private IConceptDescription conceptDescription;

        [JsonIgnore, IgnoreDataMember]
        public IConceptDescription ConceptDescription
        {
            get => conceptDescription;
            set
            {
                if (value != null && value.EmbeddedDataSpecifications?.Count() > 0)
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

        protected virtual void OnValueChanged(ValueChangedArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

        public virtual async Task<ValueScope> GetValueScope()
        {
            return await Get(this).ConfigureAwait(false);
        }
        public virtual async Task SetValueScope(ValueScope value)
        {
            await Set(this, value).ConfigureAwait(false);
        }

        public object Clone()
        {
	        return (ISubmodelElement)MemberwiseClone();
        }
    }

    [DataContract]
    public abstract class SubmodelElement<TValueScope> : SubmodelElement, ISubmodelElement<TValueScope> where TValueScope : ValueScope
    {
        private GetValueScopeHandler<TValueScope> _get;
        private SetValueScopeHandler<TValueScope> _set;

        [JsonIgnore, IgnoreDataMember]
        public new virtual GetValueScopeHandler<TValueScope> Get 
        {
            get
            {
                if (_get == null)
                {
                    _get = new GetValueScopeHandler<TValueScope>(element => Task.FromResult((TValueScope)_valueScope));
                    base.Get = new GetValueScopeHandler(async element => await _get(this));
                }
                return _get;
            }
            set
            {
                _get = value;
                if (value != null)
                    base.Get = new GetValueScopeHandler(async element => await _get(this));
                else
                    base.Get = null;
            }
        }
        [JsonIgnore, IgnoreDataMember]
        public new virtual SetValueScopeHandler<TValueScope> Set 
        {
            get
            {
                if (_set == null)
                {
                    _set = new SetValueScopeHandler<TValueScope>((element, valueScope) => { _valueScope = valueScope; return Task.CompletedTask; });
                    base.Set = new SetValueScopeHandler(async (element, valueScope) => await _set(element, (TValueScope)valueScope));
                }                    
                return _set;
            }
            set
            {
                _set = value;
                if (value != null)
                    base.Set = new SetValueScopeHandler(async (element, valueScope) => await _set(element, (TValueScope)valueScope));                    
                else
                    base.Set = null;
            }
        }

        [JsonIgnore, IgnoreDataMember]
        public new TValueScope Value { get => Get(this).Result; set => Set(this, value); }

        protected SubmodelElement(string idShort) : base(idShort) { }

    }
}
