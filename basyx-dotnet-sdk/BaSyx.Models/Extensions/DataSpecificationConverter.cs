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

namespace BaSyx.Models.Extensions
{
    public class DataSpecificationConverter : JsonConverter<IEmbeddedDataSpecification>
    {
        public static Dictionary<string, Type> DataSpecificationTypes { get; }
        public static Dictionary<string, Type> DataSpecificationContentTypes { get; }
        static DataSpecificationConverter()
        {
            DataSpecificationTypes = new Dictionary<string, Type>();
            DataSpecificationContentTypes = new Dictionary<string, Type>();
            var types = typeof(DataSpecificationConverter).Assembly.GetTypes();
            foreach (Type type in types)
            {
                var dataSpecificationAttrib = type.GetCustomAttribute(typeof(DataSpecificationAttribute), false);
                if(dataSpecificationAttrib != null && dataSpecificationAttrib is DataSpecificationAttribute dataSpecificationAttribute)
                {
                    DataSpecificationTypes.Add(dataSpecificationAttribute.Reference.First.Value, type);

                    foreach (var property in type.GetProperties())
                    {
                        var dataSpecificationContentAttrib = property.GetCustomAttribute(typeof(DataSpecificationContentAttribute), false);
                        if (dataSpecificationContentAttrib != null && dataSpecificationContentAttrib is DataSpecificationContentAttribute dataSpecificationContentAttribute)
                            DataSpecificationContentTypes.Add(dataSpecificationAttribute.Reference.First.Value, dataSpecificationContentAttribute.ContentType);
                    }
                }               
            }

        }

        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override IEmbeddedDataSpecification ReadJson(JsonReader reader, Type objectType, IEmbeddedDataSpecification existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jObject;

            try
            {
                jObject = JObject.Load(reader);
            }
            catch (Exception)
            {
                return null;
            }

            var dataSpecificationReference = jObject.SelectToken("hasDataSpecification")?.ToObject<Reference>(serializer);
            var dataSpecificationContent = jObject.SelectToken("dataSpecificationContent");

            IEmbeddedDataSpecification embeddedDataSpecification = null;

            if (dataSpecificationReference != null && 
                DataSpecificationTypes.TryGetValue(dataSpecificationReference.First.Value, out Type dataSpecificationType) &&
                DataSpecificationContentTypes.TryGetValue(dataSpecificationReference.First.Value, out Type dataSpecificationContentType))
            {
                var content = dataSpecificationContent.ToObject(dataSpecificationContentType, serializer);
                if(content != null)
                    embeddedDataSpecification = (IEmbeddedDataSpecification)Activator.CreateInstance(dataSpecificationType, content);                
            }
            return embeddedDataSpecification;
        }

        public override void WriteJson(JsonWriter writer, IEmbeddedDataSpecification value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
