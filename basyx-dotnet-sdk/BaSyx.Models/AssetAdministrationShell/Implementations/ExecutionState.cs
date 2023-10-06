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
namespace BaSyx.Models.AdminShell
{
    /// <summary>
    /// Defines the execution state of an invoked operation
    /// </summary>
    public enum ExecutionState
    {
        /// <summary>
        /// Initial state of execution
        /// </summary>
        Initiated,
        /// <summary>
        /// The operation is running
        /// </summary>
        Running,
        /// <summary>
        /// The operation execution has been completed
        /// </summary>
        Completed,
        /// <summary>
        /// The operation execution has been canceled
        /// </summary>
        Canceled,
        /// <summary>
        /// The operation execution has been failed
        /// </summary>
        Failed,
        /// <summary>
        /// The operation execution has timed out
        /// </summary>
        Timeout

    }
}