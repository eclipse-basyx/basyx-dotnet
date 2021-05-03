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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BaSyx.Models.Core.Common
{
    public class ElementPath : IEquatable<ElementPath>, IEnumerable<string>
    {
        public string Path { get; }
        public List<string> PathComponents => Path?.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)?.ToList();

        public ElementPath Parent
        {
            get
            {
                if (PathComponents?.Count > 0)
                {
                    ElementPath parentPath = new ElementPath("/");
                    foreach (var pathComponent in PathComponents)
                    {
                        parentPath.Add(new ElementPath(pathComponent));
                    }
                    return parentPath;
                }
                else
                    return this;
            }
        }

        public ElementPath(string path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));

            if (Path[0] != '/')
                Path = "/" + Path;
        }

        public ElementPath Add(ElementPath otherPath)
        {
            return new ElementPath(Path + otherPath.Path);
        }

        public bool Equals(ElementPath otherPath)
        {
            return string.Equals(Path, otherPath.Path, StringComparison.OrdinalIgnoreCase);
        }

        public bool Equals(ElementPath otherPath, StringComparison stringComparison)
        {
            return string.Equals(Path, otherPath.Path, stringComparison);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is ElementPath && Equals((ElementPath)obj, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return (Path != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Path) : 0);
        }       

        public static bool operator ==(ElementPath left, ElementPath right)
        {
            return left.Equals(right, StringComparison.OrdinalIgnoreCase);
        }

        public static bool operator !=(ElementPath left, ElementPath right)
        {
            return !left.Equals(right, StringComparison.OrdinalIgnoreCase);
        }

        public static ElementPath operator +(ElementPath left, ElementPath right)
        {
            return left.Add(right);
        }

        public static implicit operator string(ElementPath path)
        {
            return path.Path;
        }

        public static implicit operator ElementPath(string path)
        {
            return new ElementPath(path);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return PathComponents.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return PathComponents.GetEnumerator();
        }
    }
}
