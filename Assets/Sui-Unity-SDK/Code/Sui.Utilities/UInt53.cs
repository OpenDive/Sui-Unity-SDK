//
//  UInt53.cs
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
using System.Globalization;
using System.Numerics;
using Newtonsoft.Json.Linq;

namespace Sui.Utilities
{
    // TODO: Look into further implementations in the Sui ecosystem.
    public struct UInt53 : IEquatable<UInt53>, IComparable<UInt53>
    {
        private readonly ulong _value;

        public UInt53(ulong value)
        {
            if (value > (1UL << 53) - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be greater than 2^53 - 1.");
            }

            _value = value;
        }

        public static UInt53 Parse(JToken value)
        {
            if (value.Type != JTokenType.Integer)
            {
                throw new ArgumentException("Expected an integer.", nameof(value));
            }

            ulong n = value.Value<ulong>();
            return new UInt53(n);
        }

        public JToken ToJToken()
        {
            return new JValue(_value);
        }

        public override string ToString()
        {
            return _value.ToString(CultureInfo.InvariantCulture);
        }

        public static implicit operator UInt53(ulong value)
        {
            return new UInt53(value);
        }

        public static implicit operator UInt53(long value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be negative.");
            }
            return new UInt53((ulong)value);
        }

        public static implicit operator ulong(UInt53 value)
        {
            return value._value;
        }

        public static implicit operator long(UInt53 value)
        {
            return (long)value._value;
        }

        public static implicit operator BigInteger(UInt53 value)
        {
            return new BigInteger(value._value);
        }

        public int CompareTo(UInt53 other)
        {
            return _value.CompareTo(other._value);
        }

        public bool Equals(UInt53 other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            return obj is UInt53 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static bool operator ==(UInt53 left, UInt53 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(UInt53 left, UInt53 right)
        {
            return !(left == right);
        }

        public static bool operator <(UInt53 left, UInt53 right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(UInt53 left, UInt53 right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(UInt53 left, UInt53 right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(UInt53 left, UInt53 right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}