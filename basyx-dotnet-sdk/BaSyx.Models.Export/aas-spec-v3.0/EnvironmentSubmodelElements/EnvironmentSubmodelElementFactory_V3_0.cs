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
    public static class EnvironmentSubmodelElementFactory_V3_0
    {
        public static ISubmodelElement ToSubmodelElement(this SubmodelElementType_V3_0 envSubmodelElement, List<IConceptDescription> conceptDescriptions, IReferable parent)
        {
            if (envSubmodelElement == null)
                return null;

            ModelType modelType = envSubmodelElement.ModelType;

            if (modelType == null)
                return null;

            SubmodelElement submodelElement = null;

            if (modelType == ModelType.Property && envSubmodelElement is Property_V3_0 castedProperty)
            {
                DataObjectType dataObjectType;
                if (string.IsNullOrEmpty(castedProperty.ValueType))
                    dataObjectType = DataObjectType.None;
                else if (!DataObjectType.TryParse(castedProperty.ValueType, out dataObjectType))
                    return null;

                Property property = new Property(castedProperty.IdShort, new DataType(dataObjectType))
                {
                    ValueId = castedProperty.ValueId?.ToReference_V3_0(),
                    Value = new PropertyValue(new ElementValue(castedProperty.Value, castedProperty.ValueType))
                };

                submodelElement = property;
            }
            else if (modelType == ModelType.MultiLanguageProperty && envSubmodelElement is MultiLanguageProperty_V3_0 castedMultiLanguageProperty)
            {
                MultiLanguageProperty multiLanguageProperty = new MultiLanguageProperty(castedMultiLanguageProperty.IdShort)
                {
                    Value = castedMultiLanguageProperty.Value?.ConvertAll(l => new LangString(l.Language, l.Text))?.ToLangStringSet(),
                    ValueId = castedMultiLanguageProperty.ValueId?.ToReference_V3_0()
                };

                submodelElement = multiLanguageProperty;
            }
            else if (modelType == ModelType.Range && envSubmodelElement is Range_V3_0 castedRange)
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
            else if (modelType == ModelType.File && envSubmodelElement is File_V3_0 castedFile)
            {
                FileElement file = new FileElement(castedFile.IdShort)
                {
                    ContentType = castedFile.ContentType,
                    Value = castedFile.Value
                };

                submodelElement = file;
            }
            else if (modelType == ModelType.Blob && envSubmodelElement is Blob_V3_0 castedBlob)
            {
                Blob blob = new Blob(castedBlob.IdShort)
                {
                    ContentType = castedBlob.ContentType
                };
                if(castedBlob.Value != null)
                    blob.SetValue(castedBlob.Value);

                submodelElement = blob;
            }
            else if (modelType == ModelType.RelationshipElement && envSubmodelElement is RelationshipElement_V3_0 castedRelationshipElement)
            {
                RelationshipElement relationshipElement = new RelationshipElement(castedRelationshipElement.IdShort)
                {
                    First = castedRelationshipElement.First?.ToReference_V3_0<IReferable>(),
                    Second = castedRelationshipElement.Second?.ToReference_V3_0<IReferable>()
                };

                submodelElement = relationshipElement;
            }
            else if (modelType == ModelType.AnnotatedRelationshipElement && envSubmodelElement is AnnotatedRelationshipElement_V3_0 castedAnnotatedRelationshipElement)
            {
                AnnotatedRelationshipElement annotatedRelationshipElement = new AnnotatedRelationshipElement(castedAnnotatedRelationshipElement.IdShort)
                {
                    First = castedAnnotatedRelationshipElement.First?.ToReference_V3_0<IReferable>(),
                    Second = castedAnnotatedRelationshipElement.Second?.ToReference_V3_0<IReferable>()                    
                };

                var annotations = castedAnnotatedRelationshipElement.Annotations?.ConvertAll(c => c.ToSubmodelElement(conceptDescriptions, parent));
                if (annotations?.Count > 0)
                    foreach (var element in annotations)
                        annotatedRelationshipElement.Annotations.Add(element);

                submodelElement = annotatedRelationshipElement;
            }
            else if (modelType == ModelType.ReferenceElement && envSubmodelElement is ReferenceElement_V3_0 castedReferenceElement)
            {
                ReferenceElement referenceElement = new ReferenceElement(castedReferenceElement.IdShort)
                {
                    Value = castedReferenceElement.Value?.ToReference_V3_0()
                };

                submodelElement = referenceElement;
            }
            else if (modelType == ModelType.Capability && envSubmodelElement is Capability_V3_0 castedCapability)
            {
                Capability capability = new Capability(castedCapability.IdShort);

                submodelElement = capability;
            }
            else if (modelType == ModelType.BasicEvent && envSubmodelElement is BasicEventElement_V3_0 castedBasicEvent)
            {
                BasicEventElement basicEvent = new BasicEventElement(castedBasicEvent.IdShort)
                {
                    Observed = castedBasicEvent.Observed.ToReference_V3_0<IReferable>()
                };

                submodelElement = basicEvent;
            }
            else if (modelType == ModelType.Entity && envSubmodelElement is Entity_V3_0 castedEntity)
            {
                Entity entity = new Entity(castedEntity.IdShort)
                {
                    EntityType = (EntityType)Enum.Parse(typeof(EntityType), castedEntity.EntityType.ToString()),
                    GlobalAssetId = castedEntity.GlobalAssetId,
                    SpecificAssetIds = castedEntity.SpecificAssetIds?.ConvertAll(s => new SpecificAssetId()
                    {
                        ExternalSubjectId = s.ExternalSubjectId?.ToReference_V3_0(),
                        Name = s.Name,
                        SemanticId = s.SemanticId?.ToReference_V3_0(),
                        SupplementalSemanticIds = s.SupplementalSemanticIds?.ConvertAll(r => r.ToReference_V3_0()),
                        Value = s.Value
                    })
                };

                var statements = castedEntity.Statements?.ConvertAll(c => c.ToSubmodelElement(conceptDescriptions, parent));
                if (statements?.Count > 0)
                    foreach (var element in statements)
                        entity.Statements.Create(element);

                submodelElement = entity;
            }
            else if (modelType == ModelType.Operation && envSubmodelElement is Operation_V3_0 castedOperation)
            {
                Operation operation = new Operation(castedOperation.IdShort)
                {
                    InputVariables = new OperationVariableSet(),
                    OutputVariables = new OperationVariableSet(),
                    InOutputVariables = new OperationVariableSet()
                };

                var operationInElements = castedOperation.InputVariables?.ConvertAll(c => c.Value?.ToSubmodelElement(conceptDescriptions, parent));
                if(operationInElements?.Count > 0)
                    foreach (var element in operationInElements)
                        operation.InputVariables.Add(element);
                
                var operationOutElements = castedOperation.OutputVariables?.ConvertAll(c => c.Value?.ToSubmodelElement(conceptDescriptions, parent));
                if (operationOutElements?.Count > 0)
                    foreach (var element in operationOutElements)
                        operation.OutputVariables.Add(element);

                var operationInOutElements = castedOperation.InOutputVariables?.ConvertAll(c => c.Value?.ToSubmodelElement(conceptDescriptions, parent));
                if (operationInOutElements?.Count > 0)
                    foreach (var element in operationInOutElements)
                        operation.InOutputVariables.Add(element);

                submodelElement = operation;
            }
            else if (modelType == ModelType.SubmodelElementCollection && envSubmodelElement is SubmodelElementCollection_V3_0 castedSubmodelElementCollection)
            {
                SubmodelElementCollection submodelElementCollection = new SubmodelElementCollection(castedSubmodelElementCollection.IdShort);

                if (castedSubmodelElementCollection.Value?.Count > 0)
                {
                    submodelElementCollection.Value = new ElementContainer<ISubmodelElement>(parent, submodelElementCollection, null);
                    List<ISubmodelElement> smElements = castedSubmodelElementCollection.Value?.ConvertAll(c => c?.ToSubmodelElement(conceptDescriptions, parent));
                    foreach (var smElement in smElements)
                        submodelElementCollection.Value.Create(smElement);
                }

                submodelElement = submodelElementCollection;
            }
			else if (modelType == ModelType.SubmodelElementList && envSubmodelElement is SubmodelElementList_V3_0 castedSubmodelElementList)
			{
				SubmodelElementList submodelElementList = new SubmodelElementList(castedSubmodelElementList.IdShort);

				if (castedSubmodelElementList.Value?.Count > 0)
				{
					submodelElementList.Value = new ElementContainer<ISubmodelElement>(parent, submodelElementList, null);
					List<ISubmodelElement> smElements = castedSubmodelElementList.Value?.ConvertAll(c => c?.ToSubmodelElement(conceptDescriptions, parent));
					foreach (var smElement in smElements)
						submodelElementList.Value.Create(smElement);
				}

                submodelElementList.OrderRelevant = castedSubmodelElementList.OrderRelevant;
                submodelElementList.SemanticIdListElement = castedSubmodelElementList.SemanticIdListElement?.ToReference_V3_0();

                if(!string.IsNullOrEmpty(castedSubmodelElementList.TypeValueListElement))
                    submodelElementList.TypeValueListElement = castedSubmodelElementList.TypeValueListElement;

				if (!string.IsNullOrEmpty(castedSubmodelElementList.ValueTypeListElement))
					submodelElementList.ValueTypeListElement = castedSubmodelElementList.ValueTypeListElement;

				submodelElement = submodelElementList;
			}


			if (submodelElement == null)
                return null;

            submodelElement.Category = envSubmodelElement.Category;
            if(envSubmodelElement.Description?.Count  > 0)
                submodelElement.Description = new LangStringSet(envSubmodelElement.Description.ConvertAll(l => new LangString(l.Language, l.Text)));
            submodelElement.IdShort = envSubmodelElement.IdShort;
            submodelElement.Kind = envSubmodelElement.Kind;
            submodelElement.SemanticId = envSubmodelElement.SemanticId?.ToReference_V3_0();
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
        
        private static IEnumerable<IQualifier> ConvertToConstraints(List<EnvironmentConstraint_V3_0> envConstraints)
        {
            if(envConstraints?.Count > 0)
            {
                List<IQualifier> qualifiers = new List<IQualifier>();
                foreach (var envConstraint in envConstraints)
                {
                    if (envConstraint.Constraint is EnvironmentQualifier_V3_0 q)
                    {
                        Qualifier qualifier = new Qualifier()
                        {
                            Type = q.Type,
                            Value = q.Value,
                            ValueId = q.ValueId?.ToReference_V3_0(),
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
