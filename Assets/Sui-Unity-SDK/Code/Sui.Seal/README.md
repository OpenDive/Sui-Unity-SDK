# SealBridge for Unity WebGL (Sui Seal SDK Integration)

**Author:** viol3  
**Version:** 1.0.5  
**License:** MIT  

SealBridge provides a C# / Unity-side integration with the **Sui Seal encryption system**, enabling developers to securely store and retrieve encrypted data on-chain. It uses the compiled **seal.bundle.js** (from `@mysten/seal` TypeScript SDK) as a native WebGL plugin, allowing hybrid encryption and key server coordination directly within Unity.

---

## ✨ Overview

Seal is a decentralized secrets management (DSM) product. Application developers and users can use Seal to secure sensitive data at rest on decentralized storage like Walrus or any other onchain / offchain storage. Seal enables identity-based encryption and decryption of sensitive data, with access controlled by onchain policies on Sui. Lightweight key servers enforce these policies and provide threshold-based decryption keys, while developers can integrate easily using the TypeScript SDK.
You can learn more at : https://seal.mystenlabs.com/

SealBridge offers Unity developers a fully managed bridge to the **Sui Seal SDK**, built only for WebGL platform for now.  
It manages encryption and decryption flows and constructs properly authenticated Sui Move transactions that reference access policies and threshold key servers.

This SDK is ideal for:
- Storing encrypted user data or assets on the Sui blockchain  
- Gating access using Sui Move-based policy objects  
- Supporting **Enoki zkLogin sessions** and ephemeral authentication  
- Enabling **verifiable, privacy-preserving game state sharing**

---

## 🏗 Architecture

SealBridge communicates with a WebAssembly/JS runtime (`seal.bundle.js`) that performs:
- Data encryption using Seal’s decentralized key management  
- Policy validation and access delegation through Sui Move objects  
- Decryption flow authorization via private key or zkLogin proof  

The Unity C# layer wraps this process with asynchronous task-based APIs and transaction builder utilities from the **OpenDive Sui-Unity-SDK**.

Note => Ensure you have a valid deployed seal package that has got a seal_approve function. Please read this : https://seal-docs.wal.app/UsingSeal/
---

## ⚙️ Requirements

| Component | Version |
|------------|----------|
| Unity | 6000.60f1 LTS or newer |
| Platform | WebGL |
| Sui Network | Testnet / Mainnet |
| Seal JS Runtime | `seal.bundle.js` built from `@mysten/seal` v0.9.0+ |
| Sui-Unity-SDK | OpenDive.Sui SDK

---

## 📦 Installation

1. Clone or download this repository's Sui-Unity-SDK folder into your Unity project's `Assets/` folder:

2. Move **WebGLTemplates** folder to Assets folder from Sui-Unity-SDK folder

3. Open "Seal" scene in **Sui-Unity-SDK/Samples/Seal/Scenes** folder and put a testnet private key with testnet coins.

4. In Build Settings, put the Seal scene as main scene.

5. In Player Settings, find Resolution and Presentation tab and under the WebGL Template, choose the Seal template.

6. Start to build as WebGL, wait for web build, after built successful, it will open a localhost website, you can try Encryption/Decryption there.

Note => Ensure you have a valid Sui account and node endpoint configured via:
var client = new SuiClient(Constants.TestnetConnection);
var account = new Account(privateKeyHex);

---

## 🧠 Usage Example

### 1. Initialize SealBridge

<pre><code class="language-csharp">
void Start()
{
	var client = new SuiClient(Constants.TestnetConnection);
	SealBridge.Instance.SetSuiClient(client);
	SealBridge.Instance.SetPackageInformation(
	"YOUR PACKAGE ID",
	"MODULE NAME",
	"FUNCTION NAME");
}
</code></pre>


Define the threshold value used by Seal’s encryption policy —
the number of servers required to approve the encryption/decryption process.
<pre><code class="language-csharp">
SealBridge.Instance.SetThreshold(2);
</code></pre>

Assign the list of Seal server object IDs that will participate in the approval and key retrieval process.
<pre><code class="language-csharp">
SealBridge.Instance.SetServerObjectIds(
    "0x73d05d62c18d9374e3ea529e8e0ed6161da1a141a94d3f76ae3fe4e99356db75",
    "0xf5d14a81a982144ae441cd7d64b09027f116a468bd36e7eca494f750591623c8"
);
</code></pre>

### 2. Encrypt/Decrypt data

Encrypt the given plaintext for a specific Sui address.
Returns a TransactionBlock ready to be signed and executed on-chain.
<pre><code class="language-csharp">
var tx = await SealBridge.Instance.Encrypt("my secret text", suiAddress);
await suiClient.SignAndExecuteTransactionBlock(tx);
</code></pre>

Decrypt the encrypted payload using a standard private key.
Returns the original plaintext as a byte array.
<pre><code class="language-csharp">
var decrypted = await SealBridge.Instance.Decrypt(
    encryptedBytes,
    nonceBytes,
    objectId,
    privateKeyBase64,
    suiAddress
);

string result = System.Text.Encoding.UTF8.GetString(decrypted);
</code></pre>

Performs decryption using Enoki’s Zero-Knowledge Login flow.
Requires a ZK proof payload (proofInputBytes)i ephemeral private key and mex epoch.
<pre><code class="language-csharp">
var decrypted = await SealBridge.Instance.DecryptWithZKLogin(
    encryptedBytes,
    proofInputBytes,
    maxEpoch,
    nonceBytes,
    objectId,
    ephemeralPrivateKeyBase64,
    suiAddress
);
</code></pre>