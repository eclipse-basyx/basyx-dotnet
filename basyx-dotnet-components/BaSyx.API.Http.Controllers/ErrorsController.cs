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
using BaSyx.Utils.ResultHandling;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BaSyx.API.Http.Controllers
{
    /// <summary>
    /// The Error Controller
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController : ControllerBase
    {
        /// <summary>
        /// The main error function
        /// </summary>
        /// <returns></returns>
        [Route("error")]
        public IResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;

            if (exception != null)
            {
                Result result = new Result(exception);
                result.Messages.Add(new Message(MessageType.Exception, exception.StackTrace));

                Response.StatusCode = 400;

                return result;
            }
            return new Result(true, new Message(MessageType.Information, "No errors occured"));
        }
    }
}
