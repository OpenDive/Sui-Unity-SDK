using Newtonsoft.Json;
using OpenDive.BCS;
using Sui.Rpc.Client;
using Sui.Transactions;
using Sui.Types;
using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;

namespace Sui.Seal
{
/*
 * SealBridge.cs
 * 
 * Author: viol3
 * Description:
 * This class provides a managed bridge between Unity (C#) and the native Seal encryption/decryption
 * runtime compiled for WebGL or native platforms. It interacts with the Sui blockchain using
 * SuiClient, handling encrypted data storage and retrieval via Move calls under the given package/module.
 */
    public class SealBridge : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="SealBridge"/> class serves as a communication bridge between 
        /// Unity and the native Seal encryption API for managing encrypted Sui transactions.
        /// It handles encryption/decryption requests, transaction construction,
        /// and the coordination of Zero-Knowledge login-based decryption.
        /// 
        /// Uses compiled seal.bundle.js AP which built from @mysten/seal Typescript SDK.
        /// 
        /// Author: viol3
        /// </summary>
        private SuiClient _suiClient;

        private string _packageId = "0xf3dfe70b4916fecaecf7928bb8221031c28d5130c66e8fa7e645ce8785846f91";
        private string _moduleName = "private_data";
        private string _funcName = "store_entry";
        private int _threshold = 2;
        private string[] _serverObjectIds = { "0x73d05d62c18d9374e3ea529e8e0ed6161da1a141a94d3f76ae3fe4e99356db75", "0xf5d14a81a982144ae441cd7d64b09027f116a468bd36e7eca494f750591623c8" };

        private byte[] _encryptedBytes = null;
        private byte[] _decryptedBytes = null;
        private bool _canceled = false;

        /// <summary>
        /// Native Seal SDK external functions (imported through WebGL or native runtime bindings)
        /// </summary>
        [DllImport("__Internal")] private static extern void StartEncrypt(string data, string packageId, string suiAddress, string nonceB64, string suiClientUrl, string serverObjectIdsJson, int threshold);
        [DllImport("__Internal")] private static extern void StartDecrypt(string encryptedBytesBase64, string txBytesBase64, string privateKeyB64, string suiAddress, string packageId, string suiClientUrl, string serverObjectIdsJson);
        [DllImport("__Internal")] private static extern void StartDecryptWithZKLogin(string encryptedBytesBase64, string txBytesBase64, string ephemeralPrivateKeyB64, string inputBytesB64, uint maxEpoch, string suiAddress, string packageId, string suiClientUrl, string serverObjectIdsJson);


        /// <summary>
        /// Singleton instance of <see cref="SealBridge"/> accessible globally.
        /// </summary>
        public static SealBridge Instance;

        /// <summary>
        /// Unity Awake method ensuring singleton lifecycle persistence.
        /// </summary>
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Sets the <see cref="SuiClient"/> reference to be used for building and submitting Sui transactions.
        /// </summary>
        public void SetSuiClient(SuiClient suiClient)
        {
            _suiClient = suiClient;
        }

        /// <summary>
        /// Updates the threshold parameter for Seal encryption and storage policy.
        /// </summary>
        public void SetThreshold(int threshold)
        {
            _threshold = threshold;
        }

        /// <summary>
        /// Sets the object IDs of Seal servers that will be used for policy approval & key retrival processes in encryption/decryption.
        /// </summary>
        public void SetServerObjectIds(params string[] serverObjectIds)
        {
            _serverObjectIds = serverObjectIds;
        }

        /// <summary>
        /// Configures Sui smart contract references including package, module, and function name.
        /// </summary>
        public void SetPackageInformation(string packageId, string moduleName, string funcName)
        {
            _packageId = packageId;
            _moduleName = moduleName;
            _funcName = funcName;
        }

        /// <summary>
        /// Performs encryption on given data for the specified Sui address
        /// by invoking the Seal native encryption function asynchronously.
        /// </summary>
        /// <param name="data">The plaintext data to encrypt.</param>
        /// <param name="suiAddressHex">The Sui address in hexadecimal format.</param>
        /// <returns>A <see cref="TransactionBlock"/> containing encrypted data ready for submission.</returns>
        public async Task<TransactionBlock> Encrypt(string data, string suiAddressHex)
        {
            _canceled = false;
            _encryptedBytes = null;
            byte[] nonceBytes = new byte[32];
            RandomNumberGenerator.Fill(nonceBytes);
            string serverObjectIdsJson = JsonConvert.SerializeObject(new SealServerObjectWrapper { items = _serverObjectIds });
            StartEncrypt(data, _packageId, suiAddressHex, Convert.ToBase64String(nonceBytes), _suiClient.Connection.FULL_NODE, serverObjectIdsJson, _threshold);
            while (_encryptedBytes == null && !_canceled)
            {
                await Task.Yield();
            }
            if (_canceled)
            {
                return null;
            }
            return PrepareTransactionBlock(_encryptedBytes, nonceBytes);
        }

        /// <summary>
        /// Performs decryption using Enoki's Zero-Knowledge Login credentials.
        /// It builds and executes a "seal_approve" transaction as authorization context before decryption.
        /// </summary>
        /// <param name="encryptedBytes">The encrypted data bytes.</param>
        /// <param name="proofInputBytes">The proof input (e.g., zkLogin payload).</param>
        /// <param name="maxEpoch">The maximum epoch allowable for this zkLogin session.</param>
        /// <param name="nonceBytes">The nonce originally used during encryption.</param>
        /// <param name="objectId">The object identifier that contains encrypted data by the user.</param>
        /// <param name="ephemeralPrivateKeyBase64">Ephemeral private key in Base64 used for authentication.</param>
        /// <param name="suiAddressHex">The Sui address in hexadecimal format.</param>
        /// <returns>Decrypted data bytes upon success; otherwise null if canceled or failed.</returns>
        public async Task<byte[]> DecryptWithZKLogin(byte[] encryptedBytes, byte[] proofInputBytes, uint maxEpoch, byte[] nonceBytes, string objectId, string ephemeralPrivateKeyBase64, string suiAddressHex)
        {
            _canceled = false;
            _decryptedBytes = null;
            var tx_block = new TransactionBlock();
            tx_block.AddMoveCallTx
            (
                SuiMoveNormalizedStructType.FromStr($"{_packageId}::{_moduleName}::seal_approve"),
                new SerializableTypeTag[] { },
                new TransactionArgument[]
                {
                    tx_block.AddPure(new OpenDive.BCS.Bytes(Sui.Seal.Utils.CreatePolicyId(suiAddressHex, nonceBytes))),
                    tx_block.AddObjectInput(objectId)
                }
            );
            var txBytes = await tx_block.Build(new BuildOptions(_suiClient, null, true, null));
            string txBytesBase64 = Convert.ToBase64String(txBytes);
            string encryptedBytesBase64 = Convert.ToBase64String(encryptedBytes);
            string serverObjectIdsJson = JsonConvert.SerializeObject(new SealServerObjectWrapper { items = _serverObjectIds });
            string inputBytesB64 = Convert.ToBase64String(proofInputBytes);
            StartDecryptWithZKLogin(encryptedBytesBase64, txBytesBase64, ephemeralPrivateKeyBase64, inputBytesB64, maxEpoch, suiAddressHex, _packageId, _suiClient.Connection.FULL_NODE, serverObjectIdsJson);
            while (_decryptedBytes == null && !_canceled)
            {
                await Task.Yield();
            }
            if (_canceled)
            {
                return null;
            }
            return _decryptedBytes;
        }

        /// <summary>
        /// Performs decryption using a private key without Zero-Knowledge login flow.
        /// Builds an approval transaction and calls the Seal native decryption method.
        /// </summary>
        /// <param name="encryptedBytes">The encrypted bytes data.</param>
        /// <param name="nonceBytes">The nonce used during encryption.</param>
        /// <param name="objectId">The object identifier that contains encrypted data by the user.</param>
        /// <param name="privateKeyBase64">Private key in Base64 format for decryption.</param>
        /// <param name="suiAddressHex">Sui address executing the decryption.</param>
        /// <returns>Decrypted data as bytes, or null if canceled or failed.</returns>
        public async Task<byte[]> Decrypt(byte[] encryptedBytes, byte[] nonceBytes, string objectId, string privateKeyBase64, string suiAddressHex)
        {
            _canceled = false;
            _decryptedBytes = null;
            var tx_block = new TransactionBlock();
            tx_block.AddMoveCallTx
            (
                SuiMoveNormalizedStructType.FromStr($"{_packageId}::{_moduleName}::seal_approve"),
                new SerializableTypeTag[] { },
                new TransactionArgument[]
                {
                    tx_block.AddPure(new OpenDive.BCS.Bytes(Sui.Seal.Utils.CreatePolicyId(suiAddressHex, nonceBytes))),
                    tx_block.AddObjectInput(objectId)
                }
            );
            var txBytes = await tx_block.Build(new BuildOptions(_suiClient, null, true, null));
            string txBytesBase64 = Convert.ToBase64String(txBytes);
            string encryptedBytesBase64 = Convert.ToBase64String(encryptedBytes);
            string serverObjectIdsJson = JsonConvert.SerializeObject(new SealServerObjectWrapper { items = _serverObjectIds });
            StartDecrypt(encryptedBytesBase64, txBytesBase64, privateKeyBase64, suiAddressHex, _packageId, _suiClient.Connection.FULL_NODE, serverObjectIdsJson);
            while (_decryptedBytes == null && !_canceled)
            {
                await Task.Yield();
            }
            if (_canceled)
            {
                return null;
            }
            return _decryptedBytes;
        }

        /// <summary>
        /// Callback invoked from native runtime when encryption completes successfully or fails.
        /// </summary>
        /// <param name="encryptedObjectBase64">Encrypted payload or error in prefixed format.</param>
        void OnEncryptionCompleted(string encryptedObjectBase64)
        {
            if (encryptedObjectBase64.StartsWith("SEAL_ERROR|"))
            {
                _canceled = true;
                throw new Exception(encryptedObjectBase64.Split('|')[1]);
            }
            _encryptedBytes = Convert.FromBase64String(encryptedObjectBase64);
        }

        /// <summary>
        /// Callback invoked from native runtime when decryption completes successfully or fails.
        /// </summary>
        /// <param name="decryptedObjectBase64">Decrypted payload or error prefixed with "SEAL_ERROR|".</param>
        void OnDecryptionCompleted(string decryptedObjectBase64)
        {
            if (decryptedObjectBase64.StartsWith("SEAL_ERROR|"))
            {
                _canceled = true;
                throw new Exception(decryptedObjectBase64.Split('|')[1]);
            }
            _decryptedBytes = Convert.FromBase64String(decryptedObjectBase64);
        }

        /// <summary>
        /// Prepares a Sui transaction block to store encrypted Seal data on-chain.
        /// </summary>
        /// <param name="encryptedBytes">The encrypted payload bytes.</param>
        /// <param name="nonceBytes">The nonce bytes used during encryption.</param>
        /// <returns>Configured <see cref="TransactionBlock"/> ready for signing and execution.</returns>
        TransactionBlock PrepareTransactionBlock(byte[] encryptedBytes, byte[] nonceBytes)
        {
            TransactionBlock tx_block = new TransactionBlock();
            tx_block.AddMoveCallTx
            (
                SuiMoveNormalizedStructType.FromStr($"{_packageId}::{_moduleName}::{_funcName}"),
                new SerializableTypeTag[] { },
                new TransactionArgument[]
                {
                tx_block.AddPure(new OpenDive.BCS.Bytes(nonceBytes)),
                tx_block.AddPure(new OpenDive.BCS.Bytes(encryptedBytes))
                }
            );
            return tx_block;
        }
    }

    /// <summary>
    /// Serializable wrapper class representing Seal server object metadata.
    /// Used for JSON serialization in calls to native Seal SDK.
    /// 
    /// Author: viol3
    /// </summary>
    [System.Serializable]
    public class SealServerObjectWrapper
    {
        /// <summary>
        /// The IDs of Seal server objects in the form of Sui Object IDs.
        /// </summary>
        public string[] items;
        public SealServerObjectWrapper()
        {

        }
    }
}

