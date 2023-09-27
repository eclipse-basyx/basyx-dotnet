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
    public enum KeyElements : int
    {
        Undefined, 

        GlobalReference,
        FragmentReference,

        AccessPermissionRule,
        AnnotatedRelationshipElement,
        BasicEvent,
        Blob,
        Capability,
        ConceptDictionary,
        DataElement,
        File,
        Entity,
        Event,
        MultiLanguageProperty,
        Operation,
        Property,
        Range,
        ReferenceElement,
        RelationshipElement,
        SubmodelElement,
        SubmodelElementCollection,
        View,

        Asset,
        AssetAdministrationShell,
        ConceptDescription,
        Submodel
    }
}
