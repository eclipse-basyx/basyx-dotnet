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
using System.Text.Json.Nodes;
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
                    ValueId = castedMultiLanguageProperty.ValueId?.ToReference_V3_0()
                };
                var value = castedMultiLanguageProperty.Value?.ConvertAll(l => new LangString(l.Language, l.Text))?.ToLangStringSet();
                if(value != null)
                    multiLanguageProperty.Value = new MultiLanguagePropertyValue(value);

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
                    Value = new FileElementValue()
                    {
                        ContentType = castedFile.ContentType,
                        Value = castedFile.Value
                    }
                };

                submodelElement = file;
            }
            else if (modelType == ModelType.Blob && envSubmodelElement is Blob_V3_0 castedBlob)
            {
                Blob blob = new Blob(castedBlob.IdShort)
                {
                    Value = new BlobValue()
                    {
                        ContentType = castedBlob.ContentType,
                        Value = castedBlob.Value
                    }
                };

                submodelElement = blob;
            }
            else if (modelType == ModelType.RelationshipElement && envSubmodelElement is RelationshipElement_V3_0 castedRelationshipElement)
            {
                RelationshipElement relationshipElement = new RelationshipElement(castedRelationshipElement.IdShort);
                RelationshipElementValue relValue = new RelationshipElementValue()
                {
                    First = castedRelationshipElement.First?.ToReference_V3_0<IReferable>(),
                    Second = castedRelationshipElement.Second?.ToReference_V3_0<IReferable>()
                };

                relationshipElement.Value = relValue;

                submodelElement = relationshipElement;
            }
            else if (modelType == ModelType.AnnotatedRelationshipElement && envSubmodelElement is AnnotatedRelationshipElement_V3_0 castedAnnotatedRelationshipElement)
            {
                //AnnotatedRelationshipElement annotatedRelationshipElement = new AnnotatedRelationshipElement(castedAnnotatedRelationshipElement.IdShort)
                //{
                //    First = castedAnnotatedRelationshipElement.First?.ToReference_V3_0<IReferable>(),
                //    Second = castedAnnotatedRelationshipElement.Second?.ToReference_V3_0<IReferable>()                    
                //};

                //var annotations = castedAnnotatedRelationshipElement.Annotations?.ConvertAll(c => c.ToSubmodelElement(conceptDescriptions, parent));
                //if (annotations?.Count > 0)
                //    foreach (var element in annotations)
                //        annotatedRelationshipElement.Annotations.Add(element);

                //submodelElement = annotatedRelationshipElement;
            }
            else if (modelType == ModelType.ReferenceElement && envSubmodelElement is ReferenceElement_V3_0 castedReferenceElement)
            {
                ReferenceElement referenceElement = new ReferenceElement(castedReferenceElement.IdShort);

                var reference = castedReferenceElement.Value?.ToReference_V3_0();
                if(reference != null)
                    referenceElement.Value = new ReferenceElementValue(reference);

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

                var operationInElements = castedOperation.InputVariables?.ConvertAll(c => c.Value?.Value?.ToSubmodelElement(conceptDescriptions, parent));
                if(operationInElements?.Count > 0)
                    foreach (var element in operationInElements)
                        operation.InputVariables.Add(element);
                
                var operationOutElements = castedOperation.OutputVariables?.ConvertAll(c => c.Value?.Value?.ToSubmodelElement(conceptDescriptions, parent));
                if (operationOutElements?.Count > 0)
                    foreach (var element in operationOutElements)
                        operation.OutputVariables.Add(element);

                var operationInOutElements = castedOperation.InOutputVariables?.ConvertAll(c => c.Value?.Value?.ToSubmodelElement(conceptDescriptions, parent));
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
                    submodelElementCollection.Value.Value = new ElementContainer<ISubmodelElement>(parent, submodelElementCollection, null);
                    List<ISubmodelElement> smElements = castedSubmodelElementCollection.Value?.ConvertAll(c => c?.ToSubmodelElement(conceptDescriptions, parent));
                    foreach (var smElement in smElements)
                        submodelElementCollection.Value.Value.Create(smElement);
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

        private static List<EnvironmentConstraint_V3_0> ConvertToEnvironmentConstraints(IEnumerable<IQualifier> qualifiers)
        {
            if (qualifiers?.Count() > 0)
            {
                List<EnvironmentConstraint_V3_0> envConstraints = new List<EnvironmentConstraint_V3_0>();
                foreach (var constraint in qualifiers)
                {
                    if (constraint is Qualifier q)
                    {
                        EnvironmentQualifier_V3_0 envQualifier = new EnvironmentQualifier_V3_0()
                        {
                            Type = q.Type,
                            Value = q.Value?.ToString(),
                            ValueId = q.ValueId?.ToEnvironmentReference_V3_0(),
                            ValueType = q.ValueType?.ToString()
                        };
                        envConstraints.Add(new EnvironmentConstraint_V3_0() { Constraint = envQualifier });
                    }                    
                    else
                        continue;
                }
                return envConstraints;
            }
            return null;
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

        public static SubmodelElementType_V3_0 ToEnvironmentSubmodelElement_V3_0(this ISubmodelElement element)
        {
            if (element == null)
                return null;
            ModelType modelType = element.ModelType;

            if (modelType == null)
                return null;

            SubmodelElementType_V3_0 environmentSubmodelElement;

            SubmodelElementType_V3_0 submodelElementType = new SubmodelElementType_V3_0()
            {
                Category = element.Category,
                Description = element.Description?.ToEnvironmentLangStringSet(),
                IdShort = element.IdShort,
                Qualifier = ConvertToEnvironmentConstraints(element.Qualifiers),
                SemanticId = element.SemanticId?.ToEnvironmentReference_V3_0()
            };

            if (modelType == ModelType.Property && element is IProperty castedProperty)
            {
                environmentSubmodelElement = new Property_V3_0(submodelElementType)
                {
                    Value = castedProperty.Value?.ToString(),
                    ValueId = castedProperty.ValueId?.ToEnvironmentReference_V3_0(),
                    ValueType = "xs:" + castedProperty.ValueType?.ToString()
                };
            }
            else if (modelType == ModelType.MultiLanguageProperty && element is IMultiLanguageProperty castedMultiLanguageProperty)
            {
                environmentSubmodelElement = new MultiLanguageProperty_V3_0(submodelElementType)
                {
                    ValueId = castedMultiLanguageProperty.ValueId?.ToEnvironmentReference_V3_0()
                };

                var value = castedMultiLanguageProperty.GetValueScope().Result;
                if (value is MultiLanguagePropertyValue mlpValue)
                    (environmentSubmodelElement as MultiLanguageProperty_V3_0).Value = mlpValue.Value.ToEnvironmentLangStringSet();


			}
            else if (modelType == ModelType.Range && element is IRange castedRange)
            {
                environmentSubmodelElement = new Range_V3_0(submodelElementType)
                {                    
                    ValueType = "xs:" + castedRange.ValueType?.ToString()
                };
                if(castedRange.Value?.Min != null && environmentSubmodelElement is Range_V3_0 range1)
                {
                    range1.Min = JsonValue.Create(castedRange.Value.Min.Value).ToString();                  
                }
                if (castedRange.Value?.Max != null && environmentSubmodelElement is Range_V3_0 range2)
                {
                    range2.Max = JsonValue.Create(castedRange.Value.Max.Value).ToString();
                }
            }
            else if (modelType == ModelType.Operation && element is IOperation castedOperation)
            {
                environmentSubmodelElement = new Operation_V3_0(submodelElementType);
                List<OperationVariable_V3_0> inputs = new List<OperationVariable_V3_0>();
                List<OperationVariable_V3_0> outputs = new List<OperationVariable_V3_0>();
                List<OperationVariable_V3_0> inoutputs = new List<OperationVariable_V3_0>();

                if (castedOperation.InputVariables?.Count > 0)
                    foreach (var inputVar in castedOperation.InputVariables)
                        inputs.Add(new OperationVariable_V3_0() { Value = new OperationVariableValue_V3_0() { Value = inputVar.Value.ToEnvironmentSubmodelElement_V3_0() } });
                if (castedOperation.OutputVariables?.Count > 0)
                    foreach (var outputVar in castedOperation.OutputVariables)
                        outputs.Add(new OperationVariable_V3_0() { Value = new OperationVariableValue_V3_0() { Value = outputVar.Value.ToEnvironmentSubmodelElement_V3_0() } });
                if (castedOperation.InOutputVariables?.Count > 0)
                    foreach (var inoutputVar in castedOperation.InOutputVariables)
                        inoutputs.Add(new OperationVariable_V3_0() { Value = new OperationVariableValue_V3_0() { Value = inoutputVar.Value.ToEnvironmentSubmodelElement_V3_0() } });

                (environmentSubmodelElement as Operation_V3_0).InputVariables = inputs;
                (environmentSubmodelElement as Operation_V3_0).OutputVariables = outputs;
                (environmentSubmodelElement as Operation_V3_0).InOutputVariables = inoutputs;
            }
            else if (modelType == ModelType.Capability && element is ICapability castedCapability)
                environmentSubmodelElement = new Capability_V3_0(submodelElementType) { };
            else if (modelType == ModelType.BasicEventElement && element is IBasicEventElement castedBasicEvent) //TODO
            {
                environmentSubmodelElement = new BasicEventElement_V3_0(submodelElementType)
                {
                    Observed = castedBasicEvent.Observed.ToEnvironmentReference_V3_0(),
                    Direction = castedBasicEvent.Direction,
                    State = castedBasicEvent.State,
                    MessageTopic = castedBasicEvent.MessageTopic,
                    MessageBroker = castedBasicEvent.MessageBroker.ToEnvironmentReference_V3_0(),
                    LastUpdate = castedBasicEvent.LastUpdate,
                    MinInterval = castedBasicEvent.MinInterval,
                    MaxInterval = castedBasicEvent.MaxInterval
                };
            }
            else if (modelType == ModelType.Entity && element is IEntity castedEntity)
            {
                environmentSubmodelElement = new Entity_V3_0(submodelElementType)
                {
                    EntityType = (EnvironmentEntityType_V3_0)Enum.Parse(typeof(EnvironmentEntityType_V3_0), castedEntity.EntityType.ToString()),
                    GlobalAssetId = castedEntity.GlobalAssetId,
                    SpecificAssetIds = castedEntity.SpecificAssetIds?.ToList().ConvertAll(c => new EnvironmentSpecificAssetId_V3_0()
                    {
                        Name = c.Name,
                        ExternalSubjectId = c.ExternalSubjectId?.ToEnvironmentReference_V3_0(),
                        SemanticId = c.SemanticId?.ToEnvironmentReference_V3_0(),
                        SupplementalSemanticIds = c.SupplementalSemanticIds?.ToList().ConvertAll(d => d.ToEnvironmentReference_V3_0()),
                        Value = c.Value
                    })
                };

                List<SubmodelElementType_V3_0> statements = new List<SubmodelElementType_V3_0>();
                if (castedEntity.Statements?.Count() > 0)
                    foreach (var smElement in castedEntity.Statements)
                        statements.Add(smElement.ToEnvironmentSubmodelElement_V3_0());
                (environmentSubmodelElement as Entity_V3_0).Statements = statements;
            }
            else if (modelType == ModelType.Blob && element is IBlob castedBlob)
                environmentSubmodelElement = new Blob_V3_0(submodelElementType)
                {
                    Value = castedBlob.Value?.Value,
                    ContentType = castedBlob.Value?.ContentType
                };
            else if (modelType == ModelType.File && element is IFileElement castedFile)
                environmentSubmodelElement = new File_V3_0(submodelElementType)
                {                    
                    Value = castedFile.Value?.Value,
                    ContentType = castedFile.Value?.ContentType,
                };
            else if (modelType == ModelType.ReferenceElement && element is IReferenceElement castedReferenceElement)
                environmentSubmodelElement = new ReferenceElement_V3_0(submodelElementType)
                {
                    Value = castedReferenceElement.Value?.Value?.ToEnvironmentReference_V3_0()
                };
            else if (modelType == ModelType.RelationshipElement && element is IRelationshipElement castedRelationshipElement)
                environmentSubmodelElement = new RelationshipElement_V3_0(submodelElementType)
                {
                    First = castedRelationshipElement.Value?.First?.ToEnvironmentReference_V3_0(),
                    Second = castedRelationshipElement.Value?.Second?.ToEnvironmentReference_V3_0()
                };
            else if (modelType == ModelType.AnnotatedRelationshipElement && element is IAnnotatedRelationshipElement castedAnnotatedRelationshipElement)
            {
                environmentSubmodelElement = new AnnotatedRelationshipElement_V3_0(submodelElementType)
                {
                    First = castedAnnotatedRelationshipElement.Value?.First?.ToEnvironmentReference_V3_0(),
                    Second = castedAnnotatedRelationshipElement.Value?.Second?.ToEnvironmentReference_V3_0()
                };
                List<SubmodelElementType_V3_0> environmentSubmodelElements = new List<SubmodelElementType_V3_0>();
                if (castedAnnotatedRelationshipElement.Value?.Annotations?.Count() > 0)
                    foreach (var smElement in castedAnnotatedRelationshipElement.Value.Annotations)
                        environmentSubmodelElements.Add(smElement.ToEnvironmentSubmodelElement_V3_0());
                (environmentSubmodelElement as AnnotatedRelationshipElement_V3_0).Annotations = environmentSubmodelElements;

            }
            else if (modelType == ModelType.SubmodelElementCollection && element is ISubmodelElementCollection castedSubmodelElementCollection)
            {
                environmentSubmodelElement = new SubmodelElementCollection_V3_0(submodelElementType);
                List<SubmodelElementType_V3_0> environmentSubmodelElements = new List<SubmodelElementType_V3_0>();
                if (castedSubmodelElementCollection.Value?.Value?.Count() > 0)
                    foreach (var smElement in castedSubmodelElementCollection.Value.Value)
                        environmentSubmodelElements.Add(smElement.ToEnvironmentSubmodelElement_V3_0());
                (environmentSubmodelElement as SubmodelElementCollection_V3_0).Value = environmentSubmodelElements;
            }
            else if (modelType == ModelType.SubmodelElementList && element is ISubmodelElementList castedSubmodelElementList)
            {
                environmentSubmodelElement = new SubmodelElementList_V3_0(submodelElementType)
                {
                    OrderRelevant = castedSubmodelElementList.OrderRelevant,
                    TypeValueListElement = castedSubmodelElementList.TypeValueListElement?.Name,
                    SemanticIdListElement = castedSubmodelElementList.SemanticIdListElement?.ToEnvironmentReference_V3_0(),
                    ValueTypeListElement = castedSubmodelElementList.ValueTypeListElement?.DataObjectType?.Name
                };
                List<SubmodelElementType_V3_0> environmentSubmodelElements = new List<SubmodelElementType_V3_0>();
                if (castedSubmodelElementList.Value?.Count() > 0)
                    foreach (var smElement in castedSubmodelElementList.Value)
                        environmentSubmodelElements.Add(smElement.ToEnvironmentSubmodelElement_V3_0());
                (environmentSubmodelElement as SubmodelElementList_V3_0).Value = environmentSubmodelElements;
            }
            else
                return null;

            return environmentSubmodelElement;
        }
    }
}
