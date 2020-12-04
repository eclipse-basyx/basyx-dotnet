/*******************************************************************************
* Copyright (c) 2020 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using System.Runtime.Serialization;

namespace BaSyx.Models.Core.AssetAdministrationShell.Identification
{
    /// <summary>
    /// A key is a reference to an element by its id. 
    /// </summary>
    public interface IKey
    {
        /// <summary>
        /// Denote which kind of entity is referenced. In case type = GlobalReference then the element is a global unique id.  
        /// In all other cases the key references a model element of the same or of another AAS.The name of the model element is explicitly listed.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "type")]
        KeyElements Type { get; }

        /// <summary>
        /// Type of the key value.In case of idType = idShort local shall be true. 
        /// In case type=GlobalReference idType shall not be IdShort. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "idType")]
        KeyType IdType { get; }

        /// <summary>
        /// The key value, for example an IRDI if the idType=IRDI. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "value")]
        string Value { get; }

        /// <summary>
        /// Denotes if the key references a model element of the same AAS (=true) or not (=false).
        /// In case of local = false the key may reference a model element of another AAS or an entity outside any AAS that has a global unique id. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "local")]
        bool Local { get; }

        /// <summary>
        /// Returns the official string representation of a Key according to Details of Asset Administration Shell (Chapter 5.2.1)
        /// </summary>
        /// <returns></returns>
        string ToStandardizedString();
    }
}
