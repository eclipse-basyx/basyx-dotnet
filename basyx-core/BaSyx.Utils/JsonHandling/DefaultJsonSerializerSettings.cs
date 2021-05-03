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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BaSyx.Utils.JsonHandling
{
    public class DefaultJsonSerializerSettings : JsonSerializerSettings
    {
        public DefaultJsonSerializerSettings() : base()
        {
            NullValueHandling = NullValueHandling.Ignore;
            Formatting = Formatting.Indented;
            DefaultValueHandling = DefaultValueHandling.Include;
            MissingMemberHandling = MissingMemberHandling.Ignore;
            Converters.Add(new StringEnumConverter());
        }
    }
}
