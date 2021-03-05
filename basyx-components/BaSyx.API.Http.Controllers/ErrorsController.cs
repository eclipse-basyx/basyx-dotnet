/*******************************************************************************
* Copyright (c) 2020 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
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
