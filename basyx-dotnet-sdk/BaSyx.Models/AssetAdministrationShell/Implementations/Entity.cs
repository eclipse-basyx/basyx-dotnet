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
using BaSyx.Utils.ResultHandling;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace BaSyx.Models.AdminShell
{
    [DataContract]
    public class Entity : SubmodelElement<EntityValue>, IEntity, IElementContainer<ISubmodelElement>
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "modelType")]
        public override ModelType ModelType => ModelType.Entity;

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "entityType")]
        public EntityType EntityType { get; set; }

        public Entity(string idShort) : base(idShort)
        {
            Value = new EntityValue(null, new List<SpecificAssetId>(), new ElementContainer<ISubmodelElement>(this.Parent, this, null));
        }

        #region Implementation of IElementContainer Interface 

        [IgnoreDataMember, JsonIgnore]
        public IEnumerable<IElementContainer<ISubmodelElement>> Children => Value.Statements.Children;
        [IgnoreDataMember, JsonIgnore]
        public IEnumerable<ISubmodelElement> Values => Value.Statements.Values;
        [IgnoreDataMember, JsonIgnore]
        ISubmodelElement IElementContainer<ISubmodelElement>.Value { get => this; }
        public int Index { get; set; }
        [IgnoreDataMember, JsonIgnore]
        public bool IsRoot => Value.Statements.IsRoot;
        [IgnoreDataMember, JsonIgnore]
        public IElementContainer<ISubmodelElement> ParentContainer { get => Value.Statements.ParentContainer; set { Value.Statements.ParentContainer = value; } }
        [IgnoreDataMember, JsonIgnore]
        public int Count => Value.Statements.Count;
        [IgnoreDataMember, JsonIgnore]
        public bool IsReadOnly => Value.Statements.IsReadOnly;

        [IgnoreDataMember, JsonIgnore]
        public ISubmodelElement this[string idShort] { get => Value.Statements[idShort]; set => Value.Statements[idShort] = value; }
        [IgnoreDataMember, JsonIgnore]
        public ISubmodelElement this[int i] { get => Value.Statements[i]; set => Value.Statements[i] = value; }

        [IgnoreDataMember, JsonIgnore]
        public string Path
        {
            get
            {
                if (string.IsNullOrEmpty(Value.Statements.Path))
                    return IdShort;
                else
                    return Value.Statements.Path;
            }
            set { Value.Statements.Path = value; }
        }

        public event EventHandler<ElementContainerEventArgs<ISubmodelElement>> OnCreated
        {
            add
            {
                Value.Statements.OnCreated += value;
            }

            remove
            {
                Value.Statements.OnCreated -= value;
            }
        }

        public event EventHandler<ElementContainerEventArgs<ISubmodelElement>> OnUpdated
        {
            add
            {
                Value.Statements.OnUpdated += value;
            }

            remove
            {
                Value.Statements.OnUpdated -= value;
            }
        }

        public event EventHandler<ElementContainerEventArgs<ISubmodelElement>> OnDeleted
        {
            add
            {
                Value.Statements.OnDeleted += value;
            }

            remove
            {
                Value.Statements.OnDeleted -= value;
            }
        }

        public IResult<IElementContainer<ISubmodelElement>> RetrieveAll()
        {
            return Value.Statements.RetrieveAll();
        }

        public IResult<IElementContainer<ISubmodelElement>> RetrieveAll(Predicate<ISubmodelElement> predicate)
        {
            return Value.Statements.RetrieveAll(predicate);
        }

        public bool HasChildren()
        {
            return Value.Statements.HasChildren();
        }

        public bool HasChild(string idShort)
        {
            return Value.Statements.HasChild(idShort);
        }

        public bool HasChildPath(string idShortPath)
        {
            return Value.Statements.HasChildPath(idShortPath);
        }

        public void Traverse(Action<ISubmodelElement> action)
        {
            Value.Statements.Traverse(action);
        }

        public void Add(ISubmodelElement element)
        {
            Value.Statements.Add(element);
        }

        public void AddRange(IEnumerable<ISubmodelElement> elements)
        {
            Value.Statements.AddRange(elements);
        }

        public IResult<ISubmodelElement> Create(ISubmodelElement element)
        {
            return Value.Statements.Create(element);
        }

        public IResult<ISubmodelElement> Retrieve(string id)
        {
            return Value.Statements.Retrieve(id);
        }

        IResult<T> ICrudContainer<string, ISubmodelElement>.Retrieve<T>(string id)
        {
            return Value.Statements.Retrieve<T>(id);
        }

        IResult<IElementContainer<T>> ICrudContainer<string, ISubmodelElement>.RetrieveAll<T>()
        {
            return Value.Statements.RetrieveAll<T>();
        }

        IResult<IElementContainer<T>> ICrudContainer<string, ISubmodelElement>.RetrieveAll<T>(Predicate<T> predicate)
        {
            return Value.Statements.RetrieveAll(predicate);
        }

        public IResult<ISubmodelElement> CreateOrUpdate(string id, ISubmodelElement element)
        {
            return Value.Statements.CreateOrUpdate(id, element);
        }

        public IResult<ISubmodelElement> Create(string id, ISubmodelElement element)
        {
            return Value.Statements.Create(id, element);
        }

        public IResult<ISubmodelElement> Update(string id, ISubmodelElement element)
        {
            return Value.Statements.Update(id, element);
        }

        public IResult Delete(string id)
        {
            return Value.Statements.Delete(id);
        }

        public IEnumerator<ISubmodelElement> GetEnumerator()
        {
            return Value.Statements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Value.Statements.GetEnumerator();
        }

        public IElementContainer<ISubmodelElement> GetChild(string idShortPath)
        {
            return Value.Statements.GetChild(idShortPath);
        }

        public void Remove(string idShort)
        {
            Value.Statements.Remove(idShort);
        }

        public void AppendRootPath(string rootPath, bool rootIsList)
        {
            Value.Statements.AppendRootPath(rootPath, rootIsList);
        }

        public IEnumerable<ISubmodelElement> Flatten()
        {
            return Value.Statements.Flatten();
        }

        public void Clear()
        {
            Value.Statements.Clear();
        }

        public bool Contains(ISubmodelElement item)
        {
            return Value.Statements.Contains(item);
        }

        public void CopyTo(ISubmodelElement[] array, int arrayIndex)
        {
            Value.Statements.CopyTo(array, arrayIndex);
        }

        public bool Remove(ISubmodelElement item)
        {
            return Value.Statements.Remove(item);
        }

        #endregion
    }
}
