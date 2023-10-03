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
using BaSyx.Models.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BaSyx.Models.AdminShell
{
    public enum ModelTypes
    {
        Asset,
        AssetAdministrationShell,
        Submodel,
        SubmodelElement,
        SubmodelElementCollection,
        SubmodelElementList,
        Operation,
        OperationVariable,
        BasicEvent,
        Constraint,
        Formula,
        Event,
        BasicEventElement,
        DataSpecificationIec61360,
        EventElement,
        EventMessage,
        View,
        RelationshipElement,
        AnnotatedRelationshipElement,
        Capability,
        DataElement,
        Property,
        File,
        Blob,
        ReferenceElement,
        MultiLanguageProperty,
        Range,
        Entity,
        Qualifier,
        ConceptDescription,
        ConceptDictionary,
        AssetAdministrationShellRepositoryDescriptor,
        SubmodelRepositoryDescriptor,
        AssetAdministrationShellDescriptor,
        SubmodelDescriptor
    }

    [JsonConverter(typeof(ModelTypeConverter))]
    public class ModelType : DataObjectType, IEquatable<ModelType>
    {
        public static readonly ModelType Asset = new ModelType("Asset");
        public static readonly ModelType AssetAdministrationShell = new ModelType("AssetAdministrationShell");
        public static readonly ModelType Submodel = new ModelType("Submodel");
        public static readonly ModelType SubmodelElement = new ModelType("SubmodelElement");
        public static readonly ModelType SubmodelElementCollection = new ModelType("SubmodelElementCollection");
        public static readonly ModelType SubmodelElementList = new ModelType("SubmodelElementList");
        public static readonly ModelType Operation = new ModelType("Operation");
        public static readonly ModelType OperationVariable = new ModelType("OperationVariable");
        public static readonly ModelType Event = new ModelType("Event");
        public static readonly ModelType BasicEvent = new ModelType("BasicEvent");
        public static readonly ModelType DataSpecificationIec61360 = new ModelType("DataSpecificationIec61360");
        public static readonly ModelType BasicEventElement = new ModelType("BasicEventElement");
        public static readonly ModelType EventElement = new ModelType("EventElement");
        public static readonly ModelType EventMessage = new ModelType("EventMessage");
        public static readonly ModelType View = new ModelType("View");
        public static readonly ModelType RelationshipElement = new ModelType("RelationshipElement");
        public static readonly ModelType AnnotatedRelationshipElement = new ModelType("AnnotatedRelationshipElement");
        public static readonly ModelType Capability = new ModelType("Capability");
        public static readonly ModelType DataElement = new ModelType("DataElement");
        public static readonly ModelType Property = new ModelType("Property");
        public static readonly ModelType File = new ModelType("File");
        public static readonly ModelType Blob = new ModelType("Blob");
        public static readonly ModelType ReferenceElement = new ModelType("ReferenceElement");
        public static readonly ModelType MultiLanguageProperty = new ModelType("MultiLanguageProperty");
        public static readonly ModelType Range = new ModelType("Range");
        public static readonly ModelType Entity = new ModelType("Entity");

        public static readonly ModelType Constraint = new ModelType("Constraint");
        public static readonly ModelType Formula = new ModelType("Formula");
        public static readonly ModelType Qualifier = new ModelType("Qualifier");

        public static readonly ModelType ConceptDescription = new ModelType("ConceptDescription");
        public static readonly ModelType ConceptDictionary = new ModelType("ConceptDictionary");

        public static readonly ModelType AssetAdministrationShellRepositoryDescriptor = new ModelType("AssetAdministrationShellRepositoryDescriptor");
        public static readonly ModelType SubmodelRepositoryDescriptor = new ModelType("SubmodelRepositoryDescriptor");
        public static readonly ModelType AssetAdministrationShellDescriptor = new ModelType("AssetAdministrationShellDescriptor");
        public static readonly ModelType SubmodelDescriptor = new ModelType("SubmodelDescriptor");

        private static readonly Dictionary<string, ModelType> _modelTypes;

        public ModelTypes Type { get; }

        static ModelType()
        {
            var fields = typeof(ModelType).GetFields(BindingFlags.Public | BindingFlags.Static);
            _modelTypes = fields.ToDictionary(k => ((ModelType)k.GetValue(null)).Name, v => ((ModelType)v.GetValue(null)));
        }

        [JsonConstructor]
        protected ModelType(string name) : base(name)
        {
            Type = (ModelTypes)Enum.Parse(typeof(ModelTypes), name);
        }

        public static ModelType Parse(ModelTypes modelTypeEnum)
        {
            if (_modelTypes.TryGetValue(Enum.GetName(typeof(ModelTypes), modelTypeEnum), out ModelType modelType))
                return modelType;
            else
                throw new InvalidOperationException("Cannot parse " + modelType);
        }

        public static bool TryParse(string s, out ModelType modelType)
        {
            if (_modelTypes.TryGetValue(s, out modelType))
                return true;
            else
                return false;
        }

        public override string ToString()
        {
            return Name;
        }

        #region IEquatable Interface Implementation
        public bool Equals(ModelType other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Name.Equals(other.Name);
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

            return obj.GetType() == GetType() && Equals((ModelType)obj);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                var result = 0;
                result = (result * 397) ^ Name.GetHashCode();
                return result;
            }
        }

        public static bool operator ==(ModelType x, ModelType y)
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

            return x.Name == y.Name;
        }
        public static bool operator !=(ModelType x, ModelType y)
        {
            return !(x == y);
        }
        #endregion

        public static implicit operator string(ModelType modelType) => modelType.ToString();
        public static implicit operator ModelType(string modelType) => _modelTypes[modelType];
    }
}
