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
using BaSyx.API.Components;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.Common;
using BaSyx.Utils.DependencyInjection;
using BaSyx.Utils.ResultHandling;
using Newtonsoft.Json;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BaSyx.Registry.ReferenceImpl.FileBased
{
    public class FileBasedRegistry : IAssetAdministrationShellRegistry
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public const string SubmodelFolder = "Submodels";
        
        public FileBasedRegistrySettings Settings { get; }
        public JsonSerializerSettings JsonSerializerSettings { get; }
        public string FolderPath { get; }
        public FileBasedRegistry(FileBasedRegistrySettings settings = null)
        {
            Settings = settings ?? FileBasedRegistrySettings.LoadSettings();
            JsonSerializerSettings = new DependencyInjectionJsonSerializerSettings();

            FolderPath = Settings.Miscellaneous["FolderPath"];

            if (string.IsNullOrEmpty(FolderPath))
            {
                logger.Error("FolderPath is null or empty");
                throw new ArgumentNullException("FolderPath");
            }
            if (!Directory.Exists(FolderPath))
            {
                DirectoryInfo info;
                try
                {
                    info = Directory.CreateDirectory(FolderPath);
                }
                catch (Exception e)
                {
                    logger.Error("FolderPath does not exist and cannot be created: " + e.Message);
                    throw;
                }

                if (!info.Exists)
                {
                    logger.Error("FolderPath does not exist and cannot be created");
                    throw new InvalidOperationException("FolderPath does not exist and cannot be created");
                }
            }
        }
        public IResult<IAssetAdministrationShellDescriptor> CreateOrUpdateAssetAdministrationShellRegistration(string aasId, IAssetAdministrationShellDescriptor aasDescriptor)
        {
            if(string.IsNullOrEmpty(aasId))
                return new Result<IAssetAdministrationShellDescriptor>(new ArgumentNullException(nameof(aasId)));
            if (aasDescriptor == null)
                return new Result<IAssetAdministrationShellDescriptor>(new ArgumentNullException(nameof(aasDescriptor)));
            if (aasDescriptor.Identification?.Id == null)
                return new Result<IAssetAdministrationShellDescriptor>(new ArgumentNullException(nameof(aasDescriptor.Identification)));
            if (string.IsNullOrEmpty(aasDescriptor.IdShort))
                return new Result<IAssetAdministrationShellDescriptor>(new ArgumentNullException(nameof(aasDescriptor.IdShort)));

            try
            {
                string aasIdHash = GetHashString(aasId);
                string aasDirectoryPath = Path.Combine(FolderPath, aasIdHash);
                
                if (!Directory.Exists(aasDirectoryPath))
                    Directory.CreateDirectory(aasDirectoryPath);

                if(aasDescriptor.SubmodelDescriptors?.Count() > 0)
                {
                    foreach (var submodelDescriptor in aasDescriptor.SubmodelDescriptors)
                    {
                        var interimResult = CreateOrUpdateSubmodelRegistration(aasId, submodelDescriptor.Identification.Id, submodelDescriptor);
                        if (!interimResult.Success)
                            return new Result<IAssetAdministrationShellDescriptor>(interimResult);
                    }
                }
                aasDescriptor.SubmodelDescriptors = new ElementContainer<ISubmodelDescriptor>();

                string aasDescriptorContent = JsonConvert.SerializeObject(aasDescriptor, JsonSerializerSettings);
                string aasFilePath = Path.Combine(aasDirectoryPath, aasIdHash) + ".json";
                File.WriteAllText(aasFilePath, aasDescriptorContent);

                IResult<IAssetAdministrationShellDescriptor> readResult = RetrieveAssetAdministrationShellRegistration(aasId);
                return readResult;
            }
            catch (Exception e)
            {
                return new Result<IAssetAdministrationShellDescriptor>(e);
            }
        }

        public IResult<ISubmodelDescriptor> CreateOrUpdateSubmodelRegistration(string aasId, string submodelId, ISubmodelDescriptor submodelDescriptor)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result<ISubmodelDescriptor>(new ArgumentNullException(nameof(aasId)));
            if (string.IsNullOrEmpty(submodelId))
                return new Result<ISubmodelDescriptor>(new ArgumentNullException(nameof(submodelId)));
            if (submodelDescriptor == null)
                return new Result<ISubmodelDescriptor>(new ArgumentNullException(nameof(submodelDescriptor)));

            string aasIdHash = GetHashString(aasId);
            string aasDirectoryPath = Path.Combine(FolderPath, aasIdHash);
            if (!Directory.Exists(aasDirectoryPath))
                return new Result<ISubmodelDescriptor>(false, new Message(MessageType.Error, "AssetAdministrationShell does not exist - register AAS first"));

            try
            {
                string submodelDirectory = Path.Combine(aasDirectoryPath, SubmodelFolder);
                string submodelContent = JsonConvert.SerializeObject(submodelDescriptor, JsonSerializerSettings);
                if (!Directory.Exists(submodelDirectory))
                    Directory.CreateDirectory(submodelDirectory);

                string submodelIdHash = GetHashString(submodelId);
                string submodelFilePath = Path.Combine(submodelDirectory, submodelIdHash) + ".json";
                File.WriteAllText(submodelFilePath, submodelContent);

                IResult<ISubmodelDescriptor> readSubmodel = RetrieveSubmodelRegistration(aasId, submodelId);
                return readSubmodel;
            }
            catch (Exception e)
            {
                return new Result<ISubmodelDescriptor>(e);
            }
        }

        public IResult DeleteAssetAdministrationShellRegistration(string aasId)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result(new ArgumentNullException(nameof(aasId)));

            string aasIdHash = GetHashString(aasId);
            string aasDirectoryPath = Path.Combine(FolderPath, aasIdHash);
            if (!Directory.Exists(aasDirectoryPath))
                return new Result(false, new NotFoundMessage($"Asset Administration Shell with {aasId}"));
            else
            {
                try
                {
                    Directory.Delete(aasDirectoryPath, true);
                    return new Result(true);
                }
                catch (Exception e)
                {
                    return new Result(e);
                }
            }
        }

        public IResult DeleteSubmodelRegistration(string aasId, string submodelId)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result(new ArgumentNullException(nameof(aasId)));
            if (string.IsNullOrEmpty(submodelId))
                return new Result(new ArgumentNullException(nameof(submodelId)));

            string aasIdHash = GetHashString(aasId);
            string submodelIdHash = GetHashString(submodelId);
            string aasDirectoryPath = Path.Combine(FolderPath, aasIdHash);
            if (!Directory.Exists(aasDirectoryPath))
                return new Result(false, new NotFoundMessage($"Asset Administration Shell with {aasId}"));

            string submodelFilePath = Path.Combine(aasDirectoryPath, SubmodelFolder, submodelIdHash) + ".json";
            if (!File.Exists(submodelFilePath))
                return new Result(false, new NotFoundMessage($"Submodel with {submodelId}"));
            else
            {
                try
                {
                    File.Delete(submodelFilePath);
                    return new Result(true);
                }
                catch (Exception e)
                {
                    return new Result(e);
                }
            }
        }

        public IResult<IAssetAdministrationShellDescriptor> RetrieveAssetAdministrationShellRegistration(string aasId)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result<IAssetAdministrationShellDescriptor>(new ArgumentNullException(nameof(aasId)));

            string aasIdHash = GetHashString(aasId);
            string aasFilePath = Path.Combine(FolderPath, aasIdHash, aasIdHash) + ".json";
            if (File.Exists(aasFilePath))
            {
                try
                {
                    string aasContent = File.ReadAllText(aasFilePath);
                    IAssetAdministrationShellDescriptor descriptor = JsonConvert.DeserializeObject<IAssetAdministrationShellDescriptor>(aasContent, JsonSerializerSettings);

                    var submodelDescriptors = RetrieveAllSubmodelRegistrations(aasId);
                    if(submodelDescriptors.Success && submodelDescriptors.Entity?.Count() > 0)
                        descriptor.SubmodelDescriptors = submodelDescriptors.Entity;

                    return new Result<IAssetAdministrationShellDescriptor>(true, descriptor);
                }
                catch (Exception e)
                {
                    return new Result<IAssetAdministrationShellDescriptor>(e);
                }
            }
            else
                return new Result<IAssetAdministrationShellDescriptor>(false, new NotFoundMessage($"Asset Administration Shell with {aasId}"));

        }
        public IResult<IQueryableElementContainer<IAssetAdministrationShellDescriptor>> RetrieveAllAssetAdministrationShellRegistrations(Predicate<IAssetAdministrationShellDescriptor> predicate)
        {
            var allDescriptors = RetrieveAllAssetAdministrationShellRegistrations();
            return new Result<IQueryableElementContainer<IAssetAdministrationShellDescriptor>>(allDescriptors.Success, allDescriptors.Entity.Where(ConvertToFunc(predicate)).AsQueryableElementContainer());
        }

        private Func<T, bool> ConvertToFunc<T>(Predicate<T> predicate)
        {
            return new Func<T, bool>(predicate);
        }

        public IResult<IQueryableElementContainer<IAssetAdministrationShellDescriptor>> RetrieveAllAssetAdministrationShellRegistrations()
        {
            string[] aasDirectories;
            try
            {
                aasDirectories = Directory.GetDirectories(FolderPath);
            }
            catch (Exception e)
            {
                return new Result<IQueryableElementContainer<IAssetAdministrationShellDescriptor>>(e);
            }

            ElementContainer<IAssetAdministrationShellDescriptor> aasDescriptors = new ElementContainer<IAssetAdministrationShellDescriptor>();

            if (aasDirectories?.Length > 0)
                foreach (var directory in aasDirectories)
                {
                    try
                    {
                        string aasIdHash = directory.Split(Path.DirectorySeparatorChar).Last();
                        string aasFilePath = Path.Combine(directory, aasIdHash) + ".json";
                        IResult<IAssetAdministrationShellDescriptor> readAASDescriptor = ReadAssetAdministrationShell(aasFilePath);
                        if (readAASDescriptor.Success && readAASDescriptor.Entity != null)
                            aasDescriptors.Create(readAASDescriptor.Entity);
                    }
                    catch (Exception e)
                    {
                        return new Result<IQueryableElementContainer<IAssetAdministrationShellDescriptor>>(e);
                    }
                }
            return new Result<IQueryableElementContainer<IAssetAdministrationShellDescriptor>>(true, aasDescriptors.AsQueryableElementContainer());
        }

        private IResult<IAssetAdministrationShellDescriptor> ReadAssetAdministrationShell(string aasFilePath)
        {
            if (string.IsNullOrEmpty(aasFilePath))
                return new Result<IAssetAdministrationShellDescriptor>(new ArgumentNullException(nameof(aasFilePath)));

            if (File.Exists(aasFilePath))
            {
                try
                {
                    string aasContent = File.ReadAllText(aasFilePath);
                    IAssetAdministrationShellDescriptor aasDescriptor = JsonConvert.DeserializeObject<IAssetAdministrationShellDescriptor>(aasContent, JsonSerializerSettings);

                    var submodelDescriptors = RetrieveAllSubmodelRegistrations(aasDescriptor.Identification.Id);
                    if (submodelDescriptors.Success && submodelDescriptors.Entity != null)
                        aasDescriptor.SubmodelDescriptors = submodelDescriptors.Entity;

                    return new Result<IAssetAdministrationShellDescriptor>(true, aasDescriptor);
                }
                catch (Exception e)
                {
                    return new Result<IAssetAdministrationShellDescriptor>(e);
                }
            }
            else
                return new Result<IAssetAdministrationShellDescriptor>(false, new NotFoundMessage("Asset Administration Shell"));

        }

        public IResult<ISubmodelDescriptor> RetrieveSubmodelRegistration(string aasId, string submodelId)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result<ISubmodelDescriptor>(new ArgumentNullException(nameof(aasId)));
            if (string.IsNullOrEmpty(submodelId))
                return new Result<ISubmodelDescriptor>(new ArgumentNullException(nameof(submodelId)));

            string aasIdHash = GetHashString(aasId);
            string submodelIdHash = GetHashString(submodelId);
            string aasDirectoryPath = Path.Combine(FolderPath, aasIdHash);
            if (Directory.Exists(aasDirectoryPath))
            {
                string submodelPath = Path.Combine(aasDirectoryPath, SubmodelFolder, submodelIdHash) + ".json";
                if (File.Exists(submodelPath))
                {
                    try
                    {
                        string submodelContent = File.ReadAllText(submodelPath);
                        ISubmodelDescriptor descriptor = JsonConvert.DeserializeObject<ISubmodelDescriptor>(submodelContent, JsonSerializerSettings);
                        return new Result<ISubmodelDescriptor>(true, descriptor);
                    }
                    catch (Exception e)
                    {
                        return new Result<ISubmodelDescriptor>(e);
                    }
                }
                else
                    return new Result<ISubmodelDescriptor>(false, new NotFoundMessage($"Submodel with {submodelId}"));
            }
            else
                return new Result<ISubmodelDescriptor>(false, new NotFoundMessage($"Asset Administration Shell with {aasId}"));
        }

        public IResult<IQueryableElementContainer<ISubmodelDescriptor>> RetrieveAllSubmodelRegistrations(string aasId, Predicate<ISubmodelDescriptor> predicate)
        {
            var allDescriptors = RetrieveAllSubmodelRegistrations(aasId);
            return new Result<IQueryableElementContainer<ISubmodelDescriptor>>(allDescriptors.Success, allDescriptors.Entity.Where(ConvertToFunc(predicate)).AsQueryableElementContainer());
        }

        public IResult<IQueryableElementContainer<ISubmodelDescriptor>> RetrieveAllSubmodelRegistrations(string aasId)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result<IQueryableElementContainer<ISubmodelDescriptor>>(new ArgumentNullException(nameof(aasId)));

            string aasIdHash = GetHashString(aasId);
            string aasDirectoryPath = Path.Combine(FolderPath, aasIdHash);
            if (Directory.Exists(aasDirectoryPath))
            {
                try
                {
                    ElementContainer<ISubmodelDescriptor> submodelDescriptors = new ElementContainer<ISubmodelDescriptor>();
                    string submodelDirectoryPath = Path.Combine(aasDirectoryPath, SubmodelFolder);
                    if (Directory.Exists(submodelDirectoryPath))
                    {
                        string[] files = Directory.GetFiles(submodelDirectoryPath);

                        foreach (var file in files)
                        {
                            string submodelContent = File.ReadAllText(file);
                            ISubmodelDescriptor descriptor = JsonConvert.DeserializeObject<ISubmodelDescriptor>(submodelContent, JsonSerializerSettings);
                            if (descriptor != null)
                                submodelDescriptors.Create(descriptor);
                            else
                                logger.Warn($"Unable to read Submodel Descriptor from {file}");
                        }
                    }
                    return new Result<IQueryableElementContainer<ISubmodelDescriptor>>(true, submodelDescriptors.AsQueryableElementContainer());
                }
                catch (Exception e)
                {
                    return new Result<IQueryableElementContainer<ISubmodelDescriptor>>(e);
                }
            }
            else
                return new Result<IQueryableElementContainer<ISubmodelDescriptor>>(false, new NotFoundMessage($"Asset Administration Shell with {aasId}"));
        }

        private static string GetHashString(string input)
        {
            SHA256 shaAlgorithm = SHA256.Create();
            byte[] data = Encoding.UTF8.GetBytes(input);

            byte[] bHash = shaAlgorithm.ComputeHash(data);

            string hashString = string.Empty;
            for (int i = 0; i < bHash.Length; i++)
            {
                hashString += bHash[i].ToString("x2");

                //break condition for filename length
                if (i == 255)
                    break;
            }
            return hashString;
        }
    }
}
