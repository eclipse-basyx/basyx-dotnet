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
    public class SubmodelElementCollection : SubmodelElement, ISubmodelElementCollection, IElementContainer<ISubmodelElement>
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "modelType")]
        public override ModelType ModelType => ModelType.SubmodelElementCollection;        

		[DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        public new IElementContainer<ISubmodelElement> Value { get => _value; set => _value.AddRange(value); }

        [IgnoreDataMember, JsonIgnore]
        public IEnumerable<IElementContainer<ISubmodelElement>> Children => Value.Children;
        [IgnoreDataMember, JsonIgnore]
        public IEnumerable<ISubmodelElement> Values => Value.Values;
        [IgnoreDataMember, JsonIgnore]
        ISubmodelElement IElementContainer<ISubmodelElement>.Value { get => this; }
        [IgnoreDataMember, JsonIgnore]
        public bool IsRoot => Value.IsRoot;
        [IgnoreDataMember, JsonIgnore]
        public IElementContainer<ISubmodelElement> ParentContainer { get => Value.ParentContainer; set { Value.ParentContainer = value; } }
        [IgnoreDataMember, JsonIgnore]
        public int Count => Value.Count;
        [IgnoreDataMember, JsonIgnore]
        public bool IsReadOnly => Value.IsReadOnly;

        [IgnoreDataMember, JsonIgnore]
        public ISubmodelElement this[string idShort] { get => Value[idShort]; set => Value[idShort] = value; }
        [IgnoreDataMember, JsonIgnore]
        public ISubmodelElement this[int i] { get => Value[i]; set => Value[i] = value; }

		[IgnoreDataMember, JsonIgnore]
		public string Path
		{
			get
			{
				if (string.IsNullOrEmpty(Value.Path))
					return IdShort;
				else
					return Value.Path;
			}
			set { Value.Path = value; }
		}

		private readonly IElementContainer<ISubmodelElement> _value;

		public SubmodelElementCollection(string idShort) : base(idShort) 
        {
			_value = new ElementContainer<ISubmodelElement>(this.Parent, this, null);
        }

        public event EventHandler<ElementContainerEventArgs<ISubmodelElement>> OnCreated
        {
            add
            {
                Value.OnCreated += value;
            }

            remove
            {
                Value.OnCreated -= value;
            }
        }

        public event EventHandler<ElementContainerEventArgs<ISubmodelElement>> OnUpdated
        {
            add
            {
                Value.OnUpdated += value;
            }

            remove
            {
                Value.OnUpdated -= value;
            }
        }

        public event EventHandler<ElementContainerEventArgs<ISubmodelElement>> OnDeleted
        {
            add
            {
                Value.OnDeleted += value;
            }

            remove
            {
                Value.OnDeleted -= value;
            }
        }

        public IResult<IElementContainer<ISubmodelElement>> RetrieveAll()
        {
            return Value.RetrieveAll();
        }

        public IResult<IElementContainer<ISubmodelElement>> RetrieveAll(Predicate<ISubmodelElement> predicate)
        {
            return Value.RetrieveAll(predicate);
        }

        public bool HasChildren()
        {
            return Value.HasChildren();
        }

        public bool HasChild(string idShort)
        {
            return Value.HasChild(idShort);
        }

        public bool HasChildPath(string idShortPath)
        {
            return Value.HasChildPath(idShortPath);
        }

        public void Traverse(Action<ISubmodelElement> action)
        {
            Value.Traverse(action);
        }

        public void Add(ISubmodelElement element)
        {
            Value.Add(element);
        }

        public void AddRange(IEnumerable<ISubmodelElement> elements)
        {
            Value.AddRange(elements);
        }

        public IResult<ISubmodelElement> Create(ISubmodelElement element)
        {
            return Value.Create(element);
        }

        public IResult<ISubmodelElement> Retrieve(string id)
        {
            return Value.Retrieve(id);
        }

        IResult<T> ICrudContainer<string, ISubmodelElement>.Retrieve<T>(string id)
        {
            return Value.Retrieve<T>(id);
        }

        IResult<IElementContainer<T>> ICrudContainer<string, ISubmodelElement>.RetrieveAll<T>()
        {
            return Value.RetrieveAll<T>();
        }

        IResult<IElementContainer<T>> ICrudContainer<string, ISubmodelElement>.RetrieveAll<T>(Predicate<T> predicate)
        {
            return Value.RetrieveAll(predicate);
        }

        public IResult<ISubmodelElement> CreateOrUpdate(string id, ISubmodelElement element)
        {
            return Value.CreateOrUpdate(id, element);
        }

        public IResult<ISubmodelElement> Create(string id, ISubmodelElement element)
        {
            return Value.Create(id, element);
        }

        public IResult<ISubmodelElement> Update(string id, ISubmodelElement element)
        {
            return Value.Update(id, element);
        }

        public IResult Delete(string id)
        {
            return Value.Delete(id);
        }

        public IEnumerator<ISubmodelElement> GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        public IElementContainer<ISubmodelElement> GetChild(string idShortPath)
        {
            return Value.GetChild(idShortPath);
        }

        public void Remove(string idShort)
        {
            Value.Remove(idShort);
        }

        public void AppendRootPath(string rootPath)
        {
            Value.AppendRootPath(rootPath);
        }

        public IEnumerable<ISubmodelElement> Flatten()
        {
            return Value.Flatten();
        }

        public void Clear()
        {
            Value.Clear();
        }

        public bool Contains(ISubmodelElement item)
        {
            return Value.Contains(item);
        }

        public void CopyTo(ISubmodelElement[] array, int arrayIndex)
        {
            Value.CopyTo(array, arrayIndex);
        }

        public bool Remove(ISubmodelElement item)
        {
            return Value.Remove(item);            
        }
    }
    
    public class SubmodelElementCollection<T> : SubmodelElementCollection where T : class
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        public IElementContainer<ISubmodelElement> BaseValue { get => base.Value; set => base.Value = value; }

        [IgnoreDataMember]
        [JsonIgnore]
        public new T Value 
        { 
            get
            {
                T innerValue = base.Value.ToObject<T>();
                return innerValue;
            } 
            set
            {
                var smc = value.CreateSubmodelElementCollectionFromObject(IdShort, BindingFlags.Public | BindingFlags.Instance);
                foreach (var element in smc.Value)
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
            foreach (var element in smc.Value)
            {
                base.Add(element);
            }
        }

        public SubmodelElementCollection(string idShort, T entity) : this(idShort, entity, BindingFlags.Public | BindingFlags.Instance)
        { }

        public SubmodelElementCollection(string idShort, T entity, BindingFlags bindingFlags) : base(idShort)
        {
            var smc = entity.CreateSubmodelElementCollectionFromObject(this.IdShort, bindingFlags);
            foreach (var element in smc.Value)
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
            Get = collection.Get;
            Set = collection.Set;
            BaseValue = collection.Value;
        }
    }
    
}
