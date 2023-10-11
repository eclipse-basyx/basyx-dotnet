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
using BaSyx.API.Interfaces;
using BaSyx.Models.Connectivity;
using BaSyx.Utils.DependencyInjection;
using BaSyx.Utils.ResultHandling;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using BaSyx.Models.Extensions;
using System.Text.Json;

namespace BaSyx.Registry.ReferenceImpl.FileBased
{
    public class FileBasedRegistry : IAssetAdministrationShellRegistryInterface
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<FileBasedRegistry>();

        public const string SubmodelFolder = "Submodels";
        
        public FileBasedRegistrySettings Settings { get; }
        public JsonSerializerOptions JsonSerializerOptions { get; }
        public string FolderPath { get; }
        public FileBasedRegistry(FileBasedRegistrySettings settings = null)
        {
            Settings = settings ?? FileBasedRegistrySettings.LoadSettings();
            var options = new DefaultJsonSerializerOptions();
            var services = DefaultImplementation.GetStandardServiceCollection();
            options.AddDependencyInjection(new DependencyInjectionExtension(services));
            options.AddFullSubmodelElementConverter();
            JsonSerializerOptions = options.Build();

            FolderPath = Settings.Miscellaneous["FolderPath"];
            if (!Path.IsPathRooted(FolderPath))
                FolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FolderPath);

            if (string.IsNullOrEmpty(FolderPath))
            {
                logger.LogError("FolderPath is null or empty");
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
                    logger.LogError("FolderPath does not exist and cannot be created: " + e.Message);
                    throw;
                }

