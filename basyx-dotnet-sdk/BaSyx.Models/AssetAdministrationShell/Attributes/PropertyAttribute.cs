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

namespace BaSyx.Models.AdminShell
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class PropertyAttribute : SubmodelElementAttribute
    {
        private Property _property;
        public Property Property => Build();

        private Property Build()
        {
            _property = new Property(IdShort, ValueType)
            {
                SemanticId = SemanticId,
                Kind = Kind,
                Category = Category
            };
            return _property;
        }
        public string IdShort { get; }
        public string Category { get; set; }
        public Reference SemanticId { get; }
        public DataType ValueType { get; }
        public ModelingKind Kind { get; set; } = ModelingKind.Instance;

        public override SubmodelElement SubmodelElement => Property;

        public PropertyAttribute(string idShort, DataObjectTypes valueObjectType)
        {
            IdShort = idShort;
            ValueType = new DataType(DataObjectType.GetDataObjectType(valueObjectType));
        }

        public PropertyAttribute(string idShort, DataObjectTypes valueObjectType, string semanticId, KeyElements semanticKeyElement, KeyType semanticKeyType)
        {
            IdShort = idShort;
            ValueType = new DataType(DataObjectType.GetDataObjectType(valueObjectType));
            SemanticId = new Reference(new Key(semanticKeyElement, semanticKeyType, semanticId, false));
        }
    }
}
