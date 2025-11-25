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
using BaSyx.API.Interfaces;
using System.Threading.Tasks;
using BaSyx.Utils.ResultHandling.ResultTypes;
using System.Collections.Generic;

namespace BaSyx.API.Clients
{
    public interface ISubmodelClient : ISubmodelInterface, IClient
    {
        Task<IResult<ISubmodel>> RetrieveSubmodelAsync(RequestLevel level = default, RequestExtent extent = default);
        Task<IResult> UpdateSubmodelAsync(ISubmodel submodel);
        Task<IResult<ISubmodelElement>> CreateSubmodelElementAsync(string rootIdShortPath, ISubmodelElement submodelElement);
        Task<IResult> UpdateSubmodelElementAsync(string rootIdShortPath, ISubmodelElement submodelElement);
        Task<IResult<PagedResult<IElementContainer<ISubmodelElement>>>> RetrieveSubmodelElementsAsync(int limit = 100, string cursor = "", RequestLevel level = RequestLevel.Deep, RequestExtent extent = RequestExtent.WithoutBlobValue);
        IResult<PagedResult<List<string>>> RetrieveSubmodelElementsPath(int limit = 100, string cursor = "", RequestLevel level = RequestLevel.Deep);
        Task<IResult<PagedResult<List<string>>>> RetrieveSubmodelElementsPathAsync(int limit = 100, string cursor = "", RequestLevel level = RequestLevel.Deep);
        Task<IResult<ISubmodelElement>> RetrieveSubmodelElementAsync(string idShortPath);
        Task<IResult<ValueScope>> RetrieveSubmodelElementValueAsync(string idShortPath);
        IResult<List<string>> RetrieveSubmodelElementPath(string idShortPath, RequestLevel level = RequestLevel.Deep);
        Task<IResult<List<string>>> RetrieveSubmodelElementPathAsync(string idShortPath, RequestLevel level = RequestLevel.Deep);
        Task<IResult> UpdateSubmodelElementValueAsync(string idShortPath, ValueScope value);
        Task<IResult> DeleteSubmodelElementAsync(string idShortPath);
        Task<IResult<InvocationResponse>> InvokeOperationAsync(string idShortPath, InvocationRequest invocationRequest, bool async = false);
        Task<IResult<InvocationResponse>> GetInvocationResultAsync(string idShortPath, string requestId);
    }

    public static class SubmodelClientExtensions
    {
        public static IResult<ISubmodel> RetrieveSubmodel(this ISubmodelClient submodelClient)
        {
            return submodelClient.RetrieveSubmodel();
        }
    }
}
