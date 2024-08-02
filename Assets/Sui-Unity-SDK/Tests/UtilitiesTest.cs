//
//  UtilitiesTest.cs
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

using NUnit.Framework;

namespace Sui.Tests.Cryptography
{
    public class UtilitiesTest
    {
        [Test]
        public void IsValidEd25519HexKey()
        {
            string validPkHex = "0x99da9559e15e913ee9ab2e53e3dfad575da33b49be1125bb922e33494f498828";
            bool isValidHex = Utilities.Utils.IsValidHexKey(validPkHex);

            Assert.AreEqual(true, isValidHex);
        }

        [Test]
        public void IsInvalidEd25519HexKey()
        {
            string invalidPkHex1 = "0x99da9559e15e913ee9ab2e53e3dfad575da33b49be1125bb92";
            string invalidPkHex2 = "0x99da9559e15e913ee9ab2e53e3dfad5!5da33b49be1125bb922e33494f498828";

            bool isValidHex1 = Utilities.Utils.IsValidHexKey(invalidPkHex1);
            Assert.AreEqual(false, isValidHex1);
            bool isValidHex2 = Utilities.Utils.IsValidHexKey(invalidPkHex2);
            Assert.AreEqual(false, isValidHex2);
        }
    }
}