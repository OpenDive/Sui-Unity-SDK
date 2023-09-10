
using OpenDive.BCS;
using Sui.Accounts;
using Sui.BCS;

namespace Sui.Transactions.Builder
{
    //const GasConfig = object({
    //	budget: optional(StringEncodedBigint),
    //	price: optional(StringEncodedBigint),
    //	payment: optional(array(SuiObjectRef)),
    //	owner: optional(string()),
    //});
    public class GasConfig : ISerializable
    {
        public long? budget;
        public long? price;
        public SuiObjectRef[] payment;
        public AccountAddress owner;

        public GasConfig(string budget = null, string price = null, SuiObjectRef[] payment = null, AccountAddress owner = null)
        {
            this.budget = budget != null ? long.Parse(budget) : null;
            this.price = price != null ? long.Parse(price) : null;
            this.payment = payment;
            this.owner = owner;
        }
        public void Serialize(Serialization serializer)
        {
            throw new System.NotImplementedException();
        }
    }
}
