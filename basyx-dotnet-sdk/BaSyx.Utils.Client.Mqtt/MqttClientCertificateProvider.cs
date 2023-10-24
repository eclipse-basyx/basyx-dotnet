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
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using MQTTnet.Client;

namespace BaSyx.Utils.Client.Mqtt
{
    public class MqttClientCertificateProvider : IMqttClientCertificatesProvider
    {
        private List<X509Certificate> _certificates;
        public MqttClientCertificateProvider(List<X509Certificate> certificates) { _certificates = certificates; }
        public X509CertificateCollection GetCertificates() => new X509CertificateCollection(_certificates.ToArray());
    }
}
