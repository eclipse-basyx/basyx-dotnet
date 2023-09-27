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

namespace BaSyx.Models.AdminShell
{
    public abstract class UniformResource
    {
        public virtual string Organisation { get; set; }
        public virtual string SubUnit { get; set; }
        public virtual string DomainId { get; set; }
        public virtual string Version { get; set; }
        public virtual string Revision { get; set; }
        public virtual string ElementId { get; set; }
        public virtual string InstanceNumber { get; set; }

        protected UniformResource(string organisation, string subUnit, string domainId, string version, string revision, string elementId, string instanceNumber)
        {
            Organisation = organisation;
            SubUnit = subUnit;
            DomainId = domainId;
            Version = version;
            Revision = revision;
            ElementId = elementId;
            InstanceNumber = instanceNumber;
        }

        public abstract Identifier ToIdentifier();
        public virtual string ToUri() => ToUri(Organisation, SubUnit, DomainId, Version, Revision, ElementId, InstanceNumber);
        public virtual string ToUrn() => ToUrn(Organisation, SubUnit, DomainId, Version, Revision, ElementId, InstanceNumber);

        public static string ToUri(string organisation, string subUnit, string domainId, string version, string revision, string elementId, string instanceNumber)
        {
            string uri = "http://";

            uri += organisation + "/";

            if (!string.IsNullOrEmpty(subUnit))
                uri += subUnit + "/";
            if (!string.IsNullOrEmpty(domainId))
                uri += domainId + "/";
            if (!string.IsNullOrEmpty(version))
                uri += version + "/";
            if (!string.IsNullOrEmpty(revision))
                uri += revision + "/";
            if (!string.IsNullOrEmpty(elementId))
                uri += elementId + "/";

            if (!string.IsNullOrEmpty(instanceNumber))
            {
                uri = uri.Substring(0, uri.Length - 2);
                uri += "#" + instanceNumber;
            }

            if (uri.EndsWith("/"))
                uri = uri.Remove(uri.Length - 1);

            return uri;
        }
        public static string ToUrn(string organisation, string subUnit, string domainId, string version, string revision, string elementId, string instanceNumber)
        {
            string urn = "urn:";

            urn += organisation + ":";

            if (!string.IsNullOrEmpty(subUnit))
                urn += subUnit + ":";
            if (!string.IsNullOrEmpty(domainId))
                urn += domainId + ":";
            if (!string.IsNullOrEmpty(version))
                urn += version + ":";
            if (!string.IsNullOrEmpty(revision))
                urn += revision + ":";
            if (!string.IsNullOrEmpty(elementId))
                urn += elementId + ":";

            if (!string.IsNullOrEmpty(instanceNumber))
            {
                urn = urn.Substring(0, urn.Length - 2);
                urn += "#" + instanceNumber;
            }

            if (urn.EndsWith(":"))
                urn = urn.Remove(urn.Length - 1);

            return urn;
        }

        public static implicit operator Identifier(UniformResource urn)
        {
            return urn.ToIdentifier();
        }
    }
}
