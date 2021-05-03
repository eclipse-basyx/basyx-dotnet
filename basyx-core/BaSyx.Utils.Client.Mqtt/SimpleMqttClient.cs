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
using System.Text;
using System.Threading;
using NLog;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using BaSyx.Utils.ResultHandling;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;

namespace BaSyx.Utils.Client.Mqtt
{
    public class SimpleMqttClient : IMessageClient
    {
        private MqttClient mqttClient;
        public readonly MqttConfig MqttConfig;

        private Dictionary<string, Action<IMessageReceivedEventArgs>> topicMessageReceivedHandler = new Dictionary<string, Action<IMessageReceivedEventArgs>>();
        private Action<IMessagePublishedEventArgs> msgPublishedMethod = null;

        private ManualResetEvent connectionClosedResetEvent;
        public EventHandler<EventArgs> ConnectionClosed;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public SimpleMqttClient(MqttConfig config)
        {
            MqttConfig = config;        
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
        public IResult Publish(string topic, string message) => Publish(topic, message, null, 2, false);

        public IResult Publish(string topic, string message, Action<IMessagePublishedEventArgs> messagePublishedHandler, byte qosLevel, bool retain = false)
        {
            if (messagePublishedHandler != null && this.msgPublishedMethod == null)
            {
                msgPublishedMethod = messagePublishedHandler;
                mqttClient.MqttMsgPublished += MqttClient_MqttMsgPublished;
            }

            byte[] bMessage = Encoding.UTF8.GetBytes(message);
            
            ushort messageId = mqttClient.Publish(topic, bMessage, qosLevel, retain);
            return new Result<ushort>(true, messageId);
        }

        public IResult Subscribe(string topic, Action<IMessageReceivedEventArgs> messageReceivedHandler, byte qosLevel)
        {
            if (string.IsNullOrEmpty(topic))
                return new Result(new ArgumentNullException(nameof(topic), "The topic is null or empty"));
            if (messageReceivedHandler == null)
                return new Result(new ArgumentNullException(nameof(messageReceivedHandler), "The message received delegate cannot be null since subscribed messages cannot be received"));

            if (!topicMessageReceivedHandler.ContainsKey(topic))
                topicMessageReceivedHandler.Add(topic, messageReceivedHandler);

            ushort messageId = mqttClient.Subscribe(new string[] { topic }, new byte[] { qosLevel });
            return new Result<ushort>(true, messageId);
        }

        public IResult Unsubscribe(string topic)
        {
            if (topicMessageReceivedHandler.ContainsKey(topic))
                topicMessageReceivedHandler.Remove(topic);

            ushort messageId = mqttClient.Unsubscribe(new string[] { topic });
            return new Result<ushort>(true, messageId);
        }

        private void MqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string parentOrSelfTopic = GetParentOrSelfTopic(e.Topic, topicMessageReceivedHandler.Keys);
            if (topicMessageReceivedHandler.TryGetValue(parentOrSelfTopic, out Action<IMessageReceivedEventArgs> action))
                action.Invoke(new MqttMsgReceivedEventArgs(e));
        }

        private string GetParentOrSelfTopic(string topic, Dictionary<string, Action<IMessageReceivedEventArgs>>.KeyCollection keys)
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

        private void MqttClient_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            logger.Debug("Subscribed for id = " + e.MessageId);
        }

        private void MqttClient_MqttMsgPublished(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishedEventArgs e)
        {
            logger.Debug("Published for id = " + e.MessageId);
            msgPublishedMethod?.Invoke(new MqttMsgPublishedEventArgs(e));
        }

        public IResult Start()
        {
            Uri endpoint = new Uri(MqttConfig.BrokerEndpoint);
            MqttSslProtocols protocols = MqttConfig.SecureConnection ? MqttSslProtocols.TLSv1_2 : MqttSslProtocols.None;
            X509Certificate caCert = null;
            X509Certificate clientCert = null;
            if (MqttConfig.Security != null && MqttConfig.Security is MqttSecurity security)
            {
                caCert = security.CaCert ?? null;
                clientCert = security.ClientCert ?? null;
            }
            mqttClient = new MqttClient(endpoint.Host, endpoint.Port, MqttConfig.SecureConnection, caCert, clientCert, protocols, null, null);
            mqttClient.ConnectionClosed += MqttClient_ConnectionClosed;
            
            try
            {
                byte success;
                MqttConnectConfig config = MqttConfig.MqttConnectConfig;
                if (MqttConfig.Credentials is MqttCredentials mqttCreds)
                    success = mqttClient.Connect(Guid.NewGuid().ToString(), mqttCreds.UserName, mqttCreds.Password, config.WillRetain, config.WillQosLevel, config.WillFlag, config.WillTopic, config.WillMessage, config.CleanSession, config.KeepAlivePeriod);
                else
                    success = mqttClient.Connect(Guid.NewGuid().ToString(), null, null, config.WillRetain, config.WillQosLevel, config.WillFlag, config.WillTopic, config.WillMessage, config.CleanSession, config.KeepAlivePeriod);

                if (success != 0)
                    return new Result(false, new Message(MessageType.Error, "Could not connect to MQTT Broker", success.ToString()));

                mqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
                return new Result(true);
            }
            catch (Exception e)
            {
                logger.Error(e, "Could not connect MQTT-Broker");
                return new Result(e);
            }
        }

        public IResult Stop()
        {
            if (mqttClient != null)
            {
                if (mqttClient.IsConnected)
                {
                    connectionClosedResetEvent = new ManualResetEvent(false);
                    //mqttClient.ConnectionClosed += MqttClient_ConnectionClosed;
                    mqttClient.MqttMsgPublishReceived -= MqttClient_MqttMsgPublishReceived;

                    mqttClient.Disconnect();

                    bool success = connectionClosedResetEvent.WaitOne(5000);

                    if (!success)
                    {
                        logger.Error("Could not close MQTT-Client");
                        return new Result(false, new Message(MessageType.Error, "Could not close MQTT-Client"));
                    }
                }
                mqttClient = null;
            }
            return new Result(true);
        }

        private void MqttClient_ConnectionClosed(object sender, EventArgs e)
        {
            connectionClosedResetEvent?.Set();
            ConnectionClosed?.Invoke(sender, e);
        }
    }

    public class MqttMsgPublishedEventArgs : IMessagePublishedEventArgs
    {
        public bool IsPublished { get; }
        public string MessageId { get; }
        public MqttMsgPublishedEventArgs(uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishedEventArgs e)
        {
            IsPublished = e.IsPublished;
            MessageId = e.MessageId.ToString();
        }
    }
    public class MqttMsgReceivedEventArgs : IMessageReceivedEventArgs
    {
        public string Message { get; }
        public string Topic { get; }
        public byte QosLevel { get; }
        public bool Retain { get; }
        public bool DupFlag { get; }
        public MqttMsgReceivedEventArgs(MqttMsgPublishEventArgs e)
        {
            Message = Encoding.UTF8.GetString(e.Message);
            Topic = e.Topic;
            QosLevel = e.QosLevel;
            Retain = e.Retain;
            DupFlag = e.DupFlag;
        }
    }

}
