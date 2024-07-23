//
//  Account.cs
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

using System;
using Sui.Cryptography;
using Sui.Rpc.Client;
using Sui.Utilities;
using static Sui.Cryptography.SignatureUtils;

namespace Sui.Accounts
{
    public class Account: AccountBase
    {
        /// <summary>
        /// Signature scheme of the account.
        /// </summary>
        public SignatureScheme SignatureScheme { get; set; }

        /// <summary>
        /// Represents an AccoutAddress object.
        /// </summary>
        public AccountAddress AccountAddress
        {
            get => AccountAddress.FromHex(((SuiPublicKeyBase)this.PublicKey).ToSuiAddress());
        }

        public Account(string private_key, string public_key, SignatureScheme signature_scheme)
        {
            switch (signature_scheme)
            {
                case SignatureScheme.ED25519:
                    this.PrivateKey = new Cryptography.Ed25519.PrivateKey(private_key);
                    this.PublicKey = new Cryptography.Ed25519.PublicKey(public_key);
                    break;
                default:
                    this.SetError<Account, SuiError>(null, "Cryptography signature not implemented yet.", signature_scheme);
                    return;
            }

            this.SignatureScheme = signature_scheme;
        }

        public Account(byte[] private_key, byte[] public_key, SignatureScheme signature_scheme)
        {
            switch (signature_scheme)
            {
                case SignatureScheme.ED25519:
                    this.PrivateKey = new Cryptography.Ed25519.PrivateKey(private_key);
                    this.PublicKey = new Cryptography.Ed25519.PublicKey(public_key);
                    break;
                default:
                    this.SetError<Account, SuiError>(null, "Cryptography signature not implemented yet.", signature_scheme);
                    return;
            }

            this.SignatureScheme = signature_scheme;
        }

        public Account(byte[] private_key, SignatureScheme signature_scheme = SignatureScheme.ED25519)
        {
            switch (signature_scheme)
            {
                case SignatureScheme.ED25519:
                    this.PrivateKey = new Cryptography.Ed25519.PrivateKey(private_key);
                    this.PublicKey = this.PrivateKey.PublicKey();
                    break;
                default:
                    this.SetError<Account, SuiError>(null, "Cryptography signature not implemented yet.", signature_scheme);
                    return;
            }

            this.SignatureScheme = signature_scheme;
        }

        public Account(string private_key, SignatureScheme signature_scheme = SignatureScheme.ED25519)
        {
            switch (signature_scheme)
            {
                case SignatureScheme.ED25519:
                    this.PrivateKey = new Cryptography.Ed25519.PrivateKey(private_key);
                    this.PublicKey = this.PrivateKey.PublicKey();
                    break;
                default:
                    this.SetError<Account, SuiError>(null, "Cryptography signature not implemented yet.", signature_scheme);
                    return;
            }

            this.SignatureScheme = signature_scheme;
        }

        public Account(SignatureScheme signature_scheme = SignatureScheme.ED25519)
        {
            switch (signature_scheme)
            {
                case SignatureScheme.ED25519:
                    this.PrivateKey = new Cryptography.Ed25519.PrivateKey();
                    this.PublicKey = this.PrivateKey.PublicKey();
                    break;
                default:
                    this.SetError<Account, SuiError>(null, "Cryptography signature not implemented yet.", signature_scheme);
                    return;
            }

            this.SignatureScheme = signature_scheme;
        }

        /// <summary>
        /// Derives a Sui address from the account's public key.
        /// </summary>
        /// <returns>A string representing the Sui Address.</returns>
        public string SuiAddress()
            => ((SuiPublicKeyBase)this.PublicKey).ToSuiAddress();

        /// <summary>
        /// Sign messages with a specific intent. By combining
        /// the message bytes with the intent before hashing and signing,
        /// it ensures that a signed message is tied to a specific purpose
        /// and domain separator is provided
        /// </summary>
        /// <param name="bytes">The intent message itself to sign.</param>
        /// <param name="intent">The intent scope of the message.</param>
        /// <returns>A `SignatureBase` object representing the signature.</returns>
        public SignatureBase SignWithIntent(byte[] bytes, IntentScope intent)
            => ((SuiPrivateKeyBase)this.PrivateKey).SignWithIntent(bytes, intent);

        /// <summary>
        /// Create a serialized signature from:
        /// signature scheme, signature, and public key.
        /// </summary>
        /// <param name="signature">The `SignatureBase` object that represents the inputted signature values.</param>
        /// <returns>The serialized signature as a string wrapped in a result.</returns>
        public SuiResult<string> ToSerializedSignature(SignatureBase signature)
            => SignatureUtils.ToSerializedSignature
               (
                   new SerializeSignatureInput
                   (
                       this.SignatureScheme,
                       signature.SignatureBytes,
                       (SuiPublicKeyBase)this.PublicKey
                    )
               );

        /// <summary>
        /// Verifies a Sui Transaction Block with the corresponding Public Key.
        /// </summary>
        /// <param name="transaction_block">The transaction block as a byte array.</param>
        /// <param name="signature">The signature created by the corresponding Public Key.</param>
        /// <returns>A bool representing whether or not the transaction block inputted corresponds to the inputted signature.</returns>
        public bool VerifyTransactionBlock(byte[] transaction_block, SignatureBase signature)
            => ((SuiPublicKeyBase)this.PublicKey).VerifyTransactionBlock(transaction_block, signature);

        /// <summary>
        /// Signs provided transaction block by calling `signWithIntent()`
        /// with a `TransactionData` provided as intent scope
        /// </summary>
        /// <param name="bytes">The intent message as a byte array.</param>
        /// <returns>A `SignatureBase` object representing the signature.</returns>
        public SignatureBase SignTransactionBlock(byte[] bytes)
            => this.SignWithIntent(bytes, IntentScope.TransactionData);

        // TODO: Implement SignPersonalMessage
        // https://github.com/MystenLabs/sui/blob/a7c64653f084983c369baf12517992fb5c192aec/sdk/typescript/src/cryptography/keypair.ts#L59
        public SignatureWithBytes SignPersonalMessage(byte[] bytes)
            => throw new NotImplementedException();
    }
}