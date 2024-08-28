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
using BaSyx.Models.Extensions;
using BaSyx.Utils.ResultHandling;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BaSyx.API.ServiceProvider
{
    public class FileServiceProvider : IFileServiceProvider
    {
        private readonly IFileProvider _fileProvider;

        public FileServiceProvider(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        public async Task<IResult<PackageDescription>> CreatePackageAsync(PackageDescription packageDescription, Stream content)
        {
            string packageFileName = packageDescription.PackageId + ".aasx";
            IFileInfo packageFileInfo = _fileProvider.GetFileInfo(packageFileName);
            if (packageFileInfo.Exists)
                return new Result<PackageDescription>(false, new ConflictMessage(packageFileName));            

            using (FileStream fileStream = new FileStream(packageFileInfo.PhysicalPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                await content.CopyToAsync(fileStream);
                
            }

            string packageDescriptionFileName = packageDescription.PackageId + ".json";
            IFileInfo packageDescriptionFileInfo = _fileProvider.GetFileInfo(packageDescriptionFileName);
            string packageDescriptionJsonContent = JsonSerializer.Serialize(packageDescription);

            using (FileStream fileStream = new FileStream(packageDescriptionFileInfo.PhysicalPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                byte[] packageDescriptionContent = new UTF8Encoding(true).GetBytes(packageDescriptionJsonContent);
                fileStream.Write(packageDescriptionContent, 0, packageDescriptionContent.Length);
            }

            return new Result<PackageDescription>(true, packageDescription);
        }

        public Task<IResult> DeletePackageDescriptionAsync(string packageId)
        {
            throw new NotImplementedException();
        }

        public async Task<IResult<IEnumerable<PackageDescription>>> GetAllPackageDescriptionsAsync()
        {
            var contents = _fileProvider.GetDirectoryContents("");
            List<PackageDescription> packageDescriptions = new List<PackageDescription>();
            foreach (var item in contents)
            {
                if (item.Name.Contains(".json"))
                {
                    using (var stream = item.CreateReadStream())
                    {
                        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            string content = await reader.ReadToEndAsync();
                            PackageDescription packageDescription = JsonSerializer.Deserialize<PackageDescription>(content);
                            packageDescriptions.Add(packageDescription);
                        }
                    }
                }
            }
            return new Result<IEnumerable<PackageDescription>>(true, packageDescriptions);
        }

        public async Task<IResult<PackageDescription>> GetPackageDescriptionAsync(string packageId)
        {
            string fileName = packageId + ".json";
            var packageDescriptionFile = _fileProvider.GetFileInfo(fileName);
            if (!packageDescriptionFile.Exists)
                return new Result<PackageDescription>(false, new NotFoundMessage(fileName));


            using (var stream = packageDescriptionFile.CreateReadStream())
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string content = await reader.ReadToEndAsync();
                    PackageDescription packageDescription = JsonSerializer.Deserialize<PackageDescription>(content);
                    return new Result<PackageDescription>(true, packageDescription);
                }
            }
        }

        public Task<IResult> UpdatePackageAsync(string packageId, Stream content)
        {
            throw new NotImplementedException();
        }
    }
}
