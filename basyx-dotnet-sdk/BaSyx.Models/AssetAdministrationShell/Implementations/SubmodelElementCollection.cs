/*******************************************************************************
* Copyright (c) 2024 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using BaSyx.Models.Extensions;
using BaSyx.Utils.ResultHandling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace BaSyx.Models.AdminShell
{
    [DataContract]
    public class SubmodelElementCollection : SubmodelElement<SubmodelElementCollectionValue>, ISubmodelElementCollection, IElementContainer<ISubmodelElement>
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "modelType")]
        public override ModelType ModelType => ModelType.SubmodelElementCollection;

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        public new SubmodelElementCollectionValue Value { get => (SubmodelElementCollectionValue)_valueScope; set => _valueScope = value; }

        [IgnoreDataMember, JsonIgnore]
        public IEnumerable<IElementContainer<ISubmodelElement>> Children => Value.Value.Children;
        [IgnoreDataMember, JsonIgnore]
        public IEnumerable<ISubmodelElement> Values => Value.Value.Values;
        [IgnoreDataMember, JsonIgnore]
        ISubmodelElement IElementContainer<ISubmodelElement>.Value { get => this; }
        [IgnoreDataMember, JsonIgnore]
        public bool IsRoot => Value.Value.IsRoot;
        [IgnoreDataMember, JsonIgnore]
        public IElementContainer<ISubmodelElement> ParentContainer { get => Value.Value.ParentContainer; set { Value.Value.ParentContainer = value; } }
        [IgnoreDataMember, JsonIgnore]
        public int Count => Value.Value.Count;
        [IgnoreDataMember, JsonIgnore]
        public bool IsReadOnly => Value.Value.IsReadOnly;

        [IgnoreDataMember, JsonIgnore]
        public ISubmodelElement this[string idShort] { get => Value.Value[idShort]; set => Value.Value[idShort] = value; }
        [IgnoreDataMember, JsonIgnore]
        public ISubmodelElement this[int i] { get => Value.Value[i]; set => Value.Value[i] = value; }

		[IgnoreDataMember, JsonIgnore]
		public string Path
		{
			get
			{
				if (string.IsNullOrEmpty(Value.Value.Path))
					return IdShort;
				else
					return Value.Value.Path;
			}
			set { Value.Value.Path = value; }
		}        

		public SubmodelElementCollection(string idShort) : base(idShort) 
        {
            _valueScope = new SubmodelElementCollectionValue(new ElementContainer<ISubmodelElement>(this.Parent, this, null));
        }

        public event EventHandler<ElementContainerEventArgs<ISubmodelElement>> OnCreated
        {
            add
            {
                Value.Value.OnCreated += value;
            }

            remove
            {
                Value.Value.OnCreated -= value;
            }
        }

        public event EventHandler<ElementContainerEventArgs<ISubmodelElement>> OnUpdated
        {
            add
            {
                Value.Value.OnUpdated += value;
            }

            remove
            {
                Value.Value.OnUpdated -= value;
            }
        }

        public event EventHandler<ElementContainerEventArgs<ISubmodelElement>> OnDeleted
        {
            add
            {
                Value.Value.OnDeleted += value;
            }

            remove
            {
                Value.Value.OnDeleted -= value;
            }
        }

        public IResult<IElementContainer<ISubmodelElement>> RetrieveAll()
        {
            return Value.Value.RetrieveAll();
        }

        public IResult<IElementContainer<ISubmodelElement>> RetrieveAll(Predicate<ISubmodelElement> predicate)
        {
            return Value.Value.RetrieveAll(predicate);
        }

        public bool HasChildren()
        {
            return Value.Value.HasChildren();
        }

        public bool HasChild(string idShort)
        {
            return Value.Value.HasChild(idShort);
        }

        public bool HasChildPath(string idShortPath)
        {
            return Value.Value.HasChildPath(idShortPath);
        }

        public void Traverse(Action<ISubmodelElement> action)
        {
            Value.Value.Traverse(action);
        }

        public void Add(ISubmodelElement element)
        {
            Value.Value.Add(element);
        }

        public void AddRange(IEnumerable<ISubmodelElement> elements)
        {
            Value.Value.AddRange(elements);
        }

        public IResult<ISubmodelElement> Create(ISubmodelElement element)
        {
            return Value.Value.Create(element);
        }

        public IResult<ISubmodelElement> Retrieve(string id)
        {
            return Value.Value.Retrieve(id);
        }

        IResult<T> ICrudContainer<string, ISubmodelElement>.Retrieve<T>(string id)
        {
            return Value.Value.Retrieve<T>(id);
        }

        IResult<IElementContainer<T>> ICrudContainer<string, ISubmodelElement>.RetrieveAll<T>()
        {
            return Value.Value.RetrieveAll<T>();
        }

        IResult<IElementContainer<T>> ICrudContainer<string, ISubmodelElement>.RetrieveAll<T>(Predicate<T> predicate)
        {
            return Value.Value.RetrieveAll(predicate);
        }

        public IResult<ISubmodelElement> CreateOrUpdate(string id, ISubmodelElement element)
        {
            return Value.Value.CreateOrUpdate(id, element);
        }

        public IResult<ISubmodelElement> Create(string id, ISubmodelElement element)
        {
            return Value.Value.Create(id, element);
        }

        public IResult<ISubmodelElement> Update(string id, ISubmodelElement element)
        {
            return Value.Value.Update(id, element);
        }

        public IResult Delete(string id)
        {
            return Value.Value.Delete(id);
        }

        public IEnumerator<ISubmodelElement> GetEnumerator()
        {
            return Value.Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Value.Value.GetEnumerator();
        }

        public IElementContainer<ISubmodelElement> GetChild(string idShortPath)
        {
            return Value.Value.GetChild(idShortPath);
        }

        public void Remove(string idShort)
        {
            Value.Value.Remove(idShort);
        }

        public void AppendRootPath(string rootPath)
        {
            Value.Value.AppendRootPath(rootPath);
        }

        public IEnumerable<ISubmodelElement> Flatten()
        {
            return Value.Value.Flatten();
        }

        public void Clear()
        {
            Value.Value.Clear();
        }

        public bool Contains(ISubmodelElement item)
        {
            return Value.Value.Contains(item);
        }

        public void CopyTo(ISubmodelElement[] array, int arrayIndex)
        {
            Value.Value.CopyTo(array, arrayIndex);
        }

        public bool Remove(ISubmodelElement item)
        {
            return Value.Value.Remove(item);            
        }
    }
    
    public class SubmodelElementCollection<T> : SubmodelElementCollection where T : class
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        public IElementContainer<ISubmodelElement> BaseValue { get => base.Value.Value; set => base.Value.Value = value; }

        [IgnoreDataMember]
        [JsonIgnore]
        public new T Value 
        { 
            get
            {
                T innerValue = base.Value.Value.ToObject<T>();
                return innerValue;
            } 
            set
            {
                var smc = value.CreateSubmodelElementCollectionFromObject(IdShort, BindingFlags.Public | BindingFlags.Instance);
                foreach (var element in smc.Value.Value)
                {
                    var vc = element.GetValueScope().Result;
                    base[element.IdShort].SetValueScope(vc);
                }
            }
        }

        [JsonConstructor]
        public SubmodelElementCollection(string idShort) : base(idShort)
        {
            var smc = typeof(T).CreateSubmodelElementCollectionFromType(this.IdShort);
            foreach (var element in smc.Value.Value)
            {
                base.Add(element);
            }
        }

        public SubmodelElementCollection(string idShort, T entity) : this(idShort, entity, BindingFlags.Public | BindingFlags.Instance)
        { }

        public SubmodelElementCollection(string idShort, T entity, BindingFlags bindingFlags) : base(idShort)
        {
            var smc = entity.CreateSubmodelElementCollectionFromObject(this.IdShort, bindingFlags);
            foreach (var element in smc.Value.Value)
            {
                base.Add(element);
            }
        }

        public SubmodelElementCollection(ISubmodelElementCollection collection) : this(collection.IdShort)
        {
            Category = collection.Category;
            Qualifiers = collection.Qualifiers;
            EmbeddedDataSpecifications = collection.EmbeddedDataSpecifications;
            ConceptDescription = collection.ConceptDescription;
            Kind = collection.Kind;
            Parent = collection.Parent;
            Description = collection.Description;
            DisplayName = collection.DisplayName;
            SemanticId = collection.SemanticId;
            SupplementalSemanticIds = collection.SupplementalSemanticIds;
            Get = (collection as SubmodelElementCollection).Get;
            Set = (collection as SubmodelElementCollection).Set;
            BaseValue = collection.Value.Value;
        }
    }
    
}
