namespace Sui.Cryptography
{
    public interface IPrivateKey
    {
        public byte[] KeyBytes { get; }
        public string Hex();
        public string Base64();
        public ISignature Sign(byte[] data);
        //public void Serialize(Serializer serializer);
    }
}
