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
using System.Net;

namespace BaSyx.Utils.ResultHandling
{
    public class HttpMessage : Message
    {
        public HttpStatusCode HttpStatusCode { get; set; }

        public HttpMessage(MessageType messageType, HttpStatusCode httpStatusCode) : base(messageType, httpStatusCode.ToString(), ((int)httpStatusCode).ToString())
        {
            HttpStatusCode = httpStatusCode;
        }
    }
}
