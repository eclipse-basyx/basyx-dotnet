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
using System.Linq;

namespace BaSyx.Models.Extensions
{
    public static class ReferableExtensions
    {
        public static ElementTree GetElementTree(this IReferable referable)
        {
            ElementTree elementTree = new ElementTree(referable);
            if (referable is IAssetAdministrationShell shell)
            {
                if(shell.Submodels?.Count() > 0)
                {
                    foreach (var submodel in shell.Submodels.Values)
                    {
                        ElementTree submodelTree = GetElementTree(submodel);
                        elementTree.AddChild(submodelTree);
                    }
                }
            }
            else if (referable is ISubmodel submodel)
            {
                if (submodel.SubmodelElements?.Count() > 0)
                {
                    foreach (var submodelElement in submodel.SubmodelElements.Values)
                    {
                        ElementTree submodelElementTree = GetElementTree(submodelElement);
                        elementTree.AddChild(submodelElementTree);
                    }
                }
            }
            else if (referable is ISubmodelElementCollection seCollection)
            {
                foreach (var subElement in seCollection.Value.Values)
                {
                    ElementTree subElementTree = GetElementTree(subElement);
                    elementTree.AddChild(subElementTree);
                }
            }
            return elementTree;
        }

        public static IReferable GetReferable(this TreeBuilder<IReferable> elementTree)
        {
            IReferable referable = null;
            if(elementTree.Value is IAssetAdministrationShell shell)
            {
                referable = shell;
                if (elementTree.HasChildren())
                {
                    foreach (var child in elementTree.Children)
                    {
                        ISubmodel submodelChild = (ISubmodel)GetReferable(child);
                        shell.Submodels.Create(submodelChild);
                    }
                }
            }
            else if (elementTree.Value is ISubmodel submodel)
            {
                referable = submodel;
                if (elementTree.HasChildren())
                {
                    foreach (var child in elementTree.Children)
                    {
                        ISubmodelElement submodelElementChild = (ISubmodelElement)GetReferable(child);
                        submodel.SubmodelElements.Create(submodelElementChild);
                    }
                }
            }
            else if (elementTree.Value is ISubmodelElementCollection seCollection)
            {
                referable = seCollection;
                if (elementTree.HasChildren())
                {
                    foreach (var child in elementTree.Children)
                    {
                        ISubmodelElement submodelElementChild = (ISubmodelElement)GetReferable(child);
                        seCollection.Value.Create(submodelElementChild);
                    }
                }
            }
            else if (elementTree.Value is ISubmodelElement submodelElement)
            {
                referable = submodelElement;                
            }
            return referable;
        }
    }
}
