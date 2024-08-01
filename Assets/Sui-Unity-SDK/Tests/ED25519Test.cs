//
//  ED25519Test.cs
//  Sui-Unity-SDK
//
//  Copyright (c) 2024 OpenDive
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//

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
        byte[] PrivateKeyBytes = TestValues.PrivateKeyBytes;

        string ExpandedPrivateKeyHex = TestValues.PrivateKeyHex;

        byte[] PrivateKeyBytesInvalid = TestValues.PrivateKeyBytesInvalidLength;

        [Test]
        public void PrivateKeyRandom()
        {
            PrivateKey private_key = new PrivateKey();
            byte[] key_bytes = private_key.KeyBytes;
            Assert.AreEqual(32, key_bytes.Length);
        }

        [Test]
        public void PrivateKeyFromBytesSuccess()
        {
            PrivateKey private_key = new(this.PrivateKeyBytes);

            byte[] actual_key_bytes = private_key.KeyBytes;            
            Assert.AreEqual(32, actual_key_bytes.Length);
            Assert.AreEqual(this.PrivateKeyBytes, actual_key_bytes);

            string actual = private_key.KeyHex;
            Assert.AreEqual(this.ExpandedPrivateKeyHex, actual);
        }

        [Test]
        public void PrivateKeyFromBytesInvalidLength()
        {
            PrivateKey invalid_key = new PrivateKey(this.PrivateKeyBytesInvalid);
            Assert.AreEqual("Invalid key length: 29", invalid_key.Error.Message);
        }

        [Test]
        public void PrivateKeyFromHexStringSuccess()
        {
            string private_key_hex = "0x99da9559e15e913ee9ab2e53e3dfad575da33b49be1125bb922e33494f498828";
            PrivateKey pk1 = new PrivateKey(private_key_hex);

            string expected_private_key_base64 = "mdqVWeFekT7pqy5T49+tV12jO0m+ESW7ki4zSU9JiCg=";

            Assert.AreEqual(32, pk1.KeyBytes.Length);
            Assert.AreEqual(expected_private_key_base64, pk1.KeyBase64);
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
            string private_key_base64 = "mdqVWeFekT7pqy5T49+tV12jO0m+ESW7ki4zSU9JiCg=";
            PrivateKey pk = new PrivateKey(private_key_base64);

            Assert.AreEqual(private_key_base64, pk.KeyBase64);
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
            string private_key_base64 = "mdqVWeFekT7pqy5T49+tV12jO0m+ESW7ki4zSU9JiCg=";
            Assert.AreEqual(private_key_base64, pk1.ToString());
        }

        [Test]
        public void PrivateKeyToHexString()
        {
            PrivateKey private_key = new(PrivateKeyBytes);

            byte[] key_bytes = private_key.KeyBytes;
            Assert.AreEqual(32, key_bytes.Length);

            string actual = private_key.ToHex();
            Assert.AreEqual(this.ExpandedPrivateKeyHex, actual);
        }

        [Test]
        public void PrivateKeyToBase64String()
        {
            PrivateKey pk1 = new PrivateKey("0x99da9559e15e913ee9ab2e53e3dfad575da33b49be1125bb922e33494f498828");
            string private_key_base64 = "mdqVWeFekT7pqy5T49+tV12jO0m+ESW7ki4zSU9JiCg=";
            Assert.AreEqual(private_key_base64, pk1.KeyBase64);
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
            TransactionBlock tx_block = new TransactionBlock();
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
            PublicKey pk1 = new(TestValues.ValidKeyHex);
            PublicKey pk2 = new(TestValues.ValidKeyBase64);
            Assert.AreEqual(pk1, pk2, pk1.KeyHex + "!\n" + pk2.KeyHex + "!");
        }

        [Test]
        public void PublicKeyComparisonFalse()
        {
            PublicKey pk1 = new("0xd77a6cd55073e98d4029b1b0b8bd8d88f45f343dad2732fc9a7965094e635c55");
            PublicKey pk2 = new(TestValues.ValidKeyBase64);
            Assert.AreNotEqual(pk1, pk2, pk1.KeyHex + "!\n" + pk2.KeyHex + "!");
        }

        [Test]
        public void PublicKeySuiPublicKey()
        {
            for(int i = 0; i < TestValues.TestCases.Length; i++)
            {
                string raw_public_key = TestValues.TestCases[i].Item1;
                string sui_public_key = TestValues.TestCases[i].Item2;
                PublicKey public_key = new PublicKey(raw_public_key);
                Assert.AreEqual(sui_public_key, public_key.ToSuiPublicKey());
            }
        }

        [Test]
        public void PublicKeySuiAddress()
        {
            for (int i = 0; i < TestValues.TestCases.Length; i++)
            {
                string raw_public_key = TestValues.TestCases[i].Item1;
                string sui_address = TestValues.TestCases[i].Item3;

                PublicKey public_key = new PublicKey(raw_public_key);
                Assert.AreEqual(sui_address, public_key.ToSuiAddress(), "---- \n" + public_key.ToSuiBytes().ToReadableString() + "\n" + public_key.ToSuiAddress());
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