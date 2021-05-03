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
using System.IO;
using System.IO.Packaging;
using System.Linq;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using NLog;
using BaSyx.Utils.FileHandling;

namespace BaSyx.Models.Export
{
    public sealed class AASX : IDisposable
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

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    
        public List<PackagePart> SupplementaryFiles { get; } = new List<PackagePart>();

        private readonly Package _aasxPackage;
        private PackagePart originPart;
        private PackagePart specPart;

        public AASX(Package aasxPackage)
        {
            _aasxPackage = aasxPackage ?? throw new ArgumentNullException(nameof(aasxPackage));

            LoadOrCreateOrigin();
            LoadSpec();
            LoadSupplementaryFiles();
        }

        public AASX(string aasxFilePath) 
            : this(Package.Open(aasxFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
        { }

        public AASX(string aasxFilePath, FileMode fileMode, FileAccess fileAccess, FileShare fileShare) 
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
                        logger.Warn(e, "Relationsship " + relationship.TargetUri + "does not exist in the package - Exception: " + e.Message);
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

        private void ClearRelationshipAndPartFromPackagePart(PackagePart sourcePackagePart, string relationshipType, Uri targetUri)
        {
            PackageRelationshipCollection relationships = sourcePackagePart.GetRelationshipsByType(relationshipType);
            foreach (var relationship in relationships.ToList())
                if (relationship.TargetUri == targetUri)
                        sourcePackagePart.DeleteRelationship(relationship.Id);

            if (_aasxPackage.PartExists(targetUri))
                _aasxPackage.DeletePart(targetUri);
        }
        private void ClearRelationshipAndPartFromPackage(string relationshipType, Uri targetUri)
        {
            PackageRelationshipCollection relationships = _aasxPackage.GetRelationshipsByType(relationshipType);
            foreach (var relationship in relationships.ToList())
                if (relationship.TargetUri == targetUri)
                    _aasxPackage.DeleteRelationship(relationship.Id);

            if (_aasxPackage.PartExists(targetUri))
                _aasxPackage.DeletePart(targetUri);
        }


        private void ClearRelationshipsAndPartFromPackage(string relationshipType)
        {
            PackageRelationshipCollection relationships = _aasxPackage.GetRelationshipsByType(relationshipType);
            foreach (var relationship in relationships.ToList())
            {
                _aasxPackage.DeleteRelationship(relationship.Id);

                if(_aasxPackage.PartExists(relationship.TargetUri))
                    _aasxPackage.DeletePart(relationship.TargetUri);
            }           
        }

        private void ClearRelationshipsAndPartFromPackagePart(PackagePart sourcePackagePart, string relationshipType)
        {
            PackageRelationshipCollection relationships = sourcePackagePart.GetRelationshipsByType(relationshipType);
            foreach (var relationship in relationships.ToList())
            {
                sourcePackagePart.DeleteRelationship(relationship.Id);

                if (_aasxPackage.PartExists(relationship.TargetUri))
                    _aasxPackage.DeletePart(relationship.TargetUri);
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
                    logger.Warn(e, "Relationsship " + relationship.TargetUri + "does not exist in the package - Exception: " + e.Message);
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
                    logger.Warn(e, "Relationsship " + relationship.TargetUri + "does not exist in the package - Exception: " + e.Message);
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
                logger.Error(e, "Unable to write to Core Properties");
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
                logger.Error(e, "Unable to retrieve Core Properties");
                return null;
            }
        }

        public void AddThumbnail(string thumbnailPath)
        {
            using (FileStream fileStream = new FileStream(thumbnailPath, FileMode.Open, FileAccess.Read))
            {
                string contentType = GetContentType(thumbnailPath);
                string fileName = Path.GetFileName(thumbnailPath);
                AddThumbnail(fileStream, contentType, fileName);
            }
        }

        public void AddThumbnail(Stream thumbnail, string contentType, string fileName)
        {
            ClearRelationshipsAndPartFromPackage(THUMBNAIL_RELATIONSHIP_TYPE);

            string thumbnailUriPath = ROOT_FOLDER + fileName;

            Uri partUri = PackUriHelper.CreatePartUri(new Uri(thumbnailUriPath, UriKind.RelativeOrAbsolute));
            PackagePart thumbnailPart = _aasxPackage.CreatePart(partUri, contentType, CompressionOption.Maximum);
            _aasxPackage.CreateRelationship(thumbnailPart.Uri, TargetMode.Internal, THUMBNAIL_RELATIONSHIP_TYPE);
            
            using (Stream destination = thumbnailPart.GetStream())
            {
                thumbnail.CopyTo(destination);
            }
        }

        public void AddEnvironment(Identifier aasId, string aasEnvironmentFilePath)
        {
            if (aasId == null)
                throw new ArgumentNullException(nameof(aasId));
            if (string.IsNullOrEmpty(aasEnvironmentFilePath))
                throw new ArgumentNullException(nameof(aasEnvironmentFilePath));
            if(!File.Exists(aasEnvironmentFilePath))
                throw new InvalidOperationException(aasEnvironmentFilePath + " does not exist");
        
            string aasIdName = aasId.Id;
            foreach (char invalidChar in InvalidFileNameChars)
                aasIdName = aasIdName.Replace(invalidChar, '_');

            string aasFilePath = AASX_FOLDER + "/" + aasIdName + "/" + aasIdName + ".aas" + Path.GetExtension(aasEnvironmentFilePath);

            Uri partUri = PackUriHelper.CreatePartUri(new Uri(aasFilePath, UriKind.RelativeOrAbsolute));
            ClearRelationshipAndPartFromPackagePart(originPart, SPEC_RELATIONSHIP_TYPE, partUri);

            specPart = _aasxPackage.CreatePart(partUri, GetContentType(aasEnvironmentFilePath), CompressionOption.Maximum);
            originPart.CreateRelationship(specPart.Uri, TargetMode.Internal, SPEC_RELATIONSHIP_TYPE);

            CopyFileToPackagePart(specPart, aasEnvironmentFilePath);
        }

        public void AddEnvironment(Identifier aasId, AssetAdministrationShellEnvironment_V2_0 environment, ExportType exportType)
        {
            if (aasId == null)
                throw new ArgumentNullException(nameof(aasId));
            if (environment == null)
                throw new ArgumentNullException(nameof(environment));

            string aasIdName = aasId.Id;
            foreach (char invalidChar in InvalidFileNameChars)
                aasIdName = aasIdName.Replace(invalidChar, '_');

            string aasFilePath = AASX_FOLDER + "/" + aasIdName + "/" + aasIdName + ".aas." + exportType.ToString().ToLower();

            Uri partUri = PackUriHelper.CreatePartUri(new Uri(aasFilePath, UriKind.RelativeOrAbsolute));
            ClearRelationshipAndPartFromPackagePart(originPart, SPEC_RELATIONSHIP_TYPE, partUri);

            specPart = _aasxPackage.CreatePart(partUri, GetContentType(aasFilePath), CompressionOption.Maximum);
            originPart.CreateRelationship(specPart.Uri, TargetMode.Internal, SPEC_RELATIONSHIP_TYPE);

            string environmentTemp = Path.GetRandomFileName() + "." + exportType.ToString().ToLower();
            environment.WriteEnvironment_V2_0(exportType, environmentTemp);

            CopyFileToPackagePart(specPart, environmentTemp);

            File.Delete(environmentTemp);
        }

        public AssetAdministrationShellEnvironment_V1_0 GetEnvironment_V1_0()
        {
            if(specPart?.Uri != null)
            {
                string specFilePath = specPart.Uri.ToString();
                string extension = Path.GetExtension(specFilePath)?.ToLower();
                AssetAdministrationShellEnvironment_V1_0 environment;
                switch (extension)
                {
                    case ".json":
                        {
                            using (Stream file = specPart.GetStream(FileMode.Open, FileAccess.Read))
                                environment = AssetAdministrationShellEnvironment_V1_0.ReadEnvironment_V1_0(file, ExportType.Json);
                        }
                        break;
                    case ".xml":
                        {
                            using (Stream file = specPart.GetStream(FileMode.Open, FileAccess.Read))
                                environment = AssetAdministrationShellEnvironment_V1_0.ReadEnvironment_V1_0(file, ExportType.Xml);
                        }
                        break;
                    default:
                        logger.Error("Not supported file format: " + extension);
                        environment = null;
                        break;
                }
                return environment;
            }
            return null;
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
                        logger.Error("Not supported file format: " + extension);
                        environment = null;
                        break;
                }
                return environment;
            }
            return null;
        }

