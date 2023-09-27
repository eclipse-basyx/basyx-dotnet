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
using BaSyx.Utils.Json;
using BaSyx.Utils.ResultHandling;
using BaSyx.Utils.Settings;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaSyx.Utils.Client.Http
{
    public class SimpleHttpClient : IDisposable
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<SimpleHttpClient>();
        public HttpClient HttpClient { get; }
        public HttpMessageHandler HttpMessageHandler { get; }
        public JsonSerializerSettings JsonSerializerSettings { get; set; }
        public static SimpleHttpClientTimeoutHandler DEFAULT_HTTP_CLIENT_HANDLER
        {
            get
            {
                return new SimpleHttpClientTimeoutHandler()
                {
                    InnerHandler = new HttpClientHandler()
                    {
                        MaxConnectionsPerServer = 100,
                        AllowAutoRedirect = true,
                        UseProxy = false,
                        ServerCertificateCustomValidationCallback = Validate
                    }
                };
            }
        }

        private static bool Validate(HttpRequestMessage message, X509Certificate2 cert, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }

        public SimpleHttpClient() : this(DEFAULT_HTTP_CLIENT_HANDLER)
        { }

        public SimpleHttpClient(HttpMessageHandler messageHandler)
        {
            if (messageHandler == null)
                messageHandler = DEFAULT_HTTP_CLIENT_HANDLER;

            HttpMessageHandler = messageHandler;
            HttpClient = new HttpClient(HttpMessageHandler, true);

            if (HttpMessageHandler is SimpleHttpClientTimeoutHandler)
                HttpClient.Timeout = Timeout.InfiniteTimeSpan;

            JsonSerializerSettings = new DefaultJsonSerializerSettings();
        }

        public virtual void LoadProxy(ProxyConfiguration proxyConfiguration)
        {
            if (proxyConfiguration == null)
                return;

            HttpClientHandler clientHandler;
            if (HttpMessageHandler is SimpleHttpClientTimeoutHandler timeoutHandler)
                clientHandler = timeoutHandler.InnerHandler as HttpClientHandler;
            else if (HttpMessageHandler is HttpClientHandler defautHandler)
                clientHandler = defautHandler;
            else
                clientHandler = null;

            if (proxyConfiguration.UseProxy && !string.IsNullOrEmpty(proxyConfiguration.ProxyAddress))
            {
                if(clientHandler == null)
                {
                    logger.LogError("Error loading proxy settings -> Client handler is null");
                    return;
                }

                clientHandler.UseProxy = true;
                if (!string.IsNullOrEmpty(proxyConfiguration.UserName) && !string.IsNullOrEmpty(proxyConfiguration.Password))
                {
                    NetworkCredential credential;
                    if (!string.IsNullOrEmpty(proxyConfiguration.Domain))
                        credential = new NetworkCredential(proxyConfiguration.UserName, proxyConfiguration.Password, proxyConfiguration.Domain);
                    else
                        credential = new NetworkCredential(proxyConfiguration.UserName, proxyConfiguration.Password);

                    clientHandler.Proxy = new WebProxy(proxyConfiguration.ProxyAddress, false, null, credential);
                }
                else
                    clientHandler.Proxy = new WebProxy(proxyConfiguration.ProxyAddress);
            }
            else
                clientHandler.UseProxy = false;
        }

        public virtual IResult<HttpResponseMessage> SendRequest(HttpRequestMessage message, CancellationToken ct)
        {
            return SendRequestAsync(message, ct).GetAwaiter().GetResult();
        }

        public virtual async Task<IResult<HttpResponseMessage>> SendRequestAsync(HttpRequestMessage message, CancellationToken ct)
        {
            try
            {
                HttpResponseMessage response = await HttpClient.SendAsync(message, ct).ConfigureAwait(false);
                return new Result<HttpResponseMessage>(true, response);
            }
            catch (TimeoutException)
            {
                return new Result<HttpResponseMessage>(false, new Message(MessageType.Error, "Error while sending the request: Timeout"));
            }
            catch (OperationCanceledException)
            {
                return new Result<HttpResponseMessage>(false, new Message(MessageType.Error, "Request canceled"));
            }
            catch (Exception e)
            {
                return new Result<HttpResponseMessage>(e);
            }
        }    
        
        public TimeSpan GetDefaultTimeout()
        {
            if (HttpMessageHandler is SimpleHttpClientTimeoutHandler timeoutHandler)
                return timeoutHandler.DefaultTimeout;
            else
                return HttpClient.Timeout;                
        }

        public void SetDefaultTimeout(TimeSpan timeout)
        {
            if (HttpMessageHandler is SimpleHttpClientTimeoutHandler timeoutHandler)
                timeoutHandler.DefaultTimeout = timeout;
            else
                HttpClient.Timeout = timeout;
        }

        public virtual HttpRequestMessage CreateRequest(Uri uri, HttpMethod method)
        {
            return new HttpRequestMessage(method, uri);
        }

        public virtual HttpRequestMessage CreateRequest(Uri uri, HttpMethod method, HttpContent content)
        {           
            var message = CreateRequest(uri, method);
            if (content != null)
                message.Content = content;

            return message;
        }

        public virtual HttpRequestMessage CreateJsonContentRequest(Uri uri, HttpMethod method, object content)
        {
            var message = CreateRequest(uri, method, () => 
            {
                var serialized = JsonConvert.SerializeObject(content, JsonSerializerSettings);
                return new StringContent(serialized, Encoding.UTF8, "application/json");
            });
            return message;
        }

        public virtual HttpRequestMessage CreateRequest(Uri uri, HttpMethod method, Func<HttpContent> content)
        {
            var message = CreateRequest(uri, method);
            if (content != null)
                message.Content = content.Invoke();

            return message;
        }

        public virtual IResult EvaluateResponse(IResult result, HttpResponseMessage response)
        {
            return EvaluateResponseAsync(result, response).GetAwaiter().GetResult();
        }

        public async virtual Task<IResult> EvaluateResponseAsync(IResult result, HttpResponseMessage response)
        {
            List<IMessage> messageList = new List<IMessage>(result.Messages);

            if (response != null)
            {
                byte[] responseByteArray = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    messageList.Add(new Message(MessageType.Information, response.ReasonPhrase, ((int)response.StatusCode).ToString()));
                    return new Result(true, responseByteArray, typeof(byte[]), messageList);
                }
                else
                {
                    string responseString = string.Empty;
                    if (responseByteArray?.Length > 0)
                        responseString = Encoding.UTF8.GetString(responseByteArray);

                    messageList.Add(new Message(MessageType.Error, response.ReasonPhrase + " | " + responseString, ((int)response.StatusCode).ToString()));
                    return new Result(false, messageList);
                }
            }
            messageList.Add(new Message(MessageType.Error, "Evaluation of response failed - Response from host is null", null));
            return new Result(false, messageList);
        }

        public virtual IResult<T> EvaluateResponse<T>(IResult result, HttpResponseMessage response)
        {
            return EvaluateResponseAsync<T>(result, response).GetAwaiter().GetResult();
        }

        public virtual async Task<IResult<T>> EvaluateResponseAsync<T>(IResult result, HttpResponseMessage response)
        {
            List<IMessage> messageList = new List<IMessage>(result.Messages);

            if (response != null)
            {
                string responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        T requestResult = JsonConvert.DeserializeObject<T>(responseString, JsonSerializerSettings);

                        messageList.Add(new Message(MessageType.Information, response.ReasonPhrase, ((int)response.StatusCode).ToString()));
                        return new Result<T>(true, requestResult, messageList);
                    }
                    catch (Exception e)
                    {
                        messageList.Add(new ExceptionMessage(e));
                        return new Result<T>(false, messageList);
                    }
                }
                else
                {
                    messageList.Add(new Message(MessageType.Error, response.ReasonPhrase + " | " + responseString, ((int)response.StatusCode).ToString()));
                    return new Result<T>(false, messageList);
                }
            }
            messageList.Add(new Message(MessageType.Error, "Evaluation of response failed - Response from host is null", null));
            return new Result<T>(false, messageList);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    HttpClient.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion


    }
}
