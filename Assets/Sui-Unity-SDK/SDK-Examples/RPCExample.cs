using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using NBitcoin;
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
            //_ = TestGetTotalSupply();
            //_ = TestGetCommitteeInfo();
            //_ = TestGetCommitteeInfoNoParams();
            //_ = TestGetValidatorsApy();
            //_ = TestGetStakes();
            //_ = TestGetStakesById();
            //_ = TestGetSuiSystemState();
            //_ = TestResolveNameServiceAddress();
            //_ = GetLoadedChildObjects();
            //_ = GetNormalizedModule();
            //_ = GetNormalizedMoveStruct();
            _ = GetNormalizedMoveModulesByPackage();
            //_ = GetMoveFunctionArgTypes();
        }

        private async Task GetNormalizedMoveStruct()
        {
            SuiClient client = new SuiClient(Constants.MainnetConnection);
            string package = "ef9124bfbeefc494e74ef7d4f4394018b7a094ccccb9a149a67eb04d4f79c034";
            string moduleName = "arcade_champion";
            string structure = "Hero";

            RpcResult<SuiMoveNormalizedStruct> rpcResult = await client.GetNormalizedMoveStruct(package, moduleName, structure);
            SuiMoveNormalizedStruct normalizedStruct = rpcResult.Result;
            Debug.Log($"MARCUS:::: {normalizedStruct}");
            foreach (SuiMoveNormalizedField field in normalizedStruct.Fields)
            {
                Debug.Log($"MARCUS Struct Keys Field Name :::: {field.Name}  Type:::: {field.Type}");
            }

        }

        private async Task GetMoveFunctionArgTypes()
        {
            SuiClient client = new SuiClient(Constants.MainnetConnection);
            string package = "0xe2c9fdc9d962093a8f7bddc876eb30e9da7fb2124e90dc8534e1252253edceeb";
            string module = "nft_example";
            string function = "mint_to_sender";

            RpcResult<MoveFunctionArgTypes> rpcResult = await client.GetMoveFunctionArgTypes(package, module, function);

            foreach (MoveFunctionArgType arg in rpcResult.Result.ArgTypes)
            {
                Debug.Log($"MARCUS:::: {arg}");
            }
        }

        private async Task GetNormalizedMoveModulesByPackage()
        {
            SuiClient client = new SuiClient(Constants.MainnetConnection);
            string package = "903bee129a0790ed375b9266ccd02c81b6eb00e6bc0b353ef0fe69c68e365065";
            RpcResult<Dictionary<string, SuiMoveNormalizedModule>> rpcResult = await client.GetNormalizedMoveModulesByPackage(package);
            Debug.Log($"MARCUS:::: {rpcResult.Result}");

            foreach (KeyValuePair<string, SuiMoveNormalizedModule> result in rpcResult.Result)
            {
                Debug.Log($"MARCUS:::: KEY - {result.Key}");
                Debug.Log($"MARCUS:::: VALUE'S ADDRESS - {result.Value.Address}");
            }
        }

        private async Task GetEvents()
        {
            SuiClient client = new SuiClient(Constants.MainnetConnection);
            string transactionDigest = "32vzvgcc49wJiRxmf9RkLNq4Cu21NYUeKfBq3v4oLyZT";
            RpcResult<Models.Event[]> rpcResult = await client.GetEvents(transactionDigest);
            string json = JsonConvert.SerializeObject(rpcResult.Result, Formatting.Indented);
            Debug.Log($"MARCUS:::: {json}");
        }

        private async Task GetNormalizedModule()
        {
            SuiClient client = new SuiClient(Constants.MainnetConnection);
            string package = "ef9124bfbeefc494e74ef7d4f4394018b7a094ccccb9a149a67eb04d4f79c034";
            string moduleName = "arcade_champion";
            RpcResult<SuiMoveNormalizedModule> rpcResult = await client.GetNormalizedMoveModule(package, moduleName);
            SuiMoveNormalizedModule normalizedModule = rpcResult.Result;
            Debug.Log($"MARCUS:::: {normalizedModule.Address}");
            foreach(string key in normalizedModule.Structs.Keys)
            {
                Debug.Log($"MARCUS Struct Keys :::: {key}");
                Debug.Log($"MARCUS Struct Fields Length :::: {normalizedModule.Structs[key].Fields.Length}");
                foreach(SuiMoveNormalizedField field in normalizedModule.Structs[key].Fields)
                {
                    Debug.Log($"MARCUS Struct Keys Field Name :::: {field.Name}  Type:::: {field.Type}");
                }
            }
        }

        private async Task GetCheckpoints()
        {
            SuiClient client = new SuiClient(Constants.MainnetConnection);
            RpcResult<Checkpoints> rpcResult = await client.GetCheckpoints("26178", 4, false);
            Debug.Log($"MARCUS:::: {rpcResult.Result}");
        }

        private async Task GetCheckpoint()
        {
            SuiClient client = new SuiClient(Constants.MainnetConnection);
            RpcResult<Checkpoint> rpcResult = await client.GetCheckpoint("26178");
            string json = JsonConvert.SerializeObject(rpcResult.Result, Formatting.Indented);
            Debug.Log($"MARCUS:::: {json}");
        }

        private async Task GetChainId()
        {
            SuiClient client = new SuiClient(Constants.MainnetConnection);
            RpcResult<string> rpcResult = await client.GetChainIdentifier();
            Debug.Log($"MARCUS:::: {rpcResult.Result}");
        }

        private async Task GetLoadedChildObjects()
        {
            SuiClient client = new SuiClient(Constants.MainnetConnection);
            string transactionDigest = "32vzvgcc49wJiRxmf9RkLNq4Cu21NYUeKfBq3v4oLyZT";
            RpcResult<ChildObjects> rpcResult = await client.GetLoadedChildObjects(transactionDigest);
            Debug.Log($"MARCUS:::: {rpcResult.Result}");
        }

        private async Task TestClientTask()
        {
            SuiClient client = new SuiClient(Constants.MainnetConnection);

            RpcResult<BigInteger> rpcResult = await client.GetTotalTransactionBlocksAsync();
            Debug.Log("IRVIN:::: " + rpcResult.Result);
        }

        private async Task GetProtocolConfig()
        {
            SuiClient client = new SuiClient(Constants.MainnetConnection);

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
            SuiClient client = new SuiClient(Constants.MainnetConnection);

            RpcResult<ulong> rpcResult = await client.GetReferenceGasPriceAsync();
            BigInteger result = rpcResult.Result;
            Debug.Log("IRVIN:::: GET REFERENCE PRICE RESULT " + result);
        }

        private async Task GetNormalizedMoveTransaction()
        {
            SuiClient client = new SuiClient(Constants.MainnetConnection);

            RpcResult<NormalizedMoveFunctionResponse> rpcResult
                = await client.GetNormalizedMoveFunction("", "", "");
            NormalizedMoveFunctionResponse result = rpcResult.Result;
            Debug.Log("IRVIN:::: " + result);
        }

        private async Task GetCoins()
        {
            SuiClient client = new SuiClient(Constants.MainnetConnection);

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
            SuiClient client = new SuiClient(Constants.MainnetConnection);

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
            SuiClient client = new SuiClient(Constants.MainnetConnection);
            Debug.Log("IRVIN:::: START REQUEST" );
            //Debug.Log("METHOD: " + Methods.suix_getBalance.ToString());
            AccountAddress address = AccountAddress.FromHex("0x4ebe7aef1474166caa8ce2dd5bd77d72469780c91b18eb424d6211510bc2ca98");
            Debug.Log("IRVIN:::: " + address.ToHex());
            SuiStructTag structTag = SuiStructTag.FromStr("0x2::sui::SUI");
            Debug.Log("IRVIN:::: " + structTag.ToString());

            RpcResult<Balance> rpcResult = await client.GetBalanceAsync(
                address.ToHex(),
                structTag.ToString()
            );
            Balance balance = rpcResult.Result;
            Debug.Log("IRVIN:::: " + balance);
            Debug.Log("IRVIN:::: " + balance.cointType.ToString());
            Debug.Log("IRVIN:::: END REQUESET");
        }

        private async Task TestGetAllBalances()
        {
            SuiClient client = new SuiClient(Constants.MainnetConnection);
            Debug.Log("IRVIN:::: START REQUEST");
            //Debug.Log("METHOD: " + Methods.suix_getBalance.ToString());
            AccountAddress address = AccountAddress.FromHex("0x4ebe7aef1474166caa8ce2dd5bd77d72469780c91b18eb424d6211510bc2ca98");
            //AccountAddress address = AccountAddress.FromHex("0xa2da382c0a40261c675cc73c20f3d94e2ee7d8ebcf21e9dab5012600d745cb0f");
            Debug.Log("IRVIN:::: " + address.ToHex());
            RpcResult<IEnumerable<Balance>> rpcResult = await client.GetAllBalancesAsync(
                address.ToHex()
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
            SuiClient client = new SuiClient(Constants.MainnetConnection);
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
            SuiClient client = new SuiClient(Constants.MainnetConnection);
            Debug.Log("IRVIN:::: START REQUEST");
            // TODO: Discuss and identify a Coin we can test this on. Currently using Sui returns 0.
            SuiStructTag structTag = SuiStructTag.FromStr("0x2::sui::SUI");
            //SuiStructTag structTag = SuiStructTag.FromStr("0x5d4b302506645c37ff133b98c4b50a5ae14841659738d6d733d59d0d217a93bf::usdc::USDC");
            Debug.Log("IRVIN:::: ~~~~~" + structTag.ToString());

            RpcResult<TotalSupply> rpcResult = await client.GetTotalSupply(
                structTag.ToString()
            );

            TotalSupply totalSupply = rpcResult.Result;
            Debug.Log("IRVIN:::: " + totalSupply.Value);
            Debug.Log("IRVIN:::: END REQUESET");
        }

        private async Task TestGetCommitteeInfo()
        {
            SuiClient client = new SuiClient(Constants.MainnetConnection);
            Debug.Log("IRVIN:::: START REQUEST");

            RpcResult<CommitteeInfo> rpcResult = await client.GetCommitteeInfo(
                1000000000
            );

            CommitteeInfo committeeInfo = rpcResult.Result;
            Debug.Log("IRVIN:::: " + committeeInfo.Epoch);
            Debug.Log("IRVIN:::: " + committeeInfo.Validators[0].AuthorityName);
            Debug.Log("IRVIN:::: END REQUESET");
        }

        private async Task TestGetCommitteeInfoNoParams()
        {
            SuiClient client = new SuiClient(Constants.MainnetConnection);
            Debug.Log("IRVIN:::: START REQUEST");

            RpcResult<CommitteeInfo> rpcResult = await client.GetCommitteeInfo();

            CommitteeInfo committeeInfo = rpcResult.Result;
            Debug.Log("IRVIN:::: " + committeeInfo.Epoch);
            Debug.Log("IRVIN:::: " + committeeInfo.Validators[0].AuthorityName);
            Debug.Log("IRVIN:::: END REQUESET");
        }

        private async Task TestGetValidatorsApy()
        {
            SuiClient client = new SuiClient(Constants.MainnetConnection);
            Debug.Log("IRVIN:::: START REQUEST");

            RpcResult<ValidatorsApy> rpcResult = await client.GetValidatorsApy();

            ValidatorsApy validatorsApy = rpcResult.Result;
            Debug.Log("IRVIN:::: " + validatorsApy.Epoch);
            foreach(ValidatorsApy.ValidatorApy validatorApy in validatorsApy.Apys)
            {
                Debug.Log("IRVIN:::: " + validatorApy.Address.ToHex());
                Debug.Log("IRVIN:::: " + validatorApy.Apy);
            }
            Debug.Log("IRVIN:::: END REQUESET");
        }

        private async Task TestGetStakes()
        {
            SuiClient client = new SuiClient(Constants.MainnetConnection);
            Debug.Log("IRVIN:::: START REQUEST");

            AccountAddress owner = AccountAddress.FromHex("0x8a0907e2990baebbbb87c12821db4845b034e45f937167e68b4925ac3465335a");
            RpcResult<IEnumerable<Stakes>> rpcResult = await client.GetStakes(owner);

            List<Stakes> stakes = (List<Stakes>)rpcResult.Result;
            Debug.Log("IRVIN:::: " + stakes.Count);
            foreach(Stakes stakePool in stakes)
            {
                Debug.Log("IRVIN:::: VALIDATOR: " + stakePool.ValidatorAddress.ToHex());
                foreach (Stakes.Stake stake in stakePool.StakeList)
                {
                    Debug.Log("IRVIN:::: StakedSuiId: " + stake.StakedSuiId.ToHex());
                    Debug.Log("IRVIN:::: " + stake.status);
                }
                Debug.Log("IRVIN:::: END REQUESET");
            }
        }

        private async Task TestGetStakesById()
        {
            SuiClient client = new SuiClient(Constants.MainnetConnection);
            Debug.Log("IRVIN:::: START REQUEST");

            AccountAddress owner = AccountAddress.FromHex("0x8a0907e2990baebbbb87c12821db4845b034e45f937167e68b4925ac3465335a");

            List<AccountAddress> stakedSuiId = new List<AccountAddress>() {
                AccountAddress.FromHex("0x752b18cfa44304b9bbb110ba95211556f79c62bc2916c23eeb7aea959f5b3463"),
                AccountAddress.FromHex("0x8ecaf4b95b3c82c712d3ddb22e7da88d2286c4653f3753a86b6f7a216a3ca518"),
                AccountAddress.FromHex("0xe0e042d1aacba8abf6f32be86c555a08eae5ba2b7972c912f7451104391e8c57")
            };

            RpcResult<IEnumerable<Stakes>> rpcResult = await client.GetStakesByIds(stakedSuiId);

            List<Stakes> stakes = (List<Stakes>)rpcResult.Result;
            Debug.Log("IRVIN:::: " + stakes.Count);
            foreach (Stakes stakePool in stakes)
            {
                Debug.Log("IRVIN:::: VALIDATOR: " + stakePool.ValidatorAddress.ToHex());
                foreach (Stakes.Stake stake in stakePool.StakeList)
                {
                    Debug.Log("IRVIN:::: StakedSuiId: " + stake.StakedSuiId.ToHex());
                    Debug.Log("IRVIN:::: " + stake.status);
                }
                Debug.Log("IRVIN:::: END REQUESET");
            }
        }

        private async Task TestGetSuiSystemState()
        {
            SuiClient client = new SuiClient(Constants.MainnetConnection);
            Debug.Log("IRVIN:::: START REQUEST");

            RpcResult<SuiSystemSummary> rpcResult = await client.GetLatestSuiSystemState();

            SuiSystemSummary systemSummary = (SuiSystemSummary)rpcResult.Result;
            Debug.Log("IRVIN:::: " + systemSummary.Epoch);
        }

        private async Task TestResolveNameServiceAddress()
        {
            SuiClient client = new SuiClient(Constants.MainnetConnection);
            Debug.Log("IRVIN:::: START REQUEST");

            RpcResult<AccountAddress> rpcResult = await client.ResolveNameServiceAddress("example.sui");

            AccountAddress address = (AccountAddress)rpcResult.Result;
            Debug.Log("IRVIN:::: " + address.ToHex());
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}