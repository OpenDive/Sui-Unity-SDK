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

        public TransactionBlockDataBuilder(
            AccountAddress sender
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