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
using BaSyx.Models.AdminShell;
using System;
using System.Buffers.Text;
using System.Buffers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;

namespace BaSyx.Models.Extensions
{
	public class ValueScopeConverterOptions
	{
		public bool EnclosingObject { get; set; } = true;
	}
	public class ValueScopeConverter : ValueScopeConverter<ValueScope> 
	{ 
		public ValueScopeConverter(ValueScopeConverterOptions options = null, JsonSerializerOptions jsonOptions = null):
			base(null, options, jsonOptions)
		{ }
	}
    public class ValueScopeConverter<TValueScope> : JsonConverter<ValueScope> where TValueScope : ValueScope
    {
		private static JsonSerializerOptions _options;
		static ValueScopeConverter()
		{
			_options = new GlobalJsonSerializerOptions().Build();
		}

		private DataType _dataType;
		private ValueScopeConverterOptions _converterOptions;
		private JsonSerializerOptions _jsonOptions;
		public ValueScopeConverter(DataType dataType = null, ValueScopeConverterOptions options = null, JsonSerializerOptions jsonOptions = null) 
		{
			_dataType = dataType;
			_converterOptions = options ?? new ValueScopeConverterOptions();
			_jsonOptions = jsonOptions;
		}
		public override bool CanConvert(Type typeToConvert)
		{
			if (typeof(ValueScope).IsAssignableFrom(typeToConvert))
				return true;
			else
				return base.CanConvert(typeToConvert);
		}
		public override ValueScope Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if(typeof(TValueScope) == typeof(PropertyValue))
            {
				ElementValue elementValue = GetElementValue(ref reader, _dataType);
				return new PropertyValue(elementValue);
			}
			else if (typeof(TValueScope) == typeof(RangeValue))
			{
				RangeValue rangeValue = new RangeValue();

				while (reader.Read())
				{
					if (reader.TokenType == JsonTokenType.EndObject)
						return rangeValue;

					if (reader.TokenType != JsonTokenType.PropertyName)
						continue;

					string propertyName = reader.GetString();
					reader.Read();
					switch (propertyName)
					{
						case "min":
							rangeValue.Min = GetElementValue(ref reader, _dataType);
							break;
						case "max":
							rangeValue.Max = GetElementValue(ref reader, _dataType);
							break;
					}
				}
				throw new JsonException("Utf8JsonReader did not finished reading");
			}
			else if(typeof(TValueScope) == typeof(MultiLanguagePropertyValue))
			{
				MultiLanguagePropertyValue mlpValue = new MultiLanguagePropertyValue();

				while (reader.Read())
				{
					if (reader.TokenType == JsonTokenType.EndArray)
						return mlpValue;

					if (reader.TokenType != JsonTokenType.PropertyName)
						continue;

					string propertyName = reader.GetString();
					reader.Read();
					mlpValue.Value.AddLangString(propertyName, reader.GetString());
				}
				throw new JsonException("Utf8JsonReader did not finished reading");
			}
			else if (typeof(TValueScope) == typeof(ReferenceElementValue))
			{
				ReferenceElementValue refValue = new ReferenceElementValue();
				var reference = JsonSerializer.Deserialize<IReference>(ref reader, _jsonOptions);
				if (reference != null)
					refValue = new ReferenceElementValue(reference);

				return refValue;
				throw new JsonException("Utf8JsonReader did not finished reading");
			}
            else if (typeof(TValueScope) == typeof(RelationshipElementValue))
            {
				RelationshipElementValue relValue = new RelationshipElementValue();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                        return relValue;

                    if (reader.TokenType != JsonTokenType.PropertyName)
                        continue;

                    string propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "first":
                            relValue.First = JsonSerializer.Deserialize<IReference>(ref reader, _jsonOptions);
                            break;
                        case "second":
                            relValue.Second = JsonSerializer.Deserialize<IReference>(ref reader, _jsonOptions);
                            break;
                    }
                }
                return relValue;
                throw new JsonException("Utf8JsonReader did not finished reading");
            }
            else
			{
				throw new JsonException($"Unsupported modeltype: {typeof(TValueScope).Name}");
			}                    
        }

        public override void Write(Utf8JsonWriter writer, ValueScope value, JsonSerializerOptions options)
        {
            if (value is PropertyValue propValue)
            {
                JsonSerializer.Serialize(writer, JsonValue.Create(propValue.Value.Value), _options);
            }
			else if (value is RangeValue rangeValue)
			{
				if(_converterOptions.EnclosingObject)
					JsonSerializer.Serialize(writer, JsonValue.Create(new { Min = rangeValue.Min.Value, Max = rangeValue.Max.Value }), _options);
				else
				{
					writer.WriteString("min", rangeValue.Min.Value.ToString());
					writer.WriteString("max", rangeValue.Max.Value.ToString());
				}
			}
			else if (value is MultiLanguagePropertyValue mlpValue)
			{
				JsonArray mlpValueArray = new JsonArray();
				foreach (var item in mlpValue.Value)
				{
					JsonObject itemObj = new JsonObject
					{
						{ item.Language, JsonValue.Create(item.Text) }
					};
					mlpValueArray.Add(itemObj);
				}
				JsonSerializer.Serialize(writer, mlpValueArray, _options);
			}
			else if (value is ReferenceElementValue refValue)
			{
				JsonSerializer.Serialize(writer, refValue.Value, _options);
			}
			else if (value is RelationshipElementValue relValue)
			{
				if (_converterOptions.EnclosingObject)
				{
					JsonSerializer.Serialize(writer, JsonValue.Create(new 
					{ 
						First = JsonValue.Create(relValue.First), 
						Second = JsonValue.Create(relValue.Second)
					}), _options);
				}					
				else
				{
					writer.WritePropertyName("first");
					JsonSerializer.Serialize(writer, relValue.First, _options);

					writer.WritePropertyName("second");
					JsonSerializer.Serialize(writer, relValue.Second, _options);
				}
			}
		}

		public ElementValue GetElementValue(ref Utf8JsonReader reader, DataType dataType = null)
		{
			if (reader.TokenType == JsonTokenType.Number)
			{
				ReadOnlySpan<byte> span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
				if (Utf8Parser.TryParse(span, out double number, out int bytesConsumed) && span.Length == bytesConsumed)
					return new ElementValue(number, dataType ?? typeof(double));
				throw new JsonException($"Unsupported NumberFormat");
			}
			else if (reader.TokenType == JsonTokenType.String)
			{
				string value = reader.GetString();
				return new ElementValue(value, dataType ?? typeof(string));
			}
			else if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
			{
				bool value = reader.GetBoolean();
				return new ElementValue(value, dataType ?? typeof(bool));
			}
			else
			{
				throw new JsonException($"Unsupported TokenType: {reader.TokenType}");
			}
		}
    }
}
