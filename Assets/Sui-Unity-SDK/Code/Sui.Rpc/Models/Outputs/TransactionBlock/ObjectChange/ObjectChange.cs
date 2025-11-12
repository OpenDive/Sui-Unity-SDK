//
//  ObjectChange.cs
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
    /// Represents an object change event.
    /// </summary>
    [JsonConverter(typeof(ObjectChangeConverter))]
    public class ObjectChange : ReturnBase
    {
        /// <summary>
        /// The type of change event.
        /// </summary>
        public ObjectChangeType Type { get; internal set; }

        /// <summary>
        /// The data associated with the changed object.
        /// </summary>
        public IObjectChange Change { get; internal set; }

        public ObjectChange
        (
            ObjectChangeType type,
            IObjectChange change
        )
        {
            this.Type = type;
            this.Change = change;
        }

        public ObjectChange(SuiError error)
        {
            this.Error = error;
        }

        public override bool Equals(object other)
        {
            if (other is not ObjectChange)
                this.SetError<bool, SuiError>(false, "Compared object is not an ObjectChange.", other);

            ObjectChange other_object_change = (ObjectChange)other;

            return
                this.Type == other_object_change.Type &&
                this.Change.Equals(other_object_change.Change);
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}