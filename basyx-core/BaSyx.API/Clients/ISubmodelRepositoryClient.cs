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
using BaSyx.Models.Core.Common;
using BaSyx.Utils.ResultHandling;

namespace BaSyx.API.Clients
{
    public interface ISubmodelRepositoryClient
    {
        IResult<ISubmodel> CreateOrUpdateSubmodel(ISubmodel submodel);

        IResult<IElementContainer<ISubmodel>> RetrieveSubmodels();

        IResult<ISubmodel> RetrieveSubmodel(string submodelId);

        IResult DeleteSubmodel(string submodelId);
    }
}
