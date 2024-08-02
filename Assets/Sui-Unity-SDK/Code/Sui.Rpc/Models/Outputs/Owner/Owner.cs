//
//  Owner.cs
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
using Newtonsoft.Json.Serialization;
using Sui.Accounts;
using Sui.Utilities;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Represents the properties of an object's owner.
    /// </summary>
    [JsonConverter(typeof(SuiOwnerConverter))]
    public class Owner : ReturnBase
    {
        /// <summary>
        /// The type of owner of the object.
        /// </summary>
        public SuiOwnerType Type { get; internal set; }

        /// <summary>
        /// If the object is an address or object owner, this represents their account ID.
        /// </summary>
        [JsonProperty("AddressOwner", NullValueHandling = NullValueHandling.Include)]
        public AccountAddress Address { get; internal set; }

        /// <summary>
        /// If the owner is shared, this represents the initial shared version.
        /// </summary>
        [JsonProperty("Shared", NullValueHandling = NullValueHandling.Include)]
        public SharedOwner Shared { get; internal set; }

        public Owner()
        {
            Type = SuiOwnerType.Immutable;
        }

        public Owner(SuiOwnerType type, AccountAddress address)
        {
            Type = type;
            Address = address;
        }

        public Owner(int initial_shared_version)
        {
            Type = SuiOwnerType.Shared;
            Shared = new SharedOwner(initial_shared_version);
        }

        public override bool Equals(object obj)
        {
            if (obj is not Owner)
                this.SetError<bool, SuiError>(false, "Compared object is not an Owner.", obj);

            Owner other_owner = (Owner)obj;

            if (this.Type == SuiOwnerType.Immutable && other_owner.Type == SuiOwnerType.Immutable)
                return true;

            if (this.Address != null && other_owner.Address != null)
                return
                    this.Type == other_owner.Type &&
                    this.Address.Equals(other_owner.Address);

            if (this.Shared != null && other_owner.Shared != null)
                return
                    this.Type == other_owner.Type &&
                    this.Shared.Equals(other_owner.Shared);

            return false;
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}