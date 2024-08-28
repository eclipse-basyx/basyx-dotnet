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
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace BaSyx.Models.Export
{
    public sealed class AASX_V2_0 : IDisposable
    {
        public const string ROOT_FOLDER = "/";
        public const string AASX_FOLDER = "/aasx";

        public const string ORIGIN_RELATIONSHIP_TYPE = "http://www.admin-shell.io/aasx/relationships/aasx-origin";
        public const string SPEC_RELATIONSHIP_TYPE = "http://www.admin-shell.io/aasx/relationships/aas-spec";
        public const string SUPPLEMENTAL_RELATIONSHIP_TYPE = "http://www.admin-shell.io/aasx/relationships/aas-suppl";
        public const string THUMBNAIL_RELATIONSHIP_TYPE = "http://schemas.openxmlformats.org/package/2006/relationships/metadata/thumbnail";
        public const string MIMETYPE = "application/asset-administration-shell-package";

        public static readonly char[] InvalidFileNameChars = GetInvalidFileNameChars();
        public static readonly Uri ORIGIN_URI = new Uri("/aasx/aasx-origin", UriKind.RelativeOrAbsolute);

        private static readonly ILogger logger = LoggingExtentions.CreateLogger<AASX_V2_0>();

        public List<PackagePart> SupplementaryFiles { get; } = new List<PackagePart>();

        private readonly Package _aasxPackage;
        private PackagePart originPart;
        private PackagePart specPart;

        public AASX_V2_0(Package aasxPackage)
        {
            _aasxPackage = aasxPackage ?? throw new ArgumentNullException(nameof(aasxPackage));

            LoadOrCreateOrigin();
            LoadSpec();
            LoadSupplementaryFiles();
        }

        public AASX_V2_0(string aasxFilePath) 
            : this(Package.Open(aasxFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
        { }

        public AASX_V2_0(string aasxFilePath, FileMode fileMode, FileAccess fileAccess, FileShare fileShare) 
            : this(Package.Open(aasxFilePath, fileMode, fileAccess, fileShare))
        { }

        private void LoadSupplementaryFiles()
        {
            if (specPart != null)
            {
                PackageRelationshipCollection relationships = specPart.GetRelationshipsByType(SUPPLEMENTAL_RELATIONSHIP_TYPE);
                foreach (var relationship in relationships)
                {
                    try
                    {
                        PackagePart file = _aasxPackage.GetPart(relationship.TargetUri);
                        SupplementaryFiles.Add(file);
                    }
                    catch(Exception e)
                    {
                        logger.LogWarning(e, "Relationsship " + relationship.TargetUri + "does not exist in the package - Exception: " + e.Message);
                        continue;
                    }
                  
                }
            }
        }

        private void LoadOrCreateOrigin()
        {
            PackageRelationshipCollection relationships = _aasxPackage.GetRelationshipsByType(ORIGIN_RELATIONSHIP_TYPE);
            originPart = relationships?.Where(r => r.TargetUri == ORIGIN_URI)?.Select(p => _aasxPackage.GetPart(p.TargetUri))?.FirstOrDefault();
            if(originPart == null)
            {
                originPart = _aasxPackage.CreatePart(ORIGIN_URI, System.Net.Mime.MediaTypeNames.Text.Plain, CompressionOption.Maximum);
                originPart.GetStream(FileMode.Create).Dispose();
                _aasxPackage.CreateRelationship(originPart.Uri, TargetMode.Internal, ORIGIN_RELATIONSHIP_TYPE);
            }
        }

        private void LoadSpec()
        {
            if(originPart != null)
            {
                PackageRelationshipCollection relationships = originPart.GetRelationshipsByType(SPEC_RELATIONSHIP_TYPE);
                specPart = relationships?.Select(s => _aasxPackage.GetPart(s.TargetUri))?.FirstOrDefault();
            }
        }

        public Stream GetFileAsStream(string fileName, out string contentType)
        {
            PackagePart part = SupplementaryFiles.Find(p => p.Uri.ToString().Contains(fileName));
            contentType = part?.ContentType;
            return part?.GetStream();
        }

        public Stream GetFileAsStream(Uri relativeUri, out string contentType)
        {
            PackagePart part = SupplementaryFiles.Find(p => p.Uri == relativeUri);
            contentType = part?.ContentType;
            return part?.GetStream();
        }

        /// <summary>
        /// Returns the AASX-Package thumbnail as stream
        /// </summary>
        /// <returns></returns>
        public Stream GetThumbnailAsStream()
        {
            PackageRelationshipCollection relationships = _aasxPackage.GetRelationshipsByType(THUMBNAIL_RELATIONSHIP_TYPE);
            foreach (var relationship in relationships)
            {
                try
                {
                    PackagePart packagePart = _aasxPackage.GetPart(relationship.TargetUri);
                    if(packagePart != null)
                        return packagePart.GetStream(FileMode.Open, FileAccess.Read);
                }
                catch (Exception e)
                {
                    logger.LogWarning(e, "Relationsship " + relationship.TargetUri + "does not exist in the package - Exception: " + e.Message);
                    continue;
                }
            }
            return null;
        }
        /// <summary>
        /// Returns the AASX-Package thumbnail as PackagePart
        /// </summary>
        /// <returns></returns>
        public PackagePart GetThumbnailAsPackagePart()
        {
            PackageRelationshipCollection relationships = _aasxPackage.GetRelationshipsByType(THUMBNAIL_RELATIONSHIP_TYPE);
            foreach (var relationship in relationships)
            {
                try
                {
                    PackagePart packagePart = _aasxPackage.GetPart(relationship.TargetUri);
                    if (packagePart != null)
                        return packagePart;
                }
                catch (Exception e)
                {
                    logger.LogWarning(e, "Relationsship " + relationship.TargetUri + "does not exist in the package - Exception: " + e.Message);
                    continue;
                }
            }
            return null;
        }

        public void AddCoreProperties(Action<PackageProperties> pp)
        {
            try
            {
                pp.Invoke(_aasxPackage.PackageProperties);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unable to write to Core Properties");
            }
        }

        public PackageProperties GetPackageProperties()
        {
            try
            {
                return _aasxPackage.PackageProperties;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unable to retrieve Core Properties");
                return null;
            }
        }
   
        public AssetAdministrationShellEnvironment_V2_0 GetEnvironment_V2_0()
        {
            if (specPart?.Uri != null)
            {
                string specFilePath = specPart.Uri.ToString();
                string extension = Path.GetExtension(specFilePath)?.ToLower();
                AssetAdministrationShellEnvironment_V2_0 environment;
                switch (extension)
                {
                    case ".json":
                        {
                            using (Stream file = specPart.GetStream(FileMode.Open, FileAccess.Read))
                                environment = AssetAdministrationShellEnvironment_V2_0.ReadEnvironment_V2_0(file, ExportType.Json);
                        }
                        break;
                    case ".xml":
                        {
                            using (Stream file = specPart.GetStream(FileMode.Open, FileAccess.Read))
                                environment = AssetAdministrationShellEnvironment_V2_0.ReadEnvironment_V2_0(file, ExportType.Xml);
                        }
                        break;
                    default:
                        logger.LogError("Not supported file format: " + extension);
                        environment = null;
                        break;
                }
                return environment;
            }
            return null;
        }

     
        private static char[] GetInvalidFileNameChars()
        {
            List<char> invalidChars = Path.GetInvalidPathChars().ToList();
            invalidChars.Add('#');

            return invalidChars.ToArray();
        }

        #region IDisposable Support
        private bool disposedValue = false;

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _aasxPackage.Close();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
