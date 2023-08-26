namespace Sui.Utils
{
    /// <summary>
    /// Utility class to create static instances of different encoders
    /// </summary>
    public static class Encoders
    {
        private static readonly Base58Encoder _base58 = new();
        public static DataEncoder Base58 => _base58;
    }
}
