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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace BaSyx.Models.AdminShell
{
    [DataContract]
    public class Reference : IReference
    {
        [IgnoreDataMember]
        public virtual KeyType RefersTo
        {
            get
            {
                if (Keys?.Count > 0)
                    return Keys.Last().Type;
                return KeyType.Undefined;
            }
        }

        [IgnoreDataMember]
        public IKey First
        {
            get
            {
                if (Keys?.Count > 0)
                    return Keys.First();
                return null;
            }
        }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "keys")]
        public List<IKey> Keys { get; protected set; }

        public ReferenceType Type { get; set; }

        public IReference ReferredSemanticId { get; set; }

        [JsonConstructor]
        public Reference(params IKey[] keys)
        {
            keys = keys ?? throw new ArgumentNullException(nameof(keys));

            if (Keys?.Count > 0)
            {
                foreach (var key in keys)
                    if (!Keys.Contains(key))
                        Keys.Add(key);
            }
            else
                Keys = keys.ToList();
        }

        public string ToStandardizedString()
        {
            string referenceString = string.Empty;
            for (int i = 0; i < Keys.Count; i++)
            {
                referenceString += Keys[i].ToStandardizedString();

                if (i + 1 == Keys.Count)
                    break;
                else
                    referenceString += ",";
            }
            return referenceString;
        }
    }

    [DataContract]
    public class Reference<T> : Reference, IReference<T> where T : IReferable
    {
        [IgnoreDataMember]
        public override KeyType RefersTo => Key.GetKeyElementFromType(typeof(T));

        [JsonConstructor]
        public Reference(params IKey[] keys) : base(keys)
        { }

        public Reference(T element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            List<IKey> keys = new List<IKey>();

            if (element is IIdentifiable identifiable)
            {
                keys.Add(new Key(Key.GetKeyElementFromType(identifiable.GetType()), identifiable.Id));
            }
            else if (element is IReferable referable)
            {
                if (referable.Parent != null && referable.Parent is IIdentifiable parentIdentifiable)
                    keys.Add(new Key(Key.GetKeyElementFromType(parentIdentifiable.GetType()), parentIdentifiable.Id));

                keys.Add(new Key(Key.GetKeyElementFromType(referable.GetType()), referable.IdShort));
            }

            Keys = keys.ToList();
        }
    }
}
