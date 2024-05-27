/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using BaSyx.Utils.DependencyInjection.Abstractions;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;
using System.Text.Json;
using BaSyx.Models.AdminShell;
using System.Collections;
using System;
using System.Linq;

namespace BaSyx.Models.Extensions
{
    public class GlobalJsonSerializerOptions
    {
		protected JsonSerializerOptions _options;
		public GlobalJsonSerializerOptions()
        {
			_options = new JsonSerializerOptions();
			_options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault;
            _options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
			_options.Converters.Add(new JsonStringEnumConverter());
		}

        public JsonSerializerOptions Build() { return _options; }
	}

    public class DefaultJsonSerializerOptions : GlobalJsonSerializerOptions
    {
        public static JsonSerializerOptions CreateDefaultJsonSerializerOptions(IDependencyInjectionExtension extension = null)
        {
            DefaultJsonSerializerOptions options = new DefaultJsonSerializerOptions();
            if(extension != null)
                options.AddDependencyInjection(extension);
            options.AddFullSubmodelElementConverter();
            return options.Build();
        }

        public DefaultJsonSerializerOptions() : base()
        {            
            _options.Converters.Add(new DataTypeConverter());
            _options.Converters.Add(new ModelTypeConverter());
            _options.Converters.Add(new ValueScopeConverter());
            _options.Converters.Add(new IdentifierConverter());
            _options.Converters.Add(new ElementContainerConverter());
            _options.TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers = { DefaultValueModifier }
            };
        }


        public DefaultJsonSerializerOptions RemoveConverter(Type converterType)
        {
            int? _index = _options.Converters.Select((item, index) => new { Item = item, Index = index })?.FirstOrDefault(i => i.Item.GetType() == converterType)?.Index;
            if (_index.HasValue && _index.Value != -1)
                _options.Converters.RemoveAt(_index.Value);
            return this;
        }

        public DefaultJsonSerializerOptions AddDependencyInjection(IDependencyInjectionExtension extension)
        {
            _options.Converters.Add(new TypeConverter(extension));
            return this;
        }
        public DefaultJsonSerializerOptions AddFullSubmodelElementConverter()
        {
            _options.Converters.Add(new FullSubmodelElementConverter());
            return this;
        }

        public DefaultJsonSerializerOptions AddValueOnlySubmodelElementConverter()
        {
            _options.Converters.Add(new ValueOnlyConverter());
            return this;
        }

        public DefaultJsonSerializerOptions AddMetadataSubmodelElementConverter()
        {
            _options.Converters.Add(new SubmodelElementConverter());
            return this;
        }

        private static void DefaultValueModifier(JsonTypeInfo info)
        {
            foreach (var property in info.Properties)
            {
                if (DataType.IsGenericList(property.PropertyType) && property.Name != "result")
                {
                    property.ShouldSerialize = (_, val) => val is ICollection collection && collection.Count > 0;
                }
            }
        }       
    }
}
