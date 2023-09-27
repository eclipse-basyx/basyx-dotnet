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
using BaSyx.Utils.ResultHandling;
using System;
using System.Collections.Generic;

namespace BaSyx.Models.AdminShell
{
    public interface ICrudContainer<TIdentifier, TElement> : ICollection<TElement> where TElement : IReferable, IModelElement
    {
        /// <summary>
        /// Retrieves an element from the container.
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <returns></returns>
        IResult<TElement> Retrieve(TIdentifier idShortPath);
        /// <summary>
        /// Retrieves an element from the container and performs a cast.
        /// </summary>
        /// <typeparam name="T">The type to cast to</typeparam>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <returns></returns>
        IResult<T> Retrieve<T>(TIdentifier idShortPath) where T : class, TElement;

        IResult<IQueryableElementContainer<T>> RetrieveAll<T>() where T : class, IReferable, IModelElement;

        IResult<IQueryableElementContainer<T>> RetrieveAll<T>(Predicate<T> predicate) where T : class, IReferable, IModelElement;
        /// <summary>
        /// Creates a new or updates an existing element in the container. No conflict detection here.
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="element">The element to create or update</param>
        /// <returns></returns>
        IResult<TElement> CreateOrUpdate(TIdentifier idShortPath, TElement element);
        /// <summary>
        /// Creates a new element in the container. Detects already existing elements.
        /// </summary>
        /// <param name="element">The element to create</param>
        /// <returns></returns>
        IResult<TElement> Create(TElement element);
        /// <summary>
        /// Creates a new element in the container. Detects already existing elements.
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="element">The element to create</param>
        /// <returns></returns>
        IResult<TElement> Create(TIdentifier idShortPath, TElement element);
        /// <summary>
        /// Updates an existing element in the container. If it does not exist, it throws an error.
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <param name="element">The element to update</param>
        /// <returns></returns>
        IResult<TElement> Update(TIdentifier idShortPath, TElement element);
        /// <summary>
        /// Deletes an element in the container.
        /// </summary>
        /// <param name="idShortPath">IdShort path to the submodel element (dot-separated)</param>
        /// <returns></returns>
        IResult Delete(TIdentifier idShortPath);
    }

    public interface IElementContainer<TElement> : ICrudContainer<string, TElement> where TElement : IReferable, IModelElement
    {
        TElement this[int i] { get; }
        TElement this[string idShort] { get; }

        event EventHandler<ElementContainerEventArgs<TElement>> OnCreated;
        event EventHandler<ElementContainerEventArgs<TElement>> OnUpdated;
        event EventHandler<ElementContainerEventArgs<TElement>> OnDeleted;

        IResult<IQueryableElementContainer<TElement>> RetrieveAll();
        IResult<IQueryableElementContainer<TElement>> RetrieveAll(Predicate<TElement> predicate);

        IEnumerable<IElementContainer<TElement>> Children { get; }
        IEnumerable<TElement> Values { get; }
        IElementContainer<TElement> ParentContainer { get; set; }

        IReferable Parent { get; set; }
        TElement Value { get; set; }
        string Path { get; set; }
        string IdShort { get; }
        bool IsRoot { get; }

        bool HasChildren();
        bool HasChild(string idShort);
        bool HasChildPath(string idShortPath);
        void Traverse(Action<TElement> action);
        IEnumerable<TElement> Flatten();
        IElementContainer<TElement> GetChild(string idShortPath);
        void AppendRootPath(string rootPath);
        void Remove(string idShort);
        void AddRange(IEnumerable<TElement> elements);
    }

    public class ElementContainerEventArgs<TElement> : EventArgs where TElement : IReferable, IModelElement
    {
        public IElementContainer<TElement> ParentContainer { get; }
        public TElement Element { get; }
        public ChangedEventType ChangedEventType { get; }
        public string ElementIdShort { get; set; }

        public ElementContainerEventArgs(IElementContainer<TElement> parentContainer, TElement element, ChangedEventType changedEventType)
        {
            ParentContainer = parentContainer;
            Element = element;
            ChangedEventType = changedEventType;

            if (element != null)
                ElementIdShort = element.IdShort;
        }
    }

    public enum ChangedEventType
    {
        Undefined,
        Created,
        Updated,
        Deleted
    }
}
