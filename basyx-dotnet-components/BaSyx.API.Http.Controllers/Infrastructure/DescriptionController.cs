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
using Microsoft.AspNetCore.Mvc;
using BaSyx.Models.Connectivity;
using BaSyx.Utils.ResultHandling;
using System;

namespace BaSyx.API.Http.Controllers
{
    /// <summary>
    /// The Description Controller
    /// </summary>
    [ApiController]
    public class DescriptionController : Controller
    {
        private readonly IServiceDescriptor serviceDescriptor;

        /// <summary>
        /// The constructor for the Description API
        /// </summary>
        /// <param name="descriptor">The service descriptor.</param>
        public DescriptionController(IServiceDescriptor descriptor)
        {
            serviceDescriptor = descriptor;
        }

        /// <summary>
        /// Returns the self-describing information of a network resource (ServiceDescription)
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Requested Description</response>
        [HttpGet(DescriptionRoutes.DESCRIPTION, Name = "GetDescription")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ServiceDescription), 200)]
        [ProducesResponseType(typeof(Result), 401)]
        [ProducesResponseType(typeof(Result), 403)]
        public IActionResult GetDescription()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the Service Descriptor
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Requested Service Descriptor</response>
        [HttpGet(DescriptionRoutes.DESCRIPTOR, Name = "GetDescriptor")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IServiceDescriptor), 200)]
        [ProducesResponseType(typeof(Result), 401)]
        [ProducesResponseType(typeof(Result), 403)]
        public IActionResult GetDescriptor()
        {
            return new OkObjectResult(serviceDescriptor);
        }
    }
}
