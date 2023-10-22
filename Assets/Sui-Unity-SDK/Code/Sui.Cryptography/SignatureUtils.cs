using System;
using System.Collections.Generic;
using static Sui.Cryptography.SignatureUtils;

namespace Sui.Cryptography
{
    /// <summary>
    /// Implements signature definitions from this codebase:
    /// https://github.com/MystenLabs/sui/blob/main/sdk/typescript/src/cryptography/signature.ts
    /// </summary>
    public static class SignatureUtils
    {
        /// <summary>
        /// Signature schemes supported by Sui.
        /// </summary>
        public enum SignatureScheme
        {
            ED25519,
            Secp256k1,
            Secp256r1,
            MultiSig,
            Zk
        }

        /// <summary>
        /// Types of inten scope flags.
        /// An intent is a domain separator.
        /// </summary>
        public enum IntentScope
        {
            TransactionData     = 0,
            TransactionEffects  = 1,
            CheckpointSummary   = 2,
            PersonalMessage     = 3,
        }

        /// <summary>
        /// The app id.
        /// </summary>
        public enum AppId
        {
            Sui = 0
        }

        /// <summary>
        /// The intent version.
        /// </summary>
        public enum IntentVersion
        {
            V0 = 0
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static byte[] CreateIntentWithScope (IntentScope scope)
        {
            return new byte[] { (byte)scope, (byte)IntentVersion.V0, (byte)AppId.Sui };
        }

        /// <summary>
        /// Inserts a domain separator for a message that is being signed
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static byte[] CreateMessageWithIntent(IntentScope scope, byte[] message)
        {
            byte[] intent = CreateIntentWithScope(scope);
            byte[] intentMessage = new byte[intent.Length + message.Length];
            Array.Copy(intent, intentMessage, intent.Length);
            Array.Copy(message, 0, intentMessage, intent.Length, message.Length);
            return intentMessage;
        }

        public static string ToSerializedSignature(SignatureBase signature)
        {
            throw new NotImplementedException();
        }

        public static SignatureBase ToSerializedSignature(string serializedSignature)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Utility class to map a signature schem to the corresponding byte flag Sui uses.
    /// </summary>
    public static class SignatureSchemeToFlag
    {
        private static readonly Dictionary<SignatureScheme, byte> _signatureSchemeToFlag = new()
        {
            { SignatureScheme.ED25519,      0x00 },
            { SignatureScheme.Secp256k1,    0x01 },
            { SignatureScheme.Secp256r1,    0x02 },
            { SignatureScheme.MultiSig,     0x03 },
            { SignatureScheme.Zk,           0x05 }
        };

        public static byte ED25519      => _signatureSchemeToFlag[SignatureScheme.ED25519];
        public static byte Secp256k1    => _signatureSchemeToFlag[SignatureScheme.Secp256k1];
        public static byte Secp256r1    => _signatureSchemeToFlag[SignatureScheme.Secp256r1];
        public static byte MultiSig     => _signatureSchemeToFlag[SignatureScheme.MultiSig];
        public static byte Zk           => _signatureSchemeToFlag[SignatureScheme.Zk];

        /// <summary>
        /// Returns the byte flag that matches the signature scheme.
        /// </summary>
        /// <param name="signatureScheme">The signature scheme.</param>
        /// <returns></returns>
        public static byte GetFlag(SignatureScheme signatureScheme)
        {
            return _signatureSchemeToFlag[signatureScheme];
        }

        ///// <summary>
        ///// Returns the corresponding signature scheme from a flag byte.
        ///// </summary>
        ///// <param name="flag">Byte that specifies the signature scheme.</param>
        ///// <returns>SignatureScheme enum.</returns>
        //public static SignatureScheme GetSignatureScheme(byte flag)
        //{
        //    SignatureScheme signatureScheme;
        //    switch (flag)
        //    {
        //        case 0x00:
        //            signatureScheme = SignatureScheme.ED25519;
        //            break;
        //        case 0x01:
        //            signatureScheme = SignatureScheme.Secp256k1;
        //            break;
        //        case 0x02:
        //            signatureScheme = SignatureScheme.Secp256r1;
        //            break;
        //        case 0x03:
        //            signatureScheme = SignatureScheme.MultiSig;
        //            break;
        //        case 0x05:
        //            signatureScheme = SignatureScheme.Zk;
        //            break;
        //        default:
        //            signatureScheme = SignatureScheme.ED25519;
        //            break;
        //    }
        //    return signatureScheme;
        //}

    }

    /// <summary>
    /// Utility class to map a signature scheme to it's signature byte size.
    /// Presently the Sui TypeScript SDK doesn't support Multisig and Zk signature-scheme-to-size
    /// </summary>
    public static class SignatureSchemeToSize
    {
        private static readonly Dictionary<SignatureScheme, int> _signatureSchemeToSize = new()
        {
            { SignatureScheme.ED25519,      32 },
            { SignatureScheme.Secp256k1,    33 },
            { SignatureScheme.Secp256r1,    33 },
            //{ SignatureScheme.MultiSig, __ },
            //{ SignatureScheme.Zk, __ }
        };

        public static int ED25519       => _signatureSchemeToSize[SignatureScheme.ED25519];
        public static int Secp256k1     => _signatureSchemeToSize[SignatureScheme.Secp256k1];
        public static int Secp256r1     => _signatureSchemeToSize[SignatureScheme.Secp256r1];
        //public static int MultiSig      => _signatureSchemeToSize[SignatureScheme.MultiSig];
        //public static int Zk            => _signatureSchemeToSize[SignatureScheme.Zk];

        /// <summary>
        /// Returns the size of signature based on particular signature scheme.
        /// </summary>
        /// <param name="signatureScheme">The signature scheme.</param>
        /// <returns></returns>
        public static int GetSize(SignatureScheme signatureScheme)
        {
            if (!_signatureSchemeToSize.ContainsKey(signatureScheme))
                throw new NotSupportedException("Unsupported signature scheme");

            return _signatureSchemeToSize[signatureScheme];
        }

    }

    /// <summary>
    /// Utility class to map a signature byte flag to it's signature scheme name.
    /// </summary>
    public static class SignatureFlagToScheme
    {
        public static readonly Dictionary<byte, SignatureScheme> _signatureFlagToScheme = new()
        {
            { 0x00, SignatureScheme.ED25519 },
            { 0x01, SignatureScheme.Secp256k1 },
            { 0x02, SignatureScheme.Secp256r1 },
            { 0x03, SignatureScheme.MultiSig },
            { 0x05, SignatureScheme.Zk }
        };

        public static SignatureScheme FLAG_0x00     => _signatureFlagToScheme[0x00];
        public static SignatureScheme FLAG_0x01     => _signatureFlagToScheme[0x01];
        public static SignatureScheme FLAG_0x02     => _signatureFlagToScheme[0x02];
        public static SignatureScheme FLAG_0x03     => _signatureFlagToScheme[0x03];
        public static SignatureScheme FLAG_0x05     => _signatureFlagToScheme[0x05];

        /// <summary>
        /// Returns a signature scheme name from a byte flag.
        /// </summary>
        /// <param name="flagByte">The byte flag.</param>
        /// <returns></returns>
        public static SignatureScheme GetScheme(byte flagByte)
        {
            return _signatureFlagToScheme[flagByte];
        }
    }
}