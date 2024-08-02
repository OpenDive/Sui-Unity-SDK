//
//  StructTag.cs
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

namespace OpenDive.BCS
{
    /// <summary>
    /// Representation of a Struct Tag in BCS
    /// </summary>
    /// <typeparam name="T">Value of Type Arguments used</typeparam>
    public abstract class StructTag<T> : ReturnBase, ISerializable where T : ISerializable
    {
        /// <summary>
        /// The address of the struct tag.
        /// </summary>
        public AccountAddress Address { get; set; }

        /// <summary>
        /// The name of the contract module.
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// The name of the function called.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type fields for the given struct.
        /// </summary>
        public List<T> TypeArguments { get; set; }

        public virtual void Serialize(Serialization serializer) => this.SetError<BcsError>("Serialize function not implemented", null);

        public override string ToString()
        {
            string value = string.Format(
                "{0}::{1}::{2}",
                this.Address.ToHex(),
                this.Module.ToString(),
                this.Name.ToString()
            );

            if (this.TypeArguments != null && this.TypeArguments.Count > 0)
            {
                value += string.Format("<{0}", this.TypeArguments[0].ToString());

                foreach (T typeArg in this.TypeArguments.ToArray()[1..])
                    value += string.Format(", {0}", typeArg.ToString());

                value += ">";
            }

            return value;
        }

        public override bool Equals(object other)
        {
            if (other is not StructTag<T>)
                this.SetError<bool, SuiError>(false, "Compared object is not a StructTag.", other);

            StructTag<T> other_struct_tag = (StructTag<T>)other;

            return
                this.Address.KeyBytes.SequenceEqual(other_struct_tag.Address.KeyBytes) &&
                this.Module == other_struct_tag.Module &&
                this.Name == other_struct_tag.Name &&
                this.TypeArguments.SequenceEqual(other_struct_tag.TypeArguments);
        }

        public override int GetHashCode() => base.GetHashCode();

        internal static string[] FromStr_(string struct_tag) => struct_tag.Split("::");
    }
}