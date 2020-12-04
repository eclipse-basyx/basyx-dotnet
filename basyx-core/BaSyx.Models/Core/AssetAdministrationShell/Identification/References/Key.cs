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
using BaSyx.Models.Core.AssetAdministrationShell.Semantics;
using BaSyx.Models.Core.AssetAdministrationShell.Views;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Identification
{
    [DataContract]
    [XmlType("key")]
    public class Key : IKey, IEquatable<Key>
    {
        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlAttribute("type")]
        public KeyElements Type { get; set; }

        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlAttribute("idType")]
        public KeyType IdType { get; set; }

        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlText]
        public string Value { get; set; }

        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlAttribute("local")]
        public bool Local { get; set; }

        internal Key() { }

        [JsonConstructor]
        public Key(KeyElements type, KeyType idType, string value, bool local)
        {
            Type = type;
            IdType = idType;
            Value = value;
            Local = local;
        }

        public static KeyElements GetKeyElementFromType(Type type)
        {
            if (typeof(IAsset).IsAssignableFrom(type))
                return KeyElements.Asset;
            else if (typeof(IAssetAdministrationShell).IsAssignableFrom(type))
                return KeyElements.AssetAdministrationShell;
            else if (typeof(ISubmodel).IsAssignableFrom(type))
                return KeyElements.Submodel;
            else if (typeof(IView).IsAssignableFrom(type))
                return KeyElements.View;
            else if (typeof(IProperty).IsAssignableFrom(type))
                return KeyElements.Property;
            else if (typeof(IOperation).IsAssignableFrom(type))
                return KeyElements.Operation;
            else if (typeof(IEvent).IsAssignableFrom(type))
                return KeyElements.Event;
            else if (typeof(IConceptDescription).IsAssignableFrom(type))
                return KeyElements.ConceptDescription;
            else if (typeof(IReferenceElement).IsAssignableFrom(type))
                return KeyElements.ReferenceElement;
            else if (typeof(IRange).IsAssignableFrom(type))
                return KeyElements.Range;
            else if (typeof(IOperation).IsAssignableFrom(type))
                return KeyElements.Operation;
            else if (typeof(IRelationshipElement).IsAssignableFrom(type))
                return KeyElements.RelationshipElement;
            else if (typeof(IAnnotatedRelationshipElement).IsAssignableFrom(type))
                return KeyElements.AnnotatedRelationshipElement;
            else if (typeof(IEvent).IsAssignableFrom(type))
                return KeyElements.Event;
            else if (typeof(IBasicEvent).IsAssignableFrom(type))
                return KeyElements.BasicEvent;
            else if (typeof(IFile).IsAssignableFrom(type))
                return KeyElements.File;
            else if (typeof(IBlob).IsAssignableFrom(type))
                return KeyElements.Blob;
            else if (typeof(ISubmodelElementCollection).IsAssignableFrom(type))
                return KeyElements.SubmodelElementCollection;
            else if (typeof(IEntity).IsAssignableFrom(type))
                return KeyElements.Entity;
            else
                throw new InvalidOperationException("Cannot convert type " + type.FullName + "to referable element");
        }

        public string ToStandardizedString()
        {
            return string.Format("({0})({1})[{2}]{3}", Type, Local ? "local" : "no-local", IdType, Value);
        }

        #region IEquatable
        public bool Equals(Key other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.IdType.Equals(other.IdType)
                && this.Local.Equals(other.Local)
                && this.Type.Equals(other.Type)
                && this.Value.Equals(other.Type);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((Key)obj);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                var result = 0;
                result = (result * 397) ^ IdType.GetHashCode();
                result = (result * 397) ^ Type.GetHashCode();
                result = (result * 397) ^ (Local ? 1 : 0);
                return result;
            }
        }

        public static bool operator ==(Key x, Key y)
        {

            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null))
            {
                return false;
            }
            if (ReferenceEquals(y, null))
            {
                return false;
            }

            return x.IdType == y.IdType
                && x.Local == y.Local
                && x.Type == y.Type
                && x.Value == y.Value;
        }
        public static bool operator !=(Key x, Key y)
        {
            return !(x == y);
        }
        #endregion
    }

    [DataContract]
    public class Key<T> : Key
    {
        public Key(KeyType idType, string value, bool local) : base(GetKeyElementFromType(typeof(T)), idType, value, local)
        { }       
    }

}
