using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace BaSyx.Utils.Json
{
	public class ObjectJsonConverter : JsonConverter<object>
	{
		public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
		{
			if (value.GetType() == typeof(object))
			{
				writer.WriteStartObject();
				writer.WriteEndObject();
			}
			else
				JsonSerializer.Serialize(writer, value, value.GetType(), options);
		}

		public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			switch (reader.TokenType)
			{				
                case JsonTokenType.True:
                    return true;
                case JsonTokenType.False:
					return false;				
				case JsonTokenType.String:
					return reader.GetString();
				case JsonTokenType.Number:
					{
						if (reader.TryGetInt32(out int iValue))
							return iValue;
						if (reader.TryGetInt64(out long i64Value))
                            return i64Value;
						else if (reader.TryGetDouble(out double dValue))
							return dValue;
						else
						{
							using (var jDocument = JsonDocument.ParseValue(ref reader))
                            {
                                return jDocument.RootElement.Clone();
                            };                           
                        }						
					}
                case JsonTokenType.StartObject:
                    Dictionary<string, object> objDict = new Dictionary<string, object>();
                    while (reader.Read())
                    {
                        switch (reader.TokenType)
                        {
                            case JsonTokenType.EndObject:
                                return objDict;
                            case JsonTokenType.PropertyName:
                                string keyName = reader.GetString();
                                reader.Read();
                                var obj = Read(ref reader, typeof(object), options);
                                objDict.Add(keyName, obj);
                                break;
                            default:
                                throw new JsonException();
                        }
                    }
                    throw new JsonException();
                case JsonTokenType.StartArray:
					{
                        List<object> arrayList = new List<object>();
						while (reader.Read())
						{
							switch (reader.TokenType)
							{
								default:
									var obj = Read(ref reader, typeof(object), options);
                                    arrayList.Add(obj);
									break;
								case JsonTokenType.EndArray:
									return arrayList;
							}
						}
						throw new JsonException();
					}				
                case JsonTokenType.Null:
                    return null;
                default:
					throw new JsonException($"TokenType unknown: {reader.TokenType}");
			}
		}		
	}
}
