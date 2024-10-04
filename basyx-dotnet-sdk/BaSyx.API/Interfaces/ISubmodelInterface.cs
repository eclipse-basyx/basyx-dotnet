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
using BaSyx.Models.AdminShell;
using BaSyx.Utils.ResultHandling;
using BaSyx.Utils.ResultHandling.ResultTypes;

namespace BaSyx.API.Interfaces
{
    public interface ISubmodelInterface
    {
        IResult<ISubmodel> RetrieveSubmodel();

        IResult UpdateSubmodel(ISubmodel submodel);

        IResult UpdateSubmodelMetadata(ISubmodel submodel);

        IResult ReplaceSubmodel(ISubmodel submodel);

        IResult<ISubmodelElement> CreateSubmodelElement(string rootIdShortPath, ISubmodelElement submodelElement);

        IResult UpdateSubmodelElement(string rootIdShortPath, ISubmodelElement submodelElement);

        IResult<PagedResult<IElementContainer<ISubmodelElement>>> RetrieveSubmodelElements(int limit = 100,
            string cursor = "");

        IResult<ISubmodelElement> RetrieveSubmodelElement(string idShortPath);

        IResult<ValueScope> RetrieveSubmodelElementValue(string idShortPath);

        IResult<IReference> RetrieveSubmodelElementReference(string idShortPath);

        IResult<PagedResult<IReference>> RetrieveSubmodelElementsReference(int limit = 100, string cursor = "");

        IResult UpdateSubmodelElementValue(string idShortPath, ValueScope value);

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
