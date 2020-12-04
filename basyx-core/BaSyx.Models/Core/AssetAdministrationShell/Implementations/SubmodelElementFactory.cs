/*******************************************************************************
* Copyright (c) 2020 Robert Bosch GmbH
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
            if (modelType == ModelType.Property)
                return new Property(idShort, valueType);
            if (modelType == ModelType.Operation)
                return new Operation(idShort);
            if (modelType == ModelType.Event)
                return new Event(idShort);
            if (modelType == ModelType.BasicEvent)
                return new BasicEvent(idShort);
            else if (modelType == ModelType.Blob)
                return new Blob(idShort);
            else if (modelType == ModelType.File)
                return new File(idShort);
            else if (modelType == ModelType.MultiLanguageProperty)
                return new MultiLanguageProperty(idShort);
            else if (modelType == ModelType.ReferenceElement)
                return new ReferenceElement(idShort);
            else if (modelType == ModelType.RelationshipElement)
                return new RelationshipElement(idShort);
            else if (modelType == ModelType.SubmodelElementCollection)
                return new SubmodelElementCollection(idShort);
            else if (modelType == ModelType.AnnotatedRelationshipElement)
                return new AnnotatedRelationshipElement(idShort);
            else if (modelType == ModelType.Entity)
                return new Entity(idShort);
            if (modelType == ModelType.Range)
                return new Range(idShort);
            else
                return null;
        }
    }
}