        public void AddFilesToAASX(Dictionary<string, IFile> fileDestinationMapping, CompressionOption compressionOption = CompressionOption.Maximum)
        {
            for (int i = 0; i < fileDestinationMapping.Count; i++)
            {
                string filePath = fileDestinationMapping.ElementAt(i).Key;
                IFile file = fileDestinationMapping.ElementAt(i).Value;

                string relativeDestination;
                if (!file.Value.StartsWith(AASX_FOLDER))
                    relativeDestination = AASX_FOLDER + file.Value;
                else
                    relativeDestination = file.Value;

                Uri uri = PackUriHelper.CreatePartUri(new Uri(relativeDestination, UriKind.Relative));

                ClearRelationshipAndPartFromPackagePart(specPart, SUPPLEMENTAL_RELATIONSHIP_TYPE, uri);

                string contentType;
                if (!string.IsNullOrEmpty(file.MimeType))
                    contentType = file.MimeType;
                else
                    contentType = GetContentType(filePath);

                PackagePart packagePart = _aasxPackage.CreatePart(uri, contentType, compressionOption);
                specPart.CreateRelationship(packagePart.Uri, TargetMode.Internal, SUPPLEMENTAL_RELATIONSHIP_TYPE);

                CopyFileToPackagePart(packagePart, filePath);
            }
        }

