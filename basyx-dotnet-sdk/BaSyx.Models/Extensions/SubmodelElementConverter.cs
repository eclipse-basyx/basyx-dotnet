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
using System.Runtime.Serialization;
using System.Linq;

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

        public override bool CanWrite => true;
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

            ModelType modelType = (string)jObject.SelectToken("modelType");
            if (modelType == null)
            {
                logger.LogError("ModelType missing: " + jObject.ToString());
                return null;
            }

            string idShort = (string)jObject.SelectToken("idShort");
            DataType valueType = (string)jObject.SelectToken("valueType");
            object value = (object)jObject.SelectToken("value");

            SubmodelElement submodelElement = new Property(idShort, valueType); 
            //SubmodelElementFactory.CreateSubmodelElement(idShort, modelType, valueType);
            //if (submodelElement == null)
            //{
            //    logger.LogError("SubmodelElement is null: " + jObject.ToString());
            //    return null;
            //}

            //JToken embeddedDataSpecificationsToken = jObject.SelectToken("embeddedDataSpecifications");            
            //List<IEmbeddedDataSpecification> embeddedDataSpecifications = null;           
            //var embeddedDataSpecificationsTokenChildToken = embeddedDataSpecificationsToken?.Children();
            //if (embeddedDataSpecificationsTokenChildToken != null)
            //{
            //    embeddedDataSpecifications = new List<IEmbeddedDataSpecification>();
            //    foreach (var dataSpecificationToken in embeddedDataSpecificationsTokenChildToken)
            //    {
            //        var dataSpecReference = dataSpecificationToken.SelectToken("dataSpecification")?.ToObject<Reference>(serializer);
            //        if (dataSpecReference != null && DataElementInformationTypes.TryGetValue(dataSpecReference.First.Value, out Type type))
            //        {
            //            var content = dataSpecificationToken?.ToObject(type, serializer);
            //            if (content != null)
            //                embeddedDataSpecifications.Add((IEmbeddedDataSpecification)content);
            //        }
            //    }
            //    jObject.Remove("embeddedDataSpecifications");
            //}           

            //if(submodelElement.GetType().GetCustomAttribute<JsonConverterAttribute>() != null)
            //    submodelElement = (SubmodelElement)serializer.Deserialize(jObject.CreateReader(), submodelElement.GetType());
            //else
            //    serializer.Populate(jObject.CreateReader(), submodelElement);

            //if(value != null)
            //{
            //    if(modelType == ModelType.Property)
            //    {
            //        PropertyValue propertyValue = new PropertyValue(new ElementValue(value, valueType));
            //        submodelElement.SetValueScope(propertyValue).Wait();
            //    }
            //}
            //submodelElement.EmbeddedDataSpecifications = embeddedDataSpecifications;
            return submodelElement;
        }

        public override void WriteJson(JsonWriter writer, ISubmodelElement value, JsonSerializer serializer)
        {
            JObject jObject = CreateSerializationObject(value, serializer);
            serializer.Serialize(writer, jObject);
        }

        public virtual JObject CreateSerializationObject(ISubmodelElement value, JsonSerializer serializer)
        {
            JObject jObject = new JObject();

            Type t = value.GetType();
            foreach (var prop in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.CanRead)
                {
                    var propAttribute = prop.GetCustomAttributes(typeof(DataMemberAttribute), true)?.FirstOrDefault();
                    if (propAttribute == null)
                        continue;

                    object propValue;
                    try
                    {
                        propValue = prop.GetValue(value);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "Error retrieving value");
                        continue;
                    }

                    if (propAttribute is DataMemberAttribute memberAttribute && propValue != null)
                    {
                        JToken jToken;
                        switch (memberAttribute.Name)
                        {
                            default:
                                if (propValue is IValue iValue && iValue.Value != null)
                                {
                                    JProperty jValue = new JProperty(memberAttribute.Name, iValue.Value.ToString());
                                    jObject.Add(jValue);
                                    continue;
                                }
                                else
                                {
                                    jToken = JToken.FromObject(propValue, serializer);
                                    if (jToken.Type == JTokenType.Array && !jToken.HasValues)
                                        continue;
                                    JProperty jProperty = new JProperty(memberAttribute.Name, jToken);
                                    jObject.Add(jProperty);
                                }
                                break;
                        }
                    }
                }
            }            
            return jObject;
        }
    }
}
