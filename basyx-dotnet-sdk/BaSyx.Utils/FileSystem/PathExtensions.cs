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
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;

namespace BaSyx.Utils.FileSystem
{
    public static class PathExtensions
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger("PathHandling");
        public static FileInfo ToFile(this Stream stream, string filePath)
        {
            try
            {
                using (stream)
                {
                    using (FileStream dest = File.Open(filePath, FileMode.OpenOrCreate))
                        stream.CopyTo(dest);
                }
                return new FileInfo(filePath);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error writing stream to file: " + e.Message);
                return null;
            }
        }

        public static Uri Append(this Uri uri, params string[] pathElements)
        {
            return new Uri(pathElements.Aggregate(uri.AbsoluteUri, (currentElement, pathElement) => string.Format("{0}/{1}", currentElement.TrimEnd('/'), pathElement.TrimStart('/'))));
        }
    }
}
