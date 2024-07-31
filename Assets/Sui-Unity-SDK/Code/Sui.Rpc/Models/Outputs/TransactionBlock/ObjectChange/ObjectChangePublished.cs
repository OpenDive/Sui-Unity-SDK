//
//  ObjectChangePublished.cs
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

using Sui.Accounts;
using Sui.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Represents a published object.
    /// </summary>
    public class ObjectChangePublished : IObjectChange
    {
        /// <summary>
        /// The ID of the package.
        /// </summary>
        public AccountAddress PackageID { get; internal set; }

        /// <summary>
        /// The digest of the transaction that published the package.
        /// </summary>
        public string Digest { get; internal set; }

        /// <summary>
        /// The modules associated with the package as Base 64 strings.
        /// </summary>
        public List<string> Modules { get; internal set; }

        public ObjectChangePublished
        (
            AccountAddress package_id,
            BigInteger version,
            string digest,
            List<string> modules
        ) : base(version)
        {
            this.PackageID = package_id;
            this.Digest = digest;
            this.Modules = modules;
        }

        public override bool Equals(object other)
        {
            if (other is not ObjectChangePublished)
                this.SetError<bool, SuiError>(false, "Compared object is not an ObjectChangePublished.", other);

            ObjectChangePublished other_object_change_published = (ObjectChangePublished)other;

            return
                this.PackageID.Equals(other_object_change_published.PackageID) &&
                this.Version.Equals(other_object_change_published.Version) &&
                this.Digest == other_object_change_published.Digest &&
                this.Modules.SequenceEqual(other_object_change_published.Modules);
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}