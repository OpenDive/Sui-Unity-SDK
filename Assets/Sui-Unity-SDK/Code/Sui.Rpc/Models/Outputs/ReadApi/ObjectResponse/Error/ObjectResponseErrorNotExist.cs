//
//  ObjectResponseErrorNotExist.cs
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
    /// Indicates that an object with the given ID does not exist.
    /// </summary>
    public class ObjectResponseErrorNotExist : ReturnBase, IObjectResponseError
    {
        /// <summary>
        /// The inputted Object ID.
        /// </summary>
        public AccountAddress ObjectID { get; internal set; }

        public ObjectResponseErrorNotExist(AccountAddress object_id)
        {
            this.ObjectID = object_id;
        }

        public override bool Equals(object obj)
        {
            if (obj is not ObjectResponseErrorNotExist)
                return this.SetError<bool, SuiError>(false, "Compared object is not an ObjectResponseErrorNotExist.", obj);

            ObjectResponseErrorNotExist other_not_exist = (ObjectResponseErrorNotExist)obj;

            return this.ObjectID.Equals(other_not_exist.ObjectID);
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}