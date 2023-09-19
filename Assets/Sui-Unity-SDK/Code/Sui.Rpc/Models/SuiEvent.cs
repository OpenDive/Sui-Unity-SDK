using System.Numerics;
using Sui.Accounts;

namespace Sui.Rpc.Models
{
    // TODO: Clean up
    public class SuiEvent
    {
        public string Bcs { get; set; }

        public object Id { get; set; }

        public ObjectId PackageId { get; set; }

        public object ParsedJson { get; set; }

        //public SuiAddress Sender { get; set; } // TODO: Look into this
        public AccountAddress Sender { get; set; }

        public BigInteger TimestampMs { get; set; }

        public string TransactionModule { get; set; }

        public string Type { get; set; }
    }
}