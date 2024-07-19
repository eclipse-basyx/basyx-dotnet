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
        public FullSubmodelElementConverter(SubmodelElementConverterOptions options = null) : base(options)
        {
        }

        public override ISubmodelElement Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return base.Read(ref reader, typeToConvert, options);
        }

        public override void Write(Utf8JsonWriter writer, ISubmodelElement value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            WriteMetadata(writer, value, options, new SubmodelElementConverterOptions());

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
                    var beeValue = bee.GetValueScope<BasicEventElementValue>();
                    if (beeValue != null)
                    {
                        writer.WritePropertyName("observed");
                        JsonSerializer.Serialize(writer, beeValue, new JsonSerializerOptions()
                        {
                            Converters = { new ValueScopeConverter<BasicEventElementValue>(jsonOptions: options) }
                        });
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
                    var entityValue = entity.GetValueScope<EntityValue>();
                    if (entityValue != null)
                    {
                        JsonSerializer.Serialize(writer, entityValue, new JsonSerializerOptions()
                        {
                            Converters =
                            {
                                new ValueScopeConverter<EntityValue>(options: new ValueScopeConverterOptions()
                                {
                                    EnclosingObject = false
                                }, jsonOptions: options)
                            }
                        });
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
                    if (_converterOptions.RequestLevel == RequestLevel.Deep || (_converterOptions.RequestLevel == RequestLevel.Core && _converterOptions.Level == 0))
                    {
                        var smcValue = smc.Get?.Invoke(smc).Result;

                        if (_converterOptions.RequestLevel == RequestLevel.Core && _converterOptions.Level == 0)
                            _converterOptions.Level++;

                        JsonSerializer.Serialize(writer, smcValue, new JsonSerializerOptions()
                        {
                            Converters =
                            {
                                new ValueScopeConverter<SubmodelElementCollectionValue>(options: new ValueScopeConverterOptions()
                                {
                                    EnclosingObject = false,
                                    SerializationOption = SerializationOption.FullModel,
                                    RequestLevel = _converterOptions.RequestLevel,
                                    RequestExtent = _converterOptions.RequestExtent,
                                    Level = _converterOptions.Level
                                }, jsonOptions: options)
                            }
                        });
                    }
                    break;
                case ModelTypes.SubmodelElementList:
                    var sml = (SubmodelElementList)value;
                    if (_converterOptions.RequestLevel == RequestLevel.Deep || (_converterOptions.RequestLevel == RequestLevel.Core && _converterOptions.Level == 0))
                    {
                        var smlValue = sml.Get?.Invoke(sml).Result;

                        if (_converterOptions.RequestLevel == RequestLevel.Core && _converterOptions.Level == 0)
                            _converterOptions.Level++;

                        JsonSerializer.Serialize(writer, smlValue, new JsonSerializerOptions()
                        {
                            Converters =
                            {
                                new ValueScopeConverter<SubmodelElementListValue>(options: new ValueScopeConverterOptions()
                                {
                                    EnclosingObject = false,
                                    SerializationOption = SerializationOption.FullModel
                                }, jsonOptions: options)
                            }
                        });
                    }
                    break;
                default:
                    break;
            }

            writer.WriteEndObject();
        }
    }
}
