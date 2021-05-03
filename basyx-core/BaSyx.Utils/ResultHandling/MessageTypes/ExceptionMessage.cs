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

using System;

namespace BaSyx.Utils.ResultHandling
{
    public class ExceptionMessage : Message
    {
        public ExceptionMessage InnerException { get; }

        public ExceptionMessage(Exception exception) : this(exception, string.Empty)
        { }

        public ExceptionMessage(Exception exception, string message) : base(MessageType.Exception, string.Format("{0} - {1}", message, exception.Message), exception.HResult.ToString())
        {
            if (exception.InnerException != null)
                InnerException = new ExceptionMessage(exception.InnerException);
        }
    }
}
