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
namespace BaSyx.Models.AdminShell
{
    public class IRDI_Reference : Reference
    {
        /// <summary>
        /// Creates a new IRDI-Reference with a definied referred KeyElements-Type
        /// </summary>
        /// <param name="referredtype">KeyElements-Type</param>
        /// <param name="irdi">IRDI</param>
        public IRDI_Reference(KeyType referredtype, string irdi) : base(new Key(referredtype, irdi))
        { }

        /// <summary>
        /// Creates a new IRDI-Reference with KeyElements-Type: GlobalReference
        /// </summary>
        /// <param name="irdi">IRDI</param>
        public IRDI_Reference(string irdi) : this(KeyType.GlobalReference, irdi)
        { }
    }
}