                if (!info.Exists)
                {
                    logger.LogError("FolderPath does not exist and cannot be created");
                    throw new InvalidOperationException("FolderPath does not exist and cannot be created");
                }
            }
        }

        public IResult<IAssetAdministrationShellDescriptor> CreateAssetAdministrationShellRegistration(IAssetAdministrationShellDescriptor aasDescriptor)
            => UpdateAssetAdministrationShellRegistration(aasDescriptor.Identification.Id, aasDescriptor);
        public IResult<IAssetAdministrationShellDescriptor> UpdateAssetAdministrationShellRegistration(string aasId, IAssetAdministrationShellDescriptor aasDescriptor)
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
                        var interimResult = UpdateSubmodelRegistration(aasId, submodelDescriptor.Identification.Id, submodelDescriptor);
                        if (!interimResult.Success)
                            return new Result<IAssetAdministrationShellDescriptor>(interimResult);
                    }
                }
                aasDescriptor.SubmodelDescriptors = new List<ISubmodelDescriptor>();

                string aasDescriptorContent = JsonSerializer.Serialize(aasDescriptor, JsonSerializerOptions);
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

        public IResult<ISubmodelDescriptor> CreateSubmodelRegistration(string aasId, ISubmodelDescriptor submodelDescriptor)
            => UpdateSubmodelRegistration(aasId, submodelDescriptor.Identification.Id, submodelDescriptor);

        public IResult<ISubmodelDescriptor> UpdateSubmodelRegistration(string aasId, string submodelId, ISubmodelDescriptor submodelDescriptor)
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
                string submodelContent = JsonSerializer.Serialize(submodelDescriptor, JsonSerializerOptions);
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
                    IAssetAdministrationShellDescriptor descriptor = JsonSerializer.Deserialize<IAssetAdministrationShellDescriptor>(aasContent, JsonSerializerOptions);

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
        public IResult<IEnumerable<IAssetAdministrationShellDescriptor>> RetrieveAllAssetAdministrationShellRegistrations(Predicate<IAssetAdministrationShellDescriptor> predicate)
        {
            var allDescriptors = RetrieveAllAssetAdministrationShellRegistrations();
            return new Result<IEnumerable<IAssetAdministrationShellDescriptor>>(allDescriptors.Success, allDescriptors.Entity.Where(ConvertToFunc(predicate)));
        }

        private Func<T, bool> ConvertToFunc<T>(Predicate<T> predicate)
        {
            return new Func<T, bool>(predicate);
        }

        public IResult<IEnumerable<IAssetAdministrationShellDescriptor>> RetrieveAllAssetAdministrationShellRegistrations()
        {
            string[] aasDirectories;
            try
            {
                aasDirectories = Directory.GetDirectories(FolderPath);
            }
            catch (Exception e)
            {
                return new Result<IEnumerable<IAssetAdministrationShellDescriptor>>(e);
            }

            List<IAssetAdministrationShellDescriptor> aasDescriptors = new List<IAssetAdministrationShellDescriptor>();

            if (aasDirectories?.Length > 0)
                foreach (var directory in aasDirectories)
                {
                    try
                    {
                        string aasIdHash = directory.Split(Path.DirectorySeparatorChar).Last();
                        string aasFilePath = Path.Combine(directory, aasIdHash) + ".json";
                        IResult<IAssetAdministrationShellDescriptor> readAASDescriptor = ReadAssetAdministrationShell(aasFilePath);
                        if (readAASDescriptor.Success && readAASDescriptor.Entity != null)
                            aasDescriptors.Add(readAASDescriptor.Entity);
                    }
                    catch (Exception e)
                    {
                        return new Result<IEnumerable<IAssetAdministrationShellDescriptor>>(e);
                    }
                }
            return new Result<IEnumerable<IAssetAdministrationShellDescriptor>>(true, aasDescriptors);
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
                    IAssetAdministrationShellDescriptor aasDescriptor = JsonSerializer.Deserialize<IAssetAdministrationShellDescriptor>(aasContent, JsonSerializerOptions);

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
                        ISubmodelDescriptor descriptor = JsonSerializer.Deserialize<ISubmodelDescriptor>(submodelContent, JsonSerializerOptions);
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

        public IResult<IEnumerable<ISubmodelDescriptor>> RetrieveAllSubmodelRegistrations(string aasId, Predicate<ISubmodelDescriptor> predicate)
        {
            var allDescriptors = RetrieveAllSubmodelRegistrations(aasId);
            return new Result<IEnumerable<ISubmodelDescriptor>>(allDescriptors.Success, allDescriptors.Entity.Where(ConvertToFunc(predicate)));
        }

        public IResult<IEnumerable<ISubmodelDescriptor>> RetrieveAllSubmodelRegistrations(string aasId)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result<IEnumerable<ISubmodelDescriptor>>(new ArgumentNullException(nameof(aasId)));

            string aasIdHash = GetHashString(aasId);
            string aasDirectoryPath = Path.Combine(FolderPath, aasIdHash);
            if (Directory.Exists(aasDirectoryPath))
            {
                try
                {
                    List<ISubmodelDescriptor> submodelDescriptors = new List<ISubmodelDescriptor>();
                    string submodelDirectoryPath = Path.Combine(aasDirectoryPath, SubmodelFolder);
                    if (Directory.Exists(submodelDirectoryPath))
                    {
                        string[] files = Directory.GetFiles(submodelDirectoryPath);

                        foreach (var file in files)
                        {
                            string submodelContent = File.ReadAllText(file);
                            ISubmodelDescriptor descriptor = JsonSerializer.Deserialize<ISubmodelDescriptor>(submodelContent, JsonSerializerOptions);
                            if (descriptor != null)
                                submodelDescriptors.Add(descriptor);
                            else
                                logger.LogWarning($"Unable to read Submodel Descriptor from {file}");
                        }
                    }
                    return new Result<IEnumerable<ISubmodelDescriptor>>(true, submodelDescriptors);
                }
                catch (Exception e)
                {
                    return new Result<IEnumerable<ISubmodelDescriptor>>(e);
                }
            }
            else
                return new Result<IEnumerable<ISubmodelDescriptor>>(false, new NotFoundMessage($"Asset Administration Shell with {aasId}"));
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
