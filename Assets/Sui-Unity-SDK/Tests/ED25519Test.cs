using NUnit.Framework;
using System;
using System.Text;
using Sui.Cryptography.Ed25519;

namespace Sui.Tests.Cryptography
{
    public class ED25519Test
    {
        byte[] privateKeyBytes = TestValues.PrivateKeyBytes;
        string expPrivateKeyHex = TestValues.PrivateKeyHex;

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
            PrivateKey privateKey = new(privateKeyBytesInvalid);

            Assert.AreEqual(1, 0);
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
            Assert.AreEqual(1, 0);
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
    }
}