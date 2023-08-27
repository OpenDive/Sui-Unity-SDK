using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sui.Tests
{
    public class TestValues
    {
        // Extended PrivateKey for reference
        public static readonly byte[] ExtendedPrivateKeyBytes = {
            100, 245, 118, 3, 181, 138, 241, 105,
            7, 193, 138, 134, 97, 35, 40, 110,
            28, 188, 232, 151, 144, 97, 53, 88,
            220, 23, 117, 171, 179, 252, 92, 140,
            88, 110, 60, 141, 68, 125, 118, 121,
            34, 46, 19, 144, 51, 227, 130, 2,
            53, 227, 61, 165, 9, 30, 155, 11,
            184, 241, 161, 18, 207, 12, 143, 245
        };

        public static readonly byte[] PrivateKeyBytes = {
            100, 245, 118, 3, 181, 138, 241, 105,
            7, 193, 138, 134, 97, 35, 40, 110,
            28, 188, 232, 151, 144, 97, 53, 88,
            220, 23, 117, 171, 179, 252, 92, 140
        };

        public static readonly byte[] PrivateKeyBytesInvalidLength = {
            100, 245, 118, 3, 181, 138, 241, 105,
            7, 193, 138, 134, 97, 35, 40, 110,
            28, 188, 232, 151, 144, 97, 53, 88,
            220, 23, 117, 171, 179
        };

        public const string PrivateKeyHex = "0x64f57603b58af16907c18a866123286e1cbce89790613558dc1775abb3fc5c8c";

        public static readonly byte[] PrivateKeySerializedOutput =
        {
            32, 100, 245, 118, 3, 181, 138, 241,
            105, 7, 193, 138, 134, 97, 35, 40,
            110, 28, 188, 232, 151, 144, 97, 53,
            88, 220, 23, 117, 171, 179, 252, 92, 140
        };
    }
}