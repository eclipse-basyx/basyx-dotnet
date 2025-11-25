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

        public static IElementContainer<ISubmodelElement> ToElementContainer<T>(this IEnumerable<T> enumerable,
            IReferable parent = null, ISubmodelElement rootElement = null, IElementContainer<ISubmodelElement> parentContainer = null)
        {
            if (enumerable != null)
            {
                ElementContainer<ISubmodelElement> smElements = new ElementContainer<ISubmodelElement>(parent, rootElement, parentContainer);
                DataType type = DataType.GetDataTypeFromSystemType(typeof(T));
                if (type.IsCollection)
                {
                    // New behavior: Lists/Arrays become SubmodelElementList instead of SubmodelElementCollection
                    ISubmodelElementList sml = CreateSubmodelElementListFromEnumerable(enumerable, typeof(T).Name, DEFAULT_BINDING_FLAGS);
                    smElements.Add(sml);
                }
                else
                {
                    int i = 0;
                    foreach (var item in enumerable)
                    {
                        Property property = new Property($"{i}", type, item);
                        smElements.Add(property);
                        i++;
                    }
                }
                return smElements;
            }
            else
                return null;
        }

        public static IEnumerable<T> ToEnumerable<T>(this ISubmodelElementList sml)
            => ToEnumerable<T>(sml.Value.Value);

        public static IEnumerable<T> ToEnumerable<T>(this ISubmodelElementCollection smc)
            => ToEnumerable<T>(smc.Value.Value);

        public static IEnumerable<T> ToEnumerable<T>(this IElementContainer<ISubmodelElement> container)
        {
            if (container == null)
                return null;

            var targetElementType = typeof(T);
            var result = new List<T>();

            foreach (var element in container.Values)
            {
                if (element == null)
                    continue;

                if (element.ModelType == ModelType.Property)
                {
                    var value = element.Cast<IProperty>().GetValue<T>();
                    result.Add(value);
                }
                else if (element.ModelType == ModelType.SubmodelElementCollection)
                {
                    // Convert each collection element to object of T
                    var obj = element.Cast<ISubmodelElementCollection>().ToObject(targetElementType);
                    if (obj is T t)
                        result.Add(t);
                    else if (obj != null)
                        result.Add((T)Convert.ChangeType(obj, targetElementType));
                }
                else if (element.ModelType == ModelType.SubmodelElementList)
                {
                    // Nested list support (basic): try to convert list to target type if the target is an IEnumerable
                    var list = element.Cast<ISubmodelElementList>();
                    var obj = list.ToObject(targetElementType);
                    if (obj is T t)
                        result.Add(t);
                }
                else
                {
                    // other SME types are currently not supported for IEnumerable<T> conversion
                }
            }

            return result;
        }

        public static T ToObject<T>(this ISubmodelElementCollection collection) where T : class
        {
            if (collection != null)
            {
                return (T)ToObject(collection, typeof(T));
            }
            else
                return null;
        }

        //public static T ToObject<T>(this IEnumerable<ISubmodelElement> container) where T : class
        //{
        //    if (container != null)
        //    {
        //        return (T)ToObject(container, typeof(T));
        //    }
        //    else
        //        return null;
        //}

        public static T ToObject<T>(this IEnumerable<ISubmodelElement> container) where T : class
        {
            if (container != null)
            {
                // Convert IEnumerable<ISubmodelElement> to IElementContainer<ISubmodelElement>
                var elementContainer = new ElementContainer<ISubmodelElement>();
                elementContainer.AddRange(container);
                return (T)ToObject(elementContainer, typeof(T));
            }
            else
                return null;
        }


        public static object ToObject(this ISubmodelElementCollection collection, Type type)
        {
            if (collection == null || type == null)
                return null;

            return ToObject(collection.Value.Value, type);
        }

        public static object ToObject(this ISubmodelElementList list, Type targetType)
        {
            if (list == null || targetType == null)
                return null;

            var elementContainer = list.Value.Value;
            return ConvertElementContainerToCollection(elementContainer, targetType);
        }

        public static object ToObject(this IElementContainer<ISubmodelElement> container, Type type)
        {
            if (container == null || type == null)
                return null;

            // Create instance and set properties
            var instance = Activator.CreateInstance(type);
            foreach (var element in container.Values)
            {
                if (string.IsNullOrEmpty(element?.IdShort))
                    continue;

                var info = type.GetProperty(element.IdShort, DEFAULT_BINDING_FLAGS);
                if (info == null || !info.CanWrite)
                    continue;

                var targetPropertyType = info.PropertyType;
                object valueToAssign = null;

                if (element.ModelType == ModelType.Property)
                {
                    var propScope = element.Cast<IProperty>()?.Value;
                    var elemValue = propScope?.Value?.ToObject(targetPropertyType);
                    valueToAssign = elemValue;
                }
                else if (element.ModelType == ModelType.SubmodelElementCollection)
                {
                    // Nested complex type
                    var nestedObj = element.Cast<ISubmodelElementCollection>().ToObject(targetPropertyType);
                    valueToAssign = nestedObj;
                }
                else if (element.ModelType == ModelType.SubmodelElementList)
                {
                    // List/Array conversion
                    var nestedListObj = element.Cast<ISubmodelElementList>().ToObject(targetPropertyType);
                    valueToAssign = nestedListObj;
                }

                if (valueToAssign != null)
                    info.SetValue(instance, valueToAssign, null);
            }
            return instance;
        }

        public static TValueScope GetValueScope<TValueScope>(this ISubmodelElement sme) where TValueScope : ValueScope
        {
            return GetValueScopeAsync<TValueScope>(sme).Result;
        }

        public static async Task<TValueScope> GetValueScopeAsync<TValueScope>(this ISubmodelElement sme) where TValueScope : ValueScope
        {
            var scope = await sme.GetValueScope().ConfigureAwait(false);
            return (TValueScope)scope;
        }

        public static T GetValue<T>(this ISubmodelElement sme)
        {
            return GetValueAsync<T>(sme).Result;
        }

        public static async Task<T> GetValueAsync<T>(this ISubmodelElement sme)
        {
            var scope = await sme.GetValueScope().ConfigureAwait(false);
            return scope.GetValue<T>();
        }

        public static T GetValue<T>(this ValueScope valueScope)
        {
            if (valueScope is PropertyValue propValue)
            {
                return propValue.Value.ToObject<T>();
            }
            else
                return default(T);
        }

        public static void SetValue<T>(this ISubmodelElement sme, ValueScope valueScope) => SetValueAsync(sme, valueScope).Wait();

        public static async Task SetValueAsync(this ISubmodelElement sme, ValueScope valueScope)
        {
            await sme.SetValueScope(valueScope).ConfigureAwait(false);
        }

        public static void SetValue<T>(this ISubmodelElement sme, T value) => SetValueAsync<T>(sme, value).Wait();

        public static async Task SetValueAsync<T>(this ISubmodelElement sme, T value)
        {
            if (sme.ModelType == ModelType.Property)
            {
                if (value is PropertyValue propValue)
                {
                    await sme.SetValueScope(propValue).ConfigureAwait(false);
                }
                else
                {
                    PropertyValue<T> propertyValue = new PropertyValue<T>(value);
                    await sme.SetValueScope(propertyValue).ConfigureAwait(false);
                }
            }
            else if (sme.ModelType == ModelType.SubmodelElementList)
            {
                if (value is SubmodelElementListValue smlValue)
                {
                    await sme.SetValueScope(smlValue).ConfigureAwait(false);
                }
                else if (sme is SubmodelElementList sml && value is SubmodelElementList valueSml)
                {
                    sml.Value = valueSml.Value;
                }
                else if (value is ICollection collection)
                {
                    var enumerable = collection.Cast<object>();
                    IElementContainer<ISubmodelElement> smeElements = new ElementContainer<ISubmodelElement>();
                    int i = 0;
                    foreach (var element in enumerable)
                    {
                        if (element == null)
                            continue;

                        if (DataType.IsSimpleType(element.GetType()) || element is DateTime)
                        {
                            smeElements.Add(new Property($"{i}", element.GetType(), element));
                        }
                        else
                        {
                            // complex type -> element collection
                            var childCollection = CreateSubmodelElementCollectionFromType(element.GetType(), $"{i}", DEFAULT_BINDING_FLAGS, element);
                            smeElements.Add(childCollection);
                        }
                        i++;
                    }
                    SubmodelElementListValue listValue = new SubmodelElementListValue(smeElements);
                    await sme.SetValueScope(listValue).ConfigureAwait(false);
                }
            }
            else if (sme.ModelType == ModelType.SubmodelElementCollection)
            {
                if (value is SubmodelElementCollectionValue smcValue)
                {
                    await sme.SetValueScope(smcValue).ConfigureAwait(false);
                }
                else if (sme is SubmodelElementCollection smc && value is SubmodelElementCollection valueSmc)
                {
                    smc.Value = valueSmc.Value;
                }
                else
                {
                    IElementContainer<ISubmodelElement> smeElements = new ElementContainer<ISubmodelElement>();
                    var type = value.GetType();
                    foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (propertyInfo.CanRead)
                        {
                            var propertyValue = propertyInfo.GetValue(value);

                            if (propertyValue == null)
                                continue;

                            if (DataType.IsSimpleType(propertyInfo.PropertyType) || propertyInfo.PropertyType == typeof(DateTime))
                            {
                                Property smProp = new Property(propertyInfo.Name, propertyInfo.PropertyType);
                                smProp.Value = new PropertyValue(new ElementValue(propertyValue, propertyInfo.PropertyType));
                                smeElements.Add(smProp);
                            }
                            else if (DataType.IsGenericList(propertyInfo.PropertyType) || DataType.IsArray(propertyInfo.PropertyType))
                            {
                                var listElement = CreateSubmodelElementListFromEnumerable((IEnumerable)propertyValue, propertyInfo.Name, DEFAULT_BINDING_FLAGS);
                                smeElements.Add(listElement);
                            }
                            else
                            {
                                var childSmc = CreateSubmodelElementCollectionFromType(propertyInfo.PropertyType, propertyInfo.Name, DEFAULT_BINDING_FLAGS, propertyValue);
                                smeElements.Add(childSmc);
                            }
                        }
                    }
                    SubmodelElementCollectionValue collectionValue = new SubmodelElementCollectionValue(smeElements);
                    await sme.SetValueScope(collectionValue).ConfigureAwait(false);
                }
            }
        }

        public static Task<OperationResult> Invoke(this IOperation operation, IOperationVariableSet inputArguments, IOperationVariableSet inoutputArguments, IOperationVariableSet outputArguments, CancellationToken ct)
        {
            return operation?.OnMethodCalled?.Invoke(operation, inputArguments, inoutputArguments, outputArguments, ct);
        }

        public static async Task<OperationResult> InvokeAsync(this IOperation operation, IOperationVariableSet inputArguments, IOperationVariableSet inoutputArguments, IOperationVariableSet outputArguments, CancellationToken ct)
        {
            return await operation?.OnMethodCalled?.Invoke(operation, inputArguments, inoutputArguments, outputArguments, ct);
        }

        // Convenience: create a SME from an object instance (simple -> Property, list/array -> SubmodelElementList, complex -> SubmodelElementCollection)
        public static ISubmodelElement CreateSubmodelElementFromInstance(this object target, string idShort = null, BindingFlags bindingFlags = DEFAULT_BINDING_FLAGS)
        {
            if (target == null)
                return null;

            var type = target.GetType();
            idShort ??= type.Name;

            if (DataType.IsSimpleType(type) || type == typeof(DateTime))
            {
                var prop = new Property(idShort, type, target);
                return prop;
            }
            else if (DataType.IsGenericList(type) || DataType.IsArray(type))
            {
                var listSme = CreateSubmodelElementListFromEnumerable((IEnumerable)target, idShort, bindingFlags);
                return listSme;
            }
            else
            {
                var smc = CreateSubmodelElementCollectionFromType(type, idShort, bindingFlags, target);
                return smc;
            }
        }

        public static ISubmodelElementCollection CreateSubmodelElementCollectionFromObject(this object target)
            => CreateSubmodelElementCollectionFromType(target.GetType(), target.GetType().Name, DEFAULT_BINDING_FLAGS, target);

        public static ISubmodelElementCollection CreateSubmodelElementCollectionFromObject(this object target, string idShort)
            => CreateSubmodelElementCollectionFromType(target.GetType(), idShort, BindingFlags.Public | BindingFlags.Instance, target);

        public static ISubmodelElementCollection CreateSubmodelElementCollectionFromObject(this object target, BindingFlags bindingFlags)
            => CreateSubmodelElementCollectionFromType(target.GetType(), target.GetType().Name, bindingFlags, target);

        public static ISubmodelElementCollection CreateSubmodelElementCollectionFromObject(this object target, string idShort, BindingFlags bindingFlags)
            => CreateSubmodelElementCollectionFromType(target.GetType(), idShort, bindingFlags, target);

        // New: SubmodelElementList builder for enumerables
        public static ISubmodelElementList CreateSubmodelElementListFromEnumerable<T>(this IEnumerable enumerable)
            => CreateSubmodelElementListFromEnumerable(enumerable, typeof(T).Name, DEFAULT_BINDING_FLAGS);

        public static ISubmodelElementList CreateSubmodelElementListFromEnumerable(this IEnumerable enumerable, string idShort)
            => CreateSubmodelElementListFromEnumerable(enumerable, idShort, DEFAULT_BINDING_FLAGS);

        public static ISubmodelElementList CreateSubmodelElementListFromEnumerable<T>(this IEnumerable enumerable, BindingFlags bindingFlags)
            => CreateSubmodelElementListFromEnumerable(enumerable, typeof(T).Name, bindingFlags);

        public static ISubmodelElementList CreateSubmodelElementListFromEnumerable(this IEnumerable enumerable, string idShort, BindingFlags bindingFlags)
        {
            var sml = new SubmodelElementList(idShort);
            if (enumerable == null)
                return sml;

            Type itemType = null;
            var e = enumerable.GetEnumerator();
            if (e.MoveNext() && e.Current != null)
                itemType = e.Current.GetType();

            if (itemType == null)
            {
                // try to infer from IEnumerable type
                var enType = enumerable.GetType();
                if (DataType.IsArray(enType))
                    itemType = enType.GetElementType();
                else if (DataType.IsGenericList(enType))
                    itemType = enType.GetGenericArguments().FirstOrDefault();
            }

            bool simpleItem = itemType != null && (DataType.IsSimpleType(itemType) || itemType == typeof(DateTime));
            sml.TypeValueListElement = simpleItem ? ModelType.Property : ModelType.SubmodelElementCollection;
            sml.ValueTypeListElement = DataType.GetDataTypeFromSystemType(simpleItem ? itemType : typeof(object));

            int i = 0;
            foreach (var item in enumerable)
            {
                if (item == null) { i++; continue; }

                if (simpleItem)
                {
                    if (enumerable is IList list)
                    {
                        Property p = new Property($"{i}", item.GetType());
                        p.Get = (prop) => { return new PropertyValue(new ElementValue(list[Convert.ToInt32(prop.IdShort)], item.GetType())); };
                        p.Set = (prop, value) => { list[Convert.ToInt32(prop.IdShort)] = value.Value.ToObject(item.GetType()); return Task.CompletedTask; };
                        sml.Add(p);
                    }
                    else
                        sml.Add(new Property($"{i}", item.GetType(), item));
                }
                else
                {
                    var itemCollection = CreateSubmodelElementCollectionFromType(item.GetType(), $"{i}", bindingFlags, item);
                    sml.Add(itemCollection);
                }
                i++;
            }
            return sml;
        }

        public static ISubmodelElementCollection CreateSubmodelElementCollectionFromEnumerable<T>(this IEnumerable enumerable)
            => CreateSubmodelElementCollectionFromEnumerable(enumerable, typeof(T).Name, DEFAULT_BINDING_FLAGS);

        public static ISubmodelElementCollection CreateSubmodelElementCollectionFromEnumerable(this IEnumerable enumerable, string idShort)
            => CreateSubmodelElementCollectionFromEnumerable(enumerable, idShort, DEFAULT_BINDING_FLAGS);

        public static ISubmodelElementCollection CreateSubmodelElementCollectionFromEnumerable<T>(this IEnumerable enumerable, BindingFlags bindingFlags)
            => CreateSubmodelElementCollectionFromEnumerable(enumerable, typeof(T).Name, bindingFlags);

        public static ISubmodelElementCollection CreateSubmodelElementCollectionFromEnumerable(this IEnumerable enumerable, string idShort, BindingFlags bindingFlags)
        {
            // Keep this for backward compatibility when an enumerable is intended to flatten into a collection of properties/child collections.
            // For simple enumerable elements, add properties; for complex elements, add embedded collections.
            SubmodelElementCollection smCollection = new SubmodelElementCollection(idShort);

            int i = 0;
            foreach (var item in enumerable)
            {
                if (item == null) { i++; continue; }

                if (DataType.IsSimpleType(item.GetType()) || item is DateTime)
                {
                    Property p = new Property($"{i}", item.GetType());
                    if (enumerable is IList list)
                    {
                        p.Get = (prop) => { return new PropertyValue(new ElementValue(list[Convert.ToInt32(prop.IdShort)], item.GetType())); };
                        p.Set = (prop, value) => { list[Convert.ToInt32(prop.IdShort)] = value.Value.ToObject(item.GetType()); return Task.CompletedTask; };
                    }
                    smCollection.Add(p);
                }
                else
                {
                    var itemSmc = CreateSubmodelElementCollectionFromType(item.GetType(), $"{i}", bindingFlags, item);
                    smCollection.Add(itemSmc);
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
                    smCollection.Value.Value.Create(smElement);
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
                        // Override: Lists/Arrays should become SubmodelElementList, not collection
                        return BuildSubmodelElementListFromProperty(propertyInfo, idShort, bindingFlags, target, se.ConceptDescription);
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
                                seCollection.Value.Value.Create(smElement);
                        }
                    }
                    return seCollection;
                }
                else if (se is Property seProp)
                {
                    if (target != null)
                    {
                        if (propertyInfo.CanRead)
                            seProp.Get = (p) => { return new PropertyValue(new ElementValue(propertyInfo.GetValue(target), propertyInfo.PropertyType)); };
                        if (propertyInfo.CanWrite)
                            seProp.Set = (p, val) => { propertyInfo.SetValue(target, val.Value.Value); return Task.CompletedTask; };
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
                            smProp.Get = (p) => { return new PropertyValue(new ElementValue(propertyInfo.GetValue(target), propertyInfo.PropertyType)); };
                        if (propertyInfo.CanWrite)
                            smProp.Set = (p, val) => { propertyInfo.SetValue(target, val.Value.Value); return Task.CompletedTask; };
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
                            smProp.Get = (p) => { return new PropertyValue(new ElementValue(propertyInfo.GetValue(target), propertyInfo.PropertyType)); };
                        if (propertyInfo.CanWrite)
                            smProp.Set = (p, val) => { propertyInfo.SetValue(target, val.Value.Value); return Task.CompletedTask; };
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
                    // New behavior: produce SubmodelElementList
                    var sml = BuildSubmodelElementListFromProperty(propertyInfo, idShort, bindingFlags, target, conceptDescription);
                    return sml;
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
                            smCollection.Value.Value.Create(smElement);
                    }

                    smCollection.ConceptDescription = conceptDescription;
                    return smCollection;
                }
            }
        }

        private static ISubmodelElementList BuildSubmodelElementListFromProperty(PropertyInfo propertyInfo, string idShort, BindingFlags bindingFlags, object target, IConceptDescription conceptDescription)
        {
            Type elementType = propertyInfo.PropertyType.IsArray
                ? propertyInfo.PropertyType.GetElementType()
                : propertyInfo.PropertyType.GetGenericArguments().FirstOrDefault();

            var simpleItem = elementType != null && (DataType.IsSimpleType(elementType) || elementType == typeof(DateTime));

            var sml = new SubmodelElementList(idShort)
            {
                TypeValueListElement = simpleItem ? ModelType.Property : ModelType.SubmodelElementCollection,
                ValueTypeListElement = DataType.GetDataTypeFromSystemType(simpleItem ? elementType : typeof(object)),
                ConceptDescription = conceptDescription
            };

            if (target != null && propertyInfo.CanRead)
            {
                var enumerable = propertyInfo.GetValue(target) as IEnumerable;

                if (enumerable != null)
                {
                    var tempList = CreateSubmodelElementListFromEnumerable(enumerable, idShort, bindingFlags);
                    sml.Value.Value.AddRange(tempList.Value.Value);
                }

                // Bind Get/Set to the underlying object property
                sml.Get = (prop) =>
                {
                    var currentVal = propertyInfo.GetValue(target) as IEnumerable;
                    if (currentVal == null)
                        return Task.FromResult(new SubmodelElementListValue(new ElementContainer<ISubmodelElement>()));

                    var container = CreateSubmodelElementListFromEnumerable(currentVal, idShort, bindingFlags).Value;
                    return Task.FromResult(new SubmodelElementListValue(container.Value));
                };

                sml.Set = (prop, valueScope) =>
                {
                    if (valueScope is SubmodelElementListValue listValue)
                    {
                        var targetCollectionType = propertyInfo.PropertyType;
                        var obj = ConvertElementContainerToCollection(listValue.Value, targetCollectionType);
                        propertyInfo.SetValue(target, obj);
                    }
                    return Task.CompletedTask;
                };
            }

            return sml;
        }

        private static object ConvertElementContainerToCollection(IElementContainer<ISubmodelElement> container, Type targetType)
        {
            if (container == null || targetType == null)
                return null;

            Type elementType;
            bool isArray = targetType.IsArray;
            if (isArray)
                elementType = targetType.GetElementType();
            else
                elementType = targetType.IsGenericType ? targetType.GetGenericArguments()[0] : typeof(object);

            var items = new List<object>();

            foreach (var element in container.Values)
            {
                object itemVal = null;
                if (element.ModelType == ModelType.Property)
                {
                    itemVal = element.Cast<IProperty>()?.Value?.Value?.ToObject(elementType);
                }
                else if (element.ModelType == ModelType.SubmodelElementCollection)
                {
                    itemVal = element.Cast<ISubmodelElementCollection>().ToObject(elementType);
                }
                else if (element.ModelType == ModelType.SubmodelElementList)
                {
                    itemVal = element.Cast<ISubmodelElementList>().ToObject(elementType);
                }

                if (itemVal != null)
                    items.Add(itemVal);
            }

            if (isArray)
            {
                var arr = Array.CreateInstance(elementType, items.Count);
                for (int i = 0; i < items.Count; i++)
                    arr.SetValue(items[i], i);
                return arr;
            }
            else
            {
                // Try to instantiate targetType if it’s a concrete collection; otherwise, use List<T>
                object listObj = null;
                try
                {
                    listObj = Activator.CreateInstance(targetType);
                    if (listObj is IList list)
                    {
                        foreach (var item in items)
                            list.Add(item);
                        return list;
                    }
                }
                catch
                {
                    // fallback to List<T>
                }

                var genericListType = typeof(List<>).MakeGenericType(elementType);
                var genericList = Activator.CreateInstance(genericListType);
                var addMethod = genericListType.GetMethod("Add");
                foreach (var item in items)
                    addMethod.Invoke(genericList, new[] { item });
                return genericList;
            }
        }
    }
}