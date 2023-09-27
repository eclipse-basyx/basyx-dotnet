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
using BaSyx.Models.AdminShell;
using BaSyx.Models.Extensions;
using BaSyx.Utils.ResultHandling;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BaSyx.Models.AdminShell
{
    public class ElementContainer<TElement> : IElementContainer<TElement> where TElement : IReferable, IModelElement
    {
        public const char PATH_SEPERATOR = '.';

        private readonly List<IElementContainer<TElement>> _children;

        public string IdShort { get; private set; }
        public TElement Value { get; set;  }
        public string Path { get; set; }
        public bool IsRoot => ParentContainer == null;
        public IReferable Parent { get; set; }
        public IElementContainer<TElement> ParentContainer { get; set; }

        public event EventHandler<ElementContainerEventArgs<TElement>> OnCreated;
        public event EventHandler<ElementContainerEventArgs<TElement>> OnUpdated;
        public event EventHandler<ElementContainerEventArgs<TElement>> OnDeleted;

        [JsonConstructor]
        public ElementContainer()
        {
            _children = new List<IElementContainer<TElement>>();
        }

        public ElementContainer(IReferable parent) : this(parent, default, null)
        { }

        public ElementContainer(IReferable parent, IEnumerable<TElement> list) : this()
        {
            Parent = parent;
            Value = default;
            IdShort = null;

            AddRange(list);
        }

        public ElementContainer(IReferable parent, TElement rootElement, IElementContainer<TElement> parentContainer) : this()
        {
            Parent = parent;
            ParentContainer = parentContainer;

            IdShort = rootElement?.IdShort;
            Value = rootElement;

            if (ParentContainer != null && !string.IsNullOrEmpty(ParentContainer.Path))
                Path = ParentContainer.Path + PATH_SEPERATOR + IdShort;
            else
                Path = IdShort;
        }

       
        public TElement this[int i]
        {
            get
            {
                if (i < this.Count())
                    return _children[i].Value;
                else
                    return default;
            }
        }

        public TElement this[string idShortPath]
        {
            get
            {
                var child = GetChild(idShortPath);
                if (child != null && child.Value != null)
                    return child.Value;
                else 
                    return default;
            }
        }

        public IEnumerable<TElement> Values
        {
            get => _children.Select(s => s.Value);
        }

        public IEnumerable<IElementContainer<TElement>> Children
        {
            get => _children;
        }

        public int Count => _children.Count;

        public bool IsReadOnly => false;

        public void AppendRootPath(string rootPath)
        {
            if (!string.IsNullOrEmpty(rootPath))
                this.Path = rootPath + PATH_SEPERATOR + this.Path;

            foreach (var child in _children)
            {
                if (child.HasChildren())
                {
                    foreach (var subChild in child.Children)
                    {
                        subChild.AppendRootPath(rootPath);
                    }
                }
                if (!string.IsNullOrEmpty(rootPath))
                    child.Path = rootPath + PATH_SEPERATOR + child.Path;
            }
        }

        public bool HasChildren()
        {
            if (_children == null)
                return false;
            else
            {
                if (_children.Count == 0)
                    return false;
                else
                    return true;
            }
        }

        public bool HasChild(string idShort)
        {
            if (_children == null || _children.Count == 0)
                return false;
            else
            {
                var child = _children.FirstOrDefault(c => c.IdShort == idShort);
                if (child == null)
                    return false;
                else
                    return true;
            }
        }

        public bool HasChildPath(string idShortPath)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return false;

            if (_children == null || _children.Count == 0)
                return false;
            else
            {
                if (idShortPath.Contains(PATH_SEPERATOR))
                {
                    string[] splittedPath = idShortPath.Split(new char[] { PATH_SEPERATOR }, StringSplitOptions.RemoveEmptyEntries);
                    if (!HasChild(splittedPath[0]))
                        return false;
                    else
                    {
                        var child = GetChild(splittedPath[0]);
                        return (child.HasChildPath(string.Join(new string(new char[] { PATH_SEPERATOR }), splittedPath.Skip(1))));
                    }
                }
                else
                    return HasChild(idShortPath);
            }
        }

        public IElementContainer<TElement> GetChild(string idShortPath)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return null;

            if (_children == null || _children.Count == 0)
                return null;
            else
            {
                IElementContainer<TElement> superChild;
                if (idShortPath.Contains(PATH_SEPERATOR))
                {
                    string[] splittedPath = idShortPath.Split(new char[] { PATH_SEPERATOR }, StringSplitOptions.RemoveEmptyEntries);
                    if (HasChild(splittedPath[0]))                      
                    {
                        var child = GetChild(splittedPath[0]);
                        superChild = child.GetChild(string.Join(new string(new char[] { PATH_SEPERATOR }), splittedPath.Skip(1)));
                    }
                    else
                        superChild = null;
                }
                else
                    superChild = _children.FirstOrDefault(c => c.IdShort == idShortPath);

                return superChild;
            }
        }

        public IEnumerable<TElement> Flatten()
        {
            if (Value != null)
                return new[] { Value }.Concat(_children.SelectMany(c => c.Flatten()));
            else
                return _children.SelectMany(c => c.Flatten());
        }

        public void Traverse(Action<TElement> action)
        {
            action(Value);
            foreach (var child in _children)
                child.Traverse(action);
        }

        public virtual IResult<IQueryableElementContainer<TElement>> RetrieveAll()
        {
            if (this.Count() == 0)
                return new Result<IQueryableElementContainer<TElement>>(true, new ElementContainer<TElement>().AsQueryableElementContainer(), new EmptyMessage());
            else
                return new Result<IQueryableElementContainer<TElement>>(true, this.AsQueryableElementContainer());
        }

        public virtual IResult<IQueryableElementContainer<T>> RetrieveAll<T>() where T : class, IReferable, IModelElement
        {
            if (this.Count() == 0)
                return new Result<IQueryableElementContainer<T>>(true, new EmptyMessage());
            else
            {
                ElementContainer<T> container = new ElementContainer<T>();
                foreach (var element in this)
                {
                    T tElement = element.Cast<T>();
                    if (tElement != null)
                        container.Add(tElement);
                }

                if(container.Count() > 0)
                    return new Result<IQueryableElementContainer<T>>(true, container.AsQueryableElementContainer());
                else
                    return new Result<IQueryableElementContainer<T>>(true, new EmptyMessage());
            }
        }

        public IResult<IQueryableElementContainer<TElement>> RetrieveAll(Predicate<TElement> predicate)
        {
            if (this.Count() == 0)
                return new Result<IQueryableElementContainer<TElement>>(true, new EmptyMessage());
            else
            {
                ElementContainer<TElement> container = new ElementContainer<TElement>();
                var elements = Values.ToList().FindAll(predicate);
                if(elements?.Count() > 0)
                    container.AddRange(elements);

                if (container.Count() > 0)
                    return new Result<IQueryableElementContainer<TElement>>(true, container.AsQueryableElementContainer());
                else
                    return new Result<IQueryableElementContainer<TElement>>(true, new EmptyMessage());
            }
        }

        public virtual IResult<IQueryableElementContainer<T>> RetrieveAll<T>(Predicate<T> predicate) where T : class, IReferable, IModelElement
        {
            if (this.Count() == 0)
                return new Result<IQueryableElementContainer<T>>(true, new EmptyMessage());
            else
            {
                ElementContainer<T> container = new ElementContainer<T>();
                foreach (var element in this)
                {
                    T tElement = element.Cast<T>();
                    if (tElement != null)
                        container.Add(tElement);
                }

                if (container.Count() > 0)
                    return new Result<IQueryableElementContainer<T>>(true, container.AsQueryableElementContainer());
                else
                    return new Result<IQueryableElementContainer<T>>(true, new EmptyMessage());
            }
        }

        public virtual IResult<TElement> Retrieve(string idShortPath)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return new Result<TElement>(new ArgumentNullException(nameof(idShortPath)));

            var child = GetChild(idShortPath);
            if (child != null)
                return new Result<TElement>(true, child.Value);
            else
                return new Result<TElement>(false, new NotFoundMessage());
        }
        public IResult<T> Retrieve<T>(string idShortPath) where T : class, TElement
        {
            if (string.IsNullOrEmpty(idShortPath))
                return new Result<T>(new ArgumentNullException(nameof(idShortPath)));

            T element = GetChild(idShortPath)?.Value?.Cast<T>();
            if (element != null)
                return new Result<T>(true, element);
            else
                return new Result<T>(false, new NotFoundMessage());
        }

        public IResult<TElement> Create(TElement element)
        {
            if (element == null)
                return new Result<TElement>(new ArgumentNullException(nameof(element)));
            if (string.IsNullOrEmpty(element.IdShort))
                return new Result<TElement>(new ArgumentNullException(nameof(element.IdShort)));

            if (this[element.IdShort] == null)
            {
                Add(element);
                return new Result<TElement>(true, element);
            }
            else
                return new Result<TElement>(false, new ConflictMessage(element.IdShort));
        }

        public IResult<T> Create<T>(T element) where T : class, TElement
        {
            if (element == null)
                return new Result<T>(new ArgumentNullException(nameof(element)));
            if (string.IsNullOrEmpty(element.IdShort))
                return new Result<T>(new ArgumentNullException(nameof(element.IdShort)));

            if (this[element.IdShort] == null)
            {
                Add(element);
                return new Result<T>(true, element);
            }
            else
                return new Result<T>(false, new ConflictMessage(element.IdShort));
        }


        public virtual IResult<TElement> Create(string idShortPath, TElement element)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return new Result<TElement>(new ArgumentNullException(nameof(idShortPath)));
            if (element == null)
                return new Result<TElement>(new ArgumentNullException(nameof(element)));

            if (idShortPath.Length == 1 && idShortPath[0] == PATH_SEPERATOR)
                return this.Create(element);
            else
            {
                var child = GetChild(idShortPath);
                if (child != null)
                    return child.Create(element);
                else
                    return new Result<TElement>(false, new NotFoundMessage($"Parent element {idShortPath} not found"));
            }
        }

        public void Add(TElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (string.IsNullOrEmpty(element.IdShort))
                throw new ArgumentNullException(nameof(element.IdShort));

            if (this[element.IdShort] == null)
            {
                element.Parent = this.Parent;

                IElementContainer<TElement> node;
                if (element is IElementContainer<TElement> subElements)
                {
                    subElements.Parent = this.Parent;
                    subElements.ParentContainer = this;
                    subElements.AppendRootPath(this.Path);
                    node = subElements;
                }
                else
                    node = new ElementContainer<TElement>(Parent, element, this);

                this._children.Add(node);
                OnCreated?.Invoke(this, new ElementContainerEventArgs<TElement>(this, element, ChangedEventType.Created));
            }
        }

        public virtual IResult<TElement> CreateOrUpdate(string idShortPath, TElement element)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return new Result<TElement>(new ArgumentNullException(nameof(idShortPath)));
            if (element == null)
                return new Result<TElement>(new ArgumentNullException(nameof(element)));

            if(idShortPath.Length == 1 && idShortPath[0] == PATH_SEPERATOR)
            {
                if (HasChild(IdShort))
                    return this.Update(idShortPath, element);
                else
                    return this.Create(element);
            }
            else if (!idShortPath.Contains(PATH_SEPERATOR) && idShortPath != element.IdShort)
            {
                int childIndex = _children.FindIndex(p => p.IdShort == idShortPath);
                return _children[childIndex].Create(element);
            }
            else if (HasChild(idShortPath))
            {
                int childIndex = _children.FindIndex(p => p.IdShort == idShortPath);
                if(element is IElementContainer<TElement> container)
                    _children[childIndex] = container;
                else
                    _children[childIndex] = new ElementContainer<TElement>(Parent, element, this);
                OnUpdated?.Invoke(this, new ElementContainerEventArgs<TElement>(this, element, ChangedEventType.Updated));
                return new Result<TElement>(true, element);
            }
            else if (idShortPath.Contains(PATH_SEPERATOR))
            {
                string lastElement = idShortPath.Substring(idShortPath.LastIndexOf(PATH_SEPERATOR), idShortPath.Length - idShortPath.LastIndexOf(PATH_SEPERATOR));
                var child = GetChild(idShortPath);
                if (child != null)
                {
                    if (lastElement == element.IdShort)
                        return child.Update(element.IdShort, element);
                    else
                        return child.Create(element);
                }
                else
                    return new Result<TElement>(false, new NotFoundMessage($"Parent element {idShortPath} not found"));
            }
            else
                return this.Create(element);
        }


        public virtual IResult<TElement> Update(string idShortPath, TElement element)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return new Result<TElement>(new ArgumentNullException(nameof(idShortPath)));
            if (element == null)
                return new Result<TElement>(new ArgumentNullException(nameof(element)));

            if (idShortPath.Length == 1 && idShortPath[0] == PATH_SEPERATOR)
                return this.Create(element);
            else
            {
                var child = GetChild(idShortPath);
                if (child != null)
                {
                    child.Value = element;
                    OnUpdated?.Invoke(this, new ElementContainerEventArgs<TElement>(child.ParentContainer, element, ChangedEventType.Updated));
                    return new Result<TElement>(true, element);
                }
                return new Result<TElement>(false, new NotFoundMessage());
            }
        }

        public virtual IResult Delete(string idShortPath)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return new Result<TElement>(new ArgumentNullException(nameof(idShortPath)));

            var child = GetChild(idShortPath);
            if (child != null)
            {               
                child.ParentContainer.Remove(child.IdShort);                
                return new Result(true);
            }
            return new Result(false, new NotFoundMessage());
        }

        public void Remove(string idShort)
        {
            _children.RemoveAll(c => c.IdShort == idShort);
            OnDeleted?.Invoke(this, new ElementContainerEventArgs<TElement>(this, default, ChangedEventType.Deleted) { ElementIdShort = idShort });
        }

        public void AddRange(IEnumerable<TElement> collection)
        {
            foreach (var item in collection)
            {
                Add(item);
            }
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        public void Clear()
        {
            _children.Clear();
        }

        public bool Contains(TElement item)
        {
            return this[item.IdShort] != null;
        }

        public void CopyTo(TElement[] array, int arrayIndex)
        {
            for (int i = arrayIndex; i < array.Length && i < _children.Count; i++)
            {
                array[i] = _children[i].Value;
            }
        }

        public bool Remove(TElement item)
        {
            var removed = _children.RemoveAll(c => c.IdShort == item.IdShort);
            if (removed > 0)
            {
                OnDeleted?.Invoke(this, new ElementContainerEventArgs<TElement>(this, default, ChangedEventType.Deleted) { ElementIdShort = item.IdShort });
                return true;
            }
            else
                return false;
        }
    }

   
}
