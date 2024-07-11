
using System;
using Sui.Accounts;
using Sui.Transactions.Types;

namespace Sui.Transactions
{
    /// <summary>
    /// Utility class to create different types of transactions 
    /// </summary>
    public class Transactions
    {
        public enum UpgradePolicy
        {
            COMPATIBLE = 0,
            ADDITIVE = 128,
            DEP_ONLY = 192,
        }

        public static MoveCall MoveCall()
        {
            //MoveCall moveCallTx = new MoveCall();
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static TransferObjects TransferObjects(ITransaction[] objects,
            AccountAddress address)
        {
            throw new NotImplementedException();
        }

        public static SplitCoins SplitCoins()
        {
            throw new NotImplementedException();
        }

        public static MergeCoins MergeCoins()
        {
            throw new NotImplementedException();
        }

        public static Publish Publish()
        {
            throw new NotImplementedException();
        }

        public static Upgrade Upgrade()
        {
            throw new NotImplementedException();
        }

        public static MakeMoveVec MakeMoveVec()
        {
            throw new NotImplementedException();
        }
    }
}