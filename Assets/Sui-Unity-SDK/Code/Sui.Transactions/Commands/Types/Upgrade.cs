//
//  Upgrade.cs
//  Sui-Unity-SDK
//
//  Copyright (c) 2024 OpenDive
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//

using System.Collections.Generic;
using System.Linq;
using OpenDive.BCS;
using Sui.Accounts;

namespace Sui.Transactions
{
    /// <summary>
    /// A command that allows a user to upgrade an existing package object.
    /// </summary>
    public class Upgrade : ICommand
    {
        /// <summary>
        /// The updated module.
        /// </summary>
        public byte[][] Modules { get; set; }

        /// <summary>
        /// The dependencies of the updated module.
        /// </summary>
        public AccountAddress[] Dependencies { get; set; }

        /// <summary>
        /// The package ID of the existing package.
        /// </summary>
        public string PackagID { get; set; }

        /// <summary>
        /// The ticket associated with the upgrade request.
        /// </summary>
        public TransactionArgument Ticket { get; set; }

        public Upgrade
        (
            byte[][] modules,
            AccountAddress[] dependencies,
            string package_id,
            TransactionArgument ticket
        )
        {
            this.Modules = modules;
            this.Dependencies = dependencies;
            this.PackagID = package_id;
            this.Ticket = ticket;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU32AsUleb128((uint)this.Modules.Length);

            foreach (byte[] module in this.Modules)
                serializer.Serialize(module);

            serializer.Serialize(this.Dependencies);
            serializer.Serialize(this.PackagID);
            serializer.Serialize(this.Ticket);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            List<byte[]> modules = new List<byte[]>();
            int length = deserializer.DeserializeUleb128();

            for (int i = 0; i < length; ++i)
                modules.Add(deserializer.ToBytes());

            return new Upgrade
            (
                modules.ToArray(),
                deserializer.DeserializeSequence(typeof(AccountAddress)).Values.Cast<AccountAddress>().ToArray(),
                deserializer.DeserializeString().Value,
                (TransactionArgument)TransactionArgument.Deserialize(deserializer)
            );
        }
    }
}