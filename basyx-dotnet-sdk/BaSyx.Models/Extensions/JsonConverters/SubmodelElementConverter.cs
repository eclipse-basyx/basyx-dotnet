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
using BaSyx.Utils.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Range = BaSyx.Models.AdminShell.Range;

namespace BaSyx.Models.Extensions
{
    public class SubmodelElementConverter : JsonConverter<ISubmodelElement>
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<SubmodelElementConverter>();

        public override ISubmodelElement Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Utf8JsonReader jsonReader = reader;

            if (jsonReader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Json does not start with {");            

            string idShort = null, modelType = null;
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
                    default:
                        jsonReader.Skip(); 
                        break;
                }
                if (!string.IsNullOrEmpty(idShort) && !string.IsNullOrEmpty(modelType))
                    break;
            }
            if (idShort == null || modelType == null)
                throw new JsonException("IdShort or ModelType is null");

            SubmodelElement submodelElement = SubmodelElementFactory.CreateSubmodelElement(idShort, modelType, null);
            string valueType = null;
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
                        continue;
                    case "kind":
                        submodelElement.Kind = reader.GetString().GetEnum<ModelingKind>();
                        break;
                    case "category":
                        submodelElement.Category = reader.GetString();
                        break;
                    case "description":
                        submodelElement.Description = JsonSerializer.Deserialize<LangStringSet>(ref reader, options);
                        break;
                    case "displayName":
                        submodelElement.DisplayName = JsonSerializer.Deserialize<LangStringSet>(ref reader, options);
                        break;
                    case "semanticId":
                        submodelElement.SemanticId = JsonSerializer.Deserialize<IReference>(ref reader, options);
                        break;
                    case "supplementalSemanticIds":
                        submodelElement.SupplementalSemanticIds = JsonSerializer.Deserialize<IEnumerable<IReference>>(ref reader, options);
                        break;
                    case "qualifiers":
                        submodelElement.Qualifiers = JsonSerializer.Deserialize<IEnumerable<IQualifier>>(ref reader, options);
                        break;
                    case "valueId":
                        IReference valueId = JsonSerializer.Deserialize<IReference>(ref reader, options);
                        if (submodelElement is Property prop1)
                            prop1.ValueId = valueId;
                        else if (submodelElement is Range range1)
                            range1.ValueId = valueId;
                        else if (submodelElement is MultiLanguageProperty mlp1)
                            mlp1.ValueId = valueId;
                        break;
                    case "valueType":
                        valueType = reader.GetString();
                        if(submodelElement is Property prop3)
                            prop3.ValueType = valueType;
                        if(submodelElement is Range r3)
                            r3.ValueType = valueType;
                        break;
                    #region RelationshipElement
                    case "first":
                        if (submodelElement is AnnotatedRelationshipElement arel1)
                        {
                            if (arel1.Value == null)
                                arel1.Value = new AnnotatedRelationshipElementValue();

                            arel1.Value.First = JsonSerializer.Deserialize<IReference>(ref reader, options);
                        }                            
                        else if (submodelElement is RelationshipElement rel1)
                        {
                            if (rel1.Value == null)
                                rel1.Value = new AnnotatedRelationshipElementValue();

                            rel1.Value.First = JsonSerializer.Deserialize<IReference>(ref reader, options);
                        }                           
                        break;
                    case "second":
                        if (submodelElement is AnnotatedRelationshipElement arel2)
                        {
                            if (arel2.Value == null)
                                arel2.Value = new AnnotatedRelationshipElementValue();

                            arel2.Value.Second = JsonSerializer.Deserialize<IReference>(ref reader, options);
                        }                            
                        else if (submodelElement is RelationshipElement rel2)
                        {
                            if (rel2.Value == null)
                                rel2.Value = new AnnotatedRelationshipElementValue();

                            rel2.Value.Second = JsonSerializer.Deserialize<IReference>(ref reader, options);
                        }                           
                        break;
                    case "annotations":
                        if (submodelElement is AnnotatedRelationshipElement arel3)
                        {
                            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                            {
                                ISubmodelElement sme = Read(ref reader, typeof(ISubmodelElement), options);
                                arel3.Value.Annotations.Add(sme);
                            }
                        }
                        break;
                    #endregion
                    #region File/Blob
                    case "contentType":
                        if (submodelElement is Blob b1)
                        {
                            if (b1.Value == null)
                                b1.Value = new BlobValue();

                            b1.Value.ContentType = reader.GetString();
                        }                            
                        else if (submodelElement is FileElement f1)
                        {
                            if (f1.Value == null)
                                f1.Value = new FileElementValue();

                            f1.Value.ContentType = reader.GetString();
                        }                           
                        break;
                    #endregion
                    #region Operation
                    case "inputVariables":
                        if (submodelElement is Operation op1)
                            op1.InputVariables = JsonSerializer.Deserialize<IOperationVariableSet>(ref reader, options);                      
                        break;
                    case "inoutputVariables":
                        if (submodelElement is Operation op2)
                            op2.InOutputVariables = JsonSerializer.Deserialize<IOperationVariableSet>(ref reader, options);
                        break;
                    case "outputVariables":
                        if (submodelElement is Operation op3)
                            op3.OutputVariables = JsonSerializer.Deserialize<IOperationVariableSet>(ref reader, options);
                        break;
                    #endregion
                    #region Range
                    case "min":
                        if (submodelElement is Range r1)
                        {
                            string min = reader.GetString();
                            var vc = r1.GetValueScope<RangeValue>();
                            if (vc == null)
                                _ = r1.SetValueScope(new RangeValue { Min = new ElementValue(min, r1.ValueType) });
                            else
                                vc.Min = new ElementValue(min, r1.ValueType);
                        }
                        break;
                    case "max":
                        if (submodelElement is Range r2)
                        {
                            string max = reader.GetString();
                            var vc = r2.GetValueScope<RangeValue>();
                            if (vc == null)
                                _ = r2.SetValueScope(new RangeValue { Max = new ElementValue(max, r2.ValueType) });
                            else
                                vc.Max = new ElementValue(max, r2.ValueType);
                        }
                        break;
                    #endregion
                    #region SubmodelElementList
                    case "orderRelevant":
                        if(submodelElement is SubmodelElementList sml1)
                            sml1.OrderRelevant = reader.GetBoolean();
                        break;
                    case "semanticIdListElement":
                        if (submodelElement is SubmodelElementList sml2)
                            sml2.SemanticIdListElement = JsonSerializer.Deserialize<IReference>(ref reader, options);
                        break;
                    case "typeValueListElement":
                        if (submodelElement is SubmodelElementList sml3)
                            sml3.TypeValueListElement = reader.GetString();
                        break;
                    case "valueTypeListElement":
                        if (submodelElement is SubmodelElementList sml4)
                            sml4.ValueTypeListElement = reader.GetString();
                        break;
                    #endregion
                    #region Entity
                    case "globalAssetId":
                        if (submodelElement is Entity e1)
                        {
                            if (e1.Value == null)
                                e1.Value = new EntityValue();

                            e1.Value.GlobalAssetId = reader.GetString();
                        }
                        break;
                    case "specificAssetIds":
                        if (submodelElement is Entity e2)
                        {
                            if (e2.Value == null)
                                e2.Value = new EntityValue();

                            e2.Value.SpecificAssetIds = JsonSerializer.Deserialize<IEnumerable<SpecificAssetId>>(ref reader, options);
                        }
                        break;
                    case "statements":
                        if (submodelElement is Entity e3)
                        {
                            if (e3.Value == null)
                                e3.Value = new EntityValue();

                            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                            {
                                ISubmodelElement sme = Read(ref reader, typeof(ISubmodelElement), options);
                                e3.Value.Statements.Add(sme);
                            }
                        }
                        break;
                    #endregion
                    #region BasicEventElement
                    case "observed":
                        if (submodelElement is BasicEventElement bee1)
                        {
                            var reference = JsonSerializer.Deserialize<IReference>(ref reader, options);
                            bee1.Value = new BasicEventElementValue(reference);
                        }                            
                        break;
                    case "observableReference":
                        if (submodelElement is BasicEventElement bee2)
                            bee2.ObservableReference = JsonSerializer.Deserialize<IReference>(ref reader, options);
                        break;
                    case "direction":
                        if (submodelElement is BasicEventElement bee3)
                            bee3.Direction = reader.GetString().GetEnum<EventDirection>();
                        break;
                    case "state":
                        if (submodelElement is BasicEventElement bee4)
                            bee4.State = reader.GetString().GetEnum<EventState>();
                        break;
                    case "messageTopic":
                        if (submodelElement is BasicEventElement bee5)
                            bee5.MessageTopic = reader.GetString();
                        break;
                    case "messageBroker":
                        if (submodelElement is BasicEventElement bee6)
                            bee6.MessageBroker = JsonSerializer.Deserialize<IReference>(ref reader, options);
                        break;
                    case "lastUpdate":
                        if (submodelElement is BasicEventElement bee7)
                            bee7.LastUpdate = reader.GetString();
                        break;
                    case "minInterval":
                        if (submodelElement is BasicEventElement bee8)
                            bee8.MinInterval = reader.GetString();
                        break;
                    case "maxInterval":
                        if (submodelElement is BasicEventElement bee9)
                            bee9.MaxInterval = reader.GetString();
                        break;
                    #endregion
                    case "value":
                        if (submodelElement is Property prop)
                        {
                            object value = null;
                            if (reader.TokenType == JsonTokenType.String)
                                value = reader.GetString();
                            else if (reader.TokenType == JsonTokenType.Number)
                                value = reader.GetDouble();
                            else if (reader.TokenType == JsonTokenType.True ||  reader.TokenType == JsonTokenType.False)
                                value = reader.GetBoolean();
                            else
                                value = reader.GetString();

                            if (value != null)
                            {
                                PropertyValue propertyValue = new PropertyValue(new ElementValue(value, valueType));
                                _ = submodelElement.SetValueScope(propertyValue);
                            }
                        } 
                        else if (submodelElement is SubmodelElementCollection smc)
                        {
                            JsonTokenType endToken;
                            if (reader.TokenType == JsonTokenType.StartObject)
                                endToken = JsonTokenType.EndObject;
                            else if (reader.TokenType == JsonTokenType.StartArray)
                                endToken = JsonTokenType.EndArray;
                            else
                                continue;

                            while(reader.Read() && reader.TokenType != endToken)
                            {
                                if (endToken == JsonTokenType.EndArray)
                                {
                                    ISubmodelElement sme = Read(ref reader, typeof(ISubmodelElement), options);
                                    smc.Value.Value.Add(sme);
                                }
                                else if (endToken == JsonTokenType.EndObject)
                                {
                                    string smeIdShort = reader.GetString();
                                    reader.Read();
                                    ISubmodelElement sme = GetProperty(reader, smeIdShort);
                                    smc.Value.Value.Add(sme);
                                }
                                else
                                    continue;                               
                            }
                        }
                        else if (submodelElement is SubmodelElementList sml)
                        {
                            int i = 0;
                            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                            {
                                if (reader.TokenType == JsonTokenType.StartObject)
                                {
                                    ISubmodelElement sme = Read(ref reader, typeof(ISubmodelElement), options);
                                    sml.Value.Value.Add(sme);
                                }
                                else
                                {
                                    ISubmodelElement sme = GetProperty(reader, i.ToString());
                                    sml.Value.Value.Add(sme);
                                    i++;
                                }
                            }
                        }
                        else if (submodelElement is MultiLanguageProperty mlp)
                        {
                            var langStringSet = JsonSerializer.Deserialize<LangStringSet>(ref reader, options);
                            if(langStringSet != null)
                                mlp.Value = new MultiLanguagePropertyValue(langStringSet);
                        }
                        else if (submodelElement is ReferenceElement re)
                        {
                            var reference = JsonSerializer.Deserialize<IReference>(ref reader, options);
                            if(reference != null)
							    re.Value = new ReferenceElementValue(reference);
						}
                        else if (submodelElement is Blob blob)
                        {
                            if (blob.Value == null)
                                blob.Value = new BlobValue();

                            blob.Value.Value = reader.GetString();
                        }
                        else if (submodelElement is FileElement file)
                        {
                            if (file.Value == null)
                                file.Value = new FileElementValue();

                            file.Value.Value = reader.GetString();
                        }
                        break;
                }
            }
            
            return submodelElement;
        }

        public override void Write(Utf8JsonWriter writer, ISubmodelElement value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            WriteMetadata(writer, value, options);

            writer.WriteEndObject();
        }

        public static Property GetProperty(Utf8JsonReader reader, string idShort)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.False:
                    return new Property<bool>(idShort, false);
                case JsonTokenType.True:
                    return new Property<bool>(idShort, true);
                case JsonTokenType.String:
                    return new Property<string>(idShort, reader.GetString());
                case JsonTokenType.Number:
                    {
                        if (reader.TryGetInt32(out int int32))
                            return new Property<int>(idShort, int32);
                        else if (reader.TryGetInt64(out long longVal))
                            return new Property<long>(idShort, longVal);
                        else if (reader.TryGetDouble(out double doubleVal))                        
                            return new Property<double>(idShort, doubleVal);                        
                        break;
                    }                    
            }
            return null;
        }

        public static T GetValue<T>(Utf8JsonReader reader) => (T)GetValue(reader);

        public static object GetValue(Utf8JsonReader reader)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.False:
                    return false;
                case JsonTokenType.True:
                    return true;
                case JsonTokenType.String:
                    return reader.GetString();
                case JsonTokenType.Number:
                    {
                        if (reader.TryGetInt32(out int int32))
                            return int32;
                        else if (reader.TryGetInt64(out long longVal))
                            return longVal;
                        else if (reader.TryGetDouble(out double doubleVal))
                            return doubleVal;
                        break;
                    }
            }
            return null;
        }

        public static void WriteMetadata(Utf8JsonWriter writer, ISubmodelElement value, JsonSerializerOptions options)
        {
            writer.WriteString("idShort", value.IdShort);
            writer.WriteString("kind", value.Kind.ToString());
            writer.WriteString("modelType", value.ModelType.ToString());

            if (!string.IsNullOrEmpty(value.Category))
                writer.WriteString("category", value.Category);

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
                    if (bee.Direction != EventDirection.None)
                    {
                        writer.WritePropertyName("direction");
                        JsonSerializer.Serialize(writer, bee.Direction, options);
                    }
                    if (bee.State != EventState.None)
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
                    if(entity.EntityType != EntityType.None)
                    {
                        writer.WritePropertyName("entityType");
                        JsonSerializer.Serialize(writer, entity.EntityType, options);
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
                    if (op.InputVariables?.Count() > 0)
                    {
                        writer.WritePropertyName("inputVariables");
                        JsonSerializer.Serialize(writer, op.InputVariables, options);
                    }
                    if (op.InOutputVariables?.Count() > 0)
                    {
                        writer.WritePropertyName("inOutputVariables");
                        JsonSerializer.Serialize(writer, op.InOutputVariables, options);
                    }
                    if (op.OutputVariables?.Count() > 0)
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
                case ModelTypes.SubmodelElementCollection:
                    var smc = (SubmodelElementCollection)value;
                    if(smc.Value != null)
                    {
                        writer.WritePropertyName("value");
                        writer.WriteStartArray();
                        var smcValue = smc.Get?.Invoke(smc).Result;
                        foreach (var item in smcValue.Value)
                        {
                            JsonSerializer.Serialize(writer, item, options);
                        }
                        writer.WriteEndArray();
                    }
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
                    if (sml.Value?.Value != null)
                    {
                        writer.WritePropertyName("value");
                        writer.WriteStartArray();
                        var smlValue = sml.Get?.Invoke(sml).Result;
                        foreach (var item in smlValue.Value)
                        {
                            JsonSerializer.Serialize(writer, item, options);
                        }
                        writer.WriteEndArray();
                    }
                    break;
                default:
                    break;
            }
        }
    }  
}
