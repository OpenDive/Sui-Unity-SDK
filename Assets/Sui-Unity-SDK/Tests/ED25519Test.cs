using NUnit.Framework;
using Sui.Rpc.Client;
using Sui.Utilities;
using Sui.Cryptography.Ed25519;
using Sui.Accounts;
using Sui.Rpc;
using UnityEngine.TestTools;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using Sui.Transactions;
using Sui.Cryptography;
using NBitcoin.DataEncoders;

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
            PrivateKey privateKey = new PrivateKey();
            byte[] keyBytes = privateKey.KeyBytes;
            Assert.AreEqual(32, keyBytes.Length);
        }

        [Test]
        public void PrivateKeyFromBytesSuccess()
        {
            PrivateKey privateKey = new(privateKeyBytes);

            byte[] ActualKeyBytes = privateKey.KeyBytes;            
            Assert.AreEqual(32, ActualKeyBytes.Length);
            Assert.AreEqual(privateKeyBytes, ActualKeyBytes);

            string actual = privateKey.KeyHex;
            Assert.AreEqual(expPrivateKeyHex, actual);
        }

        [Test]
        public void PrivateKeyFromBytesInvalidLength()
        {
            PrivateKey invalid_key = new PrivateKey(privateKeyBytesInvalid);
            Assert.AreEqual("Invalid key length: 29", invalid_key.Error.Message);
        }

        [Test]
        public void PrivateKeyFromHexStringSuccess()
        {
            string pkHex = "0x99da9559e15e913ee9ab2e53e3dfad575da33b49be1125bb922e33494f498828";
            PrivateKey pk1 = new PrivateKey(pkHex);

            string expectedPkBase64 = "mdqVWeFekT7pqy5T49+tV12jO0m+ESW7ki4zSU9JiCg=";

            Assert.AreEqual(32, pk1.KeyBytes.Length);
            Assert.AreEqual(expectedPkBase64, pk1.KeyBase64);
        }

        [Test]
        public void PrivateKeyFromHexStringInvalid()
        {
            string invalid_pk_hex = "0x99da9559e15e913ee9ab2e53e3dfad575da3349be1125bb922e33494f49882!";
            PrivateKey invalid_key = new PrivateKey(invalid_pk_hex);
            Assert.AreEqual("Invalid key.", invalid_key.Error.Message);
        }

        [Test]
        public void PrivateKeyFromBase64tringSuccess()
        {
            //string pkHex = "0x99da9559e15e913ee9ab2e53e3dfad575da33b49be1125bb922e33494f498828";
            string pkBase64 = "mdqVWeFekT7pqy5T49+tV12jO0m+ESW7ki4zSU9JiCg=";
            byte[] pkBytes = { 153, 218, 149, 89, 225, 94, 145, 62, 233, 171, 46, 83, 227, 223, 173, 87, 93, 163, 59, 73, 190, 17, 37, 187, 146, 46, 51, 73, 79, 73, 136, 40 };
            //string publicKey = "Gy9JCW4+Xb0Pz6nAwM2S2as7IVRLNNXdSmXZi4eLmSI=";
            PrivateKey pk = new PrivateKey(pkBase64);

            //Assert.AreEqual(1, 0, pk.PublicKey().ToBase64());
            Assert.AreEqual(pkBase64, pk.KeyBase64);
        }

        [Test]
        public void PrivateKeyFromBase64StringInvalid()
        {
            string pk_base64_invalid = "mdqVWeFekT7pqy5T49+tV12jO0m+ESW7kSU9JiCg=";

            PrivateKey invalid_key = new PrivateKey(pk_base64_invalid);
            Assert.AreEqual("Invalid key.", invalid_key.Error.Message);
        }

        [Test]
        public void PrivateKeyToString()
        {
            PrivateKey pk1 = new PrivateKey("0x99da9559e15e913ee9ab2e53e3dfad575da33b49be1125bb922e33494f498828");
            string pkBase64 = "mdqVWeFekT7pqy5T49+tV12jO0m+ESW7ki4zSU9JiCg=";
            Assert.AreEqual(pkBase64, pk1.ToString());
        }

        [Test]
        public void PrivateKeyToHexString()
        {
            PrivateKey privateKey = new(privateKeyBytes);

            byte[] keyBytes = privateKey.KeyBytes;
            Assert.AreEqual(32, keyBytes.Length);

            string actual = privateKey.ToHex();
            Assert.AreEqual(expPrivateKeyHex, actual);
        }

        [Test]
        public void PrivateKeyToBase64String()
        {
            PrivateKey pk1 = new PrivateKey("0x99da9559e15e913ee9ab2e53e3dfad575da33b49be1125bb922e33494f498828");
            string pkBase64 = "mdqVWeFekT7pqy5T49+tV12jO0m+ESW7ki4zSU9JiCg=";
            Assert.AreEqual(pkBase64, pk1.KeyBase64);
        }

        [Test]
        public void PrivateKeyComparisonTrue()
        {
            PrivateKey pk1 = new PrivateKey("0x99da9559e15e913ee9ab2e53e3dfad575da33b49be1125bb922e33494f498828");
            PrivateKey pk2 = new PrivateKey("mdqVWeFekT7pqy5T49+tV12jO0m+ESW7ki4zSU9JiCg=");
            Assert.AreEqual(pk1, pk2);
        }

        [Test]
        public void PrivateKeyComparisonFalse()
        {
            PrivateKey pk1 = new PrivateKey("mdqVWeFekT7pqy5T49+tV12jO0m+ESW7ki4zSU9JiCg=");
            PrivateKey pk2 = new PrivateKey("Uz39UFseB/B38iBwjesIU1JZxY6y+TRL9P84JFw41W4=");
            Assert.AreNotEqual(pk1, pk2);
        }

        [Test]
        public async Task TransactionSigningSuccess()
        {
            Account account = new Account();
            Transactions.TransactionBlock tx_block = new Transactions.TransactionBlock();
            SuiClient client = new SuiClient(Constants.LocalnetConnection);

            tx_block.SetSender(account.SuiAddress());
            tx_block.SetGasPrice(5);
            tx_block.SetGasBudget(100);
            byte[] digest = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Base58Encoder base58Encoder = new Base58Encoder();
            tx_block.SetGasPayment(new Types.SuiObjectRef[] { new Types.SuiObjectRef
            (
                AccountAddress.FromHex(string.Format("{0:0}", new System.Random().NextDouble() * 100000).PadLeft(64, '0')),
                new System.Random().Next() * 10000,
                base58Encoder.EncodeData(digest)
            ) });
            byte[] bytes = await tx_block.Build(new BuildOptions(client));

            if (tx_block.Error != null)
                Assert.Fail(tx_block.Error.Message);

            SignatureBase serialized_signature = account.SignTransactionBlock(bytes);
            Assert.IsTrue(account.VerifyTransactionBlock(bytes, serialized_signature));
        }

        /// <summary>
        /// Public key
        /// </summary>
        [Test]
        public void PublicKeyFromBytesSuccess()
        {
            PublicKey public_key = new(TestValues.ValidKeyBytes);
            Assert.AreEqual(TestValues.ValidKeyBase64, public_key.ToBase64());
            Assert.AreEqual(TestValues.ValidKeyBase64, public_key.ToString());
            Assert.AreEqual(TestValues.ValidKeyHex, public_key.KeyHex);
            Assert.AreEqual(TestValues.ValidKeyBytes, public_key.KeyBytes);
        }

        [Test]
        public void PublicKeyFromBytesInvalidLength()
        {
            byte[] invalid_public_key = { 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            PublicKey invalid_key = new PublicKey(invalid_public_key);
            Assert.AreEqual("Invalid key length: 33", invalid_key.Error.Message);
        }

        [Test]
        public void PublicKeyFromHexStringSuccess()
        {
            PublicKey public_key = new(TestValues.ValidKeyHex);
            Assert.AreEqual(TestValues.ValidKeyBase64, public_key.ToBase64());
            Assert.AreEqual(TestValues.ValidKeyBase64, public_key.ToString());
            Assert.AreEqual(TestValues.ValidKeyHex, public_key.KeyHex);
            Assert.AreEqual(TestValues.ValidKeyBytes, public_key.KeyBytes);
        }

        [Test]
        public void PublicKeyFromHexStringInvalid()
        {
            string invalid_public_key_hex = "0x30000000";
            PublicKey invalid_key = new PublicKey(invalid_public_key_hex);
            Assert.AreEqual("Invalid key.", invalid_key.Error.Message);
        }

        [Test]
        public void PublicKeyFromBase64StringSuccess()
        {
            PublicKey public_key = new(TestValues.ValidKeyBase64);
            Assert.AreEqual(TestValues.ValidKeyBase64, public_key.ToBase64());
            Assert.AreEqual(TestValues.ValidKeyBase64, public_key.ToString());
            Assert.AreEqual(TestValues.ValidKeyHex, public_key.KeyHex);
            Assert.AreEqual(TestValues.ValidKeyBytes, public_key.KeyBytes);
        }

        [Test]
        public void PublicKeyFromBase64StringInvalid()
        {
            string invalid_public_key_hex = "Uz39UFseB/B38iBwjesIU1JZxY6y+TRL9P84JFw414=";
            PublicKey invalid_key = new PublicKey(invalid_public_key_hex);
            Assert.AreEqual("Invalid key.", invalid_key.Error.Message);
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
        public void PublicKeyToBase64String()
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
            PublicKey publicKeyOne = new("0xd77a6cd55073e98d4029b1b0b8bd8d88f45f343dad2732fc9a7965094e635c55");
            PublicKey publicKeyTwo = new(TestValues.ValidKeyBase64);
            Assert.AreNotEqual(publicKeyOne, publicKeyTwo, publicKeyOne.KeyHex + "!\n" + publicKeyTwo.KeyHex + "!");
        }

        [Test]
        public void PublicKeySuiPublicKey()
        {
            for(int i = 0; i < TestValues.TestCases.Length; i++)
            {
                string rawPublicKey = TestValues.TestCases[i].Item1;
                string suiPublicKey = TestValues.TestCases[i].Item2;
                PublicKey publicKey = new PublicKey(rawPublicKey);
                Assert.AreEqual(suiPublicKey, publicKey.ToSuiPublicKey());
            }
        }

        [Test]
        public void PublicKeySuiAddress()
        {
            for (int i = 0; i < TestValues.TestCases.Length; i++)
            {
                string rawPublicKey = TestValues.TestCases[i].Item1;
                string suiAddress = TestValues.TestCases[i].Item3;

                PublicKey publicKey = new PublicKey(rawPublicKey);
                Assert.AreEqual(suiAddress, publicKey.ToSuiAddress(), "---- \n" + publicKey.ToSuiBytes().ToReadableString() + "\n" + publicKey.ToSuiAddress());
            }
        }

        [Test]
        public void PrivateKeySignTest()
        {
            Account account = new Account();
            byte[] data = System.Text.Encoding.UTF8.GetBytes("hello world");
            SignatureBase signature = account.Sign(data);

            Assert.IsTrue(account.Verify(data, signature));
        }

        [UnityTest]
        public IEnumerator TransactionBlockSigningTest()
        {
            Account account = new Account();
            TransactionBlock tx_block = new TransactionBlock();
            SuiClient client = new SuiClient(Constants.LocalnetConnection);

            tx_block.SetSender(account.SuiAddress());
            tx_block.SetGasPrice(5);
            tx_block.SetGasBudget(100);

            byte[] digest = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Base58Encoder encoder = new Base58Encoder();
            tx_block.SetGasPayment
            (
                new Types.SuiObjectRef[]
                {
                    new Types.SuiObjectRef
                    (
                        string.Format("{0:0}", new System.Random().NextDouble() * 100000).PadLeft(64, '0'),
                        $"{string.Format("{0:0}", new System.Random().NextDouble() * 100000).PadLeft(64, '0')}",
                        encoder.EncodeData(digest)
                    )
                }
            );

            Task<byte[]> result = tx_block.Build(new BuildOptions(client));
            yield return new WaitUntil(() => result.IsCompleted);

            if (tx_block.Error != null)
                Assert.Fail(tx_block.Error.Message);

            SignatureBase serialized_signature = account.SignTransactionBlock(result.Result);

            Assert.IsTrue(account.VerifyTransactionBlock(result.Result, serialized_signature));
        }
    }
}