//
//  PackageRawData.cs
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
using Sui.Accounts;
using System.Collections.Generic;
using System.Numerics;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Represents the raw data structure for a Package in the Move programming language.
    /// </summary>
    public class PackageRawData : ARawData
    {
        /// <summary>
        /// Represents the unique identifier of the package.
        /// </summary>
        [JsonProperty("id")]
        public AccountAddress ID { get; internal set; }

        /// <summary>
        /// For each dependency, maps original package ID to the info about the (upgraded) dependency
        /// version that this package is using.
        /// </summary>
        [JsonProperty("linkageTable")]
        public Dictionary<string, UpgradeInfo> LinkageTable { get; internal set; }

        /// <summary>
        /// Represents the mapping of the modules.
        /// </summary>
        [JsonProperty("moduleMap")]
        public Dictionary<string, string> ModuleMap { get; internal set; }

        /// <summary>
        /// Maps struct/module to a package version where it was first defined, stored as a vector for
        /// simple serialization and deserialization.
        /// </summary>
        [JsonProperty("typeOriginTable")]
        public TypeOrigin[] TypeOriginTable { get; internal set; }

        public PackageRawData
        (
            AccountAddress id,
            Dictionary<string, UpgradeInfo> linkage_table,
            Dictionary<string, string> module_map,
            TypeOrigin[] type_origin_table,
            BigInteger version
        ) : base(version)
        {
            this.ID = id;
            this.LinkageTable = linkage_table;
            this.ModuleMap = module_map;
            this.TypeOriginTable = type_origin_table;
        }
    }
}