//
//  InputsHandler.cs
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
using Sui.Types;
using Sui.Utilities;

namespace Sui.Transactions
{
	/// <summary>
	/// Utility class for handling call argument and object inputs.
	/// </summary>
	public class InputsHandler
	{
        /// <summary>
        /// Determines whether the input is a mutable shared object or not.
        /// </summary>
        /// <param name="arg">The call argument.</param>
        /// <returns>A boolean value indicating whether the input is a mutable shared object.</returns>
        static public bool IsMutableSharedObjectInput(ICallArg arg)
		{
			SharedObjectRef sharedObject = GetSharedObjectInput(arg);

			if (sharedObject != null)
			{
				return sharedObject.Mutable;
			}
			return false;
		}

		/// <summary>
		/// Converts a `SharedObjectRef` input into an `ObjectArg`.
		/// </summary>
		/// <param name="shared_object_ref">The inputted shared object reference.</param>
		/// <returns>An `ObjectArg` output representing the shared object reference.</returns>
		public static ObjectArg SharedObjectRef(SharedObjectRef shared_object_ref)
        {
			return new ObjectArg
			(
				ObjectRefType.Shared,
				new SharedObjectRef
				(
					AccountAddress.FromHex(Utils.NormalizeSuiAddress(shared_object_ref.ObjectID.ToHex())),
					shared_object_ref.InitialSharedVersion,
					shared_object_ref.Mutable
				)
			);
        }

		/// <summary>
		/// Retrieve the object's ID from a call argument transaction object input.
		/// </summary>
		/// <param name="arg">The call argument transaction object input.</param>
		/// <returns>A `SuiResult` containing an `AccountAddress`.</returns>
		public static SuiResult<AccountAddress> GetIDFromCallArg(CallArgTransactionObjectInput arg)
        {
			switch(arg.Input.Type)
            {
				case ObjectRefType.ImmOrOwned:
					return new SuiResult<AccountAddress>(((Sui.Types.SuiObjectRef)arg.Input.ObjectRef).ObjectID);
				case ObjectRefType.Shared:
					return new SuiResult<AccountAddress>(((Sui.Types.SharedObjectRef)arg.Input.ObjectRef).ObjectID);
				default:
                    return new SuiResult<AccountAddress>(null, new SuiError(0, "Unable to retrieve ID from call argument.", null));
            }
        }

        /// <summary>
        /// Retrieve the object's ID from a transaction object input.
        /// </summary>
        /// <param name="value">The transaction object input.</param>
        /// <returns>A `SuiResult` containing a `string`.</returns>
        public static SuiResult<string> GetIDFromCallArg(ITransactionObjectInput value)
        {
			switch (value.Type)
			{
				case TransactionObjectInputType.ObjectCallArgument:
                    SuiResult<AccountAddress> object_call_argument = InputsHandler.GetIDFromCallArg((CallArgTransactionObjectInput)value);

					if (object_call_argument.Error != null)
						return new SuiResult<string>(null, object_call_argument.Error);

					return new SuiResult<string>(object_call_argument.Result.KeyHex);
				case TransactionObjectInputType.StringArgument:
                    return new SuiResult<string>(Utils.NormalizeSuiAddress(((StringTransactionObjectInput)value).Input));
				default:
                    return new SuiResult<string>(null, new SuiError(0, "Unable to retrieve ID from call argument.", null));
            }
		}

		/// <summary>
		/// Retrieve a shared object reference from a call argument.
		/// </summary>
		/// <param name="arg">The call argument.</param>
		/// <returns>A `SharedObjectRef` object or `null`.</returns>
		private static SharedObjectRef GetSharedObjectInput(ICallArg arg)
		{
			if (arg as ObjectCallArg != null)
			{
				ObjectCallArg object_call_arg = (ObjectCallArg)arg;

				if (object_call_arg.ObjectArg.ObjectRef as SharedObjectRef != null)
                    return (SharedObjectRef)object_call_arg.ObjectArg.ObjectRef;
            }
			return null;
		}
    }
}