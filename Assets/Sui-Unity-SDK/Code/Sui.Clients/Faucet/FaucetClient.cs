using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sui.Clients.Faucet.Request;
using UnityEngine;

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
        public async Task<bool> AirdropGasAsync(string recipient)
        {
            HttpClient client = new HttpClient();
            FaucetRequesst req = new FaucetRequesst(recipient);

            string reqJson = JsonConvert.SerializeObject(req);
            StringContent content = new StringContent(reqJson, Encoding.UTF8, "application/json");

            Debug.Log(reqJson);
            Debug.Log(content);

            // https://faucet.devnet.sui.io/gas
            var response = await client.PostAsync("https://faucet.devnet.sui.io/gas", content);
            Debug.Log(" ::: " + response.StatusCode + " ::: " + response.Content.ToString());
            return response.IsSuccessStatusCode;
        }
    }
}