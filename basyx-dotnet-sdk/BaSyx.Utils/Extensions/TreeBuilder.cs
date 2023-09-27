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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BaSyx.Utils.Extensions
{
    public class TreeBuilder<T>
    {
        protected T _value;
        protected List<object> _additionalValues;
        protected List<TreeBuilder<T>> _children;
        protected string _name;
        protected string _path;

        public const string PATH_SEPERATOR = "/";

        public TreeBuilder<T> Parent { get; protected set; }
        public List<object> AdditionalValues { get { return _additionalValues; } }
        public T Value { get { return _value; } }
        public string Name { get { return _name; } }
        public string Path { get { return _path; } }

        public TreeBuilder(string name, T value)
        {
            this._name = name;
            this._value = value;
            this._children = new List<TreeBuilder<T>>();
            this._additionalValues = new List<object>();

            if (this.Parent != null)
                _path += PATH_SEPERATOR + this._name;
            else
                _path = this._name;
        }

        public ReadOnlyCollection<TreeBuilder<T>> Children
        {
            get { return _children.AsReadOnly(); }
        }

        public TreeBuilder<T> ReplaceValue(T value)
        {
            this._value = value;
            return this;
        }

        public TreeBuilder<T> AddAdditionalValue(object value)
        {
            this._additionalValues.Add(value);
            return this;
        } 

        public bool TryGetAdditionalValue<U>(out U obj)
        {
            obj = GetAdditionalValue<U>();
            if (obj != null)
                return true;
            else
                return false;
        }

        public U GetAdditionalValue<U>()
        {
            object obj =_additionalValues.Find(o => o is U);
            if(obj != null)
                return (U)obj;
            return default;
        }

        public TreeBuilder<T> this[int i]
        {
            get { return _children[i]; }
        }

        public TreeBuilder<T> this[string name]
        {
            get { return _children.FirstOrDefault(s => s.Name == name); }
        }

        public void Traverse(Action<T> action)
        {
            action(_value);
            foreach (var child in _children)
                child.Traverse(action);
        }

        #region Child-Operations
        public TreeBuilder<T> AddChild(TreeBuilder<T> child)
        {
            child.Parent = this;
            this._children.Add(child);
            return this;
        }

        public TreeBuilder<T> AddChild(string name, T value)
        {
            TreeBuilder<T> node = new TreeBuilder<T>(name, value) { Parent = this };
            _children.Add(node);
            return node;
        }

        public string GetPath()
        {
            List<string> path = GetInternalPath();
            if(path != null)
            {
                path.Reverse();
                return string.Join("/", path);
            }
            return null;
        }

        private List<string> GetInternalPath()
        {
            List<string> path = new List<string>() { this.Name };
            if (Parent != null)
            {
                List<string> parentPath = Parent.GetInternalPath();
                path.AddRange(parentPath);
            }
            return path;
        }
        public bool HasChildPath(string childPath)
        {
            if (string.IsNullOrEmpty(childPath))
                return false;

            if (_children == null || _children.Count == 0)
                return false;
            else
            {
                if (childPath.Contains("/"))
                {
                    string[] splittedPath = childPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (!HasChild(splittedPath[0]))
                        return false;
                    else
                    {
                        var child = this[splittedPath[0]];
                        return (child.HasChildPath(string.Join("/", splittedPath.Skip(1))));
                    }
                }
                else
                    return HasChild(childPath);
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
        public bool HasChild(string childName)
        {
            if (_children == null || _children.Count == 0)
                return false;
            else
            {
                var child = _children.Find(c => c.Name == childName);
                if (child == null)
                    return false;
                else
                    return true;
            }
        }

        public TreeBuilder<T> GetChild(string childPath)
        {
            if (string.IsNullOrEmpty(childPath))
                return null;

            if (_children == null || _children.Count == 0)
                return null;
            else
            {
                TreeBuilder<T> superChild;
                if (childPath.Contains("/"))
                {
                    string[] splittedPath = childPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (!HasChild(splittedPath[0]))
                        superChild = null;
                    else
                    {
                        var child = this[splittedPath[0]];
                        superChild = child.GetChild(string.Join("/", splittedPath.Skip(1)));
                    }
                }
                else
                    superChild = this[childPath];

                return superChild;
            }
        }
        #endregion
    }
}
