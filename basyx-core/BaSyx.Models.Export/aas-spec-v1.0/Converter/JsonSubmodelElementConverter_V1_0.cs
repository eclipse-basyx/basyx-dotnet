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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;

namespace BaSyx.Models.Export.Converter
{
    public class JsonSubmodelElementConverter_V1_0 : JsonConverter<List<EnvironmentSubmodelElement_V1_0>>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public override List<EnvironmentSubmodelElement_V1_0> ReadJson(JsonReader reader, Type objectType, List<EnvironmentSubmodelElement_V1_0> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JArray jArray = null;

            try
            {
                jArray = JArray.Load(reader);
            }
            catch (Exception e)
            {
                logger.Error(e);
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
                logger.Warn("ModelType is null");
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
                logger.Warn("ModelType is unknown: " + modelType.Name);
                return null;
            }
        }
    }
}
