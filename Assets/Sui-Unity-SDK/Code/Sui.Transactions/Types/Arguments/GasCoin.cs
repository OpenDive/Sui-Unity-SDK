using OpenDive.BCS;

namespace Sui.Transactions.Types.Arguments
{
    public class GasCoin : ITransactionArgument
    {
        public ITransactionArgument.Type Kind {
            get => ITransactionArgument.Type.GasCoin;
        }

        public GasCoin()
        {

        }

        public void Serialize(Serialization serializer)
        {
            throw new System.NotImplementedException();
        }
    }
}