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
using BaSyx.Models.Core.Common;
using BaSyx.Utils.ResultHandling;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Implementations
{
    [DataContract, JsonObject]
    public class SubmodelElementCollection : SubmodelElement, ISubmodelElementCollection, IElementContainer<ISubmodelElement>
    {
        public override ModelType ModelType => ModelType.SubmodelElementCollection;
        public IElementContainer<ISubmodelElement> Value { get; set; }
        public bool AllowDuplicates { get; set; } = false;
        public bool Ordered { get; set; } = false;

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

        public int Count => Value.Count;

        public bool IsReadOnly => Value.IsReadOnly;

        [IgnoreDataMember, JsonIgnore]
        public ISubmodelElement this[string idShort] => Value[idShort];
        [IgnoreDataMember, JsonIgnore]
        public ISubmodelElement this[int i] => Value[i];

        public SubmodelElementCollection(string idShort) : base(idShort) 
        {
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
}
