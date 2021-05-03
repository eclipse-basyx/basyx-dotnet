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
using System;
using System.Runtime.Serialization;
using System.Text;

namespace BaSyx.Models.Extensions.Semantics
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
