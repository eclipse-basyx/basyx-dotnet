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
using System;
using System.Runtime.Serialization;
using System.Text;

namespace BaSyx.Models.Semantics
{
    [DataContract]
    public class ModelUrn
    {
        [IgnoreDataMember]
        public string LegalEntity { get; }
        [IgnoreDataMember]
        public string SubUnit { get; }
        [IgnoreDataMember]
        public string Submodel { get; }
        [IgnoreDataMember]
        public string Version { get; }
        [IgnoreDataMember]
        public string Revision { get; }
        [IgnoreDataMember]
        public string ElementId { get; }
        [IgnoreDataMember]
        public string ElementInstance { get; }
        [IgnoreDataMember]
        public string UrnString { get; }

        public ModelUrn(string legalEntity, string subUnit, string submodel, string version, string revision, string elementId, string elementInstance)
        {
            StringBuilder urnBuilder = new StringBuilder();

            urnBuilder.Append("urn:");

            if (legalEntity != null)
            {
                LegalEntity = legalEntity;
                urnBuilder.Append(legalEntity);
            }
            urnBuilder.Append(":");

            if (subUnit != null)
            {
                SubUnit = subUnit;
                urnBuilder.Append(subUnit);
            }
            urnBuilder.Append(":");

            if (submodel != null)
            {
                Submodel = submodel;
                urnBuilder.Append(submodel);
            }
            urnBuilder.Append(":");

            if (version != null)
            {
                Version = version;
                urnBuilder.Append(version);
            }
            urnBuilder.Append(":");

            if (revision != null)
            {
                Revision = revision;
                urnBuilder.Append(revision);
            }
            urnBuilder.Append(":");

            if (elementId != null)
            {
                ElementId = elementId;
                urnBuilder.Append(elementId);
            }

            if (elementInstance != null)
            {
                ElementInstance = elementInstance;
                urnBuilder.Append("#" + elementInstance);
            }

            UrnString = urnBuilder.ToString();
        }


        public static ModelUrn Parse(string urnString)
        {
            string[] splitted = urnString.Split(new char[] { ':' }, StringSplitOptions.None);
            if (splitted.Length != 8)
                throw new ArgumentException(urnString + " is not formatted correctly");

            return new ModelUrn(splitted[0], splitted[1], splitted[2], splitted[3], splitted[4], splitted[5], splitted[6]);
        }

        public static bool TryParse(string urnString, out ModelUrn urn)
        {
            try
            {
                urn = Parse(urnString);
                return true;
            }
            catch
            {
                urn = null;
                return false;
            }
        }

    }
}
