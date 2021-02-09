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
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Export.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using BaSyx.Models.Core.AssetAdministrationShell.Semantics;
using BaSyx.Models.Core.Common;
using NLog;

namespace BaSyx.Models.Export.EnvironmentSubmodelElements
{
    public static class EnvironmentSubmodelElementFactory_V1_0
    {
        public static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public static ISubmodelElement ToSubmodelElement(this SubmodelElementType_V1_0 envSubmodelElement, List<IConceptDescription> conceptDescriptions, IReferable parent)
        {
            if (envSubmodelElement == null)
            {
                logger.Warn("EnvironmentSubmodelElement is null");
                return null;
            }

            ModelType modelType = envSubmodelElement.ModelType;
            if (modelType == null)
            {
                logger.Warn("ModelType of Element " + envSubmodelElement.IdShort + " is null");
                return null;
            }
            SubmodelElement submodelElement = null;

            if (modelType == ModelType.Property && envSubmodelElement is Property_V1_0 castedProperty)
            {
                Property property;
                if (DataObjectType.TryParse(castedProperty.ValueType, out DataObjectType dataObjectType))
                {
                    property = new Property(castedProperty.IdShort, new DataType(dataObjectType));
                }
                else
                {
                    logger.Warn("Unable to parse ValueType of Property " + castedProperty.IdShort + " - ValueType: " + castedProperty.ValueType);
                    property = new Property(castedProperty.IdShort);
                }

                property.Value = castedProperty.Value;
                property.ValueId = castedProperty.ValueId?.ToReference_V1_0();

                submodelElement = property;
            }
            else if (modelType == ModelType.File && envSubmodelElement is File_V1_0 castedFile)
            {
                File file = new File(castedFile.IdShort)
                {
                    MimeType = castedFile.MimeType,
                    Value = castedFile.Value
                };

                submodelElement = file;
            }
            else if (modelType == ModelType.Blob && envSubmodelElement is Blob_V1_0 castedBlob)
            {
                Blob blob = new Blob(castedBlob.IdShort)
                {
                    MimeType = castedBlob.MimeType                    
                };

                blob.SetValue(castedBlob.Value);

                submodelElement = blob;
            }
            else if (modelType == ModelType.RelationshipElement && envSubmodelElement is RelationshipElement_V1_0 castedRelationshipElement)
            {
                RelationshipElement relationshipElement = new RelationshipElement(castedRelationshipElement.IdShort)
                {
                    First = castedRelationshipElement.First?.ToReference_V1_0<IReferable>(),
                    Second = castedRelationshipElement.Second?.ToReference_V1_0<IReferable>()
                };

                submodelElement = relationshipElement;
            }
            else if (modelType == ModelType.ReferenceElement && envSubmodelElement is ReferenceElement_V1_0 castedReferenceElement)
            {
                ReferenceElement referenceElement = new ReferenceElement(castedReferenceElement.IdShort)
                {
                    Value = castedReferenceElement.Value?.ToReference_V1_0()
                };

                submodelElement = referenceElement;
            }
            else if (modelType == ModelType.Event && envSubmodelElement is Event_V1_0 castedEvent)
            {
               Event eventable = new BasicEvent(castedEvent.IdShort);

                submodelElement = eventable;
            }
            else if (modelType == ModelType.Operation && envSubmodelElement is Operation_V1_0 castedOperation)
            {
                Operation operation = new Operation(castedOperation.IdShort)
                {
                    InputVariables = new OperationVariableSet(),
                    OutputVariables = new OperationVariableSet()
                };

                var operationInElements = castedOperation.In?.ConvertAll(c => c.Value?.submodelElement?.ToSubmodelElement(conceptDescriptions, parent));
                if(operationInElements?.Count > 0)
                    foreach (var element in operationInElements)
                        operation.InputVariables.Add(element);

                var operationOutElements = castedOperation.Out?.ConvertAll(c => c.Value?.submodelElement?.ToSubmodelElement(conceptDescriptions, parent));
                if (operationOutElements?.Count > 0)
                    foreach (var element in operationOutElements)
                        operation.OutputVariables.Add(element);

                submodelElement = operation;
            }
            else if (modelType == ModelType.SubmodelElementCollection && envSubmodelElement is SubmodelElementCollection_V1_0 castedSubmodelElementCollection)
            {
               SubmodelElementCollection submodelElementCollection = new SubmodelElementCollection(castedSubmodelElementCollection.IdShort);

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
            {
                logger.Warn("SubmodelElement " + envSubmodelElement.IdShort + " is still null");
                return null;
            }

            submodelElement.Category = envSubmodelElement.Category;
            submodelElement.Description = envSubmodelElement.Description;
            submodelElement.IdShort = envSubmodelElement.IdShort;
            submodelElement.Kind = envSubmodelElement.Kind;
            submodelElement.SemanticId = envSubmodelElement.SemanticId?.ToReference_V1_0();
            submodelElement.Constraints = null;

            string semanticId = envSubmodelElement.SemanticId?.Keys?.FirstOrDefault()?.Value;
            if (!string.IsNullOrEmpty(semanticId))
            {
                submodelElement.ConceptDescription =
                    conceptDescriptions.Find(f => f.Identification.Id == semanticId);
                submodelElement.EmbeddedDataSpecifications = submodelElement.ConceptDescription?.EmbeddedDataSpecifications;
            }

            return submodelElement;
        }

        public static EnvironmentSubmodelElement_V1_0 ToEnvironmentSubmodelElement_V1_0(this ISubmodelElement element)
        {
            if (element == null)
                return null;
            ModelType modelType = element.ModelType;

            if (modelType == null)
                return null;

            EnvironmentSubmodelElement_V1_0 environmentSubmodelElement = new EnvironmentSubmodelElement_V1_0();

            SubmodelElementType_V1_0 submodelElementType = new SubmodelElementType_V1_0()
            {
                Category = element.Category,
                Description = element.Description,
                IdShort = element.IdShort,
                Kind = element.Kind,
                Qualifier = null,
                SemanticId = element.SemanticId?.ToEnvironmentReference_V1_0(),
            };

            if (modelType == ModelType.Property && element is IProperty castedProperty)
                environmentSubmodelElement.submodelElement = new Property_V1_0(submodelElementType)
                {
                    Value = castedProperty.Value?.ToString(),
                    ValueId = castedProperty.ValueId?.ToEnvironmentReference_V1_0(),
                    ValueType = castedProperty.ValueType?.ToString()
                };
            else if (modelType == ModelType.Operation && element is IOperation castedOperation)
            {
                environmentSubmodelElement.submodelElement = new Operation_V1_0(submodelElementType);
                List<OperationVariable_V1_0> inputs = new List<OperationVariable_V1_0>();
                List<OperationVariable_V1_0> outputs = new List<OperationVariable_V1_0>();

                if (castedOperation.InputVariables?.Count > 0)
                    foreach (var inputVar in castedOperation.InputVariables)
                        inputs.Add(new OperationVariable_V1_0() { Value = inputVar.Value.ToEnvironmentSubmodelElement_V1_0() });
                if (castedOperation.OutputVariables?.Count > 0)
                    foreach (var outputVar in castedOperation.OutputVariables)
                        outputs.Add(new OperationVariable_V1_0() { Value = outputVar.Value.ToEnvironmentSubmodelElement_V1_0() });

                (environmentSubmodelElement.submodelElement as Operation_V1_0).In = inputs;
                (environmentSubmodelElement.submodelElement as Operation_V1_0).Out = outputs;
            }
            else if (modelType == ModelType.Event && element is IEvent castedEvent)
                environmentSubmodelElement.submodelElement = new Event_V1_0(submodelElementType) { };
            else if (modelType == ModelType.Blob && element is IBlob castedBlob)
                environmentSubmodelElement.submodelElement = new Blob_V1_0(submodelElementType)
                {
                    Value = castedBlob.Value,
                    MimeType = castedBlob.MimeType
                };
            else if (modelType == ModelType.File && element is IFile castedFile)
                environmentSubmodelElement.submodelElement = new File_V1_0(submodelElementType)
                {
                    MimeType = castedFile.MimeType,
                    Value = castedFile.Value
                };
            else if (modelType == ModelType.ReferenceElement && element is IReferenceElement castedReferenceElement)
                environmentSubmodelElement.submodelElement = new ReferenceElement_V1_0(submodelElementType)
                {
                    Value = castedReferenceElement.Value?.ToEnvironmentReference_V1_0()
                };
            else if (modelType == ModelType.RelationshipElement && element is IRelationshipElement castedRelationshipElement)
                environmentSubmodelElement.submodelElement = new RelationshipElement_V1_0(submodelElementType)
                {
                    First = castedRelationshipElement.First?.ToEnvironmentReference_V1_0(),
                    Second = castedRelationshipElement.Second?.ToEnvironmentReference_V1_0()
                };
            else if (modelType == ModelType.SubmodelElementCollection && element is ISubmodelElementCollection castedSubmodelElementCollection)
            {
                environmentSubmodelElement.submodelElement = new SubmodelElementCollection_V1_0(submodelElementType);
                List<EnvironmentSubmodelElement_V1_0> environmentSubmodelElements = new List<EnvironmentSubmodelElement_V1_0>();
                if (castedSubmodelElementCollection.Value?.Count() > 0)
                    foreach (var smElement in castedSubmodelElementCollection.Value)
                        environmentSubmodelElements.Add(smElement.ToEnvironmentSubmodelElement_V1_0());
                (environmentSubmodelElement.submodelElement as SubmodelElementCollection_V1_0).Value = environmentSubmodelElements;
            }
            else
                return null;

            return environmentSubmodelElement;
        }
    }
}
