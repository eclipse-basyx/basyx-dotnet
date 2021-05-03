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
namespace BaSyx.Models.Communication
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