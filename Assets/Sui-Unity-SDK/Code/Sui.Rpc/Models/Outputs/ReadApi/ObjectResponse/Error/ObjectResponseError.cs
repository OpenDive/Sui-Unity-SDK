//
//  ObjectResponseError.cs
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
    /// <para>`ObjectResponseError` represents different types of errors that can occur when dealing with object responses.</para>
    ///
    /// <para>- `notExist`: Indicates that an object with the given ID does not exist.</para>
    /// <para>- `dynamicFieldNotFound`: Indicates that a dynamic field is not found within the parent object.</para>
    /// <para>- `deleted`: Indicates that the object has been deleted, and additional details are provided.</para>
    /// <para>- `unknown`: Represents an unknown error.</para>
    /// <para>- `displayError`: Indicates that there's a display error, and the details are given as a string.</para>
    /// </summary>
    [JsonConverter(typeof(ObjectResponseErrorConverter))]
    public class ObjectResponseError : ReturnBase
    {
        /// <summary>
        /// The type representation of the object error.
        /// </summary>
        public ObjectResponseErrorType Type { get; internal set; }

        /// <summary>
        /// The contents of the object error.
        /// </summary>
        public IObjectResponseError ObjectError { get; internal set; }

        public ObjectResponseError
        (
            IObjectResponseError object_error,
            ObjectResponseErrorType type
        )
        {
            this.ObjectError = object_error;
            this.Type = type;
        }

        public ObjectResponseError(SuiError error)
        {
            this.Error = error;
        }

        public override bool Equals(object obj)
        {
            if (obj is not ObjectResponseError)
                return this.SetError<bool, SuiError>(false, "Compared object is not an ObjectResponseError.", obj);

            ObjectResponseError other_object_error = (ObjectResponseError)obj;

            return
                this.Type == other_object_error.Type &&
                this.ObjectError.Equals(other_object_error.ObjectError);
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}