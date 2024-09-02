/*******************************************************************************
* Copyright (c) 2024 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using BaSyx.Models.AdminShell;
using BaSyx.Utils.DependencyInjection.Abstractions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BaSyx.Models.Extensions
{
    public class TypeConverter : JsonConverterFactory
    {
        static readonly Dictionary<Type, Type> CustomConverterDictionary;

        public IDependencyInjectionExtension DependencyInjectionExtension { get; }
        static TypeConverter()
        {            
            CustomConverterDictionary = new Dictionary<Type, Type>()
            {
                { typeof(ValueScope), typeof(ValueScopeConverter) },
                { typeof(PropertyValue), typeof(ValueScopeConverter) },
                { typeof(IElementContainer<ISubmodelElement>), typeof(ElementContainerConverter) }
            };
        }

        public TypeConverter(IDependencyInjectionExtension diExtension)
        {
            DependencyInjectionExtension = diExtension;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            if(CustomConverterDictionary.ContainsKey(typeToConvert))
                return true;

            if (DependencyInjectionExtension.IsTypeRegistered(typeToConvert))
                return true;

            if (typeof(ISubmodelElement).IsAssignableFrom(typeToConvert) && typeToConvert != typeof(ISubmodelElement))
                return true;

            return false;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (CustomConverterDictionary.TryGetValue(typeToConvert, out Type converterType))
            {
                var converter = (JsonConverter)Activator.CreateInstance(converterType);
                return converter;
            }
            else if (DependencyInjectionExtension.IsTypeRegistered(typeToConvert))
            {
                var implementationType = DependencyInjectionExtension.GetRegisteredTypeFor(typeToConvert);
                var converter = (JsonConverter)Activator.CreateInstance(
                    type: typeof(TypeConverter<>).MakeGenericType(new Type[] { implementationType }),
                    bindingAttr: BindingFlags.Instance | BindingFlags.Public,
                    binder: null,
                    args: null,
                    culture: null);
                return converter;
            }
            else if (typeof(ISubmodelElement).IsAssignableFrom(typeToConvert) && typeToConvert != typeof(ISubmodelElement))
            {
                var converter = (JsonConverter)Activator.CreateInstance(typeof(FullSubmodelElementConverter));
                return converter;
            }
            else
                throw new JsonException("Unable to find implementation type for: " + typeToConvert.FullName);
        }
    }

    public class TypeConverter<T> : JsonConverter<T>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var converter = GetKeyConverter(options);
            T value = converter.Read(ref reader, typeof(T), options);
            return value;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var converter = GetKeyConverter(options);
            converter.Write(writer, value, options);
        }

        private static JsonConverter<T> GetKeyConverter(JsonSerializerOptions options)
        {
            var converter = options.GetConverter(typeof(T)) as JsonConverter<T>;

            if (converter is null)
                throw new JsonException("Unable to find converter for type: " + typeof(T).FullName);

            return converter;
        }
    }
}
