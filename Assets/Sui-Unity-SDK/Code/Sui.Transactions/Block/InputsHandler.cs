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

		private static SharedObjectRef GetSharedObjectInput(ICallArg arg)
		{
			if (arg as ObjectCallArg != null)
			{
				ObjectCallArg objectCallArg = (ObjectCallArg)arg;
				if (objectCallArg.ObjectArg as SharedObjectRef != null)
				{
					return (SharedObjectRef)objectCallArg.ObjectArg;
				}
            }
			return null;
		}
    }
}

