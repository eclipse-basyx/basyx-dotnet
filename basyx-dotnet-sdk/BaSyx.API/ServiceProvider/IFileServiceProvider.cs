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
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BaSyx.API.ServiceProvider
{
    public interface IFileServiceProvider
    {
        Task<IResult<IEnumerable<PackageDescription>>> GetAllPackageDescriptionsAsync();
        Task<IResult<PackageDescription>> GetPackageDescriptionAsync(string packageId);
        Task<IResult<PackageDescription>> CreatePackageAsync(PackageDescription packageDescription, Stream content);
        Task<IResult> UpdatePackageAsync(string packageId, Stream content);
        Task<IResult> DeletePackageDescriptionAsync(string packageId);
    }
}
