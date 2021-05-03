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
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using System;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Core.AssetAdministrationShell;

namespace BaSyx.Models.Core.Attributes
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
