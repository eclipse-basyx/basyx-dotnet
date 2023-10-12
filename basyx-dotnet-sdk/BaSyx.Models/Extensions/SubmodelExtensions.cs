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
using System;
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

        public static ISubmodel GetMetadata(this ISubmodel submodel)
        {
            Submodel trimmedSubmodel = new Submodel(submodel.Id, submodel.Id);
            trimmedSubmodel.Administration = submodel.Administration;
            trimmedSubmodel.Category = submodel.Category;
            trimmedSubmodel.Kind = submodel.Kind;
            trimmedSubmodel.SemanticId  = submodel.SemanticId;
            trimmedSubmodel.SupplementalSemanticIds = submodel.SupplementalSemanticIds;
            trimmedSubmodel.Qualifiers = submodel.Qualifiers;
            trimmedSubmodel.Description = submodel.Description;
            trimmedSubmodel.DisplayName = submodel.DisplayName;
            trimmedSubmodel.EmbeddedDataSpecifications = submodel.EmbeddedDataSpecifications;
            trimmedSubmodel.ConceptDescription = submodel.ConceptDescription;
            trimmedSubmodel.SubmodelElements = null;

            return trimmedSubmodel;
        }

        #endregion
    }
}
