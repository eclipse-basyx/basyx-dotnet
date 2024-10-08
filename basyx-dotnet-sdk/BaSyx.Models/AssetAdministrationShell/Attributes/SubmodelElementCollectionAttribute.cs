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
using System;

namespace BaSyx.Models.AdminShell
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Interface, Inherited = true, AllowMultiple = false)]
    public sealed class SubmodelElementCollectionAttribute : SubmodelElementAttribute
    {
        private SubmodelElementCollection _submodelElementCollection;
        public SubmodelElementCollection SubmodelElementCollection => Build();

        private SubmodelElementCollection Build()
        {
            _submodelElementCollection = new SubmodelElementCollection(IdShort)
            {
                SemanticId = SemanticId,
                Kind = Kind,
                Category = Category
            };
            return _submodelElementCollection;
        }
        public string IdShort { get; set; }
        public string Category { get; set; }
        public Reference SemanticId { get; set; }
        public ModelingKind Kind { get; set; } = ModelingKind.Instance;

        public override SubmodelElement SubmodelElement => SubmodelElementCollection;

        public SubmodelElementCollectionAttribute(string idShort)
        {
            IdShort = idShort;
        }

        public SubmodelElementCollectionAttribute(string idShort, string semanticId)
        {
            IdShort = idShort;
            SemanticId = new Reference(new Key(KeyType.GlobalReference, semanticId));
        }
    }
}
