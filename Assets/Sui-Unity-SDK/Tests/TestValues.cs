//
//  TestValues.cs
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

        public const string ValidSecretKeyBase64 = "mdqVWeFekT7pqy5T49+tV12jO0m+ESW7ki4zSU9JiCg=";

        public static readonly byte[] PrivateKeySerializedOutput =
        {
            32, 100, 245, 118, 3, 181, 138, 241,
            105, 7, 193, 138, 134, 97, 35, 40,
            110, 28, 188, 232, 151, 144, 97, 53,
            88, 220, 23, 117, 171, 179, 252, 92, 140
        };

        public static readonly byte[] SignatureBytes =
        {
            170, 66, 187, 194, 169, 252, 117, 27,
            238, 238, 59, 49, 43, 132, 82, 196,
            69, 199, 212, 171, 134, 152, 3, 107,
            12, 249, 242, 228, 106, 9, 139, 176,
            44, 54, 159, 188, 141, 254, 253, 35,
            26, 18, 141, 138, 75, 185, 173, 207,
            228, 94, 7, 24, 139, 117, 140, 58,
            211, 152, 215, 248, 78, 130, 239, 5
        };


        public static readonly  (string, string, string)[] TestCases
            = new (string RawPublicKey, string SuiPublicKey, string SuiAddress)[]
            {
                (   "UdGRWooy48vGTs0HBokIis5NK+DUjiWc9ENUlcfCCBE=",
                    "AFHRkVqKMuPLxk7NBwaJCIrOTSvg1I4lnPRDVJXHwggR",
                    "0xd77a6cd55073e98d4029b1b0b8bd8d88f45f343dad2732fc9a7965094e635c55"
                ),
                (   "0PTAfQmNiabgbak9U/stWZzKc5nsRqokda2qnV2DTfg=",
                    "AND0wH0JjYmm4G2pPVP7LVmcynOZ7EaqJHWtqp1dg034",
                    "0x7e8fd489c3d3cd9cc7cbcc577dc5d6de831e654edd9997d95c412d013e6eea23"
                ),
                (   "6L/l0uhGt//9cf6nLQ0+24Uv2qanX/R6tn7lWUJX1Xk=",
                    "AOi/5dLoRrf//XH+py0NPtuFL9qmp1/0erZ+5VlCV9V5",
                    "0x3a1b4410ebe9c3386a429c349ba7929aafab739c277f97f32622b971972a14a2"
                ),
            };

        public static readonly string ValidKeyBase64 = "Uz39UFseB/B38iBwjesIU1JZxY6y+TRL9P84JFw41W4=";
        public static readonly string ValidKeyHex = "0x533dfd505b1e07f077f220708deb08535259c58eb2f9344bf4ff38245c38d56e";
        public static readonly byte[] ValidKeyBytes =
        {
            83, 61, 253, 80, 91, 30, 7, 240, 119, 242, 32, 112, 141, 235, 8, 83, 82, 89, 197, 142, 178, 249, 52, 75, 244, 255, 56, 36, 92, 56, 213, 110
        };
    }
}