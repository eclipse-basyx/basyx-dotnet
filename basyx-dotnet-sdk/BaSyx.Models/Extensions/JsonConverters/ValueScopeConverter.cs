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
using System.Reflection.Metadata;
using System.Xml;
using System.Linq;

namespace BaSyx.Models.Extensions
{
	public enum SerializationOption
	{
		FullModel,
		ValueOnly
	}
	public class ValueScopeConverterOptions
	{
		public bool EnclosingObject { get; set; } = true;
        public bool ValueAsString { get; set; } = false;
        public SerializationOption SerializationOption { get; set; } = SerializationOption.FullModel;
    }
	public class ValueScopeConverter : ValueScopeConverter<ValueScope> 
	{ 
		public ValueScopeConverter(ValueScopeConverterOptions options = null, JsonSerializerOptions jsonOptions = null):
			base(null, null, options, jsonOptions)
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
		private ISubmodelElement _sme;

		public ValueScopeConverter(ISubmodelElement sme = null, DataType dataType = null, ValueScopeConverterOptions options = null, JsonSerializerOptions jsonOptions = null) 
		{
			_dataType = dataType;
			_converterOptions = options ?? new ValueScopeConverterOptions();
			_jsonOptions = jsonOptions;
			_sme = sme;
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
            if(typeof(TValueScope) == typeof(PropertyValue) || typeToConvert == typeof(PropertyValue))
            {
				ElementValue elementValue = GetElementValue(ref reader, _dataType);
				return new PropertyValue(elementValue);
			}
            else if (typeof(TValueScope) == typeof(SubmodelElementCollectionValue) || typeToConvert == typeof(SubmodelElementCollectionValue))
            {
                var smc = _sme as SubmodelElementCollection;
                var elementContainer = smc.Value.Value;
                UpdateSmcElementContainer(ref elementContainer, ref reader, _jsonOptions, JsonTokenType.StartObject, JsonTokenType.EndObject);
                return smc.Value;
            }
            else if (typeof(TValueScope) == typeof(SubmodelElementListValue) || typeToConvert == typeof(SubmodelElementListValue))
            {
                var sml = _sme as SubmodelElementList;
                var elementContainer = sml.Value.Value;
                UpdateSmlElementContainer(ref elementContainer, ref reader, _jsonOptions, JsonTokenType.StartArray, JsonTokenType.EndArray);
                return sml.Value;
            }
            else if (typeof(TValueScope) == typeof(RangeValue) || typeToConvert == typeof(RangeValue))
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
			else if(typeof(TValueScope) == typeof(MultiLanguagePropertyValue) || typeToConvert == typeof(MultiLanguagePropertyValue))
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
			else if (typeof(TValueScope) == typeof(ReferenceElementValue) || typeToConvert == typeof(ReferenceElementValue))
			{
				ReferenceElementValue refValue = new ReferenceElementValue();
				var reference = JsonSerializer.Deserialize<IReference>(ref reader, _jsonOptions);
				if (reference != null)
					refValue = new ReferenceElementValue(reference);

				return refValue;
				throw new JsonException("Utf8JsonReader did not finished reading");
			}
            else if (typeof(TValueScope) == typeof(RelationshipElementValue) || typeToConvert == typeof(RelationshipElementValue))
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
                throw new JsonException("Utf8JsonReader did not finished reading");
            }
            else if (typeof(TValueScope) == typeof(AnnotatedRelationshipElementValue) || typeToConvert == typeof(AnnotatedRelationshipElementValue))
            {
                AnnotatedRelationshipElementValue arelValue = new AnnotatedRelationshipElementValue();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                        return arelValue;

                    if (reader.TokenType != JsonTokenType.PropertyName)
                        continue;

                    string propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "first":
                            arelValue.First = JsonSerializer.Deserialize<IReference>(ref reader, _jsonOptions);
                            break;
                        case "second":
                            arelValue.Second = JsonSerializer.Deserialize<IReference>(ref reader, _jsonOptions);
                            break;
                        case "annotations":
							if(_converterOptions.SerializationOption == SerializationOption.FullModel)
								arelValue.Annotations = JsonSerializer.Deserialize<IElementContainer<ISubmodelElement>>(ref reader, _jsonOptions);
							else
                            {
                                var annotations = (_sme as AnnotatedRelationshipElement).Value.Annotations;
                                UpdateSmcElementContainer(ref annotations, ref reader, _jsonOptions, JsonTokenType.StartArray, JsonTokenType.EndArray);
                                arelValue.Annotations = annotations;
                            }								
                            break;
                    }
                }
                throw new JsonException("Utf8JsonReader did not finished reading");
            }
            else if (typeof(TValueScope) == typeof(BlobValue) || typeToConvert == typeof(BlobValue))
            {
                BlobValue blobValue = new BlobValue();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                        return blobValue;

                    if (reader.TokenType != JsonTokenType.PropertyName)
                        continue;

                    string propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "contentType":
                            blobValue.ContentType = reader.GetString();
                            break;
                        case "value":
                            blobValue.Value = reader.GetString();
                            break;
                    }
                }
                throw new JsonException("Utf8JsonReader did not finished reading");
            }
            else if (typeof(TValueScope) == typeof(FileElementValue) || typeToConvert == typeof(FileElementValue))
            {
                FileElementValue fileElementValue = new FileElementValue();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                        return fileElementValue;

                    if (reader.TokenType != JsonTokenType.PropertyName)
                        continue;

                    string propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "contentType":
                            fileElementValue.ContentType = reader.GetString();
                            break;
                        case "value":
                            fileElementValue.Value = reader.GetString();
                            break;
                    }
                }
                throw new JsonException("Utf8JsonReader did not finished reading");
            }
            else
			{
				throw new JsonException($"Unsupported modeltype: {typeof(TValueScope).Name}");
			}                    
        }

        private void UpdateSmlElementContainer(ref IElementContainer<ISubmodelElement> sourceContainer, ref Utf8JsonReader reader, JsonSerializerOptions jsonOptions, JsonTokenType startToken, JsonTokenType endToken)
        {
            if (sourceContainer == null || sourceContainer.Children?.Count() == 0)
                throw new JsonException("SubmodelElement-SourceContainer is null");

            while (reader.TokenType != startToken)
                reader.Read();

            int i = 0;
            while (reader.Read())
            {
                if (reader.TokenType == endToken || i == sourceContainer.Children.Count())
                    return;

                object value = null;
                switch(reader.TokenType)
                {
                    case JsonTokenType.True:
                    case JsonTokenType.False: value = reader.GetBoolean();break;
                    case JsonTokenType.String: value = reader.GetString(); break;
                    case JsonTokenType.Number: value = reader.GetDouble(); break;
                }

                var sme = sourceContainer.Children.ElementAt(i).Value;
                if (sme.ModelType == ModelType.Property)
                {
                    PropertyValue propertyValue = new PropertyValue(new ElementValue(value));
                    sme.SetValueScope(propertyValue);
                }
                i++;
            }
            throw new JsonException("Malformed json");
        }

        private void UpdateSmcElementContainer(ref IElementContainer<ISubmodelElement> sourceContainer, ref Utf8JsonReader reader, JsonSerializerOptions jsonOptions, JsonTokenType startToken, JsonTokenType endToken)
        {
			if(sourceContainer == null)
                throw new JsonException("SubmodelElement-SourceContainer is null");

            while (reader.TokenType != startToken)
                reader.Read();

            while (reader.Read())
            {
                if (reader.TokenType == endToken)
                    return;

                if(reader.TokenType != JsonTokenType.PropertyName)
                        continue;

                string propertyName = reader.GetString();
                reader.Read();

				var sme = sourceContainer.GetChild(propertyName).Value;
				if(sme.ModelType == ModelType.Property)
				{
					var valueScope = Read(ref reader, typeof(PropertyValue), jsonOptions);
                    sme.SetValueScope(valueScope);
                }
            }
            throw new JsonException("Malformed json");
        }

        public override void Write(Utf8JsonWriter writer, ValueScope value, JsonSerializerOptions options)
        {
            if (value is PropertyValue propValue)
            {
				if(_converterOptions.ValueAsString)
					writer.WriteStringValue(propValue.Value.ToString());                
				else
					JsonSerializer.Serialize(writer, JsonValue.Create(propValue.Value.Value), _options);
            }
            else if (value is SubmodelElementCollectionValue smcValue)
            {
                if (_converterOptions.SerializationOption == SerializationOption.FullModel)
                {
                    writer.WritePropertyName("value");
                    writer.WriteStartArray();
                    foreach (var smcElement in smcValue.Value)
                    {
                        JsonSerializer.Serialize(writer, smcElement, _jsonOptions);
                    }
                    writer.WriteEndArray();
                }
                else if (_converterOptions.SerializationOption == SerializationOption.ValueOnly)
                {
                    writer.WriteStartObject();
                    foreach (var smcElement in smcValue.Value)
                    {
                        var smcElementValueScope = smcElement.GetValueScope().Result;
                        writer.WritePropertyName(smcElement.IdShort);
                        Write(writer, smcElementValueScope, _options);
                    }
                    writer.WriteEndObject();
                }
            }
            else if (value is SubmodelElementListValue smlValue)
            {
                if (_converterOptions.SerializationOption == SerializationOption.FullModel)
                {
                    writer.WritePropertyName("value");
                    writer.WriteStartArray();
                    foreach (var smcElement in smlValue.Value)
                    {
                        JsonSerializer.Serialize(writer, smcElement, _jsonOptions);
                    }
                    writer.WriteEndArray();
                }
                else if (_converterOptions.SerializationOption == SerializationOption.ValueOnly)
                {
                    writer.WriteStartArray();
                    foreach (var smcElement in smlValue.Value)
                    {
                        var smcElementValueScope = smcElement.GetValueScope().Result;
                        Write(writer, smcElementValueScope, _options);
                    }
                    writer.WriteEndArray();
                }
            }
            else if (value is RangeValue rangeValue)
			{
				if(_converterOptions.EnclosingObject)
					JsonSerializer.Serialize(writer, JsonValue.Create(new { Min = rangeValue.Min.Value, Max = rangeValue.Max.Value }), _options);
				else
				{
					writer.WriteString("min", rangeValue.Min.ToString());
					writer.WriteString("max", rangeValue.Max.ToString());
				}
			}
			else if (value is MultiLanguagePropertyValue mlpValue)
			{
                writer.WriteStartArray();
                foreach (var langPair in mlpValue.Value)
                {
                    writer.WriteStartObject();
                    writer.WriteString(langPair.Language, langPair.Text);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
			}
			else if (value is ReferenceElementValue refValue)
			{
				JsonSerializer.Serialize(writer, refValue.Value, _options);
			}
            else if (value is AnnotatedRelationshipElementValue arelValue)
            {
                if (_converterOptions.EnclosingObject)
					writer.WriteStartObject();

                writer.WritePropertyName("first");
                JsonSerializer.Serialize(writer, arelValue.First, _options);

                writer.WritePropertyName("second");
                JsonSerializer.Serialize(writer, arelValue.Second, _options);

                writer.WritePropertyName("annotations");
                writer.WriteStartArray();
                foreach (var annotation in arelValue.Annotations)
                {
					if (_converterOptions.SerializationOption == SerializationOption.FullModel)
					{
						JsonSerializer.Serialize(writer, annotation, _jsonOptions);
					}
                    else if (_converterOptions.SerializationOption == SerializationOption.ValueOnly)
					{
                        var annotationValueScope = annotation.GetValueScope().Result;
                        writer.WriteStartObject();
                        writer.WritePropertyName(annotation.IdShort);
                        Write(writer, annotationValueScope, _options);
                        writer.WriteEndObject();
                    }                   
                }
                writer.WriteEndArray();

                if (_converterOptions.EnclosingObject)
                    writer.WriteEndObject();
            }
            else if (value is RelationshipElementValue relValue)
			{
                if (_converterOptions.EnclosingObject)
                    writer.WriteStartObject();

                writer.WritePropertyName("first");
                JsonSerializer.Serialize(writer, relValue.First, _options);

                writer.WritePropertyName("second");
                JsonSerializer.Serialize(writer, relValue.Second, _options);

                if (_converterOptions.EnclosingObject)
                    writer.WriteEndObject();
			}
            else if (value is BlobValue blobValue)
            {
                if (_converterOptions.EnclosingObject)
                    writer.WriteStartObject();

                writer.WriteString("contentType", blobValue.ContentType);
                writer.WriteString("value", blobValue.Value);

                if (_converterOptions.EnclosingObject)
                    writer.WriteEndObject();
            }
            else if (value is FileElementValue fileElementValue)
            {
                if (_converterOptions.EnclosingObject)
                    writer.WriteStartObject();

                writer.WriteString("contentType", fileElementValue.ContentType);
                writer.WriteString("value", fileElementValue.Value);

                if (_converterOptions.EnclosingObject)
                    writer.WriteEndObject();
            }
        }

		public ElementValue GetElementValue(ref Utf8JsonReader reader, DataType dataType = null)
		{
			if (reader.TokenType == JsonTokenType.Number)
			{
                //Eventually requestBody.GetRawText() passed to ElementValue instead of parsed Double value
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
