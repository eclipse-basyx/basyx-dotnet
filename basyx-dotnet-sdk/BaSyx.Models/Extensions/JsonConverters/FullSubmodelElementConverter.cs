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
using System.Text.Json;
using Range = BaSyx.Models.AdminShell.Range;

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
                    {
                        writer.WritePropertyName("value");
                        JsonSerializer.Serialize(writer, propValue, new JsonSerializerOptions()
                        {
                            Converters =
                            {
                                new ValueScopeConverter<PropertyValue>(options: new ValueScopeConverterOptions()
                                {
                                    ValueAsString = true
                                }, jsonOptions: options)
                            }
                        });
                    }
                    break;
                case ModelTypes.AnnotatedRelationshipElement:
                    var are = (AnnotatedRelationshipElement)value;
                    var areValue = are.GetValueScope<AnnotatedRelationshipElementValue>();
                    if(areValue != null)
                    {
                        JsonSerializer.Serialize(writer, areValue, new JsonSerializerOptions()
                        {
                            Converters =
                            {
                                new ValueScopeConverter<AnnotatedRelationshipElementValue>(options: new ValueScopeConverterOptions()
                                {
                                    EnclosingObject = false
                                }, jsonOptions: options)
                            }
                        });
                    }
                    break;
                case ModelTypes.RelationshipElement:
                    var re = (RelationshipElement)value;
                    var reValue = re.GetValueScope<RelationshipElementValue>();
                    if (reValue != null)
                    {
                        JsonSerializer.Serialize(writer, reValue, new JsonSerializerOptions()
                        {
                            Converters =
                            {
                                new ValueScopeConverter<RelationshipElementValue>(options: new ValueScopeConverterOptions()
                                {
                                    EnclosingObject = false
                                })
                            }
                        });
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
                    var blobValue = blob.GetValueScope<BlobValue>();
                    if (blobValue != null)
                    {
                        JsonSerializer.Serialize(writer, blobValue, new JsonSerializerOptions()
                        {
                            Converters =
                            {
                                new ValueScopeConverter<BlobValue>(options: new ValueScopeConverterOptions()
                                {
                                    EnclosingObject = false
                                })
                            }
                        });
                    }                
                    break;
                case ModelTypes.File:
                    var file = (FileElement)value;
                    var fileElementValue = file.GetValueScope<FileElementValue>();
                    if (fileElementValue != null)
                    {
                        JsonSerializer.Serialize(writer, fileElementValue, new JsonSerializerOptions()
                        {
                            Converters =
                            {
                                new ValueScopeConverter<FileElementValue>(options: new ValueScopeConverterOptions()
                                {
                                    EnclosingObject = false
                                })
                            }
                        });
                    }
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
					var mlpValue = mlp.GetValueScope<MultiLanguagePropertyValue>();
					if (mlpValue != null)
                    {
						writer.WritePropertyName("value");
						JsonSerializer.Serialize(writer, mlpValue, new JsonSerializerOptions()
                        {
                            Converters = { new ValueScopeConverter<MultiLanguagePropertyValue>() }
                        });
					}				   
                    break;
                case ModelTypes.Range:
                    var range = (Range)value;
                    var rangeValue = range.GetValueScope<RangeValue>();
                    if (rangeValue != null)
                    {
						JsonSerializer.Serialize(writer, rangeValue, new JsonSerializerOptions()
						{
							Converters = 
                            { 
                                new ValueScopeConverter<RangeValue>(null, range.ValueType, new ValueScopeConverterOptions()
                                {
                                    EnclosingObject = false
                                }) 
                            }
						});
					}
                    break;
                case ModelTypes.ReferenceElement:
                    var refElem = (ReferenceElement)value;
					var refValue = refElem.GetValueScope<ReferenceElementValue>();
					if (refValue != null)
                    {
                        writer.WritePropertyName("value");
						JsonSerializer.Serialize(writer, refValue, new JsonSerializerOptions()
						{
							Converters = { new ValueScopeConverter<ReferenceElementValue>(jsonOptions: options) }
						});
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
