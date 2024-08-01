using NUnit.Framework;

namespace Sui.Tests.Cryptography
{
    public class UtilitiesTest
    {
        [Test]
        public void IsValidEd25519HexKey()
        {
            string validPkHex = "0x99da9559e15e913ee9ab2e53e3dfad575da33b49be1125bb922e33494f498828";
            bool isValidHex = Utilities.Utils.IsValidHexKey(validPkHex);
            //bool isInvalidBase64 = Utilities.Utils.IsBase64String(invalidPkHex);

            Assert.AreEqual(true, isValidHex);
        }

        [Test]
        public void IsInvalidEd25519HexKey()
        {
            string invalidPkHex1 = "0x99da9559e15e913ee9ab2e53e3dfad575da33b49be1125bb92";
            //string pkBase64 = "mdqVWeFekT7pqy5T49+tV12jO0m+ESW7ki4zSU9JiCg=";
            string invalidPkHex2 = "0x99da9559e15e913ee9ab2e53e3dfad5!5da33b49be1125bb922e33494f498828";

            bool isValidHex1 = Utilities.Utils.IsValidHexKey(invalidPkHex1);
            Assert.AreEqual(false, isValidHex1);
            bool isValidHex2 = Utilities.Utils.IsValidHexKey(invalidPkHex2);
            Assert.AreEqual(false, isValidHex2);
        }
    }
}