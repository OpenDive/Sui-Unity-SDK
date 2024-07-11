using System;
using Sui.Accounts;
using Sui.Rpc.Models;
using Sui.Types;

namespace Sui.Transactions
{
	public class InputsHandler
	{
		static public bool isMutableSharedObjectInput(ICallArg arg)
		{
			SharedObjectRef sharedObject = GetSharedObjectInput(arg);

			if (sharedObject != null)
			{
				return sharedObject.mutable;
			}
			return false;
		}

		public static ObjectArg SharedObjectRef(SharedObjectRef shared_object_ref)
        {
			return new ObjectArg
			(
				ObjectRefType.Shared,
				new SharedObjectRef
				(
					AccountAddress.FromHex(NormalizedTypeConverter.NormalizeSuiAddress(shared_object_ref.ObjectID.ToHex())),
					shared_object_ref.InitialSharedVersion,
					shared_object_ref.mutable
				)
			);
        }

		public static string GetIDFromCallArg(CallArgTransactionObjectInput arg)
        {
			switch(arg.Input.Type)
            {
				case ObjectRefType.ImmOrOwned:
					AccountAddress object_id_immutable = ((Sui.Types.SuiObjectRef)arg.Input.ObjectRef).ObjectID;
					return NormalizedTypeConverter.NormalizeSuiAddress(object_id_immutable.ToHex());
				case ObjectRefType.Shared:
					AccountAddress object_id_shared = ((Sui.Types.SharedObjectRef)arg.Input.ObjectRef).ObjectID;
					return NormalizedTypeConverter.NormalizeSuiAddress(object_id_shared.ToHex());
				default:
					throw new Exception("Not Implemented");
			}
        }

		public static string GetIDFromCallArg(ITransactionObjectInput value)
        {
			if (value.Type == TransactionObjectInputType.objectCallArgument)
				return GetIDFromCallArg((CallArgTransactionObjectInput)value);

			if (value.Type == TransactionObjectInputType.stringArgument)
				return NormalizedTypeConverter.NormalizeSuiAddress(((StringTransactionObjectInput)value).Input);

			throw new Exception("Not Implemented");
		}

		private static SharedObjectRef GetSharedObjectInput(ICallArg arg)
		{
			if (arg as ObjectCallArg != null)
			{
				ObjectCallArg objectCallArg = (ObjectCallArg)arg;
				if (objectCallArg.ObjectArg.ObjectRef as SharedObjectRef != null)
				{
					return (SharedObjectRef)objectCallArg.ObjectArg.ObjectRef;
				}
            }
			return null;
		}
    }
}

