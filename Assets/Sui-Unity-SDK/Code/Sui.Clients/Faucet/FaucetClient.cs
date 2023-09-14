using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sui.Clients.Faucet.Request;

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
            FixedAmountRequest req = new FixedAmountRequest(recipient);

            string reqJson = JsonConvert.SerializeObject(req);
            StringContent content = new StringContent(reqJson, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://faucet.testnet.sui.io/gas", content);
            return response.IsSuccessStatusCode;
        }
    }
}