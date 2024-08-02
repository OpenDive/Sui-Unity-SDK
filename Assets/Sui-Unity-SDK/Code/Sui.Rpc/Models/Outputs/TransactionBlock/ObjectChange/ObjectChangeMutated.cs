//
//  ObjectChangeMutated.cs
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
    /// Represents an event where an object is mutated.
    /// </summary>
    public class ObjectChangeMutated : IObjectChange
    {
        /// <summary>
        /// The transaction's sender associated with the mutated object.
        /// </summary>
        public AccountAddress Sender { get; internal set; }

        /// <summary>
        /// The owner of the mutated object.
        /// </summary>
        public Owner Owner { get; internal set; }

        /// <summary>
        /// The mutated object's type.
        /// </summary>
        public string ObjectType { get; internal set; }

        /// <summary>
        /// The previous version number of the mutated object.
        /// </summary>
        public BigInteger PreviousVersion { get; internal set; }

        /// <summary>
        /// The transaction digest where the object was mutated.
        /// </summary>
        public string Digest { get; internal set; }

        public ObjectChangeMutated
        (
            AccountAddress sender,
            Owner owner,
            string object_type,
            BigInteger previous_version,
            BigInteger version,
            string digest
        ) : base(version)
        {
            this.Sender = sender;
            this.Owner = owner;
            this.ObjectType = object_type;
            this.PreviousVersion = previous_version;
            this.Digest = digest;
        }

        public override bool Equals(object other)
        {
            if (other is not ObjectChangeMutated)
                this.SetError<bool, SuiError>(false, "Compared object is not an ObjectChangeMutated.", other);

            ObjectChangeMutated other_object_change_mutated = (ObjectChangeMutated)other;

            return
                this.Sender.Equals(other_object_change_mutated.Sender) &&
                this.Owner.Equals(other_object_change_mutated.Owner) &&
                this.ObjectType == other_object_change_mutated.ObjectType &&
                this.PreviousVersion.Equals(other_object_change_mutated.PreviousVersion) &&
                this.Digest == other_object_change_mutated.Digest &&
                this.Version.Equals(other_object_change_mutated.Version);
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}