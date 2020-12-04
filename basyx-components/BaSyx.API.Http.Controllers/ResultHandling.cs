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
using BaSyx.API.Components;
using BaSyx.Utils.ResultHandling;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Web;

namespace BaSyx.API.Http.Controllers
{
    /// <summary>
    /// Helper class for handling Action Results for HTTP-Requests
    /// </summary>
    public static class ResultHandling
    {
        /// <summary>
        /// Checks whether submodelId is null or Submodel Service Provider cannot be found
        /// </summary>
        /// <param name="spRegistry">The Submodel Service Provider Registry</param>
        /// <param name="submodelId">The Submodel's unique id</param>
        /// <param name="result">The IActionResult in case aasId is null or the provider cannot be found</param>
        /// <param name="provider">The Asset Administration Shell Service Provider</param>
        /// <returns></returns>
        public static bool IsNullOrNotFound(this ISubmodelServiceProviderRegistry spRegistry, string submodelId, out IActionResult result, out ISubmodelServiceProvider provider)
        {
            if (string.IsNullOrEmpty(submodelId))
            {
                result = NullResult(nameof(submodelId));
                provider = null;
                return true;
            }
            var retrieved = spRegistry.GetSubmodelServiceProvider(submodelId);
            if (!retrieved.Success || retrieved?.Entity == null)
            {
                result = new NotFoundObjectResult(new Result(false, new NotFoundMessage("Submodel Service Provider")));
                provider = null;
                return true;
            }
            result = null;
            provider = retrieved.Entity;
            return false;
        }

        /// <summary>
        /// Checks whether aasId is null or Asset Administration Shell Service Provider cannot be found
        /// </summary>
        /// <param name="serviceProvider">The Asset Administration Shell Repository Service Provider</param>
        /// <param name="aasId">The Asset Administration Shell's unique id</param>
        /// <param name="result">The IActionResult in case aasId is null or the provider cannot be found</param>
        /// <param name="provider">The Asset Administration Shell Service Provider</param>
        /// <returns></returns>
        public static bool IsNullOrNotFound(this IAssetAdministrationShellRepositoryServiceProvider serviceProvider, string aasId, out IActionResult result, out IAssetAdministrationShellServiceProvider provider)
        {
            if (string.IsNullOrEmpty(aasId))
            {
                result = NullResult(nameof(aasId));
                provider = null;
                return true;
            }
            aasId = HttpUtility.UrlDecode(aasId);
            var retrievedProvider = serviceProvider.GetAssetAdministrationShellServiceProvider(aasId);
            if (retrievedProvider.TryGetEntity(out provider))
            {
                result = null;
                return false;
            }
            else
            {
                provider = null;
                result = new NotFoundObjectResult(new Result(false, new NotFoundMessage("Asset Administration Shell Service Provider")));
                return true;
            }
        }

        /// <summary>
        /// Checks whether submodelId is null or Submodel Service Provider cannot be found
        /// </summary>
        /// <param name="serviceProvider">The Submodel Repository Service Provider</param>
        /// <param name="submodelId">The Submodel's unique id</param>
        /// <param name="result">The IActionResult in case submodelId is null or the provider cannot be found</param>
        /// <param name="provider">The Submodel Service Provider</param>
        /// <returns></returns>        
        public static bool IsNullOrNotFound(this ISubmodelRepositoryServiceProvider serviceProvider, string submodelId, out IActionResult result, out ISubmodelServiceProvider provider)
        {
            if (string.IsNullOrEmpty(submodelId))
            {
                result = NullResult(nameof(submodelId));
                provider = null;
                return true;
            }
            submodelId = HttpUtility.UrlDecode(submodelId);
            var retrievedProvider = serviceProvider.GetSubmodelServiceProvider(submodelId);
            if (retrievedProvider.TryGetEntity(out provider))
            {
                result = null;
                return false;
            }
            else
            {
                provider = null;
                result = new NotFoundObjectResult(new Result(false, new NotFoundMessage("Submodel Service Provider")));
                return true;
            }
        }

        /// <summary>
        /// Returns a Result-Object in an ObjectResult with status code 400 and a message which element is null or empty
        /// </summary>
        /// <param name="elementName">The name of the element which is null or empty</param>
        /// <returns></returns>
        public static IActionResult NullResult(string elementName)
        {
            BadRequestObjectResult objectResult = new BadRequestObjectResult(new Result(false, new Message(MessageType.Error, $"Argument {elementName} is null or empty")));
            return objectResult;
        }

        /// <summary>
        /// Returns a Result-Object in an BadRequest(400)-ObjectResult and a message why it is a BadRequest
        /// </summary>
        /// <param name="message">The message why it is a BadRequest</param>
        /// <returns></returns>
        public static IActionResult BadRequestResult(string message)
        {
            BadRequestObjectResult objectResult = new BadRequestObjectResult(new Result(false, new Message(MessageType.Error, message)));
            return objectResult;
        }

        /// <summary>
        /// Returns a Result-Object in an MethodNotAllowed(405)-ObjectResult
        /// </summary>
        /// <returns></returns>
        public static IActionResult MethodNotAllowedResult()
        {
            ObjectResult objectResult = new ObjectResult(new Result(false, new MethodNotAllowedMessage()))
            {
                StatusCode = 405
            };
            return objectResult;
        }

        /// <summary>
        /// Returns a Result-Object wrapped in an ObjectResult according to the CRUD-operation
        /// </summary>
        /// <param name="result">The orignary Result object</param>
        /// <param name="crud">The CRUD-operation taken</param>
        /// <param name="route">Optional route for Create-Operations</param>
        /// <returns></returns>
        public static IActionResult CreateActionResult(this IResult result, CrudOperation crud, string route = null)
        {
            if (result == null)
            {
                ObjectResult objectResult = new ObjectResult(new Result(false, new Message(MessageType.Error, "Result object is null")))
                {
                    StatusCode = 500
                };
                return objectResult;
            }

            switch (crud)
            {
                case CrudOperation.Create:
                    if (result.Success && result.Entity != null)
                        return new CreatedResult(route, result.Entity);
                    break;
                case CrudOperation.Retrieve:
                    if (result.Success && result.Entity != null)
                        return new OkObjectResult(result.Entity);
                    break;
                case CrudOperation.Update:
                    if (result.Success)
                        return new OkObjectResult(result.Entity);
                    break;
                case CrudOperation.Delete:
                    if (result.Success)
                        return new NoContentResult();
                    break;
                case CrudOperation.Invoke:
                    if (result.Entity != null)
                        return new OkObjectResult(result.Entity);
                    break;
                default:
                    return new BadRequestObjectResult(result);
            }

            if (!result.Success)
            {
                ObjectResult objectResult = new ObjectResult(result);
                if (result.IsException.HasValue && result.IsException.Value)
                    objectResult.StatusCode = 500;
                else
                {
                    IMessage message = result.Messages?.Find(m => m.Code != null);
                    if (message != null && Int32.TryParse(message.Code, out int statusCode))
                        objectResult.StatusCode = statusCode;
                }
                return objectResult;
            }

            return new BadRequestObjectResult(result);
        }
    }
    /// <summary>
    /// Enumeration of the different CRUD-Operations
    /// </summary>
    public enum CrudOperation
    {
        /// <summary>
        /// Create
        /// </summary>
        Create,
        /// <summary>
        /// Retrieve
        /// </summary>
        Retrieve,
        /// <summary>
        /// Update
        /// </summary>
        Update,
        /// <summary>
        /// Delete
        /// </summary>
        Delete,
        /// <summary>
        /// Invoke
        /// </summary>
        Invoke
    }
}
