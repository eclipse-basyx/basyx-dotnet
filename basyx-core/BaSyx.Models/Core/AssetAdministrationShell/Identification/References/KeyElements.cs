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
using System.Runtime.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Identification
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
