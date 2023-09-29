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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace BaSyx.Models.Export.Converter
{
    public class JsonOperationVariableConverter_V2_0 : JsonConverter<List<OperationVariable_V2_0>>
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<JsonOperationVariableConverter_V2_0>();

        public override List<OperationVariable_V2_0> ReadJson(JsonReader reader, Type objectType, List<OperationVariable_V2_0> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            try
            {
                JArray jArray = JArray.Load(reader);
                if (jArray == null || jArray.Count == 0)
                    return null;

                List<OperationVariable_V2_0> operationVariables = new List<OperationVariable_V2_0>();
                foreach (var element in jArray)
                {
                    var variable = element.SelectToken("value");
                    if (variable == null)
                        continue;

                    ModelType modelType = variable.SelectToken("modelType")?.ToObject<ModelType>(serializer);
                    SubmodelElementType_V2_0 submodelElementType = CreateSubmodelElement(modelType);
                    if (submodelElementType != null)
                    {
                        serializer.Populate(variable.CreateReader(), submodelElementType);
                        operationVariables.Add(new OperationVariable_V2_0()
                        {
                            Value = new EnvironmentSubmodelElement_V2_0() { submodelElement = submodelElementType }
                        });
                    }
                }
                return operationVariables;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while reading JSON");
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, List<OperationVariable_V2_0> value, JsonSerializer serializer)
        {
            try
            {
                JArray jArray = new JArray();
                if (value != null && value.Count > 0)
                    foreach (var val in value)
                    {
                        JObject jObj = JObject.FromObject(val.Value.submodelElement, serializer);
                        jArray.Add(new JObject(new JProperty("value", jObj)));
                    }

                jArray.WriteTo(writer);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while writing JSON");
            }
        }

        public static SubmodelElementType_V2_0 CreateSubmodelElement(ModelType modelType)
        {
            if (modelType == null)
            {
                logger.LogWarning("ModelType is null");
                return null;
            }
                       
            if (modelType == ModelType.Property)
                return new Property_V2_0();
            else if (modelType == ModelType.MultiLanguageProperty)
                return new MultiLanguageProperty_V2_0();
            else if (modelType == ModelType.BasicEvent)
                return new BasicEvent_V2_0();
            else if (modelType == ModelType.AnnotatedRelationshipElement)
                return new AnnotatedRelationshipElement_V2_0();
            else if (modelType == ModelType.Range)
                return new Range_V2_0();
            else if (modelType == ModelType.Operation)
                return new Operation_V2_0();
            else if (modelType == ModelType.Event)
                return new Event_V2_0();
            else if (modelType == ModelType.Blob)
                return new Blob_V2_0();
            else if (modelType == ModelType.File)
                return new File_V2_0();
            else if (modelType == ModelType.Entity)
                return new Entity_V2_0();
            else if (modelType == ModelType.ReferenceElement)
                return new ReferenceElement_V2_0();
            else if (modelType == ModelType.RelationshipElement)
                return new RelationshipElement_V2_0();
            else if (modelType == ModelType.SubmodelElementCollection)
                return new SubmodelElementCollection_V2_0();
            else
            {
                logger.LogWarning("ModelType is unknown: " + modelType.Name);
                return null;
            }
        }
    }
}
