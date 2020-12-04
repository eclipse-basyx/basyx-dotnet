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
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BaSyx.Models.Core.Common
{
    public class SubmodelElementContainer : ElementContainer<ISubmodelElement>
    {
        public const string PATH_SEPERATOR = "/";

        private readonly IList<SubmodelElementContainer> _children;

        public string IdShort { get; }
        public ISubmodelElement Value { get; }
        public string Path { get; } = string.Empty;
        public bool IsRoot { get; }

        public SubmodelElementContainer ParentContainer { get; protected set; }
        public SubmodelElementContainer(IReferable parent) : base(parent)
        {
            _children = new List<SubmodelElementContainer>();

            IdShort = null;
            Value = null;
            Path = PATH_SEPERATOR;
            IsRoot = true;
        }

        public SubmodelElementContainer(ISubmodelElement rootElement) : base(rootElement)
        {
            _children = new List<SubmodelElementContainer>();

            IdShort = rootElement.IdShort;
            Value = rootElement;

            if (ParentContainer != null)
            {
                Path = ParentContainer.Path + PATH_SEPERATOR + IdShort;
                IsRoot = false;
            }
            else
            {
                Path = PATH_SEPERATOR + IdShort;
                IsRoot = true;
            }
        }

        public new SubmodelElementContainer this[int i]
        {
            get { return _children[i]; }
        }

        public new SubmodelElementContainer this[string idShortPath]
        {
            get { return _children.FirstOrDefault(c => c.IdShort == idShortPath); }
        }
        public ReadOnlyCollection<SubmodelElementContainer> Children
        {
            get { return new ReadOnlyCollection<SubmodelElementContainer>(_children); }
        }

        public SubmodelElementContainer AddChild(ISubmodelElement submodelElement)
        {
            SubmodelElementContainer node = new SubmodelElementContainer(submodelElement) 
                { ParentContainer = this };

            node.ParentContainer = this;           
            this._children.Add(node);
            return this;
        }

        public SubmodelElementContainer AddChild(SubmodelElementContainer submodelElements)
        {
            submodelElements.ParentContainer = this;
            this._children.Add(submodelElements);
            return this;
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
                if (idShortPath.Contains("/"))
                {
                    string[] splittedPath = idShortPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (!HasChild(splittedPath[0]))
                        return false;
                    else
                    {
                        var child = this[splittedPath[0]];
                        return (child.HasChildPath(string.Join("/", splittedPath.Skip(1))));
                    }
                }
                else
                    return HasChild(idShortPath);
            }
        }

        public void Traverse(Action<ISubmodelElement> action)
        {
            action(Value);
            foreach (var child in _children)
                child.Traverse(action);
        }

        public SubmodelElementContainer GetChild(string idShortPath)
        {
            if (string.IsNullOrEmpty(idShortPath))
                return null;

            if (_children == null || _children.Count == 0)
                return null;
            else
            {
                SubmodelElementContainer superChild;
                if (idShortPath.Contains("/"))
                {
                    string[] splittedPath = idShortPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (!HasChild(splittedPath[0]))
                        superChild = null;
                    else
                    {
                        var child = this[splittedPath[0]];
                        superChild = child.GetChild(string.Join("/", splittedPath.Skip(1)));
                    }
                }
                else
                    superChild = this[idShortPath];

                return superChild;
            }
        }
    }
}
