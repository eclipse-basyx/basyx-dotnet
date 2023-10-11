using BaSyx.Models.AdminShell;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace BaSyx.Models.Extensions
{
    public class SubmodelElementConverterSystemTextJson : JsonConverter<ISubmodelElement>
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<SubmodelElementConverterSystemTextJson>();

        public override ISubmodelElement Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Utf8JsonReader jsonReader = reader;

            if (jsonReader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Json does not start with {");            

            string idShort = null, modelType = null, valueType = null;
            while(jsonReader.Read())
            {
                if (jsonReader.TokenType == JsonTokenType.EndObject)
                    break;

                if (jsonReader.TokenType != JsonTokenType.PropertyName)
                    continue;
                string propertyName = jsonReader.GetString();
                jsonReader.Read();
                switch (propertyName)
                {
                    case "idShort":
                        idShort = jsonReader.GetString();
                        break;
                    case "modelType":
                        modelType = jsonReader.GetString();
                        break;
                    case "valueType":
                        valueType = jsonReader.GetString();
                        break;
                }
            }
            if (idShort == null || modelType == null)
                throw new JsonException("IdShort or ModelType is null");

            SubmodelElement submodelElement = SubmodelElementFactory.CreateSubmodelElement(idShort, modelType, valueType);

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return submodelElement;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    continue;

                string propertyName = reader.GetString();
                reader.Read();
                switch (propertyName)
                {
                    case "idShort":
                    case "modelType":
                    case "valueType":
                        continue;
                    case "value":
                        string value = reader.GetString();
                        if (value != null)
                        {
                            if (modelType == ModelType.Property)
                            {
                                PropertyValue propertyValue = new PropertyValue(new ElementValue(value, valueType));
                                _ = submodelElement.SetValueScope(propertyValue);
                            }
                        }
                        break;
                }
            }
            
            return submodelElement;
        }

        public override void Write(Utf8JsonWriter writer, ISubmodelElement value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            WriteBaseObject(writer, value, options);

            writer.WriteEndObject();
        }

        public static void WriteBaseObject(Utf8JsonWriter writer, ISubmodelElement value, JsonSerializerOptions options)
        {
            writer.WriteString("idShort", value.IdShort);
            writer.WriteString("kind", value.Kind.ToString());
            writer.WriteString("modelType", value.ModelType.ToString());

            if (!string.IsNullOrEmpty(value.Category))
                writer.WriteString("idShort", value.Category);

            if (value.Description?.Count > 0)
            {
                writer.WritePropertyName("description");
                JsonSerializer.Serialize(writer, value.Description, options);
            }

            if (value.DisplayName?.Count > 0)
            {
                writer.WritePropertyName("displayName");
                JsonSerializer.Serialize(writer, value.DisplayName, options);
            }

            if (value.SemanticId != null)
            {
                writer.WritePropertyName("semanticId");
                JsonSerializer.Serialize(writer, value.SemanticId, options);
            }

            if (value.SupplementalSemanticIds?.Count() > 0)
            {
                writer.WritePropertyName("supplementalSemanticIds");
                JsonSerializer.Serialize(writer, value.SupplementalSemanticIds, options);
            }

            if (value.Qualifiers?.Count() > 0)
            {
                writer.WritePropertyName("qualifiers");
                JsonSerializer.Serialize(writer, value.Qualifiers, options);
            }

            switch (value.ModelType.Type)
            {
                case ModelTypes.Property:
                    var property = (Property)value;
                    if (property.ValueId != null)
                    {
                        writer.WritePropertyName("valueId");
                        JsonSerializer.Serialize(writer, property.ValueId, options);
                    }
                    if (property.ValueType != null)
                        writer.WriteString("valueType", property.ValueType.ToString());
                    break;
                case ModelTypes.BasicEventElement:
                    var bee = (BasicEventElement)value;
                    if (bee.ObservableReference != null)
                    {
                        writer.WritePropertyName("observableReference");
                        JsonSerializer.Serialize(writer, bee.ObservableReference, options);
                    }
                    if (bee.Direction != null)
                    {
                        writer.WritePropertyName("direction");
                        JsonSerializer.Serialize(writer, bee.Direction, options);
                    }
                    if (bee.State != null)
                    {
                        writer.WritePropertyName("state");
                        JsonSerializer.Serialize(writer, bee.State, options);
                    }
                    if (!string.IsNullOrEmpty(bee.MessageTopic))
                        writer.WriteString("messageTopic", bee.MessageTopic);
                    if (bee.MessageBroker != null)
                    {
                        writer.WritePropertyName("messageBroker");
                        JsonSerializer.Serialize(writer, bee.MessageBroker, options);
                    }
                    if (!string.IsNullOrEmpty(bee.LastUpdate))
                        writer.WriteString("lastUpdate", bee.LastUpdate);
                    if (!string.IsNullOrEmpty(bee.MinInterval))
                        writer.WriteString("minInterval", bee.MinInterval);
                    if (!string.IsNullOrEmpty(bee.MaxInterval))
                        writer.WriteString("maxInterval", bee.MaxInterval);
                    break;
                case ModelTypes.Entity:
                    var entity = (Entity)value;   
                    if (entity.GlobalAssetId != null)
                    {
                        writer.WritePropertyName("globalAssetId");
                        JsonSerializer.Serialize(writer, entity.GlobalAssetId, options);
                    }
                    if (entity.SpecificAssetIds?.Count() > 0)
                    {
                        writer.WritePropertyName("specificAssetIds");
                        JsonSerializer.Serialize(writer, entity.SpecificAssetIds, options);
                    }
                    break;
                case ModelTypes.MultiLanguageProperty:
                    var mlp = (MultiLanguageProperty)value;
                    if (mlp.ValueId != null)
                    {
                        writer.WritePropertyName("valueId");
                        JsonSerializer.Serialize(writer, mlp.ValueId, options);
                    }
                    break;
                case ModelTypes.Operation:
                    var op = (Operation)value;
                    if (op.InputVariables.Count() > 0)
                    {
                        writer.WritePropertyName("inputVariables");
                        JsonSerializer.Serialize(writer, op.InputVariables, options);
                    }
                    if (op.InOutputVariables.Count() > 0)
                    {
                        writer.WritePropertyName("inOutputVariables");
                        JsonSerializer.Serialize(writer, op.InOutputVariables, options);
                    }
                    if (op.OutputVariables.Count() > 0)
                    {
                        writer.WritePropertyName("outputVariables");
                        JsonSerializer.Serialize(writer, op.OutputVariables, options);
                    }
                    break;
                case ModelTypes.Range:
                    var range = (Range)value;
                    if (range.ValueId != null)
                    {
                        writer.WritePropertyName("valueId");
                        JsonSerializer.Serialize(writer, range.ValueId, options);
                    }
                    if (range.ValueType != null)
                        writer.WriteString("valueType", range.ValueType.ToString());
                    break;
                case ModelTypes.SubmodelElementList:
                    var sml = (SubmodelElementList)value;
                    writer.WriteBoolean("orderRelevant", sml.OrderRelevant);
                    if (sml.SemanticIdListElement != null)
                    {
                        writer.WritePropertyName("semanticIdListElement");
                        JsonSerializer.Serialize(writer, sml.SemanticIdListElement, options);
                    }
                    if (sml.TypeValueListElement != null)
                        writer.WriteString("typeValueListElement", sml.TypeValueListElement.Name);
                    if (sml.ValueTypeListElement != null)
                        writer.WriteString("valueTypeListElement", sml.ValueTypeListElement.ToString());
                    break;
                default:
                    break;
            }
        }
    }  
}