        public void AddFileToAASX(string targetUri, string filePath, CompressionOption compressionOption = CompressionOption.Maximum)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                string contentType = GetContentType(filePath);
                AddStreamToAASX(targetUri, fileStream, contentType, compressionOption);              
            }
        }

        public void AddStreamToAASX(string targetUri, Stream stream, string contentType, CompressionOption compressionOption = CompressionOption.Maximum)
        {
            //string relativeDestination;
            //if (!targetUri.StartsWith(AASX_FOLDER))
            //    relativeDestination = AASX_FOLDER + targetUri;
            //else
            //    relativeDestination = targetUri;

            Uri uri = PackUriHelper.CreatePartUri(new Uri(targetUri, UriKind.Relative));//relativeDestination

            ClearRelationshipAndPartFromPackagePart(specPart, SUPPLEMENTAL_RELATIONSHIP_TYPE, uri);         

            PackagePart packagePart = _aasxPackage.CreatePart(uri, contentType, compressionOption);
            specPart.CreateRelationship(packagePart.Uri, TargetMode.Internal, SUPPLEMENTAL_RELATIONSHIP_TYPE);

            using (Stream destination = packagePart.GetStream())
            {
                stream.CopyTo(destination);
            }
        }

        private void CopyStreamToPackagePart(PackagePart packagePart, Stream stream)
        {
            using (stream)
            {
                using (Stream destination = packagePart.GetStream())
                {
                    stream.CopyTo(destination);
                }
            }
        }

        private void CopyFileToPackagePart(PackagePart packagePart, string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (Stream destination = packagePart.GetStream())
                {
                    fileStream.CopyTo(destination);
                }
            }
        }

        private static string GetContentType(string filePath)
        {
            if (!MimeTypes.TryGetContentType(filePath, out string contentType))
                return null;

            return contentType;
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
