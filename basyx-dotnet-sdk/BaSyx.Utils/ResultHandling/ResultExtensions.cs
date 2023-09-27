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
using System;

namespace BaSyx.Utils.ResultHandling
{
    public static class ResultExtensions
    {
        public static bool TryGetEntity<T>(this IResult<T> result, out T entity)
        {
            if(result.Entity != null)
            {
                try
                {
                    entity = result.GetEntity<T>();
                    return true;
                }
                catch (Exception)
                {
                    entity = default;
                    return false;
                }
            }
            else
            {
                entity = default;
                return false;
            }
        }
    }
}
