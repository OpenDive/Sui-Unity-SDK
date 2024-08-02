//
//  MoveFunctionArgType.cs
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
    /// The class that represents the type of Move function argument.
    /// </summary>
    [JsonConverter(typeof(MoveFunctionArgTypesConverter))]
	public class MoveFunctionArgType : ReturnBase
    {
        /// <summary>
        /// The argument's type. Can be pure or an object.
        /// </summary>
		public ArgumentType ArgumentType { get; internal set; }

        /// <summary>
        /// If the argument is an object, it can be immutable, mutable, or by value.
        /// </summary>
		public ObjectValueType? ArgumentReference { get; internal set; }

        public MoveFunctionArgType
        (
            ArgumentType arument_type,
            ObjectValueType? argument_reference = null
        )
        {
            this.ArgumentType = arument_type;
            this.ArgumentReference = argument_reference;
        }

        public MoveFunctionArgType(SuiError error)
        {
            this.Error = error;
        }

        public override bool Equals(object obj)
        {
            if (obj is not MoveFunctionArgType)
                return this.SetError<bool, SuiError>(false, "Object is not MoveFunctionArgType.", obj);

            MoveFunctionArgType other_args = (MoveFunctionArgType)obj;

            return
                this.ArgumentType == other_args.ArgumentType &&
                this.ArgumentReference == other_args.ArgumentReference;
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}