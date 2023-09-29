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
                    Value = castedProperty.Value,
                    ValueId = castedProperty.ValueId?.ToReference_V2_0()
                };

                submodelElement = property;
            }
            else if (modelType == ModelType.MultiLanguageProperty && envSubmodelElement is MultiLanguageProperty_V2_0 castedMultiLanguageProperty)
            {
                MultiLanguageProperty multiLanguageProperty = new MultiLanguageProperty(castedMultiLanguageProperty.IdShort)
                {
                    Value = castedMultiLanguageProperty.Value,
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
                    Min = new ElementValue(castedRange.Min, new DataType(dataObjectType)),
                    Max = new ElementValue(castedRange.Max, new DataType(dataObjectType)),
                    ValueType = new DataType(dataObjectType)
                };

                submodelElement = range;
            }            
            else if (modelType == ModelType.File && envSubmodelElement is File_V2_0 castedFile)
            {
                FileElement file = new FileElement(castedFile.IdShort)
                {
                    ContentType = castedFile.MimeType,
                    Value = castedFile.Value
                };

                submodelElement = file;
            }
            else if (modelType == ModelType.Blob && envSubmodelElement is Blob_V2_0 castedBlob)
            {
                Blob blob = new Blob(castedBlob.IdShort)
                {
                    MimeType = castedBlob.MimeType
                };
                if(castedBlob.Value != null)
                    blob.SetValue(castedBlob.Value);

                submodelElement = blob;
            }
            else if (modelType == ModelType.RelationshipElement && envSubmodelElement is RelationshipElement_V2_0 castedRelationshipElement)
            {
                RelationshipElement relationshipElement = new RelationshipElement(castedRelationshipElement.IdShort)
                {
                    First = castedRelationshipElement.First?.ToReference_V2_0<IReferable>(),
                    Second = castedRelationshipElement.Second?.ToReference_V2_0<IReferable>()
                };

                submodelElement = relationshipElement;
            }
            else if (modelType == ModelType.AnnotatedRelationshipElement && envSubmodelElement is AnnotatedRelationshipElement_V2_0 castedAnnotatedRelationshipElement)
            {
                AnnotatedRelationshipElement annotatedRelationshipElement = new AnnotatedRelationshipElement(castedAnnotatedRelationshipElement.IdShort)
                {
                    First = castedAnnotatedRelationshipElement.First?.ToReference_V2_0<IReferable>(),
                    Second = castedAnnotatedRelationshipElement.Second?.ToReference_V2_0<IReferable>()                    
                };

                var annotations = castedAnnotatedRelationshipElement.Annotations?.ConvertAll(c => c.submodelElement.ToSubmodelElement(conceptDescriptions, parent));
                if (annotations?.Count > 0)
                    foreach (var element in annotations)
                        annotatedRelationshipElement.Annotations.Add(element);

                submodelElement = annotatedRelationshipElement;
            }
            else if (modelType == ModelType.ReferenceElement && envSubmodelElement is ReferenceElement_V2_0 castedReferenceElement)
            {
                ReferenceElement referenceElement = new ReferenceElement(castedReferenceElement.IdShort)
                {
                    Value = castedReferenceElement.Value?.ToReference_V2_0()
                };

                submodelElement = referenceElement;
            }
            else if (modelType == ModelType.Capability && envSubmodelElement is Event_V2_0 castedCapability)
            {
                Capability capability = new Capability(castedCapability.IdShort);

                submodelElement = capability;
            }
            else if (modelType == ModelType.Event && envSubmodelElement is Event_V2_0 castedEvent)
            {
                Event eventable = new BasicEvent(castedEvent.IdShort);

                submodelElement = eventable;
            }
            else if (modelType == ModelType.BasicEvent && envSubmodelElement is BasicEvent_V2_0 castedBasicEvent)
            {
                BasicEvent basicEvent = new BasicEvent(castedBasicEvent.IdShort)
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
                    Asset = castedEntity.AssetReference.ToReference_V2_0<IAsset>()
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
                SubmodelElementCollection submodelElementCollection = new SubmodelElementCollection(castedSubmodelElementCollection.IdShort)
                {
                    AllowDuplicates = castedSubmodelElementCollection.AllowDuplicates,
                    Ordered = castedSubmodelElementCollection.Ordered
                };

                if (castedSubmodelElementCollection.Value?.Count > 0)
                {
                    submodelElementCollection.Value = new ElementContainer<ISubmodelElement>(parent, submodelElementCollection, null);
                    List<ISubmodelElement> smElements = castedSubmodelElementCollection.Value?.ConvertAll(c => c.submodelElement?.ToSubmodelElement(conceptDescriptions, parent));
                    foreach (var smElement in smElements)
                        submodelElementCollection.Value.Create(smElement);
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
                    conceptDescriptions.Find(f => f.Identification.Id == semanticId);
                submodelElement.EmbeddedDataSpecifications = submodelElement.ConceptDescription?.EmbeddedDataSpecifications;
            }

            return submodelElement;
        }

        private static List<EnvironmentConstraint_V2_0> ConvertToEnvironmentConstraints(IEnumerable<IConstraint> constraints)
        {
            if (constraints?.Count() > 0)
            {
                List<EnvironmentConstraint_V2_0> envConstraints = new List<EnvironmentConstraint_V2_0>();
                foreach (var constraint in constraints)
                {
                    if (constraint is Qualifier q)
                    {
                        EnvironmentQualifier_V2_0 envQualifier = new EnvironmentQualifier_V2_0()
                        {
                            Type = q.Type,
                            Value = q.Value?.ToString(),
                            ValueId = q.ValueId?.ToEnvironmentReference_V2_0(),
                            ValueType = q.ValueType?.ToString()
                        };
                        envConstraints.Add(new EnvironmentConstraint_V2_0() { Constraint = envQualifier });
                    }
                    else if (constraint is Formula f)
                    {
                        EnvironmentFormula_V2_0 envFormula = new EnvironmentFormula_V2_0()
                        {
                            DependsOn = f.DependsOn?.ToList()?.ConvertAll(c => c.ToEnvironmentReference_V2_0())
                        };
                        envConstraints.Add(new EnvironmentConstraint_V2_0() { Constraint = envFormula });
                    }
                    else
                        continue;
                }
                return envConstraints;
            }
            return null;
        }

        private static IEnumerable<IConstraint> ConvertToConstraints(List<EnvironmentConstraint_V2_0> envConstraints)
        {
            if(envConstraints?.Count > 0)
            {
                List<IConstraint> constraints = new List<IConstraint>();
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
                        constraints.Add(qualifier);
                    }
                    else if (envConstraint.Constraint is EnvironmentFormula_V2_0 f)
                    {
                        Formula formula = new Formula()
                        {
                            DependsOn = f.DependsOn?.ConvertAll(c => c.ToReference_V2_0())
                        };
                        constraints.Add(formula);
                    }
                    else
                        continue;
                }
                return constraints;
            }
            return null;
        }

        public static EnvironmentSubmodelElement_V2_0 ToEnvironmentSubmodelElement_V2_0(this ISubmodelElement element)
        {
            if (element == null)
                return null;
            ModelType modelType = element.ModelType;

            if (modelType == null)
                return null;

            EnvironmentSubmodelElement_V2_0 environmentSubmodelElement = new EnvironmentSubmodelElement_V2_0();

            SubmodelElementType_V2_0 submodelElementType = new SubmodelElementType_V2_0()
            {
                Category = element.Category,
                Description = element.Description,
                IdShort = element.IdShort,
                Kind = element.Kind,
                Qualifier = ConvertToEnvironmentConstraints(element.Qualifiers),
                SemanticId = element.SemanticId?.ToEnvironmentReference_V2_0()
            };

            if (modelType == ModelType.Property && element is IProperty castedProperty)
                environmentSubmodelElement.submodelElement = new Property_V2_0(submodelElementType)
                {
                    Value = castedProperty.Value?.ToString(),
                    ValueId = castedProperty.ValueId?.ToEnvironmentReference_V2_0(),
                    ValueType = castedProperty.ValueType?.ToString()
                };
            else if (modelType == ModelType.MultiLanguageProperty && element is IMultiLanguageProperty castedMultiLanguageProperty)
            {
                environmentSubmodelElement.submodelElement = new MultiLanguageProperty_V2_0(submodelElementType)
                {
                    Value = castedMultiLanguageProperty.Value,
                    ValueId = castedMultiLanguageProperty.ValueId?.ToEnvironmentReference_V2_0()
                };
            }
            else if (modelType == ModelType.Range && element is IRange castedRange)
            {
                environmentSubmodelElement.submodelElement = new Range_V2_0(submodelElementType)
                {
                    Min = castedRange.Min.ToString(),
                    Max = castedRange.Max.ToString(),
                    ValueType = castedRange.ValueType?.DataObjectType?.Name
                };
            }
            else if (modelType == ModelType.Operation && element is IOperation castedOperation)
            {
                environmentSubmodelElement.submodelElement = new Operation_V2_0(submodelElementType);
                List<OperationVariable_V2_0> inputs = new List<OperationVariable_V2_0>();
                List<OperationVariable_V2_0> outputs = new List<OperationVariable_V2_0>();
                List<OperationVariable_V2_0> inoutputs = new List<OperationVariable_V2_0>();

                if (castedOperation.InputVariables?.Count > 0)
                    foreach (var inputVar in castedOperation.InputVariables)
                        inputs.Add(new OperationVariable_V2_0() { Value = inputVar.Value.ToEnvironmentSubmodelElement_V2_0() });
                if (castedOperation.OutputVariables?.Count > 0)
                    foreach (var outputVar in castedOperation.OutputVariables)
                        outputs.Add(new OperationVariable_V2_0() { Value = outputVar.Value.ToEnvironmentSubmodelElement_V2_0() });
                if (castedOperation.InOutputVariables?.Count > 0)
                    foreach (var inoutputVar in castedOperation.InOutputVariables)
                        inoutputs.Add(new OperationVariable_V2_0() { Value = inoutputVar.Value.ToEnvironmentSubmodelElement_V2_0() });

                (environmentSubmodelElement.submodelElement as Operation_V2_0).InputVariables = inputs;
                (environmentSubmodelElement.submodelElement as Operation_V2_0).OutputVariables = outputs;
                (environmentSubmodelElement.submodelElement as Operation_V2_0).InOutputVariables = inoutputs;
            }
            else if (modelType == ModelType.Capability && element is ICapability castedCapability)
                environmentSubmodelElement.submodelElement = new Capability_V2_0(submodelElementType) { };
            else if (modelType == ModelType.Event && element is IEventElement castedEvent)
                environmentSubmodelElement.submodelElement = new Event_V2_0(submodelElementType) { };
            else if (modelType == ModelType.BasicEvent && element is IBasicEvent castedBasicEvent)
            {
                environmentSubmodelElement.submodelElement = new BasicEvent_V2_0(submodelElementType) 
                { 
                    Observed = castedBasicEvent.Observed.ToEnvironmentReference_V2_0()
                };
            }
            else if (modelType == ModelType.Entity && element is IEntity castedEntity)
            {
                environmentSubmodelElement.submodelElement = new Entity_V2_0(submodelElementType)
                {
                    EntityType = (EnvironmentEntityType)Enum.Parse(typeof(EnvironmentEntityType), castedEntity.EntityType.ToString()),
                    AssetReference = castedEntity.Asset.ToEnvironmentReference_V2_0()
                };

                List<EnvironmentSubmodelElement_V2_0> statements = new List<EnvironmentSubmodelElement_V2_0>();
                if (castedEntity.Statements?.Count() > 0)
                    foreach (var smElement in castedEntity.Statements)
                        statements.Add(smElement.ToEnvironmentSubmodelElement_V2_0());
                (environmentSubmodelElement.submodelElement as Entity_V2_0).Statements = statements;
            }
            else if (modelType == ModelType.Blob && element is IBlob castedBlob)
                environmentSubmodelElement.submodelElement = new Blob_V2_0(submodelElementType)
                {
                    Value = castedBlob.Value,
                    MimeType = castedBlob.MimeType
                };
            else if (modelType == ModelType.File && element is IFileElement castedFile)
                environmentSubmodelElement.submodelElement = new File_V2_0(submodelElementType)
                {
                    MimeType = castedFile.ContentType,
                    Value = castedFile.Value
                };
            else if (modelType == ModelType.ReferenceElement && element is IReferenceElement castedReferenceElement)
                environmentSubmodelElement.submodelElement = new ReferenceElement_V2_0(submodelElementType)
                {
                    Value = castedReferenceElement.Value?.ToEnvironmentReference_V2_0()
                };
            else if (modelType == ModelType.RelationshipElement && element is IRelationshipElement castedRelationshipElement)
                environmentSubmodelElement.submodelElement = new RelationshipElement_V2_0(submodelElementType)
                {
                    First = castedRelationshipElement.First?.ToEnvironmentReference_V2_0(),
                    Second = castedRelationshipElement.Second?.ToEnvironmentReference_V2_0()
                };
            else if (modelType == ModelType.AnnotatedRelationshipElement && element is IAnnotatedRelationshipElement castedAnnotatedRelationshipElement)
            {
                environmentSubmodelElement.submodelElement = new AnnotatedRelationshipElement_V2_0(submodelElementType)
                {
                    First = castedAnnotatedRelationshipElement.First?.ToEnvironmentReference_V2_0(),
                    Second = castedAnnotatedRelationshipElement.Second?.ToEnvironmentReference_V2_0()
                };
                List<EnvironmentSubmodelElement_V2_0> environmentSubmodelElements = new List<EnvironmentSubmodelElement_V2_0>();
                if (castedAnnotatedRelationshipElement.Annotations?.Count() > 0)
                    foreach (var smElement in castedAnnotatedRelationshipElement.Annotations)
                        environmentSubmodelElements.Add(smElement.ToEnvironmentSubmodelElement_V2_0());
                (environmentSubmodelElement.submodelElement as AnnotatedRelationshipElement_V2_0).Annotations = environmentSubmodelElements;

            }
            else if (modelType == ModelType.SubmodelElementCollection && element is ISubmodelElementCollection castedSubmodelElementCollection)
            {
                environmentSubmodelElement.submodelElement = new SubmodelElementCollection_V2_0(submodelElementType)
                {
                    AllowDuplicates = castedSubmodelElementCollection.AllowDuplicates,
                    Ordered = castedSubmodelElementCollection.Ordered
                };
                List<EnvironmentSubmodelElement_V2_0> environmentSubmodelElements = new List<EnvironmentSubmodelElement_V2_0>();
                if (castedSubmodelElementCollection.Value?.Count() > 0)
                    foreach (var smElement in castedSubmodelElementCollection.Value)
                        environmentSubmodelElements.Add(smElement.ToEnvironmentSubmodelElement_V2_0());
                (environmentSubmodelElement.submodelElement as SubmodelElementCollection_V2_0).Value = environmentSubmodelElements;
            }
            else
                return null;

            return environmentSubmodelElement;
        }       
    }
}
