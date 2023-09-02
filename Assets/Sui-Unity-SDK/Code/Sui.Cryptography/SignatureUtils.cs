
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
        public enum SignatureScheme
        {
            ED25519,
            Secp256k1,
            Secp256r1,
            MultiSig,
            Zk
        }

        public static string ToSerializedSignature(ISignature signature)
        {
            throw new NotImplementedException();
        }

        public static ISignature ToSerializedSignature(string serializedSignature)
        {
            throw new NotImplementedException();
        }
    }

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
    }

    /// <summary>
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

    }

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
    }
}