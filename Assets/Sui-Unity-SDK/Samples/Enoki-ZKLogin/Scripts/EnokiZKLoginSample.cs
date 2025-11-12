using Newtonsoft.Json;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Rpc;
using Sui.Rpc.Client;
using Sui.Rpc.Models;
using Sui.Transactions;
using Sui.ZKLogin.Enoki.Utils;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sui.ZKLogin.Enoki
{
    /// <summary>
    /// Sample implementation demonstrating how to use the EnokiZKLogin system for authentication
    /// and transaction signing on the Sui blockchain. This example shows the complete flow from
    /// initialization, login, session persistence, and executing transactions with ZK proofs.
    /// </summary>
    public class EnokiZKLoginSample : MonoBehaviour
    {
        // Network configuration - specify which Sui network to connect to (mainnet, testnet, devnet, or localnet)
        [SerializeField] private string _network;

        // Enoki public key obtained from https://portal.enoki.mystenlabs.com/
        [SerializeField] private string _enokiPublicKey;

        // Google OAuth Client ID for authentication - create one at https://console.cloud.google.com/
        [SerializeField] private string _googleClientId;

        // OAuth redirect URI - must match the one configured in Google Cloud Console
        [SerializeField] private string _redirectUri = "http://localhost:3000";

        [Space]
        // WARNING: Saving ZKP data locally using PlayerPrefs is NOT SECURE for production!
        // PlayerPrefs persists data indefinitely and is not encrypted. If an attacker gains access
        // to both the ephemeral private key and ZK proof, they can sign transactions on behalf of the user.
        // For production apps:
        // - Use secure platform-specific storage (Keychain on iOS, KeyStore on Android)
        // - Consider using session-only storage that clears on app close
        // - Never store these credentials in plain text or persistent storage
        // This flag is provided for development purposes only.
        [SerializeField] private bool _saveZKPOnDevice = true;
        [Space]
        [Header("UI")]
        [SerializeField] private Button _loginButton;
        [SerializeField] private Button _sampleTransactionButton;
        [SerializeField] private TMP_Text _addressText;
        [SerializeField] private TMP_Text _transactionLogText;

        private Account _ephemeralAccount;
        private SuiClient _client;

        // SECURITY WARNING: PlayerPrefs keys for storing ZKLogin session data locally.
        // These store sensitive cryptographic materials that should be protected:
        // - EPHEMERAL_PRIVATEKEY: Private key that can sign transactions
        // - ZKP: Zero-Knowledge Proof used for authentication
        // Together, these allow full transaction signing capability!
        // PlayerPrefs is NOT secure - data is stored unencrypted and persists indefinitely
        // This is for development purposes only - use secure storage in production!
        private const string LOGIN_PREF = "ZK_LOGIN_DATA";

        /// <summary>
        /// Initializes the ZKLogin system and attempts to restore a previous session if available.
        /// This method runs when the script starts and handles both WebGL and desktop platforms differently.
        /// </summary>
        async void Start()
        {
            // Initialize the ZKLogin manager with network and Enoki public key
            EnokiZKLogin.Init(_network, _enokiPublicKey);
            _client = EnokiZKLogin.GetClient();

            // Platform-specific JWT fetcher setup
            // WebGL builds require a different authentication flow than desktop builds
            // For specific requirements, you can create your own JwtFetcher classes which implement the IJwtFetcher interface.
#if UNITY_WEBGL && !UNITY_EDITOR
        GoogleOAuthWebGLJwtFetcher googleOAuthWebGLJwtFetcher = new GameObject("GoogleOAuthWebGLJwtFetcher").AddComponent<GoogleOAuthWebGLJwtFetcher>();
        googleOAuthWebGLJwtFetcher.SetGoogleClientId(_googleClientId);
        EnokiZKLogin.LoadJwtFetcher(googleOAuthWebGLJwtFetcher);
#else
            EnokiZKLogin.LoadJwtFetcher(new GoogleOAuthDesktopJwtFetcher(_googleClientId, _redirectUri));
#endif
            if (PlayerPrefs.HasKey(LOGIN_PREF))
            {
                EnokiZKLoginSaveableData data = JsonConvert.DeserializeObject<EnokiZKLoginSaveableData>(PlayerPrefs.GetString(LOGIN_PREF));
                EnokiZKPResponse zkpResponse = data.zkpResponse;
                EnokiZKLoginUserResponse zkLoginUser = data.loginUserResponse;
                _ephemeralAccount = new Account(data.ephemeralPrivateKeyHex);

                EnokiZKLogin.LoadZKPResponse(zkpResponse);
                EnokiZKLogin.LoadZKLoginUser(zkLoginUser);
                EnokiZKLogin.LoadEphemeralKey(_ephemeralAccount);
                EnokiZKLogin.LoadMaxEpoch(data.maxEpoch);

                // Validate that the saved session hasn't expired (check max epoch)
                // If expired, this will automatically log out the user
                await EnokiZKLogin.ValidateMaxEpoch();
            }
            UpdateUI();
        }

        /// <summary>
        /// Handles the login button click event. Initiates the ZKLogin authentication flow
        /// and optionally saves the session data locally for persistence across app restarts.
        /// </summary>
        public async void OnLoginButtonClick()
        {
            if(EnokiZKLogin.IsLogged())
            {
                EnokiZKLogin.Logout();
            }
            else
            {
                // Start the ZKLogin authentication flow
                // This will:
                // 1. Generate a nonce
                // 2. Open browser for Google OAuth
                // 3. Fetch JWT token
                // 4. Gets user information with salt and address.
                // 5. Generate Zero-Knowledge Proof
                EnokiZKPResponse zkpResponse = await EnokiZKLogin.Login();
                if (_saveZKPOnDevice)
                {
                    PlayerPrefs.SetString(LOGIN_PREF, JsonConvert.SerializeObject(EnokiZKLogin.GetSaveableData()));
                }
            }
            
            UpdateUI();
        }

        /// <summary>
        /// Handles the sample transaction button click event.
        /// Triggers a demonstration transaction on the Sui blockchain with zkLogin.
        /// </summary>
        public void OnSampleTransactionButtonClick()
        {
            SampleTransaction();
        }

        /// <summary>
        /// Demonstrates how to create, sign, and execute a transaction using ZKLogin.
        /// This example sends 0.01 SUI to a specified address by splitting coins from gas.
        /// </summary>
        async void SampleTransaction()
        {
            TransactionBlock tx_block = new TransactionBlock();

            // Split coins from the gas coin to create a new coin with specified amount
            // This example splits 10,000,000 MIST (0.01 SUI) from the gas coin
            // Note: 1 SUI = 1,000,000,000 MIST
            List<TransactionArgument> splitArgs = tx_block.AddSplitCoinsTx
            (
                tx_block.gas,
                new TransactionArgument[]
                {
                    tx_block.AddPure(new U64(10_000_000)) // Insert split amount here(0.01 Sui)
                }
            );
            tx_block.AddTransferObjectsTx
            (
                new TransactionArgument[]
                {
                splitArgs[0] // Insert split amount here
                },
                Sui.Accounts.AccountAddress.FromHex("0x0d9b5ca4ebae5f4a7bd3f17e4e36cd6f868d8f0c5a7f977f94f836631fe0288d")
            );

            // Sign and execute the transaction using ZKLogin
            // This will:
            // 1. Build the transaction
            // 2. Sign it with the ephemeral key
            // 3. Create ZK signature with the tx bytes, inputs of zkproof and maxEpoch
            // 4. Submit to the blockchain
            RpcResult<TransactionBlockResponse> response = await EnokiZKLogin.SignAndExecuteTransactionBlock(tx_block);
            if (response.Error != null)
            {
                _transactionLogText.text = "<color=red>Tx Error => " + response.Error.Message + "</color>";
            }
            else
            {
                _transactionLogText.text = "<color=green>Tx Success => " + response.Result.Digest + "</color>";
            }
        }

        /// <summary>
        /// Updates the UI elements based on the current ZKLogin authentication state.
        /// Enables/disables buttons and displays the appropriate address text.
        /// </summary>
        void UpdateUI()
        {
            TMP_Text loginText = _loginButton.GetComponentInChildren<TMP_Text>();

            loginText.text = EnokiZKLogin.IsLogged() ? "Logout" : "Login";
            _addressText.text = EnokiZKLogin.IsLogged() ? EnokiZKLogin.GetSuiAddress() : "Login to see your zkLogin Sui Address";
            _sampleTransactionButton.interactable = EnokiZKLogin.IsLogged();
            _transactionLogText.text = "";
        }


    }
}
    

