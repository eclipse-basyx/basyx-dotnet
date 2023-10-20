using BaSyx.Models.AdminShell;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace BaSyx.Models.Extensions
{
    public class ValueOnlyConverter : JsonConverter<IElementContainer<ISubmodelElement>>
    {
        public override IElementContainer<ISubmodelElement> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, IElementContainer<ISubmodelElement> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var smElement in value)
            {
                switch (smElement.ModelType.Type)
                {
                    case ModelTypes.SubmodelElementCollection:
                        {
                            ISubmodelElementCollection submodelElementCollection = smElement.Cast<ISubmodelElementCollection>();
                            writer.WritePropertyName(submodelElementCollection.IdShort);
                            Write(writer, submodelElementCollection.Value, options);
                            break;
                        }
                    case ModelTypes.RelationshipElement:
                        {
                            IRelationshipElement relationshipElement = smElement.Cast<IRelationshipElement>();
                            writer.WritePropertyName(relationshipElement.IdShort);
                            writer.WriteStartObject();
                            writer.WriteString("first", relationshipElement.First.ToStandardizedString());
                            writer.WriteString("second", relationshipElement.Second.ToStandardizedString());
                            writer.WriteEndObject();
                            break;
                        }
                    case ModelTypes.AnnotatedRelationshipElement:
                        {
                            IAnnotatedRelationshipElement annotatedRelationshipElement = smElement.Cast<IAnnotatedRelationshipElement>();
                            writer.WritePropertyName(annotatedRelationshipElement.IdShort);
                            writer.WriteStartObject();
                            writer.WriteString("first", annotatedRelationshipElement.First.ToStandardizedString());
                            writer.WriteString("second", annotatedRelationshipElement.Second.ToStandardizedString());
                            writer.WritePropertyName("annotations");
                            Write(writer, annotatedRelationshipElement.Annotations, options);
                            writer.WriteEndObject();
                            break;
                        }
                    case ModelTypes.Property:
                        {
                            IProperty property = smElement.Cast<IProperty>();
                            var valueScope = property.GetValueScope<PropertyValue>();
                            writer.WritePropertyName(property.IdShort);
                            var jValue = JsonValue.Create(valueScope.Value.Value);
                            writer.WriteRawValue(jValue.ToString());
                            break;
                        }
                    case ModelTypes.File:
                        {
                            IFileElement file = smElement.Cast<IFileElement>();
                            writer.WritePropertyName(file.IdShort);
                            writer.WriteStartObject();
                            writer.WriteString("contentType", file.ContentType);
                            writer.WriteString("value", file.Value);
                            writer.WriteEndObject();
                            break;
                        }
                    case ModelTypes.Blob:
                        {
                            IBlob blob = smElement.Cast<IBlob>();
                            writer.WritePropertyName(blob.IdShort);
                            writer.WriteStartObject();
                            writer.WriteString("contentType", blob.ContentType);
                            writer.WriteString("value", blob.Value);
                            writer.WriteEndObject();
                            break;
                        }
                    case ModelTypes.ReferenceElement:
                        {
                            IReferenceElement referenceElement = smElement.Cast<IReferenceElement>();
                            writer.WriteString(referenceElement.IdShort, referenceElement.Value.ToStandardizedString());
                            break;
                        }
                    case ModelTypes.MultiLanguageProperty:
                        {
                            IMultiLanguageProperty multiLanguageProperty = smElement.Cast<IMultiLanguageProperty>();
                            writer.WritePropertyName(multiLanguageProperty.IdShort);
                            writer.WriteStartArray();
                            foreach (var langPair in multiLanguageProperty.Value)
                            {
                                writer.WriteStartObject();
                                writer.WriteString(langPair.Language, langPair.Text);
                                writer.WriteEndObject();
                            }
                            writer.WriteEndArray();
                            break;
                        }
                    case ModelTypes.Range:
                        {
                            IRange range = smElement.Cast<IRange>();
                            var valueScope = range.GetValueScope<RangeValue>();
                            writer.WritePropertyName(range.IdShort);
                            writer.WriteStartObject();
                            var jValueMin = JsonValue.Create(valueScope.Min.Value);
                            var jValueMax = JsonValue.Create(valueScope.Max.Value);
                            writer.WritePropertyName("min");
                            writer.WriteRawValue(jValueMin.ToString());
                            writer.WritePropertyName("max");
                            writer.WriteRawValue(jValueMax.ToString());
                            writer.WriteEndObject();
                            break;
                        }
                    case ModelTypes.Entity:
                        {
                            IEntity entity = smElement.Cast<IEntity>();
                            writer.WritePropertyName(entity.IdShort);
                            writer.WriteStartObject();
                            writer.WritePropertyName("statements");
                            Write(writer, entity.Statements, options);
                            writer.WriteString("entityType", entity.EntityType.ToString());
                            writer.WriteString("globalAssetId", entity.GlobalAssetId.Id);
                            writer.WriteEndObject(); 
                            break;
                        }
                    default:
                        break;
                }
            }

            writer.WriteEndObject();
        }
    }
}
