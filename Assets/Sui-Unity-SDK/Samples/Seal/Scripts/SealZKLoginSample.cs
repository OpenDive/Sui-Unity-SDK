using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Rpc.Client;
using Sui.Rpc.Models;
using Sui.Transactions;
using Sui.ZKLogin;
using Sui.ZKLogin.Enoki;
using Sui.ZKLogin.Enoki.Utils;
using System;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sui.Seal
{
    public class SealZKLoginSample : MonoBehaviour
    {
        [SerializeField] private string _packageId = "0xf3dfe70b4916fecaecf7928bb8221031c28d5130c66e8fa7e645ce8785846f91";
        [SerializeField] private string _moduleName = "private_data";
        [SerializeField] private string _funcName = "store_entry";
        [SerializeField] private int _threshold = 2;
        [SerializeField] private string[] _serverObjectIds = { "0x73d05d62c18d9374e3ea529e8e0ed6161da1a141a94d3f76ae3fe4e99356db75", "0xf5d14a81a982144ae441cd7d64b09027f116a468bd36e7eca494f750591623c8" };

        [Space]

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
        [SerializeField] private Button _encryptButton;
        [SerializeField] private Button _decryptButton;
        [SerializeField] private TMP_Text _addressText;
        [SerializeField] private TMP_Text _encryptedText;
        [SerializeField] private TMP_Text _decryptText;
        [SerializeField] private TMP_InputField _encryptTextField;
        [SerializeField] private TMP_InputField _decryptTextField;
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
            // Seal works only with WebGL environment for now. So we choose WebGL platform.
            GoogleOAuthWebGLJwtFetcher googleOAuthWebGLJwtFetcher = new GameObject("GoogleOAuthWebGLJwtFetcher").AddComponent<GoogleOAuthWebGLJwtFetcher>();
            googleOAuthWebGLJwtFetcher.SetGoogleClientId(_googleClientId);
            EnokiZKLogin.LoadJwtFetcher(googleOAuthWebGLJwtFetcher);
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
            SealBridge.Instance.SetSuiClient(_client);
            SealBridge.Instance.SetThreshold(_threshold);
            SealBridge.Instance.SetPackageInformation(_packageId, _moduleName, _funcName);
            SealBridge.Instance.SetServerObjectIds(_serverObjectIds);
            UpdateUI();
        }

        /// <summary>
        /// Handles the login button click event. Initiates the ZKLogin authentication flow
        /// and optionally saves the session data locally for persistence across app restarts.
        /// </summary>
        public async void OnLoginButtonClick()
        {
            if (EnokiZKLogin.IsLogged())
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
            Debug.Log(EnokiZKLogin.GetSuiAddress());
            UpdateUI();
        }

        public async void OnEncryptButtonClick()
        {
            string dataToEncrypt = _encryptTextField.text;
            TransactionBlock txBlock = await SealBridge.Instance.Encrypt(dataToEncrypt, EnokiZKLogin.GetSuiAddress());
            await EnokiZKLogin.SignAndExecuteTransactionBlock(txBlock);
            _encryptedText.gameObject.SetActive(true);
        }

        public async void OnDecryptButtonClick()
        {
            EnokiZKPResponse zkp = EnokiZKLogin.GetZKP();
            string jsonData = JsonConvert.SerializeObject(zkp.data);
            Inputs inputs = JsonConvert.DeserializeObject<Inputs>(jsonData);
            Serialization enokiSerialization = new Serialization();
            inputs.Serialize(enokiSerialization);
            byte[] inputsBytes = enokiSerialization.GetBytes();

            string objectId = _decryptTextField.text;
            var response = await _client.GetObjectAsync(new Accounts.AccountAddress(objectId), new ObjectDataOptions() { ShowContent = true });
            var moveObject = (ParsedMoveObject)response.Result.Data.Content.ParsedData;

            var fields = moveObject.Fields;
            byte[] encryptedBytes = (moveObject.Fields["data"] as JArray).Select(jv => (byte)jv).ToArray();
            byte[] nonceBytes = (moveObject.Fields["nonce"] as JArray).Select(jv => (byte)jv).ToArray();
            byte[] decryptedBytes = await SealBridge.Instance.DecryptWithZKLogin(encryptedBytes, inputsBytes, (uint)EnokiZKLogin.GetMaxEpoch(), nonceBytes, objectId, _ephemeralAccount.PrivateKey.ToBase64(), EnokiZKLogin.GetSuiAddress());
            _decryptText.text = Encoding.UTF8.GetString(decryptedBytes);
        }

        /// <summary>
        /// Updates the UI elements based on the current ZKLogin authentication state & Seal state.
        /// Enables/disables buttons and displays the appropriate address text.
        /// </summary>
        void UpdateUI()
        {
            TMP_Text loginText = _loginButton.GetComponentInChildren<TMP_Text>();

            loginText.text = EnokiZKLogin.IsLogged() ? "Logout" : "Login";
            _addressText.text = EnokiZKLogin.IsLogged() ? EnokiZKLogin.GetSuiAddress() : "Login to see your zkLogin Sui Address";
            _encryptButton.interactable = EnokiZKLogin.IsLogged();
            _decryptButton.interactable = EnokiZKLogin.IsLogged();
            _encryptTextField.text = "";
            _decryptTextField.text = "";
            _encryptedText.gameObject.SetActive(false);
            _decryptText.text = "";
        }

    }

}
