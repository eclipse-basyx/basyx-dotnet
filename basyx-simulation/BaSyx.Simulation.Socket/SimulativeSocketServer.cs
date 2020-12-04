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
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BaSyx.Simulation.SocketSimulation
{
    public class StateObject
    {
        public const int BufferSize = 1024;

        public byte[] buffer;
        
        public string Message { get; set; }

        public Socket Socket { get; }

        public StateObject(Socket socket)
        {
            Socket = socket;
            Message = string.Empty;
            buffer = new byte[BufferSize];
        }
    }

    public class SocketConfiguration
    {
        public IPEndPoint EndPoint { get; }
        public SocketType SocketType { get; set; }
        public ProtocolType ProtocolType { get; set; }

        public SocketConfiguration(IPEndPoint socketEndPoint)
        {
            EndPoint = socketEndPoint;
            SocketType = SocketType.Stream;
            ProtocolType = ProtocolType.Tcp;
        }
    }

    public class SimulativeSocketServer
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly ManualResetEvent connectedEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent messageReceivedEvent = new ManualResetEvent(false);
        private readonly string messageSeperator;
        private readonly Dictionary<Regex, Func<string>> requestActionDictionary;
        private bool running = false;

        public SocketConfiguration SocketConfig { get; }

        public SimulativeSocketServer(string messageSeperator, SocketConfiguration socketConfig)
        {
            this.messageSeperator = messageSeperator;
            SocketConfig = socketConfig;

            requestActionDictionary = new Dictionary<Regex, Func<string>>();
        }
        public void RegisterAnswer(string question, string answer) => RegisterAnswer(new Regex(question), answer);

        public void RegisterAnswer(Regex question, string answer)
        {
            if (!requestActionDictionary.ContainsKey(question))
                requestActionDictionary.Add(question, () => answer);
            else
                requestActionDictionary[question] = () => answer;
        }

        public void RegisterAction(Regex question, Func<string> questionAnswerFunc)
        {
            if (!requestActionDictionary.ContainsKey(question))
                requestActionDictionary.Add(question, questionAnswerFunc);
            else
                requestActionDictionary[question] = questionAnswerFunc;
        }

        public Task StartListeningAsync()
        {
            return Task.Run(() => StartListening());
        }

        public void StartListening()
        {
            IPEndPoint localEndPoint = SocketConfig.EndPoint;
            Socket listener = new Socket(localEndPoint.AddressFamily, SocketConfig.SocketType, SocketConfig.ProtocolType);
            running = true;

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (running)
                {
                    logger.Info("Waiting for connection...");
                    Socket handler = listener.Accept();

                    StateObject state = new StateObject(handler);
                    
                    while (running)
                    {
                        int bytesReceived = handler.Receive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None);
                        if(bytesReceived > 0)
                        {
                            state.Message += Encoding.UTF8.GetString(state.buffer, 0, bytesReceived);

                            if (state.Message.IndexOf(messageSeperator) > -1)
                            {
                                logger.Info("Message received: " + state.Message);

                                Func<string> action = GetAnswerFromQuestion(state.Message);
                                if(action != null)
                                {
                                    string answer = action.Invoke();
                                    logger.Info($"Sending answer '{answer}' for question '{state.Message}'...");
                                    Answer(handler, answer);
                                }
                                else
                                    logger.Warn("No answer found matching the request");

                                state = new StateObject(handler);
                            }                            
                        }
                    } 
                }
                listener.Shutdown(SocketShutdown.Both);
                listener.Close();
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
        }

        private Func<string> GetAnswerFromQuestion(string message)
        {
            foreach (var action in requestActionDictionary)
            {
                if (action.Key.IsMatch(message))
                    return action.Value;
            }
            return null;
        }

        public void StopListening()
        {
            running = false;
            connectedEvent.Set();
            messageReceivedEvent.Set();
        }

        private void Answer(Socket handler, string content)
        {            
            byte[] data = Encoding.UTF8.GetBytes(content);
            handler.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), handler);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;

                int bytesSent = handler.EndSend(ar);

                logger.Info("Bytes sent: " + bytesSent);
                
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
        }
    }
}
