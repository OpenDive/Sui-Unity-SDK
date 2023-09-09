using OpenDive.BCS;

namespace Sui.Types.Objects
{
    public class SuiObjectRef : ISerializable
    {
        ///** Base64 string representing the object digest */
        //digest: string (),
        ///** Hex code as string representing the object id */
        //objectId: string (),
        ///** Object version */
        //version: union([number(), string()]),

        string digest;
        string objectId;
        string version;

        public SuiObjectRef(string digest, string objectId, string version)
        {
            this.digest = digest;
            this.objectId = objectId;
            this.version = version;
        }

        public void Serialize(Serialization serializer)
        {
            throw new System.NotImplementedException();
        }
    }

    public class SuiGasData : ISerializable
    {
        // payment: array(SuiObjectRef),
        // /** Gas Object's owner */
        // owner: string (),
        // price: string (),
        // budget: string (),

        SuiObjectRef[] payment;
        string owner;
        string price;
        string budget;

        public SuiGasData(SuiObjectRef[] payment, string owner, string price, string budget)
        {
            this.payment = payment;
            this.owner = owner;
            this.price = price;
            this.budget = budget;
        }

        public void Serialize(Serialization serializer)
        {
            throw new System.NotImplementedException();
        }
    }
}