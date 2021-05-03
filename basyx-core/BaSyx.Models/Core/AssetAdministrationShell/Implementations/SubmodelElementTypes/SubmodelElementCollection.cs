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
using BaSyx.Models.Core.Common;
using BaSyx.Models.Extensions;
using BaSyx.Utils.ResultHandling;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Implementations
{
    [DataContract, JsonObject]
    public class SubmodelElementCollection : SubmodelElement, ISubmodelElementCollection, IElementContainer<ISubmodelElement>
    {
        public override ModelType ModelType => ModelType.SubmodelElementCollection;
        public IElementContainer<ISubmodelElement> Value { get; set; }
        public bool AllowDuplicates { get; set; } 
        public bool Ordered { get; set; }

        [IgnoreDataMember, JsonIgnore]
        public IEnumerable<IElementContainer<ISubmodelElement>> Children => Value.Children;
        [IgnoreDataMember, JsonIgnore]
        public IEnumerable<ISubmodelElement> Values => Value.Values;
        [IgnoreDataMember, JsonIgnore]
        ISubmodelElement IElementContainer<ISubmodelElement>.Value { get => this; set { } }
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
        [IgnoreDataMember, JsonIgnore]
        public bool IsRoot => Value.IsRoot;
        [IgnoreDataMember, JsonIgnore]
        public IElementContainer<ISubmodelElement> ParentContainer { get => this; set { } }
        [IgnoreDataMember, JsonIgnore]
        public int Count => Value.Count;
        [IgnoreDataMember, JsonIgnore]
        public bool IsReadOnly => Value.IsReadOnly;

        [IgnoreDataMember, JsonIgnore]
        public ISubmodelElement this[string idShort] => Value[idShort];
        [IgnoreDataMember, JsonIgnore]
        public ISubmodelElement this[int i] => Value[i];

        public SubmodelElementCollection(string idShort) : base(idShort) 
        {
            AllowDuplicates = false;
            Ordered = false;

            Value = new ElementContainer<ISubmodelElement>(this.Parent, this, null);

            Get = element => { return new ElementValue(Value, new DataType(DataObjectType.AnyType)); };
            Set = (element, value) => { Value = value?.Value as IElementContainer<ISubmodelElement>; };
        }

        public IResult<IQueryableElementContainer<ISubmodelElement>> RetrieveAll()
        {
            return Value.RetrieveAll();
        }

        public IResult<IQueryableElementContainer<ISubmodelElement>> RetrieveAll(Predicate<ISubmodelElement> predicate)
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

        IResult<IQueryableElementContainer<T>> ICrudContainer<string, ISubmodelElement>.RetrieveAll<T>()
        {
            return Value.RetrieveAll<T>();
        }

        IResult<IQueryableElementContainer<T>> ICrudContainer<string, ISubmodelElement>.RetrieveAll<T>(Predicate<T> predicate)
        {
            return Value.RetrieveAll(predicate);
        }

        public IResult<ISubmodelElement> CreateOrUpdate(string id, ISubmodelElement element)
        {
            return Value.CreateOrUpdate(id, element);
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

    [DataContract, JsonObject]
    public class SubmodelElementCollection<T> : SubmodelElementCollection, ICollection<T>
    {       
        [JsonConstructor]
        public SubmodelElementCollection(string idShort) : base(idShort)
        {
            AllowDuplicates = true;
            Ordered = true;
        }

        public SubmodelElementCollection(string idShort, IEnumerable<T> enumerable) : this(idShort)
        {
            if(enumerable?.Count() > 0)
            {
                foreach (var item in enumerable)
                {
                    Add(item);
                }
            }
        }

        public SubmodelElementCollection(ISubmodelElementCollection collection) : this(collection.IdShort)
        {
            AllowDuplicates = collection.AllowDuplicates;
            Ordered = collection.Ordered;
            Category = collection.Category;
            Constraints = collection.Constraints;
            EmbeddedDataSpecifications = collection.EmbeddedDataSpecifications;
            ConceptDescription = collection.ConceptDescription;
            Kind = collection.Kind;
            Parent = collection.Parent;
            Description = collection.Description;
            SemanticId = collection.SemanticId;
            Get = collection.Get;
            Set = collection.Set;

            foreach (var element in collection.Value)
            {
                T value = element.Cast<IProperty>().ToObject<T>();
                Add(value);
            }
        }



        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        public IElementContainer<ISubmodelElement> BaseValue { get => base.Value; set => base.Value = value; }

        [IgnoreDataMember]
        [JsonIgnore]
        public new IEnumerable<T> Value
        {
            get
            {
                var enumerable = base.Value.Select(s => s.Cast<IProperty>().ToObject<T>());
                return enumerable;
            }
            set
            {
                if (value?.Count() > 0)
                {
                    foreach (var item in value)
                    {
                        Add(item);
                    }
                }
            }
        }

        public new T this[int index] { 
            get => base.Value[index].Cast<IProperty>().ToObject<T>(); 
            set => base.Value[index].Cast<IProperty>().Value = value; }

        public static implicit operator List<T>(SubmodelElementCollection<T> collection)
        {
            return collection?.Value?.ToList();
        }

        public static implicit operator T[](SubmodelElementCollection<T> collection)
        {
            return collection?.Value?.ToArray();
        }

        public void Add(T item)
        {
            if (item == null)
                return;

            string idShort = (base.Value.Count + 1).ToString();
            base.Value.Add(new Property<T>(idShort, item));
        }

        public bool Contains(T item)
        {
            if (item == null)
                return false;

            for (int i = 0; i < Count; i++)
            {
                if (this[i].Equals(item))
                    return true;
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < Count; i++)
            {
                array.SetValue(this[i], arrayIndex++);
            }
        }

        public int IndexOf(T item)
        {
            if (item == null)
                return -1;

            for (int i = 0; i < Count; i++)
            {
                if (this[i].Equals(item))
                    return i;
            }
            return -1;
        }

        public bool Remove(T item)
        {
            if (item == null)
                return false;

            var index = IndexOf(item);
            if (index != -1)
            {
                RemoveAt(index);
                return true;
            }
            else
                return false;   
        }

        public void RemoveAt(int index)
        {
            if ((index >= 0) && (index < Count))
            {
                string idShort = base.Value[index]?.IdShort;
                if (!string.IsNullOrEmpty(idShort))
                    Remove(idShort);
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return Value.GetEnumerator();
        }
    }

    public class SubmodelElementCollectionOfEntity<T> : SubmodelElementCollection where T : class
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        public IElementContainer<ISubmodelElement> BaseValue { get => base.Value; set => base.Value = value; }

        [IgnoreDataMember]
        [JsonIgnore]
        public new T Value 
        { 
            get
            {
                T innerValue = base.Value.MinimizeSubmodelElements().ToObject<T>();
                return innerValue;
            } 
            set
            {
                var smc = value.CreateSubmodelElementCollectionFromObject(IdShort, BindingFlags.Public | BindingFlags.Instance);
                foreach (var element in smc.Value)
                {
                    base.Add(element);
                }
            }
        }

        [JsonConstructor]
        public SubmodelElementCollectionOfEntity(string idShort) : base(idShort)
        {
            AllowDuplicates = false;
            Ordered = false;
        }

        public SubmodelElementCollectionOfEntity(string idShort, T entity) : this(idShort, entity, BindingFlags.Public | BindingFlags.Instance)
        { }

        public SubmodelElementCollectionOfEntity(string idShort, T entity, BindingFlags bindingFlags) : base(idShort)
        {
            var smc = entity.CreateSubmodelElementCollectionFromObject(this.IdShort, bindingFlags);
            foreach (var element in smc.Value)
            {
                base.Add(element);
            }
        }

        public SubmodelElementCollectionOfEntity(ISubmodelElementCollection collection) : this(collection.IdShort)
        {
            AllowDuplicates = collection.AllowDuplicates;
            Ordered = collection.Ordered;
            Category = collection.Category;
            Constraints = collection.Constraints;
            EmbeddedDataSpecifications = collection.EmbeddedDataSpecifications;
            ConceptDescription = collection.ConceptDescription;
            Kind = collection.Kind;
            Parent = collection.Parent;
            Description = collection.Description;
            SemanticId = collection.SemanticId;
            Get = collection.Get;
            Set = collection.Set;
            BaseValue = collection.Value;
        }
    }
}
