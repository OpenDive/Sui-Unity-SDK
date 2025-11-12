using Org.BouncyCastle.Math;
using Sui.Utilities;
using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class SaltGenerator
{
    // HKDF Extract
    private static byte[] Extract(byte[] salt, byte[] ikm)
    {
        using (var hmac = new HMACSHA256(salt ?? new byte[32]))
        {
            return hmac.ComputeHash(ikm);
        }
    }

    // HKDF Expand
    private static byte[] Expand(byte[] prk, byte[] info, int length)
    {
        int hashLen = 32;
        int n = (int)Math.Ceiling((double)length / hashLen);
        byte[] okm = new byte[length];
        byte[] previous = Array.Empty<byte>();
        int offset = 0;

        using (var hmac = new HMACSHA256(prk))
        {
            for (int i = 1; i <= n; i++)
            {
                hmac.Initialize();
                hmac.TransformBlock(previous, 0, previous.Length, null, 0);
                if (info != null && info.Length > 0)
                    hmac.TransformBlock(info, 0, info.Length, null, 0);
                hmac.TransformFinalBlock(new byte[] { (byte)i }, 0, 1);
                previous = hmac.Hash;

                int toCopy = Mathf.Min(hashLen, length - offset);
                System.Array.Copy(previous, 0, okm, offset, toCopy);
                offset += toCopy;
            }
        }
        return okm;
    }

    // HKDF Derive
    private static byte[] Derive(byte[] ikm, byte[] salt, byte[] info, int length)
    {
        var prk = Extract(salt, ikm);
        return Expand(prk, info, length);
    }

    public static byte[] GenerateUserSaltBytes(string masterSeed, string iss, string aud, string sub)
    {
        byte[] ikm = Encoding.UTF8.GetBytes(masterSeed);
        byte[] salt = Encoding.UTF8.GetBytes(iss + "|" + aud);
        byte[] info = Encoding.UTF8.GetBytes(sub);

        return Derive(ikm, salt, info, 16); // 16 byte
    }

    public static BigInteger GenerateUserSalt(string masterSeed, string iss, string aud, string sub)
    {

        return ToBigInt(GenerateUserSaltBytes(masterSeed, iss, aud, sub)); // 16 byte
    }

    public static string ToHex(byte[] data)
    {
        return System.BitConverter.ToString(data).Replace("-", "").ToLowerInvariant();
    }

    private static BigInteger ToBigInt(byte[] saltBytes)
    {
        if (saltBytes.Length != 16)
            throw new ArgumentException("Salt must be 16 bytes");


        byte[] bigEndian = new byte[saltBytes.Length + 1];
        for (int i = 0; i < saltBytes.Length; i++)
        {
            bigEndian[i] = saltBytes[saltBytes.Length - 1 - i];
        }
        bigEndian[16] = 0; // pozitif işaret

        return new BigInteger(bigEndian);
    }
}
