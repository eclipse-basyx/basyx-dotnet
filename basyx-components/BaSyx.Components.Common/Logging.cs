/*******************************************************************************
* Copyright (c) 2020 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using NLog;

namespace BaSyx.Components.Common
{
    public static class Logging
    {
        public static Microsoft.Extensions.Logging.LogLevel GetLogLevel(this ILogger logger)
        {
            if (logger.IsDebugEnabled)
                return Microsoft.Extensions.Logging.LogLevel.Debug;
            else if (logger.IsErrorEnabled)
                return Microsoft.Extensions.Logging.LogLevel.Error;
            else if (logger.IsFatalEnabled)
                return Microsoft.Extensions.Logging.LogLevel.Critical;
            else if (logger.IsInfoEnabled)
                return Microsoft.Extensions.Logging.LogLevel.Information;
            else if (logger.IsTraceEnabled)
                return Microsoft.Extensions.Logging.LogLevel.Trace;
            else if (logger.IsWarnEnabled)
                return Microsoft.Extensions.Logging.LogLevel.Warning;
            else
                return Microsoft.Extensions.Logging.LogLevel.None;
        }
    }
}
