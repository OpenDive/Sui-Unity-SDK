//
//  ObjectResponseErrorDynamicFieldNotFound.cs
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

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Indicates that a dynamic field is not found within the parent object.
    /// </summary>
    public class ObjectResponseErrorDynamicFieldNotFound : ReturnBase, IObjectResponseError
    {
        /// <summary>
        /// The parent object's ID.
        /// </summary>
        public AccountAddress ParentObjectID { get; internal set; }

        public ObjectResponseErrorDynamicFieldNotFound(AccountAddress parent_object_id)
        {
            this.ParentObjectID = parent_object_id;
        }

        public override bool Equals(object obj)
        {
            if (obj is not ObjectResponseErrorDynamicFieldNotFound)
                return this.SetError<bool, SuiError>(false, "Compared object is not an ObjectResponseErrorDynamicFieldNotFound.", obj);

            ObjectResponseErrorDynamicFieldNotFound other_dynamic_field_not_found = (ObjectResponseErrorDynamicFieldNotFound)obj;

            return this.ParentObjectID.Equals(other_dynamic_field_not_found.ParentObjectID);
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}