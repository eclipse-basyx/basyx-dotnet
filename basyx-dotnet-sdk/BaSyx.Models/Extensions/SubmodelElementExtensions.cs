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
using BaSyx.Models.Semantics;
using BaSyx.Utils.ResultHandling;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace BaSyx.Models.Extensions
{
    public static class SubmodelElementExtensions
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger("SubmodelElementExtensions");

        public const BindingFlags DEFAULT_BINDING_FLAGS = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

        internal static T As<T>(this IReferable referable) where T : class, IReferable
        {
            return referable as T;
        }

        public static T Cast<T>(this IReferable referable) where T : class, IReferable
        {
            return referable as T;
        }

        //TODO
        //public static IEnumerable<T> ToEnumerable<T>(this ISubmodelElementCollection collection)
        //{
        //    if (collection != null)
        //    {         
        //        return collection.Value.Select(s => s.Cast<IProperty>().ToObject<T>());
        //    }
        //    else
        //        return null;
        //}

        //public static T ToEntity<T>(this ISubmodelElementCollection collection) where T : class
        //{
        //    if (collection != null)
        //    {
        //        return new SubmodelElementCollectionOfEntity<T>(collection).Value;
        //    }
        //    else
        //        return null;
        //}
        
        
        public static IValue GetValue(this ISubmodelElement submodelElement)
        {
            return submodelElement?.Get?.Invoke(submodelElement).Result;
        }

        public static T GetValue<T>(this ISubmodelElement submodelElement)
        {
            IValue value = submodelElement?.Get?.Invoke(submodelElement).Result;
            if (value != null)
                return value.ToObject<T>();
            else
                return default;
        }

        public static void SetValue<T>(this ISubmodelElement submodelElement, T value)
        {
            if (typeof(IValue).IsAssignableFrom(typeof(T)))
                submodelElement?.Set?.Invoke(submodelElement, value as IValue);
            else
                submodelElement?.Set?.Invoke(submodelElement, new ElementValue<T>(value));
        }

        public static void SetValue(this ISubmodelElement submodelElement, IValue value)
        {
            submodelElement?.Set?.Invoke(submodelElement, value);
        }

        public static void SetValue(this ISubmodelElement submodelElement, object value, DataType valueType)
        {
            submodelElement?.Set?.Invoke(submodelElement, new ElementValue(value, valueType));
        }
        
        public static Task<OperationResult> Invoke(this IOperation operation, IOperationVariableSet inputArguments, IOperationVariableSet inoutputArguments, IOperationVariableSet outputArguments, CancellationToken ct)
        {
            return operation?.OnMethodCalled?.Invoke(operation, inputArguments, inoutputArguments, outputArguments, ct);
        }

        public static async Task<OperationResult> InvokeAsync(this IOperation operation, IOperationVariableSet inputArguments, IOperationVariableSet inoutputArguments, IOperationVariableSet outputArguments, CancellationToken ct)
        {
            return await operation?.OnMethodCalled?.Invoke(operation, inputArguments, inoutputArguments, outputArguments, ct);
        }

        public static ISubmodelElementCollection CreateSubmodelElementCollectionFromObject(this object target)
            => CreateSubmodelElementCollectionFromType(target.GetType(), target.GetType().Name, DEFAULT_BINDING_FLAGS, target);

        public static ISubmodelElementCollection CreateSubmodelElementCollectionFromObject(this object target, string idShort)
            => CreateSubmodelElementCollectionFromType(target.GetType(), idShort, BindingFlags.Public | BindingFlags.Instance, target);

        public static ISubmodelElementCollection CreateSubmodelElementCollectionFromObject(this object target, BindingFlags bindingFlags)
            => CreateSubmodelElementCollectionFromType(target.GetType(), target.GetType().Name, bindingFlags, target);

        public static ISubmodelElementCollection CreateSubmodelElementCollectionFromObject(this object target, string idShort, BindingFlags bindingFlags)
            => CreateSubmodelElementCollectionFromType(target.GetType(), idShort, bindingFlags, target);

        public static ISubmodelElementCollection CreateSubmodelElementCollectionFromEnumerable<T>(this IEnumerable enumerable)
            => CreateSubmodelElementCollectionFromEnumerable(enumerable, typeof(T).Name, DEFAULT_BINDING_FLAGS);

        public static ISubmodelElementCollection CreateSubmodelElementCollectionFromEnumerable(this IEnumerable enumerable, string idShort)
            => CreateSubmodelElementCollectionFromEnumerable(enumerable, idShort, DEFAULT_BINDING_FLAGS);

        public static ISubmodelElementCollection CreateSubmodelElementCollectionFromEnumerable<T>(this IEnumerable enumerable, BindingFlags bindingFlags)
         => CreateSubmodelElementCollectionFromEnumerable(enumerable, typeof(T).Name, bindingFlags);

        public static ISubmodelElementCollection CreateSubmodelElementCollectionFromEnumerable(this IEnumerable enumerable, string idShort, BindingFlags bindingFlags)
        {
            SubmodelElementCollection smCollection = new SubmodelElementCollection(idShort);

            int i = 0;
            foreach (var item in enumerable)
            {
                if (DataType.IsSimpleType(item.GetType()))
                {
                    Property p = new Property($"{i}", item.GetType());
                    if (enumerable is IList list)
                    {
                        p.Get = (prop) => { return new ElementValue(list[Convert.ToInt32(prop.IdShort)], item.GetType()); };
                        p.Set = (prop, value) => { list[Convert.ToInt32(prop.IdShort)] = value.ToObject(item.GetType()); return Task.CompletedTask; };
                    }
                    smCollection.Add(p);
                }
                else
                {
                    foreach (var propertyInfo in item.GetType().GetProperties(bindingFlags))
                    {
                        ISubmodelElement smElement = CreateSubmodelElementFromPropertyInfo(propertyInfo, propertyInfo.Name, bindingFlags, item);
                        if (smElement != null)
                            smCollection.Value.Create(smElement);
                    }
                }
                i++;
            }
            return smCollection;
        }

        public static ISubmodelElementCollection CreateSubmodelElementCollectionFromType(this Type type)
            => CreateSubmodelElementCollectionFromType(type, type.Name, DEFAULT_BINDING_FLAGS, null);

        public static ISubmodelElementCollection CreateSubmodelElementCollectionFromType(this Type type, string idShort)
            => CreateSubmodelElementCollectionFromType(type, idShort, DEFAULT_BINDING_FLAGS, null);

        public static ISubmodelElementCollection CreateSubmodelElementCollectionFromType(this Type type, BindingFlags bindingFlags)
            => CreateSubmodelElementCollectionFromType(type, type.Name, bindingFlags, null);

        public static ISubmodelElementCollection CreateSubmodelElementCollectionFromType(this Type type, string idShort, BindingFlags bindingFlags)
            => CreateSubmodelElementCollectionFromType(type, idShort, bindingFlags, null);

        public static ISubmodelElementCollection CreateSubmodelElementCollectionFromType(this Type type, string idShort, BindingFlags bindingFlags, object target)
        {
            Attribute attribute = Attribute.GetCustomAttribute(type, typeof(SubmodelElementCollectionAttribute), true);
            SubmodelElementCollection smCollection;
            if (attribute is SubmodelElementCollectionAttribute seCollectionAttribute)
            {
                smCollection = seCollectionAttribute.SubmodelElementCollection;
                if (!string.IsNullOrEmpty(idShort) && idShort != type.Name)
                    smCollection.IdShort = idShort;
            }
            else
            {
                smCollection = new SubmodelElementCollection(idShort);
            }
            foreach (var propertyInfo in type.GetProperties(bindingFlags))
            {
                ISubmodelElement smElement = CreateSubmodelElementFromPropertyInfo(propertyInfo, propertyInfo.Name, bindingFlags, target);
                if (smElement != null)
                    smCollection.Value.Create(smElement);
            }
            return smCollection;
        }

        public static ISubmodelElement CreateSubmodelElementFromPropertyInfo(this PropertyInfo propertyInfo)
            => CreateSubmodelElementFromPropertyInfo(propertyInfo, propertyInfo.Name, DEFAULT_BINDING_FLAGS, null);

        public static ISubmodelElement CreateSubmodelElementFromPropertyInfo(this PropertyInfo propertyInfo, string idShort)
            => CreateSubmodelElementFromPropertyInfo(propertyInfo, idShort, DEFAULT_BINDING_FLAGS, null);

        public static ISubmodelElement CreateSubmodelElementFromPropertyInfo(this PropertyInfo propertyInfo, BindingFlags bindingFlags)
           => CreateSubmodelElementFromPropertyInfo(propertyInfo, propertyInfo.Name, bindingFlags, null);

        public static ISubmodelElement CreateSubmodelElementFromPropertyInfo(this PropertyInfo propertyInfo, string idShort, BindingFlags bindingFlags)
           => CreateSubmodelElementFromPropertyInfo(propertyInfo, idShort, bindingFlags, null);

        public static ISubmodelElement CreateSubmodelElementFromPropertyInfo(this PropertyInfo propertyInfo, string idShort, BindingFlags bindingFlags, object target)
        {
            Attribute attribute = Attribute.GetCustomAttribute(propertyInfo, typeof(SubmodelElementAttribute), true);
            if (attribute is SubmodelElementAttribute seAttribute)
            {
                SubmodelElement se = seAttribute.SubmodelElement;
                if (!string.IsNullOrEmpty(idShort) && idShort != propertyInfo.Name)
                    se.IdShort = idShort;

                if (Attribute.IsDefined(propertyInfo, typeof(DataSpecificationIEC61360Attribute)))
                {
                    var specAttribute = Attribute.GetCustomAttribute(propertyInfo, typeof(DataSpecificationIEC61360Attribute)) as DataSpecificationIEC61360Attribute;
                    se.ConceptDescription = new ConceptDescription()
                    {
                        Id = specAttribute.Identification,
                        EmbeddedDataSpecifications = new List<IEmbeddedDataSpecification>()
                        {
                            new DataSpecificationIEC61360(specAttribute.Content)
                        }
                    };
                }

                if (se is SubmodelElementCollection seCollection)
                {
                    if (DataType.IsGenericList(propertyInfo.PropertyType) || DataType.IsArray(propertyInfo.PropertyType))
                    {
                        ISubmodelElementCollection tempSeCollection;
                        if (target != null && propertyInfo.CanRead && propertyInfo.GetValue(target) is IEnumerable enumerable)
                            tempSeCollection = enumerable.CreateSubmodelElementCollectionFromEnumerable(idShort, bindingFlags);
                        else
                            tempSeCollection = new SubmodelElementCollection(idShort);

                        seCollection.Value.AddRange(tempSeCollection.Value);

                        return seCollection;
                    }
                    else
                    {
                        object subTarget = null;
                        if (target != null && propertyInfo.CanRead)
                            subTarget = propertyInfo.GetValue(target);

                        foreach (var subPropertyInfo in propertyInfo.PropertyType.GetProperties(bindingFlags))
                        {
                            ISubmodelElement smElement = CreateSubmodelElementFromPropertyInfo(subPropertyInfo, subPropertyInfo.Name, bindingFlags, subTarget);
                            if (smElement != null)
                                seCollection.Value.Create(smElement);
                        }
                    }
                    return seCollection;
                }
                else if (se is Property seProp)
                {
                    if (target != null)
                    {
                        if (propertyInfo.CanRead)
                            seProp.Get = (p) => { return new ElementValue(propertyInfo.GetValue(target), propertyInfo.PropertyType); };
                        if (propertyInfo.CanWrite)
                            seProp.Set = (p, val) => { propertyInfo.SetValue(target, val.Value); return Task.CompletedTask; };
                    }
                    return seProp;
                }
                else
                {
                    return se;
                }
            }
            else if (Attribute.IsDefined(propertyInfo, typeof(IgnoreElementAttribute)))
                return null;
            else
            {
                DataType dataType = DataType.GetDataTypeFromSystemType(propertyInfo.PropertyType);
                if (dataType == null)
                {
                    logger.LogWarning($"Unable to convert system type {propertyInfo.PropertyType} to DataType");
                    return null;
                }

                IConceptDescription conceptDescription = null;
                if (Attribute.IsDefined(propertyInfo, typeof(DataSpecificationIEC61360Attribute)))
                {
                    var specAttribute = Attribute.GetCustomAttribute(propertyInfo, typeof(DataSpecificationIEC61360Attribute)) as DataSpecificationIEC61360Attribute;
                    conceptDescription = new ConceptDescription()
                    {
                        Id = specAttribute.Identification,
                        EmbeddedDataSpecifications = new List<IEmbeddedDataSpecification>()
                        {
                            new DataSpecificationIEC61360(specAttribute.Content)
                        }
                    };
                }

                if (DataType.IsSimpleType(propertyInfo.PropertyType))
                {
                    Property smProp = new Property(idShort, dataType);
                    if (target != null)
                    {
                        if (propertyInfo.CanRead)
                            smProp.Get = (p) => { return new ElementValue(propertyInfo.GetValue(target), propertyInfo.PropertyType); };
                        if (propertyInfo.CanWrite)
                            smProp.Set = (p, val) => { propertyInfo.SetValue(target, val.Value); return Task.CompletedTask; };
                    }

                    smProp.ConceptDescription = conceptDescription;
                    return smProp;
                }
                else if (propertyInfo.PropertyType == typeof(DateTime))
                {
                    Property smProp = new Property(idShort, new DataType(DataObjectType.DateTime));
                    if (target != null)
                    {
                        if (propertyInfo.CanRead)
                            smProp.Get = (p) => { return new ElementValue(propertyInfo.GetValue(target), propertyInfo.PropertyType); };
                        if (propertyInfo.CanWrite)
                            smProp.Set = (p, val) => { propertyInfo.SetValue(target, val.Value); return Task.CompletedTask; };
                    }

                    smProp.ConceptDescription = conceptDescription;
                    return smProp;
                }
                else if (DataType.IsDictionary(propertyInfo.PropertyType))
                {
                    Type keyType = propertyInfo.PropertyType.GetGenericArguments()[0];
                    Type valueType = propertyInfo.PropertyType.GetGenericArguments()[1];
                    if (keyType != typeof(string) && valueType != typeof(string))
                        return null;

                    SubmodelElementCollection smcDictionary = new SubmodelElementCollection(idShort);

                    if (target != null && propertyInfo.CanRead)
                    {
                        var targetDic = propertyInfo.GetValue(target) as Dictionary<string, string>;
                        foreach (var item in targetDic)
                        {
                            Property<string> p = new Property<string>(item.Key);
                            p.Get = (prop) => { return Task.FromResult(targetDic[item.Key]); };
                            p.Set = (prop, value) => { targetDic[item.Key] = value; return Task.CompletedTask; };
                            smcDictionary.Add(p);
                        }
                    }
                    return smcDictionary;
                }
                else if (DataType.IsGenericList(propertyInfo.PropertyType) || DataType.IsArray(propertyInfo.PropertyType))
                {
                    SubmodelElementCollection seCollection;
                    if (target != null && propertyInfo.CanRead && propertyInfo.GetValue(target) is IEnumerable enumerable)
                        seCollection = (SubmodelElementCollection)enumerable.CreateSubmodelElementCollectionFromEnumerable(idShort, bindingFlags);
                    else
                        seCollection = new SubmodelElementCollection(idShort);

                    seCollection.ConceptDescription = conceptDescription;
                    return seCollection;
                }
                else
                {
                    SubmodelElementCollection smCollection = new SubmodelElementCollection(idShort);

                    object subTarget = null;
                    if (target != null && propertyInfo.CanRead)
                        subTarget = propertyInfo.GetValue(target);

                    foreach (var subPropertyInfo in dataType.SystemType.GetProperties(bindingFlags))
                    {
                        ISubmodelElement smElement = CreateSubmodelElementFromPropertyInfo(subPropertyInfo, subPropertyInfo.Name, bindingFlags, subTarget);
                        if (smElement != null)
                            smCollection.Value.Create(smElement);
                    }

                    smCollection.ConceptDescription = conceptDescription;
                    return smCollection;
                }
            }
        }
    }
}
