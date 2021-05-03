/*******************************************************************************
* Copyright (c) 2020, 2021 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Utils.ResultHandling;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Communication;

namespace BaSyx.API.Clients
{
    public interface ISubmodelClient
    {
        IResult<ISubmodel> RetrieveSubmodel();

        IResult<ISubmodelElement> CreateOrUpdateSubmodelElement(string rootSeIdShortPath, ISubmodelElement submodelElement);

        IResult<IElementContainer<ISubmodelElement>> RetrieveSubmodelElements();

        IResult<ISubmodelElement> RetrieveSubmodelElement(string seIdShortPath);

        IResult<IValue> RetrieveSubmodelElementValue(string seIdShortPath);

        IResult UpdateSubmodelElementValue(string seIdShortPath, IValue value);

        IResult DeleteSubmodelElement(string seIdShortPath);

        /// <summary>
        /// Invokes a specific Operation synchronously
        /// </summary>
        /// <param name="operationIdShortPath">IdShort-Path to the Operation</param>
        /// <param name="invocationRequest">Request-Parameters for the invocation</param>
        /// <returns></returns>
        IResult<InvocationResponse> InvokeOperation(string operationIdShortPath, InvocationRequest invocationRequest);

        /// <summary>
        /// Invokes a specific Operation asynchronously
        /// </summary>
        /// <param name="operationIdShortPath">IdShort-Path to the Operation</param>
        /// <param name="invocationRequest">Request-Parameters for the invocation</param>
        /// <returns></returns>
        IResult<CallbackResponse> InvokeOperationAsync(string operationIdShortPath, InvocationRequest invocationRequest);

        /// <summary>
        /// Returns the Invocation Result of specific Operation
        /// </summary>
        /// <param name="operationIdShortPath">IdShort-Path to the Operation</param>
        /// <param name="requestId">Request-Id</param>
        /// <returns></returns>
        IResult<InvocationResponse> GetInvocationResult(string operationIdShortPath, string requestId);
    }
}
