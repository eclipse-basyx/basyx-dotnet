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
using BaSyx.Models.Core.Common;

namespace BaSyx.Models.Core.AssetAdministrationShell.Implementations
{
    public static class SubmodelElementFactory
    {
        public static SubmodelElement CreateSubmodelElement(string idShort, ModelType modelType, DataType valueType = null)
        {
            switch (modelType.Type)
            {              
                case ModelTypes.SubmodelElementCollection:
                    return new SubmodelElementCollection(idShort);
                case ModelTypes.Operation:
                    return new Operation(idShort);
                case ModelTypes.BasicEvent:
                    return new BasicEvent(idShort);
                case ModelTypes.EventElement:
                    return new EventElement(idShort);
                case ModelTypes.RelationshipElement:
                    return new RelationshipElement(idShort);
                case ModelTypes.AnnotatedRelationshipElement:
                    return new AnnotatedRelationshipElement(idShort); 
                case ModelTypes.Property:
                    return new Property(idShort, valueType);
                case ModelTypes.File:
                    return new File(idShort);
                case ModelTypes.Blob:
                    return new Blob(idShort);
                case ModelTypes.ReferenceElement:
                    return new ReferenceElement(idShort);
                case ModelTypes.MultiLanguageProperty:
                    return new MultiLanguageProperty(idShort);
                case ModelTypes.Range:
                    return new Range(idShort, valueType);
                case ModelTypes.Entity:
                    return new Entity(idShort);
                default:
                    return null;
            }
        }
    }
}
