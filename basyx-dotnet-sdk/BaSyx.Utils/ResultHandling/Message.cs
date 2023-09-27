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
using System.Globalization;

namespace BaSyx.Utils.ResultHandling
{
    public class Message : IMessage
    {
        public MessageType MessageType { get; set; }
        public string Text { get; set; }
        public string Code { get; set; }

        public Message(MessageType messageType, string text) : this(messageType, text, null)
        { }
        [JsonConstructor]
        public Message(MessageType messageType, string text, string code)
        {
            MessageType = messageType;
            Text = text;
            Code = code;
        }


        public override string ToString()
        {
            if(!string.IsNullOrEmpty(Code))
                return string.Format(CultureInfo.CurrentCulture, "{0} | {1} | {2}", MessageType, Code, Text);
            else
                return string.Format(CultureInfo.CurrentCulture, "{0} | {1}", MessageType, Text);
        }
    }

    public class ErrorMessage : Message
    {
        public ErrorMessage(string text) : base(MessageType.Error, text) { }
        public ErrorMessage(string text, string code) : base(MessageType.Error, text, code) { }
    }

    public class InfoMessage : Message
    {
        public InfoMessage(string text) : base(MessageType.Information, text) { }
        public InfoMessage(string text, string code) : base(MessageType.Information, text, code) { }
    }

    public class WarningMessage : Message
    {
        public WarningMessage(string text) : base(MessageType.Warning, text) { }
        public WarningMessage(string text, string code) : base(MessageType.Warning, text, code) { }
    }

    public class DebugMessage : Message
    {
        public DebugMessage(string text) : base(MessageType.Debug, text) { }
        public DebugMessage(string text, string code) : base(MessageType.Debug, text, code) { }
    }

    public class FatalMessage : Message
    {
        public FatalMessage(string text) : base(MessageType.Fatal, text) { }
        public FatalMessage(string text, string code) : base(MessageType.Fatal, text, code) { }
    }
}
