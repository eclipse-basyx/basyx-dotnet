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
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaSyx.Utils.Server.Http
{
    public class SimpleLocalHttpServer
    {
        private HttpListener listener;
        private string _uriPrefix;

        private CancellationTokenSource cancellationToken;
        private Action<HttpListenerRequest> messageReception = null;
        private Action<HttpListenerResponse> messageResponse = null;
        private Action<HttpListenerContext> messageHandler = null;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public SimpleLocalHttpServer(string uriPrefix)
        {
            if (string.IsNullOrEmpty(uriPrefix))
                throw new ArgumentNullException(nameof(uriPrefix));

            if (!uriPrefix.EndsWith("/"))
                _uriPrefix = uriPrefix + "/";
            else
                _uriPrefix = uriPrefix;
        }


        public void Start()
        {
            listener = new HttpListener();
            listener.Prefixes.Add(_uriPrefix);
            cancellationToken = new CancellationTokenSource();

            if (!listener.IsListening)
            {
                listener.Start();

                Task.Factory.StartNew(async () =>
                {
                    while (!cancellationToken.IsCancellationRequested)
                        await Listen(listener);
                }, cancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);

                logger.Info("Http-Listener started");
            }
        }

        public void Start(Action<HttpListenerRequest> receiveMessageMethod)
        {
            messageReception = receiveMessageMethod;
            Start();
        }

        public void Start(Action<HttpListenerRequest> receiveMessageMethod, Action<HttpListenerResponse> responseMessageMethod)
        {
            messageReception = receiveMessageMethod;
            messageResponse = responseMessageMethod;
            Start();
        }

        public void Start(Action<HttpListenerContext> messageHandlerMethod)
        {
            messageHandler = messageHandlerMethod;
            Start();
        }
        private async Task Listen(HttpListener listener)
        {
            try
            {
                HttpListenerContext context = await listener.GetContextAsync();
                if (messageHandler != null)
                    messageHandler.Invoke(context);
                else
                {
                    if (messageReception != null)
                        messageReception.Invoke(context.Request);
                    if (messageResponse != null)
                        messageResponse.Invoke(context.Response);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Http-Listener Exception: " + e.Message);
            }
        }

        public void Stop()
        {
            if (listener.IsListening)
            {
                cancellationToken.Cancel();
                listener.Stop();
                logger.Info("Http-Listener stopped");
            }
        }        

        /// <summary>
        /// Formats the entire response, e.g. suitable for a MessageBox
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Beautiful formatted response</returns>
        public static string GetCompleteResponseAsString(HttpListenerRequest request)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("URI: " + request.Url.AbsoluteUri);
            sb.AppendLine("-----------HEADER-----------");

            NameValueCollection headers = request.Headers;
            for (int i = 0; i < headers.Count; i++)
            {
                string key = headers.GetKey(i);
                string value = headers.Get(i);
                sb.AppendLine(key + " = " + value);
            }

            sb.AppendLine("---------HEADER-END--------");

            sb.AppendLine("-----------BODY-----------");
            string body = new StreamReader(request.InputStream).ReadToEnd();
            sb.AppendLine(body);
            sb.AppendLine("---------BODY-END--------");

            return sb.ToString();
        }

        public static string GetResponseBodyAsString(HttpListenerRequest request)
        {
            string responseBody = null;
            using (var stream = new StreamReader(request.InputStream))
                responseBody = stream.ReadToEnd();
            return responseBody;
        }
    }
}
