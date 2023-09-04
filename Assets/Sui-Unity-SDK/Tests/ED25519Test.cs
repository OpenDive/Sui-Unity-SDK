using NUnit.Framework;
using System;
using Sui.Utilities;
using Sui.Cryptography.Ed25519;
using UnityEngine;

namespace Sui.Tests.Cryptography
{
    public class ED25519Test
    {
        byte[] privateKeyBytes = TestValues.PrivateKeyBytes;
        string expPrivateKeyHex = TestValues.PrivateKeyHex;
        byte[] expSignatureBytes = TestValues.SignatureBytes;

        byte[] privateKeyBytesInvalid = TestValues.PrivateKeyBytesInvalidLength;

        [Test]
        public void PrivateKeyRandom()
        {
            PrivateKey privateKey = PrivateKey.Random();
            byte[] keyBytes = privateKey.KeyBytes;
            Assert.AreEqual(32, keyBytes.Length);
        }

        [Test]
        public void PrivateKeyFromBytesSuccess()
        {
            PrivateKey privateKey = new(privateKeyBytes);

            byte[] keyBytes = privateKey.KeyBytes;
            Assert.AreEqual(32, keyBytes.Length);

            string actual = privateKey.KeyHex;
            Assert.AreEqual(expPrivateKeyHex, actual);
        }

        [Test]
        public void PrivateKeyFromBytesInvalidLength()
        {
            var ex = Assert.Throws<ArgumentException>(() => new PrivateKey(privateKeyBytesInvalid));
            Assert.AreEqual("Invalid key length: \nParameter name: privateKey", ex.Message);
        }

        [Test]
        public void PrivateKeyFromHexStringSuccess()
        {
            Assert.AreEqual(1, 0);
        }

        [Test]
        public void PrivateKeyFromHexStringInvalid()
        {
            Assert.AreEqual(1, 0);
        }

        [Test]
        public void PrivateKeyFromBase58StringSuccess()
        {
            Assert.AreEqual(1, 0);
        }

        [Test]
        public void PrivateKeyFromBase58StringInvalid()
        {
            Assert.AreEqual(1, 0);
        }

        [Test]
        public void PrivateKeyToString()
        {
            Assert.AreEqual(1, 0);
        }

        [Test]
        public void PrivateKeyToHexString()
        {
            PrivateKey privateKey = new(privateKeyBytes);

            byte[] keyBytes = privateKey.KeyBytes;
            Assert.AreEqual(32, keyBytes.Length);

            string actual = privateKey.Hex();
            Assert.AreEqual(expPrivateKeyHex, actual);
        }

        [Test]
        public void PrivateKeyToBase58String()
        {
            Assert.AreEqual(1, 0);
        }

        [Test]
        public void PrivateKeyComparisonTrue()
        {
            Assert.AreEqual(1, 0);
        }

        [Test]
        public void PrivateKeyComparisonFalse()
        {
            Assert.AreEqual(1, 0);
        }

        [Test]
        public void PrivateKeySignature()
        {
            Assert.AreEqual(1, 0);
        }

        /// <summary>
        /// Public key
        /// </summary>
        [Test]
        public void PublicKeyFromBytesSuccess()
        {
            PublicKey publicKey = new(TestValues.ValidKeyBytes);
            Assert.AreEqual(TestValues.ValidKeyBase64, publicKey.ToBase64());
            Assert.AreEqual(TestValues.ValidKeyBase64, publicKey.ToString());
            Assert.AreEqual(TestValues.ValidKeyHex, publicKey.KeyHex);
            Assert.AreEqual(TestValues.ValidKeyBytes, publicKey.KeyBytes);
        }

        [Test]
        public void PublicKeyFromBytesInvalidLength()
        {
            byte[] invalidPublicKey = { 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            var ex = Assert.Throws<ArgumentException>(() => new PublicKey(invalidPublicKey));
            Assert.AreEqual("Invalid key length: \nParameter name: publicKey", ex.Message);
        }

        [Test]
        public void PublicKeyFromHexStringSuccess()
        {
            PublicKey publicKey = new(TestValues.ValidKeyHex);
            Assert.AreEqual(TestValues.ValidKeyBase64, publicKey.ToBase64());
            Assert.AreEqual(TestValues.ValidKeyBase64, publicKey.ToString());
            Assert.AreEqual(TestValues.ValidKeyHex, publicKey.KeyHex);
            Assert.AreEqual(TestValues.ValidKeyBytes, publicKey.KeyBytes);
        }

        [Test]
        public void PublicKeyFromHexStringInvalid()
        {
            string invalidPublicKeyHex = "0x30000000";
            var ex = Assert.Throws<ArgumentException>(() => new PublicKey(invalidPublicKeyHex));
            Assert.AreEqual("Invalid key: \nParameter name: publicKey", ex.Message);
        }

        [Test]
        public void PublicKeyFromBase64StringSuccess()
        {
            PublicKey publicKey = new(TestValues.ValidKeyBase64);
            Assert.AreEqual(TestValues.ValidKeyBase64, publicKey.ToBase64());
            Assert.AreEqual(TestValues.ValidKeyBase64, publicKey.ToString());
            Assert.AreEqual(TestValues.ValidKeyHex, publicKey.KeyHex);
            Assert.AreEqual(TestValues.ValidKeyBytes, publicKey.KeyBytes);
        }

        [Test]
        public void PublicKeyFromBase58StringInvalid()
        {
            Assert.AreEqual(1, 0);
        }

        [Test]
        public void PublicKeyToString()
        {
            PublicKey publicKey = new(TestValues.ValidKeyBase64);
            Assert.AreEqual(TestValues.ValidKeyBase64, publicKey.ToString());
        }

        [Test]
        public void PublicKeyToHexString()
        {
            PublicKey publicKey = new(TestValues.ValidKeyBase64);
            Assert.AreEqual(TestValues.ValidKeyHex, publicKey.KeyHex);
        }

        [Test]
        public void PublicKeyToBase58String()
        {
            PublicKey publicKey = new(TestValues.ValidKeyBase64);
            Assert.AreEqual(TestValues.ValidKeyBase64, publicKey.ToBase64());
        }

        [Test]
        public void PublicKeyComparisonTrue()
        {
            PublicKey publicKeyOne = new(TestValues.ValidKeyHex);
            PublicKey publicKeyTwo = new(TestValues.ValidKeyBase64);
            Assert.AreEqual(publicKeyOne, publicKeyTwo, publicKeyOne.KeyHex + "!\n" + publicKeyTwo.KeyHex + "!");
        }

        [Test]
        public void PublicKeyComparisonFalse()
        {
            Assert.AreEqual(1, 0);
        }
    }
}