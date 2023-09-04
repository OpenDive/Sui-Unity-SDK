using NUnit.Framework;
using System;
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
            Assert.AreEqual(1, 0);
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
            Assert.AreEqual(1, 0);
        }

        [Test]
        public void PublicKeyFromHexStringInvalid()
        {
            Assert.AreEqual(1, 0);
        }

        [Test]
        public void PublicKeyFromBase58StringSuccess()
        {
            Assert.AreEqual(1, 0);
        }

        [Test]
        public void PublicKeyFromBase58StringInvalid()
        {
            Assert.AreEqual(1, 0);
        }

        [Test]
        public void PublicKeyToString()
        {
            Assert.AreEqual(1, 0);
        }

        [Test]
        public void PublicKeyToHexString()
        {
            PrivateKey privateKey = new(privateKeyBytes);

            byte[] keyBytes = privateKey.KeyBytes;
            Assert.AreEqual(32, keyBytes.Length);

            string actual = privateKey.Hex();
            Assert.AreEqual(expPrivateKeyHex, actual);
        }

        [Test]
        public void PublicKeyToBase58String()
        {
            Assert.AreEqual(1, 0);
        }

        [Test]
        public void PublicKeyComparisonTrue()
        {
            Assert.AreEqual(1, 0);
        }

        [Test]
        public void PublicKeyComparisonFalse()
        {
            Assert.AreEqual(1, 0);
        }
    }
}