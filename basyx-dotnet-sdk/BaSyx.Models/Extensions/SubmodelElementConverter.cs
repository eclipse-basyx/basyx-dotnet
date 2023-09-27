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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace BaSyx.Models.Extensions
{
    public class SubmodelElementConverter : JsonConverter<ISubmodelElement>
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<SubmodelElementConverter>();

        static Dictionary<string, Type> DataElementInformationTypes;
        static SubmodelElementConverter()
        {
            DataElementInformationTypes = new Dictionary<string, Type>();
            var types = typeof(SubmodelElementConverter).Assembly.GetTypes();
            foreach (Type type in types)
            {
                var attrib = type.GetCustomAttribute(typeof(DataSpecificationAttribute), false);
                if(attrib != null && attrib is DataSpecificationAttribute dataSpecificationAttribute)
                {
                    DataElementInformationTypes.Add(dataSpecificationAttribute.Reference.First.Value, type);
                }
            }

        }

        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override ISubmodelElement ReadJson(JsonReader reader, Type objectType, ISubmodelElement existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jObject;

            try
            {
                jObject = JObject.Load(reader);
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Unable to load JObject from type ${objectType.Name}");
                return null;
            }

            ModelType modelType = jObject.SelectToken("modelType")?.ToObject<ModelType>(serializer);
            if (modelType == null)
            {
                logger.LogError("ModelType missing: " + jObject.ToString());
                return null;
            }

            string idShort = jObject.SelectToken("idShort")?.ToObject<string>();
            DataType valueType = jObject.SelectToken("valueType")?.ToObject<DataType>(serializer);

            JToken embeddedDataSpecificationsToken = jObject.SelectToken("embeddedDataSpecifications");
            JToken conceptDescriptionToken = jObject.SelectToken("conceptDescription");

            SubmodelElement submodelElement;
            List<IEmbeddedDataSpecification> embeddedDataSpecifications = null;
            ConceptDescription conceptDescription = null;
            var embeddedDataSpecificationsTokenChildToken = embeddedDataSpecificationsToken?.Children();
            if (embeddedDataSpecificationsTokenChildToken != null)
            {
                embeddedDataSpecifications = new List<IEmbeddedDataSpecification>();
                foreach (var dataSpecificationToken in embeddedDataSpecificationsTokenChildToken)
                {
                    var dataSpecReference = dataSpecificationToken.SelectToken("hasDataSpecification")?.ToObject<Reference>(serializer);
                    if (dataSpecReference != null && DataElementInformationTypes.TryGetValue(dataSpecReference.First.Value, out Type type))
                    {
                        var content = dataSpecificationToken?.ToObject(type, serializer);
                        if (content != null)
                            embeddedDataSpecifications.Add((IEmbeddedDataSpecification)content);
                    }
                }
                jObject.Remove("embeddedDataSpecifications");
            }
            if (conceptDescriptionToken != null)
            {
                var dataSpecifications = conceptDescriptionToken.SelectToken("embeddedDataSpecifications")?.Children();
                if (dataSpecifications != null)
                {
                    conceptDescription = new ConceptDescription();
                    foreach (var dataSpecificationToken in dataSpecifications)
                    {
                        var dataSpecReference = dataSpecificationToken.SelectToken("hasDataSpecification")?.ToObject<Reference>(serializer);
                        if (dataSpecReference != null && DataElementInformationTypes.TryGetValue(dataSpecReference.First.Value, out Type type))
                        {
                            var content = dataSpecificationToken.ToObject(type, serializer);
                            if (content != null)
                                (conceptDescription.EmbeddedDataSpecifications as List<IEmbeddedDataSpecification>).Add((IEmbeddedDataSpecification)content);
                        }
                    }
                }
                serializer.Populate(conceptDescriptionToken.CreateReader(), conceptDescription);
            }

            submodelElement = SubmodelElementFactory.CreateSubmodelElement(idShort, modelType, valueType);

            if (submodelElement == null)
            {
                logger.LogError("SubmodelElement is null: " + jObject.ToString());
                return null;
            }

            if(submodelElement.GetType().GetCustomAttribute<JsonConverterAttribute>() != null)
                submodelElement = (SubmodelElement)serializer.Deserialize(jObject.CreateReader(), submodelElement.GetType());
            else
                serializer.Populate(jObject.CreateReader(), submodelElement);

            submodelElement.EmbeddedDataSpecifications = embeddedDataSpecifications;
            submodelElement.ConceptDescription = conceptDescription;
            return submodelElement;
        }

        public override void WriteJson(JsonWriter writer, ISubmodelElement value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
