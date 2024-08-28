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

using System;
using System.Text.Json.Serialization;

namespace BaSyx.Utils.ResultHandling
{
    public class ExceptionMessage : Message
    {
        public ExceptionMessage InnerException { get; }

        [JsonConstructor]
        public ExceptionMessage(Exception innerException) : this(innerException, string.Empty)
        { }

        public ExceptionMessage(Exception exception, string message) : base(MessageType.Exception, string.Format("{0} - {1}", message, exception.Message), exception.HResult.ToString())
        {
            if (exception.InnerException != null)
                InnerException = new ExceptionMessage(exception.InnerException);
        }
    }
}
