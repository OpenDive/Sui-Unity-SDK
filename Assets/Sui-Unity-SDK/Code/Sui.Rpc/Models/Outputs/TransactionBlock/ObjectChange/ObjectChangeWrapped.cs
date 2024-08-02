//
//  ObjectChangeWrapped.cs
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
    /// Represents an event where an object is wrapped.
    /// </summary>
    public class ObjectChangeWrapped : IObjectChange
    {
        /// <summary>
        /// The wrapped object's sender.
        /// </summary>
        public AccountAddress Sender { get; internal set; }

        /// <summary>
        /// The wrapped object's type.
        /// </summary>
        public string ObjectType { get; internal set; }

        /// <summary>
        /// The wrapped object's ID.
        /// </summary>
        public AccountAddress ObjectID { get; internal set; }

        public ObjectChangeWrapped
        (
            AccountAddress sender,
            string object_type,
            AccountAddress object_id,
            BigInteger version
        ) : base(version)
        {
            this.Sender = sender;
            this.ObjectType = object_type;
            this.ObjectID = object_id;
        }

        public override bool Equals(object other)
        {
            if (other is not ObjectChangeWrapped)
                this.SetError<bool, SuiError>(false, "Compared object is not an ObjectChangeWrapped.", other);

            ObjectChangeWrapped other_object_change_wrapped = (ObjectChangeWrapped)other;

            return
                this.Sender.Equals(other_object_change_wrapped.Sender) &&
                this.ObjectType == other_object_change_wrapped.ObjectType &&
                this.ObjectID.Equals(other_object_change_wrapped.ObjectID) &&
                this.Version.Equals(other_object_change_wrapped.Version);
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}