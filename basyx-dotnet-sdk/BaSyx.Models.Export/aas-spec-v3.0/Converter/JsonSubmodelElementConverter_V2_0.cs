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
    public class JsonSubmodelElementConverter_V2_0 : JsonConverter<List<EnvironmentSubmodelElement_V2_0>>
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<JsonSubmodelElementConverter_V2_0>();

        public override List<EnvironmentSubmodelElement_V2_0> ReadJson(JsonReader reader, Type objectType, List<EnvironmentSubmodelElement_V2_0> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JArray jArray = null;

            try
            {
                jArray = JArray.Load(reader);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while reading JSON");
            }

            if (jArray == null || jArray.Count == 0)
                return null;

            List<EnvironmentSubmodelElement_V2_0> submodelElements = new List<EnvironmentSubmodelElement_V2_0>();
            foreach (var element in jArray)
            {
                ModelType modelType = element.SelectToken("modelType")?.ToObject<ModelType>(serializer);
                SubmodelElementType_V2_0 submodelElementType = CreateSubmodelElement(modelType);
                if (submodelElementType != null)
                {
                    serializer.Populate(element.CreateReader(), submodelElementType);
                    submodelElements.Add(new EnvironmentSubmodelElement_V2_0()
                    {
                        submodelElement = submodelElementType
                    });
                }
            }
            return submodelElements;
        }

        public override void WriteJson(JsonWriter writer, List<EnvironmentSubmodelElement_V2_0> value, JsonSerializer serializer)
        {
            JArray jArray = new JArray();
            if (value != null && value.Count > 0)
                foreach (var val in value)
                {
                    JObject jObj = JObject.FromObject(val.submodelElement, serializer);
                    jArray.Add(jObj);
                }

            jArray.WriteTo(writer);
        }

        public static SubmodelElementType_V2_0 CreateSubmodelElement(ModelType modelType)
        {
            if(modelType == null)
            {
                logger.LogWarning("ModelType is null");
                return null;
            }
                       
            if (modelType == ModelType.Property)
                return new Property_V2_0();
            else if (modelType == ModelType.MultiLanguageProperty)
                return new MultiLanguageProperty_V2_0();
            if (modelType == ModelType.Operation)
                return new Operation_V2_0();
            if (modelType == ModelType.Capability)
                return new Capability_V2_0();
            if (modelType == ModelType.BasicEvent)
                return new BasicEvent_V2_0();
            if (modelType == ModelType.Event)
                return new Event_V2_0();
            if (modelType == ModelType.Entity)
                return new Entity_V2_0();
            else if (modelType == ModelType.Blob)
                return new Blob_V2_0();
            else if (modelType == ModelType.File)
                return new File_V2_0();
            else if (modelType == ModelType.Range)
                return new Range_V2_0();
            else if (modelType == ModelType.ReferenceElement)
                return new ReferenceElement_V2_0();
            else if (modelType == ModelType.RelationshipElement)
                return new RelationshipElement_V2_0();
            else if (modelType == ModelType.AnnotatedRelationshipElement)
                return new AnnotatedRelationshipElement_V2_0();
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
