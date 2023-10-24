using BaSyx.Models.AdminShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace BaSyx.Models.Extensions
{
    public class FullSubmodelElementConverter : SubmodelElementConverter
    {
        public override ISubmodelElement Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return base.Read(ref reader, typeToConvert, options);
        }

        public override void Write(Utf8JsonWriter writer, ISubmodelElement value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            WriteBaseObject(writer, value, options);

            switch (value.ModelType.Type)
            {
                case ModelTypes.Property:
                    var property = (Property)value;
                    var propValue = property.GetValueScope<PropertyValue>();
                    if (propValue != null)
                        writer.WriteString("value", propValue.ToString());
                    break;
                case ModelTypes.AnnotatedRelationshipElement:
                    var are = (AnnotatedRelationshipElement)value;
                    if (are.First != null)
                    {
                        writer.WritePropertyName("first");
                        JsonSerializer.Serialize(writer, are.First, options);
                    }
                    if (are.Second != null)
                    {
                        writer.WritePropertyName("second");
                        JsonSerializer.Serialize(writer, are.Second, options);
                    }
                    if (are.Annotations != null)
                    {
                        writer.WritePropertyName("annotations");
                        JsonSerializer.Serialize(writer, are.Annotations, options);
                    }
                    break;
                case ModelTypes.RelationshipElement:
                    var re = (RelationshipElement)value;
                    if (re.First != null)
                    {
                        writer.WritePropertyName("first");
                        JsonSerializer.Serialize(writer, re.First, options);
                    }
                    if (re.Second != null)
                    {
                        writer.WritePropertyName("second");
                        JsonSerializer.Serialize(writer, re.Second, options);
                    }
                    break;
                case ModelTypes.BasicEventElement:
                    var bee = (BasicEventElement)value;
                    if (bee.Observed != null)
                    {
                        writer.WritePropertyName("observed");
                        JsonSerializer.Serialize(writer, bee.Observed, options);
                    } 
                    break;
                case ModelTypes.Blob:
                    var blob = (Blob)value;
                    if (!string.IsNullOrEmpty(blob.ContentType))
                        writer.WriteString("contentType", blob.ContentType);
                    if (!string.IsNullOrEmpty(blob.Value))
                        writer.WriteString("value", blob.Value);
                    break;
                case ModelTypes.File:
                    var file = (FileElement)value;
                    if (!string.IsNullOrEmpty(file.ContentType))
                        writer.WriteString("contentType", file.ContentType);
                    if (!string.IsNullOrEmpty(file.Value))
                        writer.WriteString("value", file.Value);
                    break;
                case ModelTypes.Entity:
                    var entity = (Entity)value;
                    if (entity.Statements?.Count > 0)
                    {
                        writer.WritePropertyName("statements");
                        JsonSerializer.Serialize(writer, entity.Statements, options);
                    }
                    if (entity.EntityType != EntityType.None)
                    {
                        writer.WritePropertyName("entityType");
                        JsonSerializer.Serialize(writer, entity.EntityType, options);
                    }
                    break;
                case ModelTypes.MultiLanguageProperty:
                    var mlp = (MultiLanguageProperty)value;
                    if (mlp.Value?.Count() > 0)
                    {
                        writer.WritePropertyName("value");
                        JsonSerializer.Serialize(writer, mlp.Value, options);
                    }
                    break;
                case ModelTypes.Range:
                    var range = (Range)value;
                    var rangeValue = range.GetValueScope<RangeValue>();
                    if (rangeValue != null)
                    {
                        writer.WriteString("min", rangeValue.Min.Value.ToString());
                        writer.WriteString("max", rangeValue.Max.Value.ToString());
                    }
                    break;
                case ModelTypes.ReferenceElement:
                    var refElem = (ReferenceElement)value;
                    if (refElem.Value != null)
                    {
                        writer.WritePropertyName("value");
                        JsonSerializer.Serialize(writer, refElem.Value, options);
                    }
                    break;
                case ModelTypes.SubmodelElementCollection:
                    var smc = (SubmodelElementCollection)value;
                    if (smc.Value != null)
                    {
                        writer.WritePropertyName("value");
                        JsonSerializer.Serialize(writer, smc.Value, options);
                    }
                    break;
                case ModelTypes.SubmodelElementList:
                    var sml = (SubmodelElementList)value;
                    if (sml.Value != null)
                    {
                        writer.WritePropertyName("value");
                        JsonSerializer.Serialize(writer, sml.Value, options);
                    }
                    break;
                default:
                    break;
            }

            writer.WriteEndObject();
        }
    }
}
