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
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using System;

namespace BaSyx.Models.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true, AllowMultiple = true)]
    sealed class DataSpecificationAttribute : Attribute
    {
        public IReference Reference { get; }

        public DataSpecificationAttribute(string dataSpecificationReference)
        {
            Reference = new Reference(
                new GlobalKey(KeyElements.GlobalReference, KeyType.IRI, dataSpecificationReference));
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    sealed class DataSpecificationContentAttribute : Attribute
    {
        public string ShortNamespace { get; }
        public Type ContentType { get; }

        public DataSpecificationContentAttribute(Type contentType, string shortNamespace)
        {
            ContentType = contentType;
            ShortNamespace = shortNamespace;
        }
    }
}
