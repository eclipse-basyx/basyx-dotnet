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
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{    
    [DataContract]
    public enum KeyType
    {
        Undefined,
        AnnotatedRelationshipElement,
        AssetAdministrationShell,
        BasicEventElement,
        Blob,
        Capability,
        ConceptDescription,
        DataElement,
        Entity,
        EventElement,
        File,
        FragmentReference,
        GlobalReference,
        Identifiable,
        MultiLanguageProperty,
        Operation,
        Property,
        Range,
        Referable,
        ReferenceElement,
        RelationshipElement,
        Submodel,
        SubmodelElement,
        SubmodelElementCollection,
        SubmodelElementList
    }
}
