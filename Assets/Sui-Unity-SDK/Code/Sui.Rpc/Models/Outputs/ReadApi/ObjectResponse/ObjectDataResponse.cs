//
//  ObjectDataResponse.cs
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

namespace Sui.Rpc.Models
{
    /// <summary>
    /// A structure representing the response from a request for an `ObjectData`,
    /// containing either the object data or an error.
    /// </summary>
    [JsonObject]
    public class ObjectDataResponse
    {
        /// <summary>
        /// An optional `ObjectData` representing the data of the requested SuiObject, if the request was successful.
        /// </summary>
        [JsonProperty("data", NullValueHandling = NullValueHandling.Include)]
        public ObjectData Data { get; internal set; }

        /// <summary>
        /// An optional `ObjectResponseError` representing any error that occurred during the request for the Sui object.
        /// </summary>
        [JsonProperty("error", NullValueHandling = NullValueHandling.Include)]
        public ObjectResponseError Error { get; internal set; }

        /// <summary>
        /// Retrieves the initial version of the shared object, if applicable.
        /// This method will return `null` if the `data` is `null` or if the owner of the object is not shared.
        /// </summary>
        /// <returns>An optional `int` representing the initial version of the shared object.</returns>
        public int? GetSharedObjectInitialVersion()
        {
            if (this.Data == null)
                return null;

            Owner owner = this.Data.Owner;

            if (owner == null || owner.Shared == null)
                return null;

            return owner.Shared.InitialSharedVersion;
        }

        /// <summary>
        /// Constructs and retrieves a `SuiObjectRef` reference from the object data, if available.
        /// This method will return `null` if the `data` is `null`.
        /// </summary>
        /// <returns>An optional `SuiObjectRef` representing the object reference constructed from the object data.</returns>
        public Sui.Types.SuiObjectRef GetObjectReference()
        {
            if (this.Data == null || this.Data.Version == null)
                return null;

            return new Sui.Types.SuiObjectRef
            (
                this.Data.ObjectID,
                this.Data.Version,
                this.Data.Digest
            );
        }
    }
}