using System.Text;
using NUnit.Framework;
using UnityEngine;
using Sui.Utilities;

namespace Sui.Tests.Cryptography
{
    public class UtilitiesTest
    {
        [Test]
        public void IsValidEd25519HexKey()
        {
            string validPkHex = "0x99da9559e15e913ee9ab2e53e3dfad575da33b49be1125bb922e33494f498828";
            bool isValidHex = Utilities.Utils.IsValidEd25519HexKey(validPkHex);
            //bool isInvalidBase64 = Utilities.Utils.IsBase64String(invalidPkHex);

            Assert.AreEqual(true, isValidHex);
        }

        [Test]
        public void IsInvalidEd25519HexKey()
        {
            string invalidPkHex1 = "0x99da9559e15e913ee9ab2e53e3dfad575da33b49be1125bb92";
            //string pkBase64 = "mdqVWeFekT7pqy5T49+tV12jO0m+ESW7ki4zSU9JiCg=";
            string invalidPkHex2 = "0x99da9559e15e913ee9ab2e53e3dfad5!5da33b49be1125bb922e33494f498828";

            bool isValidHex1 = Utilities.Utils.IsValidEd25519HexKey(invalidPkHex1);
            Assert.AreEqual(false, isValidHex1);
            bool isValidHex2 = Utilities.Utils.IsValidEd25519HexKey(invalidPkHex2);
            Assert.AreEqual(false, isValidHex2);
        }

        [Test]
        public void HashTypedData()
        {
            string typeTag = "TransactionData::";
            byte[] expected = new byte[]{ 84, 114, 97, 110, 115, 97, 99, 116, 105, 111, 110, 68, 97, 116, 97, 58, 58 };
            byte[] bytes = Encoding.ASCII.GetBytes(typeTag);
            Debug.Log(bytes.ToReadableString());
            Debug.Log(Encoding.UTF8.GetBytes(typeTag).ToReadableString());
        }
    }
}