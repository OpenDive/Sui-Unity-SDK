//
//  ARawData.cs
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
using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// An abstract class representing a generic raw data object.
    /// </summary>
    public abstract class ARawData
    {
        /// <summary>
        /// <para>Most move packages are uniquely identified by their ID (i.e. there is only one version per
        /// ID), but the version is still stored because one package may be an upgrade of another (at a
        /// different ID), in which case its version will be one greater than the version of the
        /// upgraded package.</para>
        ///
        /// <para>Framework packages are an exception to this rule -- all versions of the framework packages
        /// exist at the same ID, at increasing versions.</para>
        ///
        /// <para>In all cases, packages are referred to by move calls using just their ID, and they are
        /// always loaded at their latest version.</para>
        /// </summary>
        [JsonProperty("version")]
        public BigInteger Version { get; internal set; }

        public ARawData(BigInteger version)
        {
            this.Version = version;
        }
    }
}