//
//  ObjectChangeCreated.cs
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

using System.Numerics;
using Sui.Accounts;
using Sui.Utilities;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Represents an object being created.
    /// </summary>
    public class ObjectChangeCreated : IObjectChange
    {
        /// <summary>
        /// The created object's sender.
        /// </summary>
        public AccountAddress Sender { get; internal set; }

        /// <summary>
        /// The owner of the created object.
        /// </summary>
        public Owner Owner { get; internal set; }

        /// <summary>
        /// The type of object created.
        /// </summary>
        public string ObjectType { get; internal set; }

        /// <summary>
        /// The created object's ID.
        /// </summary>
        public AccountAddress ObjectID { get; internal set; }

        /// <summary>
        /// The digest associated with the transaction that created the object.
        /// </summary>
        public string Digest { get; internal set; }

        public ObjectChangeCreated
        (
            AccountAddress sender,
            Owner owner,
            string object_type,
            AccountAddress object_id,
            BigInteger version,
            string digest
        ) : base(version)
        {
            this.Sender = sender;
            this.Owner = owner;
            this.ObjectType = object_type;
            this.ObjectID = object_id;
            this.Digest = digest;
        }

        public override bool Equals(object other)
        {
            if (other is not ObjectChangeCreated)
                this.SetError<bool, SuiError>(false, "Compared object is not an ObjectChangeCreated.", other);

            ObjectChangeCreated other_object_change_created = (ObjectChangeCreated)other;

            return
                this.Sender.Equals(other_object_change_created.Sender) &&
                this.Owner.Equals(other_object_change_created.Owner) &&
                this.ObjectType == other_object_change_created.ObjectType &&
                this.ObjectID.Equals(other_object_change_created.ObjectID) &&
                this.Digest == other_object_change_created.Digest &&
                this.Version.Equals(other_object_change_created.Version);
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}