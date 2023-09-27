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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace BaSyx.Utils.ResultHandling
{
    public class Result : IResult
    {
        public bool Success { get; private set; }

        public bool? IsException { get; }

        public object Entity { get; private set; }

        public Type EntityType { get; private set; }

        private MessageCollection messages;
        public MessageCollection Messages
        {
            get
            {
                if (messages == null)
                    messages = new MessageCollection();
                return messages;
            }
        }
        public Result(bool success) : this(success, null, null, null)
        { }
        public Result(bool success, IMessage message) : this(success, new MessageCollection { message })
        { }

        public Result(bool success, IEnumerable<IMessage> messages) : this(success, null, null, messages)
        { }

        public Result(bool success, object entity, Type entityType) : this(success, entity, entityType, null)
        { }

        public Result(Exception e) :
            this(false, GetMessageListFromException(e))
        { }

        public Result(IResult result) : this(result.Success, result.Entity, result.EntityType, result.Messages)
        { }

        public static MessageCollection GetMessageListFromException(Exception e)
        {
            MessageCollection messageList = new MessageCollection();

            if (e.InnerException != null)
                messageList.AddRange(GetMessageListFromException(e.InnerException));

            messageList.Add(GetMessageFromException(e));

            return messageList;
        }

        public static IMessage GetMessageFromException(Exception e)
        {
            if (e != null)
                return new Message(MessageType.Exception, e.GetType().Name + ":" + e.Message);
            else
                return new Message(MessageType.Error, "Exception itself is null");
        }
        [JsonConstructor]
        public Result(bool success, object entity, Type entityType, IEnumerable<IMessage> messages)
        {
            Success = success;

            if (messages != null)
                foreach (Message msg in messages)
                {
                    if (msg != null)
                    {
                        if (msg.MessageType == MessageType.Exception)
                            IsException = true;

                        Messages.Add(msg);
                    }
                }

            if (entity != null && entityType != null)
            {
                Entity = entity;
                EntityType = entityType;
            }

        }

        public T GetEntity<T>()
        {
            if (Entity != null && 
                (Entity is T ||
                Entity.GetType().IsAssignableFrom(typeof(T)) ||
                Entity.GetType().GetInterfaces().Contains(typeof(T))))
                return (T)Entity;
            return default;
        }
        
        public override string ToString()
        {
            string messageTxt = Messages?.ToString();
            string entityTxt = Entity?.ToString();
            string resultTxt =  $"Success: {Success}";

            if (!string.IsNullOrEmpty(entityTxt))
                resultTxt += " | Entity: " + entityTxt;
            if (!string.IsNullOrEmpty(messageTxt))
                resultTxt += " | Messages: " + messageTxt;
            return resultTxt;
        }

        public bool ShouldSerializeMessages()
        {
            if (Messages?.Count() > 0)
                return true;
            else
                return false;
        }
    }

    public class Result<TEntity> : Result, IResult<TEntity>
    {
        [IgnoreDataMember]
        public new TEntity Entity { get; private set; }
        public Result(bool success) : this(success, default(TEntity), new MessageCollection())
        { }
        public Result(bool success, TEntity entity) : this(success, entity, new MessageCollection())
        { }
        public Result(bool success, IMessage message) : this(success, default(TEntity), new MessageCollection { message })
        { }
        public Result(bool success, IEnumerable<IMessage> messages) : this(success, default(TEntity), messages)
        { }
        public Result(bool success, TEntity entity, IMessage message) : this(success, entity, new MessageCollection { message })
        { }
        public Result(Exception e) : base(e)
        { }
        public Result(IResult result) : base(result)
        { }
        public Result(bool success, TEntity entity, IEnumerable<IMessage> messages) : base(success, entity, typeof(TEntity), messages)
        {
            Entity = entity;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
