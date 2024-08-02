//
//  Utilities.cs
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

namespace OpenDive.BCS
{
    public static class ByteArrayComparer
    {
        /// <summary>
        /// Compares two byte arrays and returns either:
        /// - 1: LHS is greater.
        /// - 0: Both are equal.
        /// - -1: RHS is greater.
        /// </summary>
        /// <param name="array1">The left hand side.</param>
        /// <param name="array2">The right hand side.</param>
        /// <returns>An integer representing the equality value of the left and right hand side byte arrays.</returns>
        public static int Compare(byte[] array1, byte[] array2)
        {
            if (array1 == null && array2 == null)
                return 0;
            if (array1 == null)
                return -1;
            if (array2 == null)
                return 1;

            int minLength = Math.Min(array1.Length, array2.Length);

            for (int i = 0; i < minLength; i++)
            {
                if (array1[i] < array2[i])
                    return -1;
                if (array1[i] > array2[i])
                    return 1;
            }

            // If all compared elements are equal, the longer array is greater
            if (array1.Length < array2.Length)
                return -1;
            if (array1.Length > array2.Length)
                return 1;

            return 0; // Arrays are equal
        }

        /// <summary>
        /// Compares two tuple arrays of type Tuple<byte[]. byte[]>.
        /// The first item of both tuples are compared.
        /// </summary>
        /// <param name="tuple_1">The left hand side (first item).</param>
        /// <param name="tuple_2">The right hand side (first item).</param>
        /// <returns>An integer representing the equality value of the left and right hand side tuples.</returns>
        public static int CompareTuple(Tuple<byte[], byte[]> tuple_1, Tuple<byte[], byte[]> tuple_2)
            => ByteArrayComparer.Compare(tuple_1.Item1, tuple_2.Item1);
    }
}