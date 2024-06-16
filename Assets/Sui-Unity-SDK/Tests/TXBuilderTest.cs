using NUnit.Framework;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Transactions.Builder;
using Sui.Transactions.Kinds;
using UnityEngine;
using Sui.Utilities;
using Sui.Transactions.Types;
using Sui.Transactions.Types.Arguments;
using Sui.Types;
using Sui.Transactions;

namespace Sui.Tests
{
    public class TXBuilderTest: MonoBehaviour
    {
        string suiAddressHex = "0x0000000000000000000000000000000000000000000000000000000000000002";

        [Test]
        public void SimpleTransactionMoveCallBuildTest()
        {
            var tx = new TransactionBlock();
            AccountAddress suiAddress = AccountAddress.FromHex(suiAddressHex);
            var account = new Account();

            tx.AddMoveCallTx(
                new SuiMoveNormalizedStructType(new SuiStructTag(suiAddress, "display", "new", new ISerializableTag[0])), // TODO: THIS IS A NORMALIZED STRUCT
                new ISerializableTag[] { new StructTag(suiAddress, "capy", "Capy", new ISerializableTag[0]) },
                new SuiTransactionArgument[] { new SuiTransactionArgument(new TransactionBlockInput(0)) } // TODO: We should not use this abstract, this should be a "pure" or an "object.
            );

            tx.SetSenderIfNotSet(account.AccountAddress);
            //var digest = tx.dig
        }
    }
}
