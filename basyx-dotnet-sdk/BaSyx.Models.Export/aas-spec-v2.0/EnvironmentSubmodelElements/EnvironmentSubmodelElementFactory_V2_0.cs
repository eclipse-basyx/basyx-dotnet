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
using BaSyx.Models.AdminShell;
using BaSyx.Models.Export.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using Range = BaSyx.Models.AdminShell.Range;

namespace BaSyx.Models.Export.EnvironmentSubmodelElements
{
    public static class EnvironmentSubmodelElementFactory_V2_0
    {
        public static ISubmodelElement ToSubmodelElement(this SubmodelElementType_V2_0 envSubmodelElement, List<IConceptDescription> conceptDescriptions, IReferable parent)
        {
            if (envSubmodelElement == null)
                return null;

            ModelType modelType = envSubmodelElement.ModelType;

            if (modelType == null)
                return null;

            SubmodelElement submodelElement = null;

            if (modelType == ModelType.Property && envSubmodelElement is Property_V2_0 castedProperty)
            {
                DataObjectType dataObjectType;
                if (string.IsNullOrEmpty(castedProperty.ValueType))
                    dataObjectType = DataObjectType.None;
                else if (!DataObjectType.TryParse(castedProperty.ValueType, out dataObjectType))
                    return null;

                Property property = new Property(castedProperty.IdShort, new DataType(dataObjectType))
                {
                    ValueId = castedProperty.ValueId?.ToReference_V2_0(),
                    Value = new PropertyValue(new ElementValue(castedProperty.Value, castedProperty.ValueType))
                };

                submodelElement = property;
            }
            else if (modelType == ModelType.MultiLanguageProperty && envSubmodelElement is MultiLanguageProperty_V2_0 castedMultiLanguageProperty)
            {
                MultiLanguageProperty multiLanguageProperty = new MultiLanguageProperty(castedMultiLanguageProperty.IdShort)
                {
                    Value = new MultiLanguagePropertyValue(castedMultiLanguageProperty.Value),
                    ValueId = castedMultiLanguageProperty.ValueId?.ToReference_V2_0()
                };

                submodelElement = multiLanguageProperty;
            }
            else if (modelType == ModelType.Range && envSubmodelElement is Range_V2_0 castedRange)
            {
                if (!DataObjectType.TryParse(castedRange.ValueType, out DataObjectType dataObjectType))
                    return null;

                Range range = new Range(castedRange.IdShort)
                {
                    Value = new RangeValue(
                        min : new ElementValue(castedRange.Min, new DataType(dataObjectType)), 
                        max: new ElementValue(castedRange.Max, new DataType(dataObjectType))),
                    ValueType = new DataType(dataObjectType)
                };

                submodelElement = range;
            }            
            else if (modelType == ModelType.File && envSubmodelElement is File_V2_0 castedFile)
            {
                FileElement file = new FileElement(castedFile.IdShort)
                {
                  Value = new FileElementValue()
                  {
                      ContentType = castedFile.MimeType,
                      Value = castedFile.Value
                  }
                };

                submodelElement = file;
            }
            else if (modelType == ModelType.Blob && envSubmodelElement is Blob_V2_0 castedBlob)
            {
                Blob blob = new Blob(castedBlob.IdShort)
                {
                    Value = new BlobValue
                    {
                        ContentType = castedBlob.MimeType,
                        Value = castedBlob.Value
                    }
                };

                submodelElement = blob;
            }
            else if (modelType == ModelType.RelationshipElement && envSubmodelElement is RelationshipElement_V2_0 castedRelationshipElement)
            {
                RelationshipElement relationshipElement = new RelationshipElement(castedRelationshipElement.IdShort);
                RelationshipElementValue relValue = new RelationshipElementValue()
                {
                    First = castedRelationshipElement.First?.ToReference_V2_0<IReferable>(),
                    Second = castedRelationshipElement.Second?.ToReference_V2_0<IReferable>()
                };

                relationshipElement.Value = relValue;

                submodelElement = relationshipElement;
            }
            else if (modelType == ModelType.AnnotatedRelationshipElement && envSubmodelElement is AnnotatedRelationshipElement_V2_0 castedAnnotatedRelationshipElement)
            {
                //AnnotatedRelationshipElement annotatedRelationshipElement = new AnnotatedRelationshipElement(castedAnnotatedRelationshipElement.IdShort)
                //{
                //    First = castedAnnotatedRelationshipElement.First?.ToReference_V2_0<IReferable>(),
                //    Second = castedAnnotatedRelationshipElement.Second?.ToReference_V2_0<IReferable>()                    
                //};

                //var annotations = castedAnnotatedRelationshipElement.Annotations?.ConvertAll(c => c.submodelElement.ToSubmodelElement(conceptDescriptions, parent));
                //if (annotations?.Count > 0)
                //    foreach (var element in annotations)
                        //annotatedRelationshipElement.Annotations.Add(element);

                //submodelElement = annotatedRelationshipElement;
            }
            else if (modelType == ModelType.ReferenceElement && envSubmodelElement is ReferenceElement_V2_0 castedReferenceElement)
            {
                ReferenceElement referenceElement = new ReferenceElement(castedReferenceElement.IdShort);

                var reference = castedReferenceElement.Value?.ToReference_V2_0();
                if(reference != null)
                    referenceElement.Value = new ReferenceElementValue(reference);

				submodelElement = referenceElement;
            }
            else if (modelType == ModelType.Capability && envSubmodelElement is Event_V2_0 castedCapability)
            {
                Capability capability = new Capability(castedCapability.IdShort);

                submodelElement = capability;
            }
            else if (modelType == ModelType.BasicEvent && envSubmodelElement is BasicEvent_V2_0 castedBasicEvent)
            {
                BasicEventElement basicEvent = new BasicEventElement(castedBasicEvent.IdShort)
                {
                    Observed = castedBasicEvent.Observed.ToReference_V2_0<IReferable>()
                };

                submodelElement = basicEvent;
            }
            else if (modelType == ModelType.Entity && envSubmodelElement is Entity_V2_0 castedEntity)
            {
                Entity entity = new Entity(castedEntity.IdShort)
                {
                    EntityType = (EntityType)Enum.Parse(typeof(EntityType), castedEntity.EntityType.ToString()),
                    GlobalAssetId = castedEntity.AssetReference?.Keys?.First()?.Value
                };

                var statements = castedEntity.Statements?.ConvertAll(c => c.submodelElement.ToSubmodelElement(conceptDescriptions, parent));
                if (statements?.Count > 0)
                    foreach (var element in statements)
                        entity.Statements.Create(element);

                submodelElement = entity;
            }
            else if (modelType == ModelType.Operation && envSubmodelElement is Operation_V2_0 castedOperation)
            {
                Operation operation = new Operation(castedOperation.IdShort)
                {
                    InputVariables = new OperationVariableSet(),
                    OutputVariables = new OperationVariableSet(),
                    InOutputVariables = new OperationVariableSet()
                };

                var operationInElements = castedOperation.InputVariables?.ConvertAll(c => c.Value?.submodelElement?.ToSubmodelElement(conceptDescriptions, parent));
                if(operationInElements?.Count > 0)
                    foreach (var element in operationInElements)
                        operation.InputVariables.Add(element);
                
                var operationOutElements = castedOperation.OutputVariables?.ConvertAll(c => c.Value?.submodelElement?.ToSubmodelElement(conceptDescriptions, parent));
                if (operationOutElements?.Count > 0)
                    foreach (var element in operationOutElements)
                        operation.OutputVariables.Add(element);

                var operationInOutElements = castedOperation.InOutputVariables?.ConvertAll(c => c.Value?.submodelElement?.ToSubmodelElement(conceptDescriptions, parent));
                if (operationInOutElements?.Count > 0)
                    foreach (var element in operationInOutElements)
                        operation.InOutputVariables.Add(element);

                submodelElement = operation;
            }
            else if (modelType == ModelType.SubmodelElementCollection && envSubmodelElement is SubmodelElementCollection_V2_0 castedSubmodelElementCollection)
            {
                SubmodelElementCollection submodelElementCollection = new SubmodelElementCollection(castedSubmodelElementCollection.IdShort);

                if (castedSubmodelElementCollection.Value?.Count > 0)
                {
                    submodelElementCollection.Value.Value = new ElementContainer<ISubmodelElement>(parent, submodelElementCollection, null);
                    List<ISubmodelElement> smElements = castedSubmodelElementCollection.Value?.ConvertAll(c => c.submodelElement?.ToSubmodelElement(conceptDescriptions, parent));
                    foreach (var smElement in smElements)
                        submodelElementCollection.Value.Value.Create(smElement);
                }

                submodelElement = submodelElementCollection;
            }


            if (submodelElement == null)
                return null;

            submodelElement.Category = envSubmodelElement.Category;
            submodelElement.Description = envSubmodelElement.Description;
            submodelElement.IdShort = envSubmodelElement.IdShort;
            submodelElement.Kind = envSubmodelElement.Kind;
            submodelElement.SemanticId = envSubmodelElement.SemanticId?.ToReference_V2_0();
            submodelElement.Qualifiers = ConvertToConstraints(envSubmodelElement.Qualifier);

            string semanticId = envSubmodelElement.SemanticId?.Keys?.FirstOrDefault()?.Value;
            if (!string.IsNullOrEmpty(semanticId))
            {
                submodelElement.ConceptDescription =
                    conceptDescriptions.Find(f => f.Id == semanticId);
                submodelElement.EmbeddedDataSpecifications = submodelElement.ConceptDescription?.EmbeddedDataSpecifications;
            }

            return submodelElement;
        }
        
        private static IEnumerable<IQualifier> ConvertToConstraints(List<EnvironmentConstraint_V2_0> envConstraints)
        {
            if(envConstraints?.Count > 0)
            {
                List<IQualifier> qualifiers = new List<IQualifier>();
                foreach (var envConstraint in envConstraints)
                {
                    if (envConstraint.Constraint is EnvironmentQualifier_V2_0 q)
                    {
                        Qualifier qualifier = new Qualifier()
                        {
                            Type = q.Type,
                            Value = q.Value,
                            ValueId = q.ValueId?.ToReference_V2_0(),
                        };
                        if (string.IsNullOrEmpty(q.ValueType))
                            qualifier.ValueType = new DataType(DataObjectType.None);
                        qualifiers.Add(qualifier);
                    }                   
                    else
                        continue;
                }
                return qualifiers;
            }
            return null;
        }     
    }
}
