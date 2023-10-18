using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Rpc.Api;
using Sui.Rpc.Models;
using UnityEngine;

namespace Sui.Rpc
{
    public class RPCExample : MonoBehaviour
    {

        // Start is called before the first frame update
        void Start()
        {
            //_ = GetProtocolConfig();
            //_ = GetReferenceGasPrice();
            //_ = GetCoins();
            //_ = DryTransactionBlock();
            //_ = TestGetBalance();
            //_ = TestGetAllBalances();
            //_ = TestGetCoinMetadata();
            _ = TestGetTotalSupply();
        }

        private async Task TestClientTask()
        {
            string rpcUri = Constants.DevnetConnection.FULL_NODE;
            UnityRpcClient rpcClient = new UnityRpcClient(rpcUri);

            SuiClient client = new SuiClient(rpcClient);

            RpcResult<BigInteger> rpcResult = await client.GetTotalTransactionBlocksAsync();
            Debug.Log("IRVIN:::: " + rpcResult.Result);
        }

        private async Task GetProtocolConfig()
        {
            string rpcUri = Constants.DevnetConnection.FULL_NODE;
            UnityRpcClient rpcClient = new UnityRpcClient(rpcUri);

            SuiClient client = new SuiClient(rpcClient);

            RpcResult<ProtocolConfig> rpcResult = await client.GetProtocolConfigAsync();
            ProtocolConfig result = rpcResult.Result;
            Debug.Log("IRVIN:::: " + result.ProtocolVersion);
            Debug.Log("IRVIN:::: " + result.MaxSupportedProtocolVersion);
            Debug.Log("IRVIN:::: " + result.MinSupportedProtocolVersion);
            string json = JsonConvert.SerializeObject(result);
            Debug.Log("PROTOCOL CONFIG: " + json);
        }

        private async Task GetReferenceGasPrice()
        {
            string rpcUri = Constants.DevnetConnection.FULL_NODE;
            UnityRpcClient rpcClient = new UnityRpcClient(rpcUri);

            SuiClient client = new SuiClient(rpcClient);

            RpcResult<BigInteger> rpcResult = await client.GetReferenceGasPriceAsync();
            BigInteger result = rpcResult.Result;
            Debug.Log("IRVIN:::: " + result);
        }

        private async Task GetNormalizedMoveTransaction()
        {
            string rpcUri = Constants.DevnetConnection.FULL_NODE;
            UnityRpcClient rpcClient = new UnityRpcClient(rpcUri);

            SuiClient client = new SuiClient(rpcClient);

            RpcResult<NormalizedMoveFunctionResponse> rpcResult
                = await client.GetNormalizedMoveFunction("", "", "");
            NormalizedMoveFunctionResponse result = rpcResult.Result;
            Debug.Log("IRVIN:::: " + result);
        }

        private async Task GetCoins()
        {
            string rpcUri = Constants.DevnetConnection.FULL_NODE;
            UnityRpcClient rpcClient = new UnityRpcClient(rpcUri);

            SuiClient client = new SuiClient(rpcClient);

            RpcResult<CoinPage> rpcResult = await client.GetCoins(
                "0x4ebe7aef1474166caa8ce2dd5bd77d72469780c91b18eb424d6211510bc2ca98",
                "0x2::sui::SUI",
                //"0xe5c651321915b06c81838c2e370109b554a448a78d3a56220f798398dde66eab",
                null,
                3
                );
            CoinPage result = rpcResult.Result;
            Debug.Log("IRVIN:::: " + result);
            string json = JsonConvert.SerializeObject(result);
            Debug.Log("GET COINS :::: " + json);
        }

        private async Task DryTransactionBlock()
        {
            string rpcUri = Constants.DevnetConnection.FULL_NODE;
            UnityRpcClient rpcClient = new UnityRpcClient(rpcUri);

            SuiClient client = new SuiClient(rpcClient);

            //string txBytes_NOT_WORKING = "AAACACB7qR3cfnF89wjJNwYPBASHNuwz+xdG2Zml5YzVxnftgAEAT4LxyFh7mNZMAL+0bDhDvYv2zPp8ZahhOGmM0f3Kw9wCAAAAAAAAACCxDABG4pPAjOwPQHg9msS/SrtNf4IGR/2F0ZGD3ufH/wEBAQEBAAEAAGH7tbTzQqQL2/h/5KlGueONGM+P/HsAALl1F1x7apV2AejYx86GPzE9o9vZKoPvJtEouI/ma/JuDg0Jza9yfR2EAgAAAAAAAAAgzMqpegLMOpgEFnDhYJ23FOmFjJbp5GmFXxzzv9+X6GVh+7W080KkC9v4f+SpRrnjjRjPj/x7AAC5dRdce2qVdgoAAAAAAAAAoIYBAAAAAAAA";
            string txBytes = "AAABACAMVn/9+BYsttUa90vgGZRDuS6CPUumztJN5cbEY3l9RgEBAQABAADSTIAH0TbAKI1W7ZjQ3vTmECWRDZmXEByXG0LZJwl/QwDSTIAH0TbAKI1W7ZjQ3vTmECWRDZmXEByXG0LZJwl/Q+gDAAAAAAAAAHQ7pAsAAAAA";
            RpcResult<TransactionBlockResponse> rpcResult = await client.DryRunTransactionBlock(txBytes);
            TransactionBlockResponse result = rpcResult.Result;
            Debug.Log("IRVIN:::: " + result.ObjectChanges[0].Digest);
            string json = JsonConvert.SerializeObject(result);
            Debug.Log("GET TransactionBlockResponse :::: " + json);
        }

