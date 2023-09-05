using System;

namespace Sui.Accounts
{
    public class AccountAddress
    {
        private static readonly int Length = 32;
        private readonly byte[] AddressBytes;

        /// <summary>
        /// Create an AccountAddress object from a byte array representation.
        /// </summary>
        /// <param name="address"></param>
        public AccountAddress(byte[] address)
        {
            if (address.Length != Length)
                throw new ArgumentException("Invalid address length. It must be " + Length + " bytes");
            AddressBytes = address;
        }
    }
}