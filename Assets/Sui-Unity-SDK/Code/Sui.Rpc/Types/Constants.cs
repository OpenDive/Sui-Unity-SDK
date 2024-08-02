//
//  Constants.cs
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

namespace Sui.Rpc
{
    /// <summary>
    /// Constants that represent different types of Sui network connections.
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// Represents a connection to a local instance of a Sui network.
        /// </summary>
        public static Connection LocalnetConnection = new Connection(
            "http://127.0.0.1:9000",
            "http://127.0.0.1:9123/gas"
        );

        /// <summary>
        /// Represents a connection to Sui's developer network.
        /// </summary>
        public static Connection DevnetConnection = new Connection(
            "https://fullnode.devnet.sui.io:443/",
            "https://faucet.devnet.sui.io/gas"
        );

        /// <summary>
        /// Represents a connection to Sui's test network.
        /// </summary>
        public static Connection TestnetConnection = new Connection(
            "https://fullnode.testnet.sui.io:443/",
            "https://faucet.testnet.sui.io/gas"
        );

        /// <summary>
        /// <para>Represents a connection to Sui's main network.</para>
        ///
        /// <para>Note: This connection does not have a faucet node.</para>
        /// </summary>
        public static Connection MainnetConnection = new Connection(
            "https://fullnode.mainnet.sui.io:443/"
        );
    }
}