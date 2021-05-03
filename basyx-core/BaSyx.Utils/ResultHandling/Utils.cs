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
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace BaSyx.Utils.ResultHandling
{
    public static class Utils
    {
        public static async Task<bool> RetryUntilSuccessOrTimeout(Func<bool> task, TimeSpan timeout, TimeSpan pause)
        {
            if (pause.TotalMilliseconds < 0)
            {
                throw new ArgumentException("Pause must be >= 0 milliseconds");
            }
            if (timeout.TotalMilliseconds < 0)
            {
                throw new ArgumentException("Timeout must be >= 0 milliseconds");
            }

            var stopwatch = Stopwatch.StartNew();
            do
            {
                if (task())
                    return true; 

                await Task.Delay((int)pause.TotalMilliseconds);
            }
            while (stopwatch.Elapsed < timeout);
            return false;
        }

        public static bool TryParseStatusCode(IResult result, out int iHttpStatusCode)
        {
            try
            {
                bool success = false;
                var msgs = result.Messages.FindAll(m => !string.IsNullOrEmpty(m.Code));
                if (msgs != null && msgs.Count > 0)
                    foreach (var msg in msgs)
                    {
                        success = Enum.TryParse(msg.Code, out HttpStatusCode httpStatusCode);
                        if (success)
                        {
                            iHttpStatusCode = (int)httpStatusCode;
                            return success;
                        }
                    }
                iHttpStatusCode = (int)HttpStatusCode.BadRequest;
                return success;
            }
            catch
            {
                iHttpStatusCode = (int)HttpStatusCode.BadRequest;
                return false;
            }
        }
    }
}
