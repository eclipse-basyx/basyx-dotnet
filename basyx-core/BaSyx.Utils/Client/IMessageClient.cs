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
using BaSyx.Utils.ResultHandling;

namespace BaSyx.Utils.Client
{
    public interface IMessageClient
    {
        bool IsConnected { get; }

        IResult Publish(string topic, string message, Action<IMessagePublishedEventArgs> messagePublishedHandler, byte qosLevel, bool retain);
        IResult Subscribe(string topic, Action<IMessageReceivedEventArgs> messageReceivedHandler, byte qosLevel);
        IResult Unsubscribe(string topic);

        IResult Start();
        IResult Stop();
    }
    public interface IMessagePublishedEventArgs
    {
        bool IsPublished { get; }
        string MessageId { get; }        
    }
    public interface IMessageReceivedEventArgs
    {
        string Message { get; }
        string Topic { get; }
        byte QosLevel { get; }
    }

}
