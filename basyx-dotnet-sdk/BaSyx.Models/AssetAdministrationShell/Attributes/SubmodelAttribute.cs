/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
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
    public sealed class SubmodelAttribute : Attribute
    {
        private Submodel _submodel;
        public Submodel Submodel => Build();

        private Submodel Build()
        {
            _submodel = new Submodel(IdShort, Identification)
            {
                SemanticId = SemanticId,
                Kind = Kind,
                Category = Category
            };
            return _submodel;
        }
        public string IdShort { get; }
        public Identifier Identification { get; }
        public string Category { get; set; }
        public Reference SemanticId { get; }
        public ModelingKind Kind { get; set; } = ModelingKind.Instance;

        public SubmodelAttribute(string idShort, string id)
        {
            IdShort = idShort;
            Identification = new Identifier(id);
        }

        public SubmodelAttribute(string idShort, string id,string semanticId)
        {
            IdShort = idShort;
            Identification = new Identifier(id);
            SemanticId = new Reference(new Key(KeyType.GlobalReference, semanticId));
        }
    }
}
