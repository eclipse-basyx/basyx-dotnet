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
namespace BaSyx.Models.Core.AssetAdministrationShell.Identification
{
    public class IRDI_Reference : Reference
    {
        /// <summary>
        /// Creates a new IRDI-Reference with a definied referred KeyElements-Type
        /// </summary>
        /// <param name="referredtype">KeyElements-Type</param>
        /// <param name="irdi">IRDI</param>
        public IRDI_Reference(KeyElements referredtype, string irdi) : base(new GlobalKey(referredtype, KeyType.IRDI, irdi))
        { }

        /// <summary>
        /// Creates a new IRDI-Reference with KeyElements-Type: GlobalReference
        /// </summary>
        /// <param name="irdi">IRDI</param>
        public IRDI_Reference(string irdi) : this(KeyElements.GlobalReference, irdi)
        { }
    }
}
