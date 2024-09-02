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
using System.Linq;
using System.Text.Json.Serialization;

namespace BaSyx.Models.AdminShell
{
    public class ElementContainer<TElement> : IElementContainer<TElement> where TElement : IReferable, IModelElement
    {
        public const char PATH_SEPERATOR = '.';

        private readonly List<IElementContainer<TElement>> _children;

        public string IdShort { get; private set; }
        public int Index { get; set; }
        public TElement Value { get; set;  }
        public string Path { get; set; }
        public bool IsRoot => ParentContainer == null;
        [JsonIgnore]
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

        private IdShortPathResolver idShortResolver;
        public ElementContainer(IReferable parent, TElement rootElement, IElementContainer<TElement> parentContainer) : this()
        {
            Parent = parent;
            ParentContainer = parentContainer;

            if (!string.IsNullOrEmpty(rootElement?.IdShort))
            {
                IdShort = rootElement?.IdShort;
                this.Path = IdShort;
            }
            
            Value = rootElement;

            SetPath();
        }

        /// <summary>
        /// Set the initial Path of the current ElementContainer.
        /// If it is a ListChild, the Path will be the index of the child in its ParentContainer in brackets.
        /// This is used to append the Path for nested elements more easily and to include SubmodelElementList children accordingly.
        /// </summary>
        private void SetPath()
        {
            if (IsListChild())
            {
                Index = ParentContainer.Children.Count();
                Path = "[" + Index + "]";
            }
            else if (ParentContainer != null && !string.IsNullOrEmpty(ParentContainer.Path))
                Path = ParentContainer.Path + PATH_SEPERATOR + this.Path;
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
            set
            {
                if (i < this.Count())
                {
                    _children[i] = new ElementContainer<TElement>(Parent, value, null);
					OnUpdated?.Invoke(this, new ElementContainerEventArgs<TElement>(this, value, ChangedEventType.Updated));
				}
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
            set
            {
				if (HasChild(idShortPath))
				{
					int index = _children.FindIndex(c => c.IdShort == idShortPath);
					if (index != -1)
					{
						if (value is IElementContainer<TElement> containerElement)
							_children[index] = containerElement;
						else
							_children[index] = new ElementContainer<TElement>(Parent, value, this);

						OnUpdated?.Invoke(this, new ElementContainerEventArgs<TElement>(this, value, ChangedEventType.Updated));
					}
				}
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


        public void AppendRootPath(string rootPath, bool rootIsList)
        {
            if (string.IsNullOrEmpty(this.Path))
            {
                Index = ParentContainer.Children.Count();
                Path = "[" + Index + "]";
            }

            if (!string.IsNullOrEmpty(rootPath))
            {
                if (rootIsList)
                    this.Path = rootPath + this.Path;
                else
                    this.Path = rootPath + PATH_SEPERATOR + this.Path;
            }

            foreach (var child in _children)
            {
                if (!string.IsNullOrEmpty(rootPath))
                {
                    if (rootIsList && this.Value?.ModelType == ModelType.SubmodelElementList)
                        child.AppendRootPath(this.Path, true);
                    else if (rootIsList && this.Value?.ModelType != ModelType.SubmodelElementList)
                        child.AppendRootPath(this.Path, false);
                    else
                        child.AppendRootPath(rootPath, false);
                }
            }
        }

        /// <summary>
        /// Check if the current element is a child of a SubmodelElementList by its ParentContainer. 
        /// </summary>
        /// <returns>True if the element has a ParentContainer of Type SubmodelElementList</returns>
        private bool IsListChild()
        {
            if (ParentContainer != null && ParentContainer.Value?.ModelType == ModelType.SubmodelElementList)
                return true;
            else
                return false;
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
                if (idShortPath.Contains(PATH_SEPERATOR) || idShortPath.Contains('['))
                {
                    idShortResolver = new IdShortPathResolver((IElementContainer<ISubmodelElement>)this);
                    superChild = (IElementContainer<TElement>)idShortResolver.GetChild(idShortPath);
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

        public virtual IResult<IElementContainer<TElement>> RetrieveAll()
        {
            if (this.Count() == 0)
                return new Result<IElementContainer<TElement>>(true, new ElementContainer<TElement>(), new EmptyMessage());
            else
                return new Result<IElementContainer<TElement>>(true, this);
        }

        public virtual IResult<IElementContainer<T>> RetrieveAll<T>() where T : class, IReferable, IModelElement
        {
            if (this.Count() == 0)
                return new Result<IElementContainer<T>>(true, new EmptyMessage());
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
                    return new Result<IElementContainer<T>>(true, container);
                else
                    return new Result<IElementContainer<T>>(true, new EmptyMessage());
            }
        }

        public IResult<IElementContainer<TElement>> RetrieveAll(Predicate<TElement> predicate)
        {
            if (this.Count() == 0)
                return new Result<IElementContainer<TElement>>(true, new EmptyMessage());
            else
            {
                ElementContainer<TElement> container = new ElementContainer<TElement>();
                var elements = Values.ToList().FindAll(predicate);
                if(elements?.Count() > 0)
                    container.AddRange(elements);

                if (container.Count() > 0)
                    return new Result<IElementContainer<TElement>>(true, container);
                else
                    return new Result<IElementContainer<TElement>>(true, new EmptyMessage());
            }
        }

        public virtual IResult<IElementContainer<T>> RetrieveAll<T>(Predicate<T> predicate) where T : class, IReferable, IModelElement
        {
            if (this.Count() == 0)
                return new Result<IElementContainer<T>>(true, new EmptyMessage());
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
                    return new Result<IElementContainer<T>>(true, container);
                else
                    return new Result<IElementContainer<T>>(true, new EmptyMessage());
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
            string idShortOrIndex = GetIdShortOrIndex(element);

            if (this[idShortOrIndex] == null)
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

            string idShortOrIndex = GetIdShortOrIndex(element);

            if (this[idShortOrIndex] == null)
            {
                element.Parent = this.Parent;
                bool isListParent = this.Value?.ModelType == ModelType.SubmodelElementList;
                IElementContainer<TElement> node;
                if (element is IElementContainer<TElement> subElements)
                {
                    subElements.Parent = this.Parent;
                    subElements.ParentContainer = this;

                    if (isListParent) 
                        Console.WriteLine("");
                    subElements.AppendRootPath(this.Path, isListParent);

                    node = subElements;
                    // set index of nested SubmodelElements of type IElementContainer
                    node.Index = _children.Count;
                }
                else
                {
                    node = new ElementContainer<TElement>(Parent, element, this);
                    if (isListParent)
                        node.AppendRootPath(this.Path, true);
                }

                this._children.Add(node);
                OnCreated?.Invoke(this, new ElementContainerEventArgs<TElement>(this, element, ChangedEventType.Created));
            } 
        }

        /// <summary>
        /// Get the IdShort of the element or the future index if the element will be a ListChild.
        /// </summary>
        /// <param name="element"></param>
        /// <returns>Value that will be used to add/ create a new element as child.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        private string GetIdShortOrIndex(TElement element)
        {
            string idShortOrIndex = "";
            if (string.IsNullOrEmpty(element.IdShort))
            {
                if (this.Value.ModelType == ModelType.SubmodelElementList)
                {
                    int newIndex = _children.Count; // index starts with 0!
                    idShortOrIndex = newIndex.ToString();
                }
                else
                    throw new ArgumentNullException(nameof(element.IdShort));
            }
            else
                idShortOrIndex = element.IdShort;

            return idShortOrIndex;
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
                if (HasChild(idShortPath))
                {
                    int index = _children.FindIndex(c => c.IdShort == idShortPath);
                    if (index != -1)
                    {
                        if(element is IElementContainer<TElement> containerElement)
                            _children[index] = containerElement;
                        else
							_children[index] = new ElementContainer<TElement>(Parent, element, this);

						OnUpdated?.Invoke(this, new ElementContainerEventArgs<TElement>(this, element, ChangedEventType.Updated));
                        return new Result<TElement>(true, element);
                    }
                }
                var child = GetChild(idShortPath);
                if (child != null)
                {
                    return child.ParentContainer.Update(child.IdShort, element);
                }
                return new Result<TElement>(false, new NotFoundMessage($"Element {idShortPath} not found"));
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
