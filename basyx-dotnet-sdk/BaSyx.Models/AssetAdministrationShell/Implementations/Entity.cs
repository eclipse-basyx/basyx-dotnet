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
    public class Entity : SubmodelElement, IEntity, IElementContainer<ISubmodelElement>
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "modelType")]
        public override ModelType ModelType => ModelType.Entity;

        private readonly IElementContainer<ISubmodelElement> _statements;

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "statements")]
        public IElementContainer<ISubmodelElement> Statements { get => _statements; set => _statements.AddRange(value); }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "entityType")]
        public EntityType EntityType { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "globalAssetId")]
        public Identifier GlobalAssetId { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "specificAssetIds")]
        public IEnumerable<SpecificAssetId> SpecificAssetIds { get; set; }

        public Entity(string idShort) : base(idShort)
        {
            SpecificAssetIds = new List<SpecificAssetId>();
            _statements = new ElementContainer<ISubmodelElement>(this.Parent, this, null);
        }

        #region Implementation of IElementContainer Interface 

        [IgnoreDataMember, JsonIgnore]
        public IEnumerable<IElementContainer<ISubmodelElement>> Children => Statements.Children;
        [IgnoreDataMember, JsonIgnore]
        public IEnumerable<ISubmodelElement> Values => Statements.Values;
        [IgnoreDataMember, JsonIgnore]
        ISubmodelElement IElementContainer<ISubmodelElement>.Value { get => this; }
        [IgnoreDataMember, JsonIgnore]
        public bool IsRoot => Statements.IsRoot;
        [IgnoreDataMember, JsonIgnore]
        public IElementContainer<ISubmodelElement> ParentContainer { get => Statements.ParentContainer; set { Statements.ParentContainer = value; } }
        [IgnoreDataMember, JsonIgnore]
        public int Count => Statements.Count;
        [IgnoreDataMember, JsonIgnore]
        public bool IsReadOnly => Statements.IsReadOnly;

        [IgnoreDataMember, JsonIgnore]
        public ISubmodelElement this[string idShort] { get => Statements[idShort]; set => Statements[idShort] = value; }
        [IgnoreDataMember, JsonIgnore]
        public ISubmodelElement this[int i] { get => Statements[i]; set => Statements[i] = value; }

        [IgnoreDataMember, JsonIgnore]
        public string Path
        {
            get
            {
                if (string.IsNullOrEmpty(Statements.Path))
                    return IdShort;
                else
                    return Statements.Path;
            }
            set { Statements.Path = value; }
        }

        public event EventHandler<ElementContainerEventArgs<ISubmodelElement>> OnCreated
        {
            add
            {
                Statements.OnCreated += value;
            }

            remove
            {
                Statements.OnCreated -= value;
            }
        }

        public event EventHandler<ElementContainerEventArgs<ISubmodelElement>> OnUpdated
        {
            add
            {
                Statements.OnUpdated += value;
            }

            remove
            {
                Statements.OnUpdated -= value;
            }
        }

        public event EventHandler<ElementContainerEventArgs<ISubmodelElement>> OnDeleted
        {
            add
            {
                Statements.OnDeleted += value;
            }

            remove
            {
                Statements.OnDeleted -= value;
            }
        }

        public IResult<IElementContainer<ISubmodelElement>> RetrieveAll()
        {
            return Statements.RetrieveAll();
        }

        public IResult<IElementContainer<ISubmodelElement>> RetrieveAll(Predicate<ISubmodelElement> predicate)
        {
            return Statements.RetrieveAll(predicate);
        }

        public bool HasChildren()
        {
            return Statements.HasChildren();
        }

        public bool HasChild(string idShort)
        {
            return Statements.HasChild(idShort);
        }

        public bool HasChildPath(string idShortPath)
        {
            return Statements.HasChildPath(idShortPath);
        }

        public void Traverse(Action<ISubmodelElement> action)
        {
            Statements.Traverse(action);
        }

        public void Add(ISubmodelElement element)
        {
            Statements.Add(element);
        }

        public void AddRange(IEnumerable<ISubmodelElement> elements)
        {
            Statements.AddRange(elements);
        }

        public IResult<ISubmodelElement> Create(ISubmodelElement element)
        {
            return Statements.Create(element);
        }

        public IResult<ISubmodelElement> Retrieve(string id)
        {
            return Statements.Retrieve(id);
        }

        IResult<T> ICrudContainer<string, ISubmodelElement>.Retrieve<T>(string id)
        {
            return Statements.Retrieve<T>(id);
        }

        IResult<IElementContainer<T>> ICrudContainer<string, ISubmodelElement>.RetrieveAll<T>()
        {
            return Statements.RetrieveAll<T>();
        }

        IResult<IElementContainer<T>> ICrudContainer<string, ISubmodelElement>.RetrieveAll<T>(Predicate<T> predicate)
        {
            return Statements.RetrieveAll(predicate);
        }

        public IResult<ISubmodelElement> CreateOrUpdate(string id, ISubmodelElement element)
        {
            return Statements.CreateOrUpdate(id, element);
        }

        public IResult<ISubmodelElement> Create(string id, ISubmodelElement element)
        {
            return Statements.Create(id, element);
        }

        public IResult<ISubmodelElement> Update(string id, ISubmodelElement element)
        {
            return Statements.Update(id, element);
        }

        public IResult Delete(string id)
        {
            return Statements.Delete(id);
        }

        public IEnumerator<ISubmodelElement> GetEnumerator()
        {
            return Statements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Statements.GetEnumerator();
        }

        public IElementContainer<ISubmodelElement> GetChild(string idShortPath)
        {
            return Statements.GetChild(idShortPath);
        }

        public void Remove(string idShort)
        {
            Statements.Remove(idShort);
        }

        public void AppendRootPath(string rootPath)
        {
            Statements.AppendRootPath(rootPath);
        }

        public IEnumerable<ISubmodelElement> Flatten()
        {
            return Statements.Flatten();
        }

        public void Clear()
        {
            Statements.Clear();
        }

        public bool Contains(ISubmodelElement item)
        {
            return Statements.Contains(item);
        }

        public void CopyTo(ISubmodelElement[] array, int arrayIndex)
        {
            Statements.CopyTo(array, arrayIndex);
        }

        public bool Remove(ISubmodelElement item)
        {
            return Statements.Remove(item);
        }

        #endregion
    }
}
