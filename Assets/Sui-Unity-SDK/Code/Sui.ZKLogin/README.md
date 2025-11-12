### Sui-Enoki-ZKLogin Unity SDK
A complete Unity implementation for authenticating users and signing transactions on the Sui blockchain using Enoki-ZKLogin (Zero-Knowledge Login) with Google OAuth.

#### Enoki ZK-Login Features

1. Google OAuth Integration: OAuth 2.0 with Google accounts for WebGL/Desktop/Mobile
2. Multi-Platform Support: Works on Desktop (Windows, Mac, Linux), WebGL and Mobile.
3. Session Persistence: Optional local storage of authentication data & ZK-Proof data.
4. Transaction Signing: Sign and execute Sui blockchain transactions with ZKLogin.
5. Epoch Validation: Automatic session expiration checking.

#### Enoki ZK-Login Prerequisites

1. Google OAuth Web Client ID from Google Cloud Console
2. Enoki Public Key from Enoki Portal

#### Google Cloud Console Setup

1. Go to Google Cloud Console
2. Create a new project or select an existing one
3. Navigate to APIs & Services > Credentials
4. Create OAuth 2.0 Client ID:

Application type: Web application

Authorized JavaScript origins:
- http://localhost:3000 (for desktop testing & mobile devices)
- [Your WebGL deployment URL]

Authorized redirect URIs:
- http://localhost:3000 (for desktop & mobile devices)
- [Your WebGL deployment URL]

Save your clientId and clientSecret for later use.

#### Enoki Setup

- Visit Enoki Portal
- Create an account & application with ZKLogin feature and obtain your Public Key
- Note your network (testnet, mainnet, devnet)

#### Enoki ZK-Login Installation
1. Import the Sui Unity SDK into your project
2. Copy all ZKLogin scripts into your project:

- EnokiZKLogin.cs
- EnokiZkLoginUtils.cs
- IJwtFetcher.cs (interface)
- And Samples folder.

#### Platform Support
1. Desktop [Windows(tested), Mac(not tested), Linux(not tested)]
2. Mobile [Android(tested), iOS(not tested)]
3. WebGL(tested)

#### Desktop
Uses GoogleOAuthDesktopJwtFetcher which:

- Opens browser for Google authentication
- Starts local HTTP server on specified port (default: 3000)
- Captures OAuth redirect automatically
- Uses returned JWT token to generate ZKUserData.

#### WebGL
Uses GoogleOAuthWebGLJwtFetcher which:

- Requires JavaScript plugin (ZKLogin.jslib)
- Handles OAuth 2.0 authentication in browser context
- Must be served from proper web server with CORS headers

### Enoki-ZK Login Security Considerations
⚠️ CRITICAL: PlayerPrefs Storage Warning

The _saveZKPOnDevice option uses Unity's PlayerPrefs which:

- Stores data in plain text
- Persists indefinitely
- Is NOT encrypted
- Can be accessed if device is compromised

Stored sensitive data includes:

- Ephemeral private key (can sign transactions)
- Zero-Knowledge Proof (authentication data)
Together, these allow full transaction signing capability

#### Production Recommendations
DO NOT use PlayerPrefs rawly in production! Instead:

1. Mobile Apps:

- iOS: Use Keychain Services
- Android: Use Android Keystore

2. Session-Only Storage:

- Store credentials only in memory
- Clear on application exit
- Require re-authentication on restart

3. Desktop Apps:

- Use OS-specific secure storage
- Windows: Credential Manager
- macOS: Keychain
- Linux: Secret Service API

WebGL:

- We don't recommend store credentials locally with rawData
- Use some encryption solutions
- Use session-only authentication
- Implement proper server-side session management

### Enoki-ZKLogin API Reference

Initialize the ZKLogin system with network and Enoki key:
```c#
EnokiZKLogin.Init(network, enokiPublicKey);
```
Authentication:
```c#
await EnokiZKLogin.Login();
```

Logout:
```c#
await EnokiZKLogin.Logout();
```

Check if zk-login user currently logged:
```c#
EnokiZKLogin.IsLogged();
```

Validate if current session has expired:
```c#
await EnokiZKLogin.ValidateMaxEpoch();
```

Get User's ZKLogin Sui Address:
```c#
EnokiZKLogin.GetSuiAddress();
```

Get User's ZKLogin Ephemeral Account:
```c#
EnokiZKLogin.GetEphemeralAccount();
```

Get User's ZKLogin Datas:
```c#
EnokiZKLogin.GetZKLoginUser();
EnokiZKLogin.GetZKP();
EnokiZKLogin.GetMaxEpoch();
```
Load previous session:
```c#
EnokiZKLogin.LoadZKPResponse(zkpResponse);
EnokiZKLogin.LoadZKLoginUser(zkLoginUser);
EnokiZKLogin.LoadEphemeralKey(ephemeralAccount);
EnokiZKLogin.LoadMaxEpoch(maxEpoch);
```

Sign and execute transactions:
```c#
EnokiZKLogin.SignAndExecuteTransactionBlock(TransactionBlock tx);
```

Get the sui client of ZKLogin system or set external sui client:
```c#
EnokiZKLogin.GetClient()
EnokiZKLogin.SetClient(SuiClient client)
```