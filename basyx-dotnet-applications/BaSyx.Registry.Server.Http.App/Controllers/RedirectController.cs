/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using BaSyx.API.Http;
using BaSyx.API.Http.Controllers;
using BaSyx.API.Interfaces;
using BaSyx.Models.Connectivity;
using BaSyx.Utils.Network;
using BaSyx.Utils.ResultHandling;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BaSyx.Registry.Server.Http.App.Controllers
{
    public class RedirectController : Controller
    {
        private readonly IAssetAdministrationShellRegistryInterface serviceProvider;

        /// <summary>
        /// The constructor for the Asset Administration Shell Redirect Controller
        /// </summary>
        /// <param name="aasRegistry">The backend implementation for the IAssetAdministrationShellRegistry interface. Usually provided by the Depedency Injection mechanism.</param>
        public RedirectController(IAssetAdministrationShellRegistryInterface aasRegistry)
        {
            serviceProvider = aasRegistry;
        }


        /// <summary>
        /// Redirects (302) to the first reachable endpoint of Asset Administration Shell registered
        /// </summary>
        /// <param name="aasIdentifier">The Asset Administration Shell's unique id</param>
        /// <param name="toWhat">The path at the found endpoint</param>
        /// <returns></returns>
        /// <response code="200">Returns the requested Asset Administration Shell</response>
        /// <response code="400">Bad Request</response> 
        /// <response code="404">No Asset Administration Shell with passed id found</response>     
        [HttpGet(AssetAdministrationShellRegistryRoutes.SHELL_DESCRIPTOR_ID + "/redirect/{toWhat}", Name = "RedirectToAssetAdministrationShell")]
        public async Task<IActionResult> RedirectToAssetAdministrationShell(string aasIdentifier, string toWhat)
        {
            if (string.IsNullOrEmpty(aasIdentifier))
                return ResultHandling.NullResult(nameof(aasIdentifier));

            aasIdentifier = ResultHandling.Base64UrlDecode(aasIdentifier);
            var result = serviceProvider.RetrieveAssetAdministrationShellRegistration(aasIdentifier);

            if(!result.Success)
                return result.CreateActionResult(CrudOperation.Retrieve);

            try
            {
                IAssetAdministrationShellDescriptor descriptor = result.Entity;
                foreach (var endpoint in descriptor.Endpoints.Where(s => s.ProtocolInformation.EndpointProtocol == Uri.UriSchemeHttp || s.ProtocolInformation.EndpointProtocol == Uri.UriSchemeHttps))
                {
                    bool pingable = await NetworkUtils.PingHostAsync(endpoint.ProtocolInformation.Uri.Host);
                    if (pingable)
                    {
                        return Redirect(endpoint.ProtocolInformation.EndpointAddress.Replace("/aas", "/" + toWhat));
                    }
                }

                result.Messages.Add(new Message(MessageType.Error, "Endpoints are not reachable"));
            }
            catch (Exception e)
            {
                var tempResult = new Result(e);
                result.Messages.AddRange(tempResult.Messages);
            }
            return new BadRequestObjectResult(result);
        }
    }
}
