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

		/// <summary>
		/// Returns a clone of a given submodel
		/// </summary>
		/// <param name="submodel">Given submodel</param>
		/// <returns>Cloned submodel</returns>
		public static ISubmodel Clone(this ISubmodel submodel)
        {
            // use existing function to clone (without submodel elements)
	        var submodelClone = GetMetadata(submodel) as Submodel;

			// backup case if casting went wrong
			if (submodelClone == null)
                return null;

            // clone the submodel elements
            var submodelElementsClone = CloneSubElements(submodel.SubmodelElements);
            submodelElementsClone.Parent = submodelClone;
            submodelClone.SubmodelElements = submodelElementsClone;

			return submodelClone;
        }

		/// <summary>
		/// Clone a list of given submodel elements
		/// </summary>
		/// <param name="submodelodelElements">Given submodel elements</param>
		/// <returns>List of cloned submodel elements</returns>
		private static IElementContainer<ISubmodelElement> CloneSubElements(IElementContainer<ISubmodelElement> submodelodelElements)
        {
	        IElementContainer<ISubmodelElement> submodelElementsClone = new ElementContainer<ISubmodelElement>();

			foreach (var submodelElement in submodelodelElements)
	        {
				// copy each submodel element
				var submodelElementClone = submodelElement.Clone() as ISubmodelElement;

                // backup case if casting went wrong
                if (submodelElementClone == null)
                    continue;

                submodelElementsClone.Add(submodelElementClone);
			}

            return submodelElementsClone;
        }

        public static ISubmodel GetMetadata(this ISubmodel submodel)
        {
            var trimmedSubModel = new Submodel(submodel.Id, submodel.Id)
            {
	            Administration = submodel.Administration,
	            Category = submodel.Category,
	            Kind = submodel.Kind,
	            SemanticId = submodel.SemanticId,
	            SupplementalSemanticIds = submodel.SupplementalSemanticIds,
	            Qualifiers = submodel.Qualifiers,
	            Description = submodel.Description,
	            DisplayName = submodel.DisplayName,
	            EmbeddedDataSpecifications = submodel.EmbeddedDataSpecifications,
	            ConceptDescription = submodel.ConceptDescription,
	            SubmodelElements = null
            };

			return trimmedSubModel;
        }

        #endregion
    }
}
