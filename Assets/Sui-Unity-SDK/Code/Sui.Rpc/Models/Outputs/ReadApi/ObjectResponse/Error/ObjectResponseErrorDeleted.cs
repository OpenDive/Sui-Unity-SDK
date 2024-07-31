//
//  ObjectResponseErrorConverter.cs
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
    /// Indicates that the object has been deleted. The digest, object ID, and version are provided for context.
    /// </summary>
    public class ObjectResponseErrorDeleted : ReturnBase, IObjectResponseError
    {
        /// <summary>
        /// Base64 string representing the object digest.
        /// </summary>
        public string Digest { get; internal set; }

        /// <summary>
        /// The deleted object's ID.
        /// </summary>
        public AccountAddress ObjectID { get; internal set; }

        /// <summary>
        /// Object version.
        /// </summary>
        public BigInteger Version { get; internal set; }

        public ObjectResponseErrorDeleted
        (
            string digest,
            AccountAddress object_id,
            BigInteger version
        )
        {
            this.Digest = digest;
            this.ObjectID = object_id;
            this.Version = version;
        }

        public override bool Equals(object obj)
        {
            if (obj is not ObjectResponseErrorDeleted)
                return this.SetError<bool, SuiError>(false, "Compared object is not an ObjectResponseErrorDeleted.", obj);

            ObjectResponseErrorDeleted other_deleted_error = (ObjectResponseErrorDeleted)obj;

            return
                this.Digest == other_deleted_error.Digest &&
                this.ObjectID.Equals(other_deleted_error.ObjectID) &&
                this.Version.Equals(other_deleted_error.Version);
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}