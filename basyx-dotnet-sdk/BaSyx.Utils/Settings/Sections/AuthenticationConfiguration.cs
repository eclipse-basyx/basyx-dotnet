/*******************************************************************************
* Copyright (c) 2024 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using System.Xml;
using System.Xml.Serialization;

namespace BaSyx.Utils.Settings
{
    public class AuthenticationConfiguration
    {
        [XmlElement]
        public bool Activated { get; set; }

        [XmlElement]
        public string Username { get; set; }

        [XmlElement]
        public string Password { get; set; }

        [XmlElement]
        public string BaseAddress { get; set; }

        [XmlElement]
        public string GetTokenRelativeUrl { get; set; }

        [XmlElement]
        public string ValidateTokenRelativeUrl { get; set; }

        [XmlElement]
        public string TokenType { get; set; }

        [XmlElement]
        public OAuth2Configuration OAuth2Config { get; set; }

        [XmlElement]
        public ProxyConfiguration ProxyConfig { get; set; }

        public AuthenticationConfiguration()
        {
        }
    }

    public class OAuth2Configuration
    {
        [XmlElement]
        public string AuthorizationEndpoint { get; set; }

        [XmlElement]
        public string TokenEndpoint { get; set; }

        [XmlElement]
        public string UserInformationEndpoint { get; set; }

        [XmlElement]
        public string IntrospectEndpoint { get; set; }

        [XmlElement]
        public string ClientId { get; set; }

        [XmlElement]
        public string ClientSecret { get; set; }

        [XmlElement]
        public string ServiceClientId { get; set; }

        [XmlElement]
        public string ServiceClientSecret { get; set; }

        [XmlElement]
        public string RedirectUri { get; set; }

        [XmlElement]
        public int CachedTokenValidityInMinutes { get; set; } = 3;
    }
}
