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
using System;
using System.Threading.Tasks;
using BaSyx.Utils.ResultHandling;

namespace BaSyx.Utils.Client
{
    public interface IMessageClient : IDisposable
    {
        bool IsConnected { get; }

        Task<IResult> PublishAsync(string topic, string message);
        Task<IResult> SubscribeAsync(string topic, Action<IMessageReceivedEventArgs> messageReceivedHandler);
        Task<IResult> UnsubscribeAsync(string topic);

        Task<IResult> StartAsync();
        Task<IResult> StopAsync();
    }

    public interface IMessageReceivedEventArgs
    {
        string Message { get; }
        string Topic { get; }
    }
}
