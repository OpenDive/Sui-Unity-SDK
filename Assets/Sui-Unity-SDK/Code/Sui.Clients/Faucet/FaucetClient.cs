//
//  FaucetClient.cs
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

using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sui.Accounts;
using Sui.Clients.Faucet.Request;
using Sui.Cryptography;
using Sui.Rpc;

namespace Sui.Clients
{
    /// <summary>
    /// The client used to faucet SUI coins to a Sui account.
    /// </summary>
    public class FaucetClient
    {
        /// <summary>
        /// The connection used for fauceting the account.
        ///
        /// NOTE: The connection must have a Faucet endpoint.
        /// </summary>
        public Connection Connection;

        public FaucetClient(Connection connection)
        {
            this.Connection = connection;
        }

        /// <summary>
        /// Airdrops a set amount of SUI tokens to the recipient account.
        /// </summary>
        /// <param name="recipient">The recipient of the fauceted tokens.</param>
        /// <returns>An asynchronous value that represents a bool of whether the faucet suceeded or not.</returns>
        public async Task<bool> AirdropGasAsync(AccountAddress recipient)
        {
            HttpClient client = new HttpClient();
            FaucetRequest req = new FaucetRequest(recipient.KeyHex);

            StringContent content = new StringContent
            (
                JsonConvert.SerializeObject(req),
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = await client.PostAsync(this.Connection.FAUCET, content);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Airdrops a set amount of SUI tokens to the recipient account.
        /// </summary>
        /// <param name="recipient">The recipient of the fauceted tokens.</param>
        /// <returns>An asynchronous value that represents a bool of whether the faucet suceeded or not.</returns>
        public async Task<bool> AirdropGasAsync(SuiPublicKeyBase recipient)
        {
            HttpClient client = new HttpClient();
            FaucetRequest req = new FaucetRequest(recipient.ToSuiAddress().KeyHex);

            StringContent content = new StringContent
            (
                JsonConvert.SerializeObject(req),
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = await client.PostAsync(this.Connection.FAUCET, content);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Airdrops a set amount of SUI tokens to the recipient account.
        /// </summary>
        /// <param name="recipient">The recipient of the fauceted tokens.</param>
        /// <returns>An asynchronous value that represents a bool of whether the faucet suceeded or not.</returns>
        public async Task<bool> AirdropGasAsync(Account recipient)
        {
            HttpClient client = new HttpClient();
            FaucetRequest req = new FaucetRequest(recipient.SuiAddress().KeyHex);

            StringContent content = new StringContent
            (
                JsonConvert.SerializeObject(req),
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = await client.PostAsync(this.Connection.FAUCET, content);

            return response.IsSuccessStatusCode;
        }
    }
}