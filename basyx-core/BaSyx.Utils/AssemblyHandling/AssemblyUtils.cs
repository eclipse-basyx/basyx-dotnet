/*******************************************************************************
* Copyright (c) 2020, 2021 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BaSyx.Utils.AssemblyHandling
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
        public static List<Assembly> GetLoadedAssemblies()
        {
            List<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies()
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

            List<Assembly> referencedAssemblies = assemblyNames.ConvertAll(c => Assembly.Load(c));
            assemblies.AddRange(referencedAssemblies);

            return assemblies;
        }
    }
}
