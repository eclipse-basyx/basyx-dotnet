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

namespace BaSyx.API.Components
{
    public interface IAssetAdministrationShellRepository
    {
        IResult<IAssetAdministrationShell> CreateAssetAdministrationShell(IAssetAdministrationShell aas);

        IResult<IAssetAdministrationShell> RetrieveAssetAdministrationShell(string aasId);

        IResult<IElementContainer<IAssetAdministrationShell>> RetrieveAssetAdministrationShells();

        IResult UpdateAssetAdministrationShell(string aasId, IAssetAdministrationShell aas);

        IResult DeleteAssetAdministrationShell(string aasId);       
    }
}
