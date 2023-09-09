
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Types.Objects;

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
        public long budget;
        public long price;
        public SuiObjectRef[] payment;
        public AccountAddress owner;

        public GasConfig(string budget, string price, SuiObjectRef[] payment, AccountAddress owner)
        {
            this.budget = long.Parse(budget);
            this.price = long.Parse(price);
            this.payment = payment;
            this.owner = owner;
        }
        public void Serialize(Serialization serializer)
        {
            throw new System.NotImplementedException();
        }
    }
}
