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
namespace BaSyx.Models.AdminShell
{
    public static class SubmodelElementFactory
    {
        public static SubmodelElement CreateSubmodelElement(string idShort, ModelType modelType, DataType valueType = null)
        {
            switch (modelType.Type)
            {              
                case ModelTypes.SubmodelElementCollection:
                    return new SubmodelElementCollection(idShort);
                case ModelTypes.SubmodelElementList:
                    return new SubmodelElementList(idShort);
                case ModelTypes.Operation:
                    return new Operation(idShort);
                case ModelTypes.BasicEventElement:
                    return new BasicEventElement(idShort);
                case ModelTypes.RelationshipElement:
                    return new RelationshipElement(idShort);
                case ModelTypes.AnnotatedRelationshipElement:
                    return new AnnotatedRelationshipElement(idShort); 
                case ModelTypes.Property:
                    return new Property(idShort, valueType);
                case ModelTypes.File:
                    return new FileElement(idShort);
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
