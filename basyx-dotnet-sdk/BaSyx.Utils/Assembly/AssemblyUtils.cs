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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BaSyx.Utils.Assembly
{
    public static class AssemblyUtils
    {
        private class AssemblyNameComparer : EqualityComparer<AssemblyName>
        {
            public override bool Equals(AssemblyName x, AssemblyName y)
            {
                if (x.FullName == y.FullName)
                    return true;
                else
                    return false;
            }

            public override int GetHashCode(AssemblyName obj)
            {
                unchecked
                {
                    var result = 0;
                    result = (result * 397) ^ obj.FullName.GetHashCode();
                    return result;
                }
            }
        }
        /// <summary>
        /// Returns all loaded or referenced assemblies within the current application domain. Microsoft or system assemblies are excluded.
        /// </summary>
        /// <returns></returns>
        public static List<System.Reflection.Assembly> GetLoadedAssemblies()
        {
            List<System.Reflection.Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.FullName.StartsWith("Microsoft") && !a.FullName.StartsWith("System"))
                    .ToList();
            List<AssemblyName> assemblyNames = new List<AssemblyName>();
            foreach (var assembly in assemblies)
            {
                List<AssemblyName> referencedAssemblyNames = assembly.GetReferencedAssemblies().ToList();
                assemblyNames.AddRange(referencedAssemblyNames);
            }
            assemblyNames = assemblyNames
                .Distinct(new AssemblyNameComparer())
                .Where(a => !a.FullName.StartsWith("Microsoft") && !a.FullName.StartsWith("System"))?
                .ToList();

            List<System.Reflection.Assembly> referencedAssemblies = assemblyNames.ConvertAll(c => System.Reflection.Assembly.Load(c));
            assemblies.AddRange(referencedAssemblies);

            return assemblies;
        }
    }
}
