using System;
using Sui.Accounts;

namespace Sui.Transactions.Builder
{
    public class TransactionBlockDataBuilder
    {
        //   version = 1 as const;
        //   sender?: string;
	    //expiration?: TransactionExpiration;
	    //gasConfig: GasConfig;
	    //inputs: TransactionBlockInput[];
	    //transactions: TransactionType[];

        public int Version { get => 1;  }
        public AccountAddress Sender { get; set; }

        /// <summary>
        /// TODO: Implement
        /// https://github.com/MystenLabs/sui/blob/3253d9a3c629fb142dbf492c22afca14114d1df8/sdk/typescript/src/builder/TransactionBlockData.ts#L156
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="gasConfig"></param>
        public TransactionBlockDataBuilder(
            AccountAddress sender,
            GasConfig gasConfig

            )
        {

        }

        public static TransactionBlockDataBuilder FromKindBytes(byte[] bytes)
        {
            //return new TransactionBlockDataBuilder();
            throw new NotImplementedException();
        }

        public static TransactionBlockDataBuilder FromBytes(byte[] bytes)
        {
            //return new TransactionBlockDataBuilder();
            throw new NotImplementedException();
        }

        //public static TransactionBlockDataBuilder Restore(SerializedTransactionDataBuilder data)
        //{
        //    throw new NotImplementedException();
        //}


            public static string GetDigestFromBytes(byte[] bytes)
        {
            //return new TransactionBlockDataBuilder();
            throw new NotImplementedException();
        }
    }
}