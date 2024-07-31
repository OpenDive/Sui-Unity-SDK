//
//  ObjectChangeTransferred.cs
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
    /// The event where the object is transferred.
    /// </summary>
    public class ObjectChangeTransferred : IObjectChange
    {
        /// <summary>
        /// The transaction's sender.
        /// </summary>
        public AccountAddress Sender { get; internal set; }

        /// <summary>
        /// The new owner of the object.
        /// </summary>
        public Owner Recipient { get; internal set; }

        /// <summary>
        /// The transferred object's type.
        /// </summary>
        public string ObjectType { get; internal set; }

        /// <summary>
        /// The ID of the transferred object.
        /// </summary>
        public AccountAddress ObjectID { get; internal set; }

        /// <summary>
        /// The digest of the transaction where the object was transferred.
        /// </summary>
        public string Digest { get; internal set; }

        public ObjectChangeTransferred
        (
            AccountAddress sender,
            Owner recipient,
            string object_type,
            AccountAddress object_id,
            BigInteger version,
            string digest
        ) : base(version)
        {
            this.Sender = sender;
            this.Recipient = recipient;
            this.ObjectType = object_type;
            this.ObjectID = object_id;
            this.Digest = digest;
        }

        public override bool Equals(object other)
        {
            if (other is not ObjectChangeTransferred)
                this.SetError<bool, SuiError>(false, "Compared object is not an ObjectChangeTransferred.", other);

            ObjectChangeTransferred other_object_change_transferred = (ObjectChangeTransferred)other;

            return
                this.Sender.Equals(other_object_change_transferred.Sender) &&
                this.Recipient.Equals(other_object_change_transferred.Recipient) &&
                this.ObjectType == other_object_change_transferred.ObjectType &&
                this.ObjectID.Equals(other_object_change_transferred.ObjectID) &&
                this.Digest == other_object_change_transferred.Digest &&
                this.Version.Equals(other_object_change_transferred.Version);
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}