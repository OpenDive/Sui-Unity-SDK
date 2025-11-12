using Chaos.NaCl;
using Newtonsoft.Json;
using OpenDive.Utils.Jwt;
using Sui.Accounts;
using Sui.Cryptography;
using Sui.Rpc;
using Sui.Rpc.Client;
using Sui.Rpc.Models;
using Sui.Transactions;
using Sui.Utilities;
using Sui.ZKLogin.Enoki.Utils;
using Sui.ZKLogin.Utils;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Sui.ZKLogin.Enoki
{
    /// <summary>
    /// Manages ZK Login authentication and integration with the Sui blockchain network.
    /// Handles client initialization, ephemeral account management, Google OAuth authentication, Signing and Executing Transactions with ZKLogin Account.
    /// </summary>
    public static class EnokiZKLogin
    {
        private static string _network;
        private static string _enokiPublicKey;

        private static SuiClient _client;
        private static Account _ephemeralAccount;
        private static EnokiZKLoginUserResponse _zkLoginUser;
        private static EnokiZKPResponse _zkpResponse;
        private static int _maxEpoch;

        private static IJwtFetcher _jwtFetcher;

        private static bool _inited = false;

        /// <summary>
        /// Initializes the ZKLoginManager with the specified configuration parameters.
        /// </summary>
        /// <param name="network">The Sui network to connect to (mainnet, testnet, devnet, or localnet).</param>
        /// <param name="enokiPublicKey">The Enoki public key for ZK Login authentication => https://portal.enoki.mystenlabs.com/</param>

        public static void Init(string network, string enokiPublicKey)
        {
            if (_inited)
            {
                Debug.Log("ZKLoginManager already inited");
                return;
            }
            _network = network;
            _enokiPublicKey = enokiPublicKey;
            _ephemeralAccount = new Account();
            CreateSuiClient();
            _inited = true;
            Debug.Log("ZKLoginManager inited succesfully.");
        }

        /// <summary>
        /// Creates and configures a SuiClient instance based on the specified network.
        /// If a client already exists, logs a message and returns without creating a new one.
        /// Defaults to testnet if the specified network is unknown.
        /// </summary>
        static void CreateSuiClient()
        {
            if (_client != null)
            {
                Debug.Log("Sui client already created.");
                return;
            }
            switch (_network)
            {
                case "mainnet":
                    _client = new SuiClient(Constants.MainnetConnection);
                    break;
                case "testnet":
                    _client = new SuiClient(Constants.TestnetConnection);
                    break;
                case "devnet":
                    _client = new SuiClient(Constants.DevnetConnection);
                    break;
                case "localnet":
                    _client = new SuiClient(Constants.LocalnetConnection);
                    break;
                default:
                    Debug.LogWarning($"Unknown network:{_network}, creating client with testnet...");
                    _client = new SuiClient(Constants.TestnetConnection);
                    break;
            }
        }

        /// <summary>
        /// Sets a custom SuiClient instance for the ZKLogin manager.
        /// </summary>
        /// <param name="client">The SuiClient instance to use.</param>
        public static void SetClient(SuiClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Gets the current SuiClient instance being used by the ZKLogin manager.
        /// </summary>
        /// <returns>The active SuiClient instance.</returns>
        public static SuiClient GetClient() 
        { 
            return _client; 
        }

        /// <summary>
        /// Retrieves the Sui blockchain address associated with the currently logged-in ZKLogin user.
        /// </summary>
        /// <returns>The Sui address as a string, or null if no user is logged in.</returns>
        public static string GetSuiAddress()
        {
            if (_zkLoginUser == null)
            {
                Debug.LogError("You need to login or load ZKLoginUser data to get sui address.");
                return null;
            }
            return _zkLoginUser.data.address;
        }

        /// <summary>
        /// Checks whether the ZKLogin manager has been initialized.
        /// </summary>
        /// <returns>True if initialized, false otherwise.</returns>
        public static bool IsInited()
        {
            return _inited;
        }

        /// <summary>
        /// Checks whether a user is currently logged in with ZKLogin.
        /// </summary>
        /// <returns>True if a user is logged in (ZKP response exists), false otherwise.</returns>
        public static bool IsLogged()
        {
            return _zkpResponse != null;
        }

        /// <summary>
        /// Gets the current ZKLogin user data.
        /// </summary>
        /// <returns>The EnokiZKLoginUser object containing user information.</returns>
        public static EnokiZKLoginUserResponse GetZKLoginUser()
        {
            return _zkLoginUser;
        }

        /// <summary>
        /// Gets the current Zero-Knowledge Proof (ZKP) response.
        /// </summary>
        /// <returns>The EnokiZKPResponse object containing ZKP data.</returns>
        public static EnokiZKPResponse GetZKP()
        {
            return _zkpResponse;
        }

        /// <summary>
        /// Gets the ephemeral account used for temporary key generation.
        /// </summary>
        /// <returns>The ephemeral Account instance.</returns>
        public static Account GetEphemeralAccount()
        { 
            return _ephemeralAccount; 
        }

        /// <summary>
        /// Gets the maximum epoch value for the current ZKLogin session.
        /// </summary>
        /// <returns>The maximum epoch as an integer.</returns>
        public static int GetMaxEpoch()
        { 
            return _maxEpoch; 
        }

        /// <summary>
        /// Gets a saveable data that can be used to save user information to auto-login after restart game.
        /// </summary>
        /// <returns>The EnokiZKLoginSaveableData object containing login information.</returns>
        public static EnokiZKLoginSaveableData GetSaveableData()
        {
            EnokiZKLoginSaveableData result = new EnokiZKLoginSaveableData();
            result.loginUserResponse = _zkLoginUser;
            result.zkpResponse = _zkpResponse;
            result.ephemeralPrivateKeyHex = _ephemeralAccount.PrivateKey.ToHex();
            result.maxEpoch = _maxEpoch;
            return result;
        }

        /// <summary>
        /// Loads a previously obtained ZKP response into the manager.
        /// </summary>
        /// <param name="zkpResponse">The ZKP response to load.</param>
        public static void LoadZKPResponse(EnokiZKPResponse zkpResponse)
        {
            _zkpResponse = zkpResponse;
        }

        /// <summary>
        /// Loads previously obtained ZKLogin user data into the manager.
        /// </summary>
        /// <param name="zkLoginUser">The ZKLogin user data to load.</param>
        public static void LoadZKLoginUser(EnokiZKLoginUserResponse zkLoginUser)
        {
            _zkLoginUser = zkLoginUser;
        }

        /// <summary>
        /// Loads a previously created ephemeral account into the manager.
        /// </summary>
        /// <param name="ephemeralAccount">The ephemeral account to load.</param>
        public static void LoadEphemeralKey(Account ephemeralAccount)
        {
            _ephemeralAccount = ephemeralAccount;
        }

        /// <summary>
        /// Loads a previously obtained maximum epoch value into the manager.
        /// </summary>
        /// <param name="maxEpoch">The maximum epoch value to load.</param>
        public static void LoadMaxEpoch(int maxEpoch)
        {
            _maxEpoch = maxEpoch;
        }

        /// <summary>
        /// Loads a custom JWT fetcher implementation for handling JWT token retrieval.
        /// </summary>
        /// <param name="jwtFetcher">The IJwtFetcher implementation to use.</param>
        public static void LoadJwtFetcher(IJwtFetcher jwtFetcher)
        {
            _jwtFetcher = jwtFetcher;
        }

        /// <summary>
        /// Performs the ZKLogin authentication flow, including nonce generation, JWT token fetching,
        /// and Zero-Knowledge Proof generation.
        /// </summary>
        /// <returns>A task that returns the EnokiZKPResponse upon successful login, or null if login fails.</returns>
        public static async Task<EnokiZKPResponse> Login(CancellationToken cancellationToken = default)
        {
            if (!_inited)
            {
                Debug.LogWarning("ZKLoginManager is not inited. Use Init() first.");
                return null;
            }
            if (_zkpResponse == null)
            {
                EnokiNonceResponse nr = await EnokiZkLoginUtils.FetchNonce(_enokiPublicKey, _network, _ephemeralAccount.PublicKey.ToSuiPublicKey(), 2, cancellationToken);
                if (nr == null)
                {
                    return null;
                }
                _maxEpoch = nr.data.maxEpoch;
                if(_jwtFetcher == null)
                {
                    Debug.LogWarning("JwtFetcher is not assigned. You need to create a JwtFetcher and assign it via LoadJwtFetcher().");
                    return null;
                }
                _jwtFetcher.SetCancellationToken(cancellationToken);
                string jwtToken = await _jwtFetcher.FetchJwt(nr.data.nonce);
                JWT jwt = JWTDecoder.DecodeJWT(jwtToken);
                if (jwt == null)
                {
                    return null;
                }
                _zkLoginUser = await EnokiZkLoginUtils.FetchZKLoginUserData(jwtToken, _enokiPublicKey, cancellationToken);
                if (_zkLoginUser == null)
                {
                    return null;
                }
                _zkpResponse = await EnokiZkLoginUtils.FetchZKP(_network, _ephemeralAccount.PublicKey.ToSuiPublicKey(), jwtToken, _enokiPublicKey, nr.data.maxEpoch, nr.data.randomness, cancellationToken);
            }
            return _zkpResponse;
        }

        /// <summary>
        /// Logs out the current user by clearing ZKLogin session data including user information,
        /// ZKP response, and max epoch.
        /// </summary>
        public static void Logout()
        {
            if (!_inited)
            {
                Debug.LogWarning("ZKLoginManager is not inited. Use Init() first.");
                return;
            }
            _zkLoginUser = null;
            _zkpResponse = null;
            _maxEpoch = 0;
        }

        /// <summary>
        /// Validates whether the current maximum epoch is still valid by comparing it against
        /// the latest Sui system state. Logs out automatically if the epoch has expired.
        /// </summary>
        /// <returns>A task representing the asynchronous validation operation.</returns>
        public static async Task ValidateMaxEpoch()
        {
            RpcResult<SuiSystemSummary> summary = await _client.GetLatestSuiSystemStateAsync();
            if (summary.Result.Epoch > BigInteger.Parse(_maxEpoch.ToString()))
            {
                Debug.LogWarning($"Max Epoch is not valid anymore({_maxEpoch}), logging out...");
                Logout();
            }
        }

        /// <summary>
        /// Signs and executes a transaction block on the Sui blockchain using ZKLogin authentication.
        /// Creates a ZK signature from the ephemeral account and ZKP data, then submits the transaction.
        /// </summary>
        /// <param name="transactionBlock">The transaction block to sign and execute.</param>
        /// <returns>A task that returns the TransactionBlockResponse containing the execution results, or null if the operation fails.</returns>
        public static async Task<RpcResult<TransactionBlockResponse>> SignAndExecuteTransactionBlock(TransactionBlock transactionBlock)
        {
            string jsonData = JsonConvert.SerializeObject(_zkpResponse.data);
            Inputs inputs = JsonConvert.DeserializeObject<Inputs>(jsonData);
            
            transactionBlock.SetSenderIfNotSet(Sui.Accounts.AccountAddress.FromHex(_zkLoginUser.data.address));
            byte[] userTxBytes = await transactionBlock.Build(new BuildOptions(_client));
            if (transactionBlock.Error != null)
            {
                Debug.LogError(transactionBlock.Error.Message);
                return null;
            }

            SignatureBase signature = _ephemeralAccount.SignTransactionBlock(userTxBytes);
            SuiResult<string> signature_result = _ephemeralAccount.ToSerializedSignature(signature);
            if (signature_result.Error != null)
            {
                Debug.LogError(signature_result.Error.Message);
                return null;
            }

            string zkSignature = ZkLoginSignature.GetZkLoginSignature(inputs, (ulong)_maxEpoch, CryptoBytes.FromBase64String(signature_result.Result));
            //Debug.Log("Zk Signature => " + zkSignature);
            TransactionBlockResponseOptions opts = new TransactionBlockResponseOptions
            {
                ShowInput = false,
                ShowEffects = true,
                ShowEvents = true,
                ShowObjectChanges = true,
                ShowBalanceChanges = true
            };
            RpcResult<TransactionBlockResponse> response = await _client.ExecuteTransactionBlockAsync(userTxBytes, new List<string>() { zkSignature }, opts);

            return response;
        }
    }

}


