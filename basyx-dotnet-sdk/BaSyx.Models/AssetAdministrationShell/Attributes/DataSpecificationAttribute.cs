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
using System;

namespace BaSyx.Models.AdminShell
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
