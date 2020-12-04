/*******************************************************************************
* Copyright (c) 2020 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Attributes;
using BaSyx.Models.Core.Common;
using BaSyx.Utils.StringOperations;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            => CreateSubmodelFromType(type, type.Name, new Identifier(type.FullName, KeyType.Custom), DEFAULT_BINDING_FLAGS, null);

        public static ISubmodel CreateSubmodelFromType(this Type type, BindingFlags bindingFlags)
            => CreateSubmodelFromType(type, type.Name, new Identifier(type.FullName, KeyType.Custom), bindingFlags, null);

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
                    submodel.Identification = identification;
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

        private static JArray CustomizeSubmodelElement(ISubmodelElement element, string[] columns)
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
                    var info = propertyInfos.Find(p => p.Name == column.UppercaseFirst());
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

        public static JObject MinimizeSubmodel(this ISubmodel submodel)
        {
            JObject jObject = new JObject();
            if (submodel.SubmodelElements?.Count() > 0)
            {
                JObject submodelElementsObject = MinimizeSubmodelElements(submodel.SubmodelElements.Values);
                jObject.Merge(submodelElementsObject, JsonMergeSettings);
            }
            return jObject;
        }

        private static JObject MinimizeSubmodelElements(IEnumerable<ISubmodelElement> submodelElements)
        {
            JObject jObject = new JObject();
            foreach (var smElement in submodelElements)
            {
                if (smElement.ModelType == ModelType.SubmodelElementCollection)
                {
                    JObject subObjects = MinimizeSubmodelElements((smElement as SubmodelElementCollection).Value.Values);
                    jObject.Add(smElement.IdShort, subObjects);
                }
                else
                {
                    if (smElement is IFile file)
                        jObject.Add(file.IdShort, new JObject(new JProperty("mimeType", file.MimeType), new JProperty("value", file.Value)));
                    else if (smElement is Property property)
                    {
                        object value = property.Value;
                        if (value == null)
                            value = property.Get?.Invoke(property)?.Value;

                        jObject.Add(property.IdShort, new JValue(value));
                    }
                }
            }
            return jObject;
        }
    }
}
