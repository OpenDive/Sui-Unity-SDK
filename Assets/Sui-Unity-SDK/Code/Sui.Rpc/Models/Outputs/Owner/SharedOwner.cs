//
//  SharedOwner.cs
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

using Newtonsoft.Json;
using Sui.Utilities;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Represents a shared owner of an object.
    /// </summary>
    [JsonObject]
    public class SharedOwner : ReturnBase
    {
        /// <summary>
        /// The version where the object became shared.
        /// </summary>
        [JsonProperty("initial_shared_version")]
        public int? InitialSharedVersion { get; internal set; }

        public SharedOwner(int initial_shared_version)
        {
            this.InitialSharedVersion = initial_shared_version;
        }

        public override bool Equals(object obj)
        {
            if (obj is not SharedOwner)
                this.SetError<bool, SuiError>(false, "Compared object is not a SharedOwner.", obj);

            SharedOwner other_shared_owner = (SharedOwner)obj;

            return
                this.InitialSharedVersion == other_shared_owner.InitialSharedVersion;
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}