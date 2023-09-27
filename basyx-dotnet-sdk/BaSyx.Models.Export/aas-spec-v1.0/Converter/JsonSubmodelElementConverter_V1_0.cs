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
    public class JsonSubmodelElementConverter_V1_0 : JsonConverter<List<EnvironmentSubmodelElement_V1_0>>
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<JsonSubmodelElementConverter_V1_0>();

        public override List<EnvironmentSubmodelElement_V1_0> ReadJson(JsonReader reader, Type objectType, List<EnvironmentSubmodelElement_V1_0> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JArray jArray = null;

            try
            {
                jArray = JArray.Load(reader);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception");
            }

            if (jArray == null || jArray.Count == 0)
                return null;

            List<EnvironmentSubmodelElement_V1_0> submodelElements = new List<EnvironmentSubmodelElement_V1_0>();
            foreach (var element in jArray)
            {
                ModelType modelType = element.SelectToken("modelType")?.ToObject<ModelType>(serializer);
                SubmodelElementType_V1_0 submodelElementType = CreateSubmodelElement(modelType);
                if (submodelElementType != null)
                {
                    serializer.Populate(element.CreateReader(), submodelElementType);
                    submodelElements.Add(new EnvironmentSubmodelElement_V1_0()
                    {
                        submodelElement = submodelElementType
                    });
                }
            }
            return submodelElements;
        }

        public override void WriteJson(JsonWriter writer, List<EnvironmentSubmodelElement_V1_0> value, JsonSerializer serializer)
        {
            if (value == null || value.Count == 0)
                return;
            JArray jArray = new JArray();

            foreach (var val in value)
            {
                JObject jObj = JObject.FromObject(val.submodelElement, serializer);
                jArray.Add(jObj);
            }

            jArray.WriteTo(writer);
        }

        public static SubmodelElementType_V1_0 CreateSubmodelElement(ModelType modelType)
        {
            if(modelType == null)
            {
                logger.LogWarning("ModelType is null");
                return null;
            }

            if (modelType == ModelType.Property)
                return new Property_V1_0();
            if (modelType == ModelType.Operation)
                return new Operation_V1_0();
            if (modelType == ModelType.Event)
                return new Event_V1_0();
            else if (modelType == ModelType.Blob)
                return new Blob_V1_0();
            else if (modelType == ModelType.File)
                return new File_V1_0();
            else if (modelType == ModelType.ReferenceElement)
                return new ReferenceElement_V1_0();
            else if (modelType == ModelType.RelationshipElement)
                return new RelationshipElement_V1_0();
            else if (modelType == ModelType.SubmodelElementCollection)
                return new SubmodelElementCollection_V1_0();
            else
            {
                logger.LogWarning("ModelType is unknown: " + modelType.Name);
                return null;
            }
        }
    }
}
