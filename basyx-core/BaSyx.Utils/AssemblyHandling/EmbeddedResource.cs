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
using Microsoft.Extensions.FileProviders;
using NLog;
using System.IO;
using System.Reflection;

namespace BaSyx.Utils.AssemblyHandling
{
    public static class EmbeddedResource
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Checks whether a resource is available in the assembly
        /// </summary>
        /// <param name="assembly">Assembly where the embedded resoure is supposed to reside in</param>
        /// <param name="resourceFileName">File name of the resource</param>
        /// <returns>
        /// true = Resource was found in the assembly
        /// false = Resource was not found in the assembly
        /// </returns>
        public static bool CheckResourceAvailability(Assembly assembly, string resourceFileName)
        {
            ManifestEmbeddedFileProvider embeddedFileProvider = new ManifestEmbeddedFileProvider(assembly);
            IFileInfo fileInfo = embeddedFileProvider.GetFileInfo(resourceFileName);
            if (fileInfo != null && fileInfo.Exists)
                return true;
            else 
                return false;
        }
        /// <summary>
        /// Writes an embedded resource to a file in the executing directory
        /// </summary>
        /// <param name="resourceName">Name of the embedded resourcre</param>
        /// <returns>
        /// true = Resource was written successfully
        /// false = Resource was not written successfully
        /// </returns>
        public static bool WriteEmbeddedRessourceToFile(Assembly assembly, string resourceFileName, string destinationFilename)
        {
            ManifestEmbeddedFileProvider embeddedFileProvider = new ManifestEmbeddedFileProvider(assembly);
            IFileInfo fileInfo = embeddedFileProvider.GetFileInfo(resourceFileName);

            if(fileInfo != null && fileInfo.Exists)
            {
                using(Stream stream = fileInfo.CreateReadStream())
                {
                    using(FileStream fileStream = File.OpenWrite(destinationFilename))
                    {
                        stream.CopyTo(fileStream);
                    }
                }
                logger.Info($"Resource '{resourceFileName}' successfully created at {destinationFilename}");
                return true;
            }
            else
            {
                logger.Error($"Resource '{resourceFileName}' not found in assembly {assembly.GetName().Name}");
                return false;
            }
        }
        /// <summary>
        /// Checks whether a ressource file exists at an expected location or creates it if not existing
        /// </summary>
        /// <param name="assembly">Assembly where the embedded resoure is supposed to reside in</param>
        /// <param name="expectedResourceFullPath">Expected path (including filename) where the ressource file should be on the file system</param>
        /// <returns>
        /// true = Resource exists or has been written successfully
        /// false = Resource could not be written
        /// </returns>
        public static bool CheckOrWriteRessourceToFile(Assembly assembly, string expectedResourceFullPath)
        {
            if (File.Exists(expectedResourceFullPath))
            {
                return true;
            }
            else
            {
                string fileName = Path.GetFileName(expectedResourceFullPath);
                return WriteEmbeddedRessourceToFile(assembly, fileName, expectedResourceFullPath);
            }
        }
    }
}
