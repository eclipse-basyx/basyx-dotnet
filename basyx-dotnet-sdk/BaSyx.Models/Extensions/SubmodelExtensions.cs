/*******************************************************************************
* Copyright (c) 2022 Bosch Rexroth AG
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
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BaSyx.Models.Extensions
{
    public static class SubmodelExtensions
    {
        public const BindingFlags DEFAULT_BINDING_FLAGS = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

        #region Create Submodel from object
        public static ISubmodel CreateSubmodelFromObject(this object target)
           => CreateSubmodelFromType(target.GetType(), null, null, BindingFlags.Public | BindingFlags.Instance, target);

        public static ISubmodel CreateSubmodelFromObject(this object target, BindingFlags bindingFlags)
           => CreateSubmodelFromType(target.GetType(), null, null, bindingFlags, target);

        public static ISubmodel CreateSubmodelFromObject(this object target, string idShort, Identifier identification)
            => CreateSubmodelFromType(target.GetType(), idShort, identification, BindingFlags.Public | BindingFlags.Instance, target);

        public static ISubmodel CreateSubmodelFromObject(this object target, string idShort, Identifier identification, BindingFlags bindingFlags)
            => CreateSubmodelFromType(target.GetType(), idShort, identification, bindingFlags, target);

        #endregion

        #region Create Submodel from Type
        public static ISubmodel CreateSubmodelFromType(this Type type)
            => CreateSubmodelFromType(type, type.Name, new Identifier(type.FullName), DEFAULT_BINDING_FLAGS, null);

        public static ISubmodel CreateSubmodelFromType(this Type type, BindingFlags bindingFlags)
            => CreateSubmodelFromType(type, type.Name, new Identifier(type.FullName), bindingFlags, null);

        public static ISubmodel CreateSubmodelFromType(this Type type, string idShort, Identifier identification)
            => CreateSubmodelFromType(type, idShort, identification, DEFAULT_BINDING_FLAGS, null);

        public static ISubmodel CreateSubmodelFromType(this Type type, string idShort, Identifier identification, BindingFlags bindingFlags)
            => CreateSubmodelFromType(type, idShort, identification, bindingFlags, null);

        public static ISubmodel CreateSubmodelFromType(this Type type, string idShort, Identifier identification, BindingFlags bindingFlags, object target)
        {
            Attribute attribute = Attribute.GetCustomAttribute(type, typeof(SubmodelAttribute), true);
            Submodel submodel;
            if (attribute is SubmodelAttribute smAttribute)
            {
                submodel = smAttribute.Submodel;
                if (!string.IsNullOrEmpty(idShort) && idShort != type.Name)
                    submodel.IdShort = idShort;
                if (identification != null && identification.Id != type.FullName)
                    submodel.Id = identification;
            }
            else
            {
                submodel = new Submodel(idShort, identification);
            }

            foreach (var propertyInfo in type.GetProperties(bindingFlags))
            {
                ISubmodelElement smElement = propertyInfo.CreateSubmodelElementFromPropertyInfo(propertyInfo.Name, bindingFlags, target);
                if(smElement != null)
                    submodel.SubmodelElements.Create(smElement);
            }
            return submodel;
        }

        #endregion

        private static readonly JsonMergeSettings JsonMergeSettings = new JsonMergeSettings
        {
            MergeArrayHandling = MergeArrayHandling.Merge,
            MergeNullValueHandling = MergeNullValueHandling.Ignore
        };

        public static JArray CustomizeSubmodel(this ISubmodel submodel, string[] columns)
        {
            JArray jArray = new JArray();
            if (submodel.SubmodelElements?.Count() > 0)
            {
                foreach (var element in submodel.SubmodelElements.Values)
                {
                    JArray elementJArray = CustomizeSubmodelElement(element, columns);
                    jArray.Merge(elementJArray, new JsonMergeSettings() { MergeArrayHandling = MergeArrayHandling.Union } );
                }
            }
            return jArray;
        }

        public static JArray CustomizeSubmodelElement(this ISubmodelElement element, string[] columns)
        {
            JArray jArray = new JArray();
            Type elementType = element.GetType();
            List<PropertyInfo> propertyInfos = elementType.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
            JObject jObj = new JObject();
            if (element.ModelType == ModelType.SubmodelElementCollection)
            {
                var valueContainer = element as SubmodelElementCollection;
                foreach (var subElement in valueContainer.Value.Values)
                {
                    JArray subJArray = CustomizeSubmodelElement(subElement, columns);
                    jArray.Merge(subJArray, new JsonMergeSettings() { MergeArrayHandling = MergeArrayHandling.Union });
                }
            }
            else
            {
                foreach (var column in columns)
                {
                    var info = propertyInfos.Find(p => p.Name == column.ToUpperFirstChar());
                    if (info != null)
                    {
                        var value = info.GetValue(element);
                        if (value != null)
                        {
                            jObj.Add(column, JToken.FromObject(value));
                        }
                    }
                }
            }
            if(jObj.HasValues)
                jArray.Add(jObj);

            return jArray;
        }

        public static async Task<JObject> MinimizeSubmodelAsync(this ISubmodel submodel)
        {
            JObject jObject = new JObject();
            if (submodel.SubmodelElements?.Count() > 0)
            {
                JObject submodelElementsObject = await MinimizeSubmodelElementsAsync(submodel.SubmodelElements.Values);
                jObject.Merge(submodelElementsObject, JsonMergeSettings);
            }
            return jObject;
        }

        public static async Task<JObject> MinimizeSubmodelElementAsync(this ISubmodelElement submodelElement)
            => await MinimizeSubmodelElementsAsync(new ElementContainer<ISubmodelElement>() { submodelElement });

        public static async Task<JObject> MinimizeSubmodelElementsAsync(this IEnumerable<ISubmodelElement> submodelElements)
        {
            JObject jObject = new JObject();
            foreach (var smElement in submodelElements)
            {
                switch (smElement.ModelType.Type)
                {
                   case ModelTypes.SubmodelElementCollection:
                        {
                            ISubmodelElementCollection submodelElementCollection = smElement.Cast<ISubmodelElementCollection>();
                            JObject subObjects = await MinimizeSubmodelElementsAsync(submodelElementCollection.Value);
                            jObject.Add(smElement.IdShort, subObjects);
                            break;
                        }
                    case ModelTypes.RelationshipElement:
                        {
                            IRelationshipElement relationshipElement = smElement.Cast<IRelationshipElement>();
                            jObject.Add(relationshipElement.IdShort, 
                                new JObject(
                                    new JProperty("first", relationshipElement.First.ToStandardizedString()), 
                                    new JProperty("second", relationshipElement.Second.ToStandardizedString())));
                            break;
                        }
                    case ModelTypes.AnnotatedRelationshipElement:
                        {
                            IAnnotatedRelationshipElement annotatedRelationshipElement = smElement.Cast<IAnnotatedRelationshipElement>();
                            jObject.Add(annotatedRelationshipElement.IdShort,
                                new JObject(
                                    new JProperty("first", annotatedRelationshipElement.First?.ToStandardizedString()),
                                    new JProperty("second", annotatedRelationshipElement.Second?.ToStandardizedString()),
                                    new JProperty("annotations", await MinimizeSubmodelElementsAsync(annotatedRelationshipElement.Annotations))));
                            break;
                        }
                    case ModelTypes.Property:
                        {
                            IProperty property = smElement.Cast<IProperty>();
                            var valueScope = await property.GetValueScope<PropertyValue>();
                            jObject.Add(property.IdShort, new JValue(valueScope.Value.Value));
                            break;
                        }
                    case ModelTypes.File:
                        {
                            IFileElement file = smElement.Cast<IFileElement>();
                            jObject.Add(file.IdShort, 
                                new JObject(
                                    new JProperty("contentType", file.ContentType), 
                                    new JProperty("value", file.Value)));
                            break;
                        }
                    case ModelTypes.Blob:
                        {
                            IBlob blob = smElement.Cast<IBlob>();
                            jObject.Add(blob.IdShort,
                                new JObject(
                                    new JProperty("contentType", blob.ContentType),
                                    new JProperty("value", blob.Value)));
                            break;
                        }
                    case ModelTypes.ReferenceElement:
                        {
                            IReferenceElement referenceElement = smElement.Cast<IReferenceElement>();
                            jObject.Add(referenceElement.IdShort, new JValue(referenceElement.Value?.ToStandardizedString()));
                            break;
                        }
                    case ModelTypes.MultiLanguageProperty:
                        {
                            IMultiLanguageProperty multiLanguageProperty = smElement.Cast<IMultiLanguageProperty>();
                            List<JProperty> content = new List<JProperty>();
                            foreach (var langPair in multiLanguageProperty.Value)
                            {
                                content.Add(new JProperty(langPair.Language, langPair.Text));
                            }
                            jObject.Add(multiLanguageProperty.IdShort, new JObject(content));
                            break;
                        }
                    case ModelTypes.Range:
                        {
                            IRange range = smElement.Cast<IRange>();
                            jObject.Add(range.IdShort,
                                new JObject(
                                    new JProperty("min", range.Value.Min?.ToObject<string>()),
                                    new JProperty("max", range.Value.Max?.ToObject<string>())));
                            break;
                        }
                    case ModelTypes.Entity:
                        {
                            IEntity entity = smElement.Cast<IEntity>();
                            JObject statements = await MinimizeSubmodelElementsAsync(entity.Statements);
                            jObject.Add(entity.IdShort,
                                new JObject(
                                    new JProperty("statements", statements),
                                    new JProperty("entityType", Enum.GetName(typeof(EntityType), entity.EntityType)),
                                    new JProperty("globalAssetId",entity.GlobalAssetId)));
                            break;
                        }
                    default:
                        break;
                }
            }
            return jObject;
        }
    }
}
