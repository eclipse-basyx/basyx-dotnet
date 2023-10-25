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
using Microsoft.Extensions.Logging;

namespace BaSyx.Deployment.AppDataService
{
    [ApiController]
    public class AppDataController : Controller
    {
        private readonly ILogger<AppDataController> _logger;
        private readonly AppDataService _appDataService;
        public AppDataController(ILogger<AppDataController> logger, AppDataService appDataService)
        {
            _logger = logger;
            _appDataService = appDataService;
        }

        [HttpPost("appdata/api/v1/load", Name = "LoadAppData")]
        public IActionResult LoadAppData([FromBody] AppDataHttpRequest appDataRequest)
        {
            if (appDataRequest == null)
                return new BadRequestResult();

            switch (appDataRequest.Phase)
            {
                case "query":
                case "prepare":
                case "validate":
                case "activate":
                case "abort":
                    return NoContent();
                case "load":
                    _logger.LogInformation("Trying to load...");
                    var result = _appDataService.Load();
                    if (result.Success)
                    {
                        _logger.LogInformation($"Loading AppData successful");
                        return Accepted();
                    }
                    else
                        return new StatusCodeResult(500);
                default:
                    return NoContent();
            }
        }

        [HttpPost("appdata/api/v1/save", Name = "SaveAppData")]
        public IActionResult SaveAppData([FromBody] AppDataHttpRequest appDataRequest)
        {
            if (appDataRequest == null)
                return new BadRequestResult();

            switch (appDataRequest.Phase)
            {
                case "save":
                    var result = _appDataService.Save();
                    if (result.Success)
                    {
                        _logger.LogInformation($"Saving AppData successful");
                        return Accepted();
                    }
                    else
                        return new StatusCodeResult(500);
                default:
                    return NoContent();
            }
        }
    }
}