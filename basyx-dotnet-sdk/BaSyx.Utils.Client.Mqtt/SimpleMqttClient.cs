/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using System;
using System.Text;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaSyx.Utils.ResultHandling;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Microsoft.Extensions.Logging;
using System.Security.Authentication;
using System.Collections.Concurrent;
using System.Linq;

namespace BaSyx.Utils.Client.Mqtt
{
    public class SimpleMqttClient : IMessageClient
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<SimpleMqttClient>();

        private IMqttClient mqttClient;
        private MqttClientOptions mqttOptions;
        private readonly ConcurrentDictionary<string, Action<IMessageReceivedEventArgs>> msgReceivedHandler;
        private bool disposedValue;
        private bool manualStop = false;
        private byte DEFAULT_QOS_LEVEL = 2;

        public event EventHandler<ConnectionEstablishedEventArgs> ConnectionEstablished;
        public event EventHandler<ConnectionClosedEventArgs> ConnectionClosed;

        public MqttClientConfiguration MqttConfig { get; private set; }

        public SimpleMqttClient(MqttClientConfiguration config)
        {
            MqttConfig = config;
            msgReceivedHandler = new ConcurrentDictionary<string, Action<IMessageReceivedEventArgs>>();
        }

        private void LoadConfiguration(MqttClientConfiguration config)
        {
            var builder = new MqttClientOptionsBuilder();
            if (!string.IsNullOrEmpty(config.ClientId))
                builder.WithClientId(config.ClientId);
            if (!string.IsNullOrEmpty(config.BrokerEndpoint))
            {
                Uri endpoint = new Uri(config.BrokerEndpoint);
                builder.WithTcpServer(endpoint.Host, endpoint.Port);
            }
            else
                throw new ArgumentNullException("BrokerEndpoint");
            if (config.Credentials != null)
                builder.WithCredentials(config.Credentials.Username, config.Credentials.Password);
            if (config.Security != null)
            {                
                var options = new MqttClientTlsOptions();
                if (config.Security.UseTls)
                    options.UseTls = true;
                if (!string.IsNullOrEmpty(config.Security.SslProtocols))
                    options.SslProtocol = (SslProtocols)Enum.Parse(typeof(SslProtocols), config.Security.SslProtocols);
                if (config.Security.AllowUntrustedCertificates)
                    options.AllowUntrustedCertificates = true;
                if (config.Security.IgnoreCertificateChainErrors)
                    options.IgnoreCertificateChainErrors = true;
                if (config.Security.IgnoreCertificateRevocationErrors)
                    options.IgnoreCertificateRevocationErrors = true;

                if (config.Security.CaCert != null && config.Security.ClientCert != null)
                {
                    options.ClientCertificatesProvider = new MqttClientCertificateProvider(new List<X509Certificate>()
                    {
                        config.Security.CaCert, config.Security.ClientCert
                    });                    
                }
                builder.WithTlsOptions(options);
            }

            builder.WithCleanSession(config.CleanSession);
            builder.WithKeepAlivePeriod(TimeSpan.FromSeconds(config.KeepAlivePeriod));
            if (config.WillFlag)
            {
                builder
                    .WithWillTopic(config.WillTopic)
                    .WithWillPayload(config.WillMessage)
                    .WithWillQualityOfServiceLevel((MqttQualityOfServiceLevel)config.WillQosLevel)
                    .WithWillRetain(config.WillRetain);
            }

            mqttOptions = builder.Build();
        }

        public bool IsConnected
        {
            get
            {
                if (mqttClient != null)
                    return mqttClient.IsConnected;
                else
                    return false;
            }
        }

        public bool IsSubscribed(string topic)
        {
            if (msgReceivedHandler.ContainsKey(topic))
                return true;
            else
                return false;
        }

        public void SetDefaultQualityOfServiceLevel(byte qos)
        {
            DEFAULT_QOS_LEVEL = qos;
        }

        public async Task<IResult> PublishAsync(string topic, string message)
            => await PublishAsync(topic, message, DEFAULT_QOS_LEVEL, false).ConfigureAwait(false);

        public async Task<IResult> PublishAsync(string topic, string message, byte qosLevel, bool retain)
        {
            var msg = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(Encoding.UTF8.GetBytes(message))
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qosLevel)
                .WithRetainFlag(retain)
                .Build();

            var result = await mqttClient.PublishAsync(msg, CancellationToken.None).ConfigureAwait(false);

            if (result.ReasonCode == MqttClientPublishReasonCode.Success)
                return new Result(true);
            else
                return new Result(false, new Message(MessageType.Error, result.ReasonString, Enum.GetName(typeof(MqttClientPublishReasonCode), result.ReasonCode)));
        }

        public async Task<IResult> SubscribeAsync(string topic, Action<IMessageReceivedEventArgs> messageReceivedHandler)
            => await SubscribeAsync(topic, messageReceivedHandler, DEFAULT_QOS_LEVEL).ConfigureAwait(false);

        public async Task<IResult> SubscribeAsync(string topic, Action<IMessageReceivedEventArgs> messageReceivedHandler, byte qosLevel)
        {
            if (string.IsNullOrEmpty(topic))
                return new Result(new ArgumentNullException(nameof(topic), "The topic is null or empty"));
            if (messageReceivedHandler == null)
                return new Result(new ArgumentNullException(nameof(messageReceivedHandler), "The message received delegate cannot be null since subscribed messages cannot be received"));

            MqttQualityOfServiceLevel level = (MqttQualityOfServiceLevel)qosLevel;

            var options = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(topic, level)
                .Build();

            bool added = msgReceivedHandler.TryAdd(topic, messageReceivedHandler);
            if (added)
            {
                var result = await mqttClient.SubscribeAsync(options, CancellationToken.None).ConfigureAwait(false);

                foreach (var item in result.Items)
                {
                    if ((int)item.ResultCode > DEFAULT_QOS_LEVEL)
                        return new Result(false, new Message(MessageType.Error, "Unable to subscribe topic " + item.TopicFilter.Topic, Enum.GetName(typeof(MqttSubscribeReasonCode), item.ResultCode)));
                }
            }
            else
				logger.LogInformation($"Already subscribed to topic {topic}");

			return new Result(true);
        }

        public async Task<IResult> UnsubscribeAsync(string topic)
        {
            msgReceivedHandler.TryRemove(topic, out _);

            var result = await mqttClient.UnsubscribeAsync(topic).ConfigureAwait(false);

            foreach (var item in result.Items)
            {
                if (item.ResultCode != MqttClientUnsubscribeResultCode.Success)
                    return new Result(false, new Message(MessageType.Error, "Unable to unsubscribe topic " + item.TopicFilter, Enum.GetName(typeof(MqttUnsubscribeReasonCode), item.ResultCode)));
            }
            return new Result(true);
        }

        public async Task<IResult> StartAsync()
        {
            try
            {
                LoadConfiguration(MqttConfig);
                manualStop = false;

                mqttClient = new MqttFactory().CreateMqttClient();
                mqttClient.ApplicationMessageReceivedAsync += MessageReceivedHandler;
                mqttClient.ConnectedAsync += ConnectedHandler;
                mqttClient.DisconnectedAsync += DisconnectedHandler;
                var result = await mqttClient.ConnectAsync(mqttOptions, CancellationToken.None).ConfigureAwait(false);
                if (result.ResultCode == MqttClientConnectResultCode.Success)
                    return new Result(true);
                else
                    return new Result(false, new Message(MessageType.Error, "Unable to connect", Enum.GetName(typeof(MqttClientConnectResultCode), result.ResultCode)));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Could not connect MQTT-Broker");
                return new Result(e);
            }
        }

        public async Task<IResult> StopAsync()
        {
            if (mqttClient != null)
            {
                if (mqttClient.IsConnected)
                {
                    manualStop = true;
                    await mqttClient.DisconnectAsync().ConfigureAwait(false);
                }
                mqttClient.Dispose();
            }
            return new Result(true);
        }

        private string GetParentOrSelfTopic(string topic, IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                if (key == topic)
                    return key;
                else
                {
                    string[] splittedKey = key.Split('/');
                    string[] splittedTopic = topic.Split('/');
                    int minLength = Math.Min(splittedKey.Length, splittedTopic.Length);
                    for (int i = 0; i < minLength; i++)
                    {
                        if (splittedKey[i] != splittedTopic[i])
                        {
                            if (splittedKey[i] == "#")
                                return key;
                        }
                    }
                }
            }
            return topic;
        }

        private Task MessageReceivedHandler(MqttApplicationMessageReceivedEventArgs e)
        {
            string parentOrSelfTopic = GetParentOrSelfTopic(e.ApplicationMessage.Topic, msgReceivedHandler.Keys);
            if (msgReceivedHandler.TryGetValue(parentOrSelfTopic, out Action<IMessageReceivedEventArgs> action))
                action.Invoke(new MqttMsgReceivedEventArgs(e));

            return Task.CompletedTask;
        }

        private Task ConnectedHandler(MqttClientConnectedEventArgs e)
        {
            string result = Enum.GetName(typeof(MqttClientConnectResultCode), e.ConnectResult.ResultCode);
            logger.LogInformation($"Connected. Result: {result}");
            ConnectionEstablished?.Invoke(this, new ConnectionEstablishedEventArgs(result));
            return Task.CompletedTask;
        }

        private async Task DisconnectedHandler(MqttClientDisconnectedEventArgs e)
        {
            string reason = Enum.GetName(typeof(MqttClientDisconnectReason), e.Reason);
            logger.LogWarning($"Disconnected. Reason: {reason}");
            ConnectionClosed?.Invoke(this, new ConnectionClosedEventArgs(reason) { Exception = e.Exception });

            if (!manualStop && MqttConfig.Reconnect)
            {
                await Task.Delay(MqttConfig.ReconnectDelay).ConfigureAwait(false);

                try
                {
                    var result = await mqttClient.ConnectAsync(mqttOptions, CancellationToken.None).ConfigureAwait(false);
                    if (result.ResultCode == MqttClientConnectResultCode.Success)
                    {
                        var tempHandler = CloneDictionary(msgReceivedHandler);
						msgReceivedHandler.Clear();
                        foreach (var handler in tempHandler)
                        {
                            _ = SubscribeAsync(handler.Key, handler.Value).ConfigureAwait(false);
                        }
                        logger.LogInformation($"Successfully reconnected");
                        manualStop = false;
                    }
                    else
                        logger.LogError("Unable to reconnect | ResultCode: {0}", Enum.GetName(typeof(MqttClientConnectResultCode), result.ResultCode));
                }
                catch (Exception exc)
                {
                    logger.LogError(exc, $"Reconnect failed. Trying again in {MqttConfig.ReconnectDelay}ms ...");
                }
            }
        }

		private static Dictionary<string, Action<IMessageReceivedEventArgs>> CloneDictionary(ConcurrentDictionary<string, Action<IMessageReceivedEventArgs>> source)
		{
			var target = new Dictionary<string, Action<IMessageReceivedEventArgs>>();
			foreach (var kvp in source)
			{
				target.Add(kvp.Key, kvp.Value);
			}
			return target;
		}

		protected virtual async void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    await StopAsync().ConfigureAwait(false);
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public class ConnectionEstablishedEventArgs : EventArgs
    {
        public string Result { get; }

        public ConnectionEstablishedEventArgs(string result)
        {
            Result = result;
        }
    }

    public class ConnectionClosedEventArgs : EventArgs
    {
        public string Reason { get; }
        public Exception Exception { get; set; }

        public ConnectionClosedEventArgs(string reason)
        {
            Reason = reason;
        }
    }

    public class MqttMsgReceivedEventArgs : IMessageReceivedEventArgs
    {
        public string Message { get; }
        public string Topic { get; }
        public byte QoSLevel { get; }
        public bool Retain { get; }
        public bool DupFlag { get; }
        public MqttMsgReceivedEventArgs(MqttApplicationMessageReceivedEventArgs e)
        {
            Message = e.ApplicationMessage.ConvertPayloadToString();
            Topic = e.ApplicationMessage.Topic;
            QoSLevel = (byte)e.ApplicationMessage.QualityOfServiceLevel;
            Retain = e.ApplicationMessage.Retain;
            DupFlag = e.ApplicationMessage.Dup;
        }
    }
}
