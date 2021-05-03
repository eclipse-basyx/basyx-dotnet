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
