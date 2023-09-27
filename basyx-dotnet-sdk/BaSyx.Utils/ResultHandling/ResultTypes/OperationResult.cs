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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaSyx.Utils.ResultHandling
{
    public class OperationResult : Result
    {
        public OperationResult(bool success) : base(success)
        { }
        public OperationResult(Exception e) : base(e)
        { }
        public OperationResult(bool success, IMessage message) : base(success, message)
        { }
        public OperationResult(IResult result) : base(result)
        { }

        [JsonConstructor]
        public OperationResult(bool success, IEnumerable<IMessage> messages) : base(success, messages)
        { }

        public static implicit operator Task<OperationResult>(OperationResult operationResult)
        {
            return Task.FromResult(operationResult);
        }

        public static explicit operator OperationResult(Task<OperationResult> taskOperationResult)
        {
            return taskOperationResult.Result;
        }
    }
}
