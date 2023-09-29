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
using BaSyx.Models.Extensions;
using BaSyx.Utils.ResultHandling;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace BaSyx.Models.AdminShell
{
    [DataContract, JsonObject]
    public class SubmodelElementList : SubmodelElement, ISubmodelElementList, IElementContainer<ISubmodelElement>
    {
        public override ModelType ModelType => ModelType.SubmodelElementList;
        public IElementContainer<ISubmodelElement> Value { get; set; }
        public bool OrderRelevant { get; set; }
        public IReference SemanticIdListElement { get; set; }
        public ModelType TypeValueListElement { get; set; }
        public DataType ValueTypeListElement { get; set; }


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

        public SubmodelElementList(string idShort) : base(idShort) 
        {
            Value = new ElementContainer<ISubmodelElement>(this.Parent, this, null);

            Get = element => { return new ElementValue(Value, typeof(ElementContainer<ISubmodelElement>)); };
            Set = (element, value) => { Value = (IElementContainer<ISubmodelElement>)value.Value; return Task.CompletedTask; };
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
    /*
    [DataContract, JsonObject]
    public class SubmodelElementList<T> : SubmodelElementList, ICollection<T>
    {       
        [JsonConstructor]
        public SubmodelElementList(string idShort) : base(idShort)
        {
        }

        public SubmodelElementList(string idShort, IEnumerable<T> enumerable) : this(idShort)
        {
            if(enumerable?.Count() > 0)
            {
                foreach (var item in enumerable)
                {
                    Add(item);
                }
            }
        }

        public SubmodelElementList(SubmodelElementList collection) : this(collection.IdShort)
        {
            Category = collection.Category;
            Qualifiers = collection.Qualifiers;
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
                Add(element);
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
                var enumerable = base.Value;
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

        public static implicit operator List<T>(SubmodelElementList<T> collection)
        {
            return collection?.Value?.ToList();
        }

        public static implicit operator T[](SubmodelElementList<T> collection)
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
            Category = collection.Category;
            Qualifiers = collection.Qualifiers;
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
    */
}
