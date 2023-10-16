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
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    /// <summary>
    /// Used to uniquely identify an entity by using an identifier.
    /// </summary>
    [DataContract]
    public class Identifier
    {
        /// <summary>
        /// The globally unique identification of the element
        /// </summary>
        public string Id { get; set; }

        internal Identifier() { }

        public Identifier(string id)
        {
            Id = id;
        }

        public static implicit operator string(Identifier identifier)
        {
            if (identifier == null || string.IsNullOrEmpty(identifier.Id))
                return null;
            else
                return identifier.Id;
        }

        public static implicit operator Identifier(string id)
        {
            if(string.IsNullOrEmpty(id)) 
                return null;
            else
                return new Identifier(id);
        }

        public override string ToString() => Id;
    }

}
