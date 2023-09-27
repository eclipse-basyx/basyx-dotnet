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
using BaSyx.API.Interfaces;
using System.Threading.Tasks;

namespace BaSyx.API.Clients
{
    public interface ISubmodelClient : ISubmodelInterface, IClient
    {
        Task<IResult<ISubmodel>> RetrieveSubmodelAsync(RequestLevel level = default, RequestContent content = default, RequestExtent extent = default);
        Task<IResult> UpdateSubmodelAsync(ISubmodel submodel);
        Task<IResult<ISubmodelElement>> CreateSubmodelElementAsync(string rootIdShortPath, ISubmodelElement submodelElement);
        Task<IResult<ISubmodelElement>> UpdateSubmodelElementAsync(string rootIdShortPath, ISubmodelElement submodelElement);
        Task<IResult<IElementContainer<ISubmodelElement>>> RetrieveSubmodelElementsAsync();
        Task<IResult<ISubmodelElement>> RetrieveSubmodelElementAsync(string idShortPath);
        Task<IResult<IValue>> RetrieveSubmodelElementValueAsync(string idShortPath);
        Task<IResult> UpdateSubmodelElementValueAsync(string idShortPath, IValue value);
        Task<IResult> DeleteSubmodelElementAsync(string idShortPath);
        Task<IResult<InvocationResponse>> InvokeOperationAsync(string idShortPath, InvocationRequest invocationRequest, bool async = false);
        Task<IResult<InvocationResponse>> GetInvocationResultAsync(string idShortPath, string requestId);
    }

    public static class SubmodelClientExtensions
    {
        public static IResult<ISubmodel> RetrieveSubmodel(this ISubmodelClient submodelClient)
        {
            return submodelClient.RetrieveSubmodel(RequestLevel.Deep, RequestContent.Normal, RequestExtent.WithoutBlobValue);
        }
    }
}
