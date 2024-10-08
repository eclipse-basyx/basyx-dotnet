/*******************************************************************************
* Copyright (c) 2024 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BaSyx.Models.AdminShell
{
    [DataContract]
    [XmlType("key")]
    public class Key : IKey, IEquatable<Key>
    {
        [XmlAttribute("type")]
        public KeyType Type { get; set; }

        [XmlText]
        public string Value { get; set; }

        internal Key() { }

        public Key(KeyType type, string value)
        {
            Type = type;
            Value = value;
        }

        public static KeyType GetKeyElementFromType(Type type)
        {
            if (typeof(IAssetAdministrationShell).IsAssignableFrom(type))
                return KeyType.AssetAdministrationShell;
            else if (typeof(ISubmodel).IsAssignableFrom(type))
                return KeyType.Submodel;
            else if (typeof(IProperty).IsAssignableFrom(type))
                return KeyType.Property;
            else if (typeof(IOperation).IsAssignableFrom(type))
                return KeyType.Operation;
            else if (typeof(IConceptDescription).IsAssignableFrom(type))
                return KeyType.ConceptDescription;
            else if (typeof(IReferenceElement).IsAssignableFrom(type))
                return KeyType.ReferenceElement;
            else if (typeof(IRange).IsAssignableFrom(type))
                return KeyType.Range;
            else if (typeof(IOperation).IsAssignableFrom(type))
                return KeyType.Operation;
            else if (typeof(IRelationshipElement).IsAssignableFrom(type))
                return KeyType.RelationshipElement;
            else if (typeof(IAnnotatedRelationshipElement).IsAssignableFrom(type))
                return KeyType.AnnotatedRelationshipElement;
            else if (typeof(IEventElement).IsAssignableFrom(type))
                return KeyType.EventElement;
            else if (typeof(IBasicEventElement).IsAssignableFrom(type))
                return KeyType.BasicEventElement;
            else if (typeof(IFileElement).IsAssignableFrom(type))
                return KeyType.File;
            else if (typeof(IBlob).IsAssignableFrom(type))
                return KeyType.Blob;
            else if (typeof(ISubmodelElementCollection).IsAssignableFrom(type))
                return KeyType.SubmodelElementCollection;
            else if (typeof(ISubmodelElementList).IsAssignableFrom(type))
                return KeyType.SubmodelElementList;
            else if (typeof(IEntity).IsAssignableFrom(type))
                return KeyType.Entity;
            else
                throw new InvalidOperationException("Cannot convert type " + type.FullName + "to referable element");
        }

        public static KeyType GetKeyElementFromModelType(ModelType type)
        {
            if (type == ModelType.AssetAdministrationShell)
                return KeyType.AssetAdministrationShell;
            else if (type == ModelType.Submodel)
                return KeyType.Submodel;
            else if (type == ModelType.Property)
                return KeyType.Property;
            else if (type == ModelType.Operation)
                return KeyType.Operation;
            else if (type == ModelType.ConceptDescription)
                return KeyType.ConceptDescription;
            else if (type == ModelType.ReferenceElement)
                return KeyType.ReferenceElement;
            else if (type == ModelType.Range)
                return KeyType.Range;
            else if (type == ModelType.RelationshipElement)
                return KeyType.RelationshipElement;
            else if (type == ModelType.AnnotatedRelationshipElement)
                return KeyType.AnnotatedRelationshipElement;
            else if (type == ModelType.BasicEventElement)
                return KeyType.BasicEventElement;
            else if (type == ModelType.File)
                return KeyType.File;
            else if (type == ModelType.Blob)
                return KeyType.Blob;
            else if (type == ModelType.SubmodelElementCollection)
                return KeyType.SubmodelElementCollection;
            else if (type == ModelType.SubmodelElementList)
                return KeyType.SubmodelElementList;
            else if (type == ModelType.Entity)
                return KeyType.Entity;
            else
                throw new InvalidOperationException("Cannot convert type " + type.Name + "to referable element");
        }

        public string ToStandardizedString()
        {
            return string.Format("({0}){1}", Type, Value);
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

            return this.Type.Equals(other.Type)
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
                result = (result * 397) ^ Type.GetHashCode();
                result = (result * 397) ^ Value.GetHashCode();
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

            return x.Type == y.Type
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
        public Key(string value) : base(GetKeyElementFromType(typeof(T)), value)
        { }       
    }
}
