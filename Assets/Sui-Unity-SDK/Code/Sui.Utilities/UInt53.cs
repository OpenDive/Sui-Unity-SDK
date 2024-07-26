using System;
using System.Globalization;
using System.Numerics;
using Newtonsoft.Json.Linq;

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