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
using BaSyx.CLI.Options;
using CommandLine;
using NLog;
using System.Collections.Generic;

namespace BaSyx.CLI
{
    class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();


        static int Main(string[] args)
        {
            logger.Info("BaSyx Cli starting job...");

            return Parser.Default.ParseArguments<RefineOptions>(args)
                    .MapResult(
                      (RefineOptions opts) => RefineOptions.RunRefineAndReturnExitCode(opts), 
                      HandleParseError);
        }

        static int HandleParseError(IEnumerable<Error> errs) => -1;
    }
}
