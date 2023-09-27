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
using BaSyx.Utils.DependencyInjection.Abstractions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace BaSyx.Models.Extensions
{
    public class ElementContainerConverter : JsonConverter
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<ElementContainerConverter>();

        public override bool CanWrite => false;
        public override bool CanRead => true;
        public override bool CanConvert(Type objectType) => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (!objectType.IsInterface)
            {
                try
                {
                    object container = Activator.CreateInstance(objectType);
                    serializer.Populate(reader, container);
                    return container;
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error deserializing ElementContainer");
                    return null;
                }
            }
            else
            {
                try
                {
                    Type outerType = objectType.GetGenericTypeDefinition();
                    Type resolvedOuterType = (serializer.ContractResolver as IDependencyInjectionContractResolver)
                        .DependencyInjectionExtension
                        .GetRegisteredTypeFor(outerType);

                    Type innerType = objectType.GetGenericArguments()[0];
                    Type containerType = resolvedOuterType.MakeGenericType(innerType);
                    object container = Activator.CreateInstance(containerType);

                    serializer.Populate(reader, container);
                    return container;
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error deserializing ElementContainer");
                    return null;
                }
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
