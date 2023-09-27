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
using BaSyx.Models.AdminShell;
using BaSyx.Utils.ResultHandling;

namespace BaSyx.API.Interfaces
{
    public interface ISubmodelInterface
    {
        IResult<ISubmodel> RetrieveSubmodel(RequestLevel level = default, RequestContent content = default, RequestExtent extent = default);

        IResult UpdateSubmodel(ISubmodel submodel);

        IResult<ISubmodelElement> CreateSubmodelElement(string rootIdShortPath, ISubmodelElement submodelElement);

        IResult<ISubmodelElement> UpdateSubmodelElement(string rootIdShortPath, ISubmodelElement submodelElement);

        IResult<IElementContainer<ISubmodelElement>> RetrieveSubmodelElements();

        IResult<ISubmodelElement> RetrieveSubmodelElement(string idShortPath);

        IResult<IValue> RetrieveSubmodelElementValue(string idShortPath);

        IResult UpdateSubmodelElementValue(string idShortPath, IValue value);

        IResult DeleteSubmodelElement(string idShortPath);

        /// <summary>
        /// Invokes a specific Operation synchronously or asynchronously
        /// </summary>
        /// <param name="idShortPath">IdShort-Path to the Operation</param>
        /// <param name="invocationRequest">Request-Parameters for the invocation</param>
        /// <param name="async">Server-side operation execution (true = asynchronous | false (default) = synchronous)</param>
        /// <returns></returns>
        IResult<InvocationResponse> InvokeOperation(string idShortPath, InvocationRequest invocationRequest, bool async = false);

        /// <summary>
        /// Returns the Invocation Result of specific Operation
        /// </summary>
        /// <param name="idShortPath">IdShort-Path to the Operation</param>
        /// <param name="requestId">Request-Id</param>
        /// <returns></returns>
        IResult<InvocationResponse> GetInvocationResult(string idShortPath, string requestId);
    }
}
