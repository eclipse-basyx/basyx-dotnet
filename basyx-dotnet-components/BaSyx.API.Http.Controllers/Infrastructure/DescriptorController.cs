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
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using BaSyx.Utils.ResultHandling;
using BaSyx.Models.Connectivity;
using BaSyx.API.Interfaces;

namespace BaSyx.API.Http.Controllers
{
    /// <summary>
    /// The Descriptor Controller
    /// </summary>
    [ApiController]
    public class DescriptorController : Controller
    {
        private readonly IServiceDescriptor serviceDescriptor;

        /// <summary>
        /// The constructor for the Descriptor Controller
        /// </summary>
        /// <param name="descriptor">The service descriptor.</param>
        public DescriptorController(IServiceDescriptor descriptor)
        {
            serviceDescriptor = descriptor;
        }

        /// <summary>
        /// Returns the descriptor
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns the descriptor</response>
        [HttpGet(DescriptorRoutes.DESCRIPTOR, Name = "GetDescriptor")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IServiceDescriptor), 200)]
        public IActionResult GetDescriptor()
        {
            return new OkObjectResult(serviceDescriptor);
        }
    }
}
