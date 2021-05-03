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
using NLog;
using System;
using System.IO;
using System.Linq;

namespace BaSyx.Utils.PathHandling
{
    public static class PathExtensions
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
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
                logger.Error(e, "Error writing stream to file: " + e.Message);
                return null;
            }
        }

        public static Uri Append(this Uri uri, params string[] pathElements)
        {
            return new Uri(pathElements.Aggregate(uri.AbsoluteUri, (currentElement, pathElement) => string.Format("{0}/{1}", currentElement.TrimEnd('/'), pathElement.TrimStart('/'))));
        }
    }
}