        private async Task TestGetBalance()
        {
            string rpcUri = Constants.DevnetConnection.FULL_NODE;
            UnityRpcClient rpcClient = new UnityRpcClient(rpcUri);

            SuiClient client = new SuiClient(rpcClient);
            Debug.Log("IRVIN:::: START REQUEST" );
            //Debug.Log("METHOD: " + Methods.suix_getBalance.ToString());
            AccountAddress address = AccountAddress.FromHex("0x4ebe7aef1474166caa8ce2dd5bd77d72469780c91b18eb424d6211510bc2ca98");
            Debug.Log("IRVIN:::: " + address.ToHex());
            SuiStructTag structTag = SuiStructTag.FromStr("0x2::sui::SUI");
            Debug.Log("IRVIN:::: " + structTag.ToString());

            RpcResult<Balance> rpcResult = await client.GetBalanceAsync(
                address,
                structTag
            );
            Balance balance = rpcResult.Result;
            Debug.Log("IRVIN:::: " + balance);
            Debug.Log("IRVIN:::: " + balance.cointType.ToString());
            Debug.Log("IRVIN:::: END REQUESET");
        }

        private async Task TestGetAllBalances()
        {
            string rpcUri = Constants.DevnetConnection.FULL_NODE;
            UnityRpcClient rpcClient = new UnityRpcClient(rpcUri);

            SuiClient client = new SuiClient(rpcClient);
            Debug.Log("IRVIN:::: START REQUEST");
            //Debug.Log("METHOD: " + Methods.suix_getBalance.ToString());
            AccountAddress address = AccountAddress.FromHex("0x4ebe7aef1474166caa8ce2dd5bd77d72469780c91b18eb424d6211510bc2ca98");
            //AccountAddress address = AccountAddress.FromHex("0xa2da382c0a40261c675cc73c20f3d94e2ee7d8ebcf21e9dab5012600d745cb0f");
            Debug.Log("IRVIN:::: " + address.ToHex());
            RpcResult<IEnumerable<Balance>> rpcResult = await client.GetAllBalancesAsync(
                address
            );
            Debug.Log("IRVIN:::: ~~~");
            List<Balance> balances = (List<Balance>)rpcResult.Result;
            foreach (Balance balance in balances)
            {
                Debug.Log("IRVIN ====:::: " + balance.cointType);
            }
            
            Debug.Log("IRVIN:::: END REQUESET");
        }

        private async Task TestGetCoinMetadata()
        {
            string rpcUri = Constants.DevnetConnection.FULL_NODE;
            UnityRpcClient rpcClient = new UnityRpcClient(rpcUri);

            SuiClient client = new SuiClient(rpcClient);
            Debug.Log("IRVIN:::: START REQUEST");
            //SuiStructTag structTag = SuiStructTag.FromStr("0x168da5bf1f48dafc111b0a488fa454aca95e0b5e::usdc::USDC");
            SuiStructTag structTag = SuiStructTag.FromStr("0x2::sui::SUI");
            Debug.Log("IRVIN:::: ~~~~~" + structTag.ToString());

            RpcResult<CoinMetadata> rpcResult = await client.GetCoinMetadata(
                structTag
            );

            CoinMetadata coinMetadata = rpcResult.Result;
            Debug.Log("IRVIN:::: " + coinMetadata);
            Debug.Log("IRVIN:::: " + coinMetadata.Id.ToHex());
            Debug.Log("IRVIN:::: " + coinMetadata.Description);
            Debug.Log("IRVIN:::: END REQUESET");
        }

        private async Task TestGetTotalSupply()
        {
            string rpcUri = Constants.MainnetConnection.FULL_NODE;
            UnityRpcClient rpcClient = new UnityRpcClient(rpcUri);

            SuiClient client = new SuiClient(rpcClient);
            Debug.Log("IRVIN:::: START REQUEST");
            // TODO: Discuss and identify a Coin we can test this on. Currently using Sui returns 0.
            SuiStructTag structTag = SuiStructTag.FromStr("0x2::sui::SUI");
            //SuiStructTag structTag = SuiStructTag.FromStr("0x5d4b302506645c37ff133b98c4b50a5ae14841659738d6d733d59d0d217a93bf::usdc::USDC");
            Debug.Log("IRVIN:::: ~~~~~" + structTag.ToString());

            RpcResult<TotalSupply> rpcResult = await client.GetTotalSupply(
                structTag
            );

            TotalSupply totalSupply = rpcResult.Result;
            Debug.Log("IRVIN:::: " + totalSupply.Value);
            Debug.Log("IRVIN:::: END REQUESET");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}