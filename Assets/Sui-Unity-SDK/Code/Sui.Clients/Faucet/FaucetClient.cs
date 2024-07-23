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
using Sui.Clients.Faucet.Request;
using Sui.Rpc;

namespace Sui.Clients
{
    /// <summary>
    /// <code>
    ///     curl --location --request POST 'https://faucet.devnet.sui.io/gas' \
    ///     --header 'Content-Type: application/json' \
    ///     --data-raw '{
    ///         "FixedAmountRequest": {
    ///             "recipient": "<YOUR SUI ADDRESS>"
    ///         }
    ///     }'
    /// </code>
    /// </summary>
    public class FaucetClient
    {
        public Connection Connection;

        public FaucetClient(Connection connection)
        {
            this.Connection = connection;
        }

        public async Task<bool> AirdropGasAsync(string recipient)
        {
            HttpClient client = new HttpClient();
            FaucetRequest req = new FaucetRequest(recipient);

            string reqJson = JsonConvert.SerializeObject(req);
            StringContent content = new StringContent(reqJson, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(Connection.FAUCET, content);
            return response.IsSuccessStatusCode;
        }
    }
}