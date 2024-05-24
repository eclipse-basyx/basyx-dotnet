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

using System.Globalization;
using System.Text.Json.Serialization;

namespace BaSyx.Utils.ResultHandling
{
    public class Message : IMessage
    {
        public MessageType MessageType { get; set; }
        public string Text { get; set; }
        public string Code { get; set; }

        [JsonConstructor]
        public Message() { }

        public Message(MessageType messageType, string text) : this(messageType, text, null)
        { }
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
        [JsonConstructor]
        public ErrorMessage() : base(MessageType.Error, string.Empty) { }

        public ErrorMessage(string text) : base(MessageType.Error, text) { }
        public ErrorMessage(string text, string code) : base(MessageType.Error, text, code) { }
    }

    public class InfoMessage : Message
    {
        [JsonConstructor]
        public InfoMessage() : base(MessageType.Information, string.Empty) { }

        public InfoMessage(string text) : base(MessageType.Information, text) { }
        public InfoMessage(string text, string code) : base(MessageType.Information, text, code) { }
    }

    public class WarningMessage : Message
    {
        [JsonConstructor]
        public WarningMessage() : base(MessageType.Warning, string.Empty) { }

        public WarningMessage(string text) : base(MessageType.Warning, text) { }
        public WarningMessage(string text, string code) : base(MessageType.Warning, text, code) { }
    }

    public class DebugMessage : Message
    {
        [JsonConstructor]
        public DebugMessage() : base(MessageType.Debug, string.Empty) { }

        public DebugMessage(string text) : base(MessageType.Debug, text) { }
        public DebugMessage(string text, string code) : base(MessageType.Debug, text, code) { }
    }

    public class FatalMessage : Message
    {
        [JsonConstructor]
        public FatalMessage() : base(MessageType.Fatal, string.Empty) { }

        public FatalMessage(string text) : base(MessageType.Fatal, text) { }
        public FatalMessage(string text, string code) : base(MessageType.Fatal, text, code) { }
    }
}
