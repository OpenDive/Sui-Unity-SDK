using System;
using UnityEngine;
using Sui.Accounts;
using Sui.Rpc;
using Sui.Clients;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;
using Sui.Transactions;
using System.Linq;
using Sui.Transactions.Types.Arguments;
using Sui.Rpc.Models;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Collections;
using OpenDive.BCS;

namespace Sui.Tests
{
    public class PublishedPackage
    {
        public string PackageID;
        public TransactionBlockResponse PublishedTX;

        public PublishedPackage(string package_id, TransactionBlockResponse published_tx)
        {
            this.PackageID = package_id;
            this.PublishedTX = published_tx;
        }
    }

    public class TestToolbox: MonoBehaviour
    {
        public int DefaultGasBudget = 10_000_000;
        public int DefaultSendAmount = 1_000;
        string DefaultRecipient = "0x0c567ffdf8162cb6d51af74be0199443b92e823d4ba6ced24de5c6c463797d46";
        private readonly string customResourcePath = Path.Combine(Application.dataPath, "Sui-Unity-SDK/Tests/Resources");

        public Account Account;
        public SuiClient Client;

        public TestToolbox
        (
            Account account,
            SuiClient client = null
        )
        {
            this.Account = account;
            this.Client = client;
        }

        public TestToolbox()
        {
            this.Account = new Account();
            this.Client = new SuiClient(Constants.LocalnetConnection);
        }

        public string Address()
        {
            return this.Account.SuiAddress();
        }

        public async Task<RpcResult<CoinPage>> GetAllCoins()
        {
            return await this.Client.GetAllCoins(this.Account.SuiAddress(), 50);
        }

        public async Task<RpcResult<CoinPage>> GetCoins()
        {
            return await this.Client.GetCoins(this.Account.SuiAddress(), "0x2::sui::SUI");
        }

        public async Task<List<Validator>> GetActiveValidators()
        {
            RpcResult<SuiSystemSummary> system = await this.Client.GetLatestSuiSystemState();
            return system.Result.ActiveValidators;
        }

        public async Task<PublishedPackage> PublishPackage(string name)
        {
            JObject file_data = GetModule(name);
            Transactions.TransactionBlock tx = new Transactions.TransactionBlock();

            JArray modules_jarray = (JArray)file_data["modules"];
            JArray dependencies_jarray = (JArray)file_data["dependencies"];

            List<string> modules = new List<string>();
            List<string> dependencies = new List<string>();

            foreach (JToken jtoken in modules_jarray.Values())
                modules.Add((string)jtoken);

            foreach (JToken jtoken in dependencies_jarray.Values())
                dependencies.Add((string)jtoken);

            List<SuiTransactionArgument> cap = tx.AddPublishTx
            (
                modules.ToArray(),
                dependencies.ToArray()
            );

            tx.AddTransferObjectsTx(cap.ToArray(), this.Account.SuiAddress());

            TransactionBlockResponseOptions options = new TransactionBlockResponseOptions(showEffects: true, showObjectChanges: true);

            RpcResult<TransactionBlockResponse> published_tx_block = await this.Client.SignAndExecuteTransactionBlock(tx, this.Account, options);

            if(published_tx_block.Result.Effects.Status.Status == ExecutionStatus.Failure)
                throw new Exception("Transaction Failed");

            await this.Client.WaitForTransaction(published_tx_block.Result.Digest);

            if (published_tx_block.Result.ObjectChanges == null) 
                throw new Exception("Objects Change was not Queried");

            List<ObjectChange> object_changes = published_tx_block.Result.ObjectChanges;

            List<PublishedObjectChange> package_ids = object_changes.Select((obj) =>
            {
                if (obj.GetType() == typeof(PublishedObjectChange))
                    return (PublishedObjectChange)obj;
                else
                    return null;
            }).Where((value) => value != null).ToList();

            if (package_ids.Count() == 0)
                throw new Exception("Package ID cannot be found");

            string package_id = Regex.Replace(package_ids[0].PackageId, "^(0x)(0+)", "0x");

            Debug.Log($"Published package {package_id} from address {this.Address()}");

            return new PublishedPackage(package_id, published_tx_block.Result);
        }

        public string[] GetRandomAddresses(int amount) => Enumerable.Range(0, amount).Select(_ => new Account().SuiAddress()).ToArray();

        public async Task<RpcResult<TransactionBlockResponse>> PaySui
        (
            int num_recipients = 1,
            string[] recipients = null,
            int[] amounts = null,
            string coin_id = null
        )
        {
            Transactions.TransactionBlock tx_block = new Transactions.TransactionBlock();

            string[] recipients_tx = recipients ??= this.GetRandomAddresses(num_recipients);
            int[] amounts_tx =
                amounts ??=
                Enumerable.Range(0, num_recipients).Select(_ => this.DefaultSendAmount).ToArray();

            if (recipients_tx.Count() != amounts_tx.Count())
                return RpcResult<TransactionBlockResponse>.GetErrorResult("Recipients and amounts do not match.");

            string coin_id_tx = coin_id ??= (await this.Client.GetCoins(this.Address(), "0x2::sui::SUI")).Result.Data[0].CoinObjectId;

            foreach (Tuple<int, string> recipient in recipients_tx.Select((rec, i) => new Tuple<int, string>(i, rec)))
            {
                List<SuiTransactionArgument> coin = tx_block.AddSplitCoinsTx
                (
                    tx_block.AddObjectInput(coin_id_tx),
                    new SuiTransactionArgument[]
                    {
                        tx_block.AddPure(new U64((ulong)amounts_tx[recipient.Item1]))
                    }
                );
                tx_block.AddTransferObjectsTx(coin.ToArray(), recipient.Item2);
            }

            RpcResult<TransactionBlockResponse> published_tx_block = await this.Client.SignAndExecuteTransactionBlock
            (
                tx_block,
                this.Account,
                new TransactionBlockResponseOptions(showEffects: true, showObjectChanges: true)
            );

            if (published_tx_block.Error != null || published_tx_block.Result.Effects.Status.Status == ExecutionStatus.Failure)
                return RpcResult<TransactionBlockResponse>.GetErrorResult
                (
                    $"Transaction failed with message: {published_tx_block.Error.Message ??= published_tx_block.Result.Effects.Status.Error}"
                );

            return published_tx_block;
        }

        public async Task<RpcResult<IEnumerable<TransactionBlockResponse>>> ExecutePaySuiNTimes
        (
            int n_times,
            int num_recipients_per_txn = 1,
            string[] recipients = null,
            int[] amounts = null
        )
        {
            List<TransactionBlockResponse> txns = new List<TransactionBlockResponse>();

            for (int i = 0; i < n_times; ++i)
            {
                RpcResult<TransactionBlockResponse> tx_response = await this.PaySui(num_recipients_per_txn, recipients, amounts);

                if (tx_response.Error != null || tx_response.Result.Effects.Status.Status == ExecutionStatus.Failure)
                    return RpcResult<IEnumerable<TransactionBlockResponse>>.GetErrorResult
                    (
                        $"One of the Transactions failed with message: {tx_response.Error.Message ??= tx_response.Result.Effects.Status.Error}"
                    );

                await this.Client.WaitForTransaction(tx_response.Result.Digest);
                txns.Add(tx_response.Result);
            }

            return new RpcResult<IEnumerable<TransactionBlockResponse>>(txns);
        }

        public IEnumerator Setup()
        {
            bool is_initializing = true;
            while (is_initializing)
            {
                FaucetClient faucet = new FaucetClient(this.Client.Connection);
                Task<bool> result = faucet.AirdropGasAsync(this.Account.SuiAddress());
                yield return new WaitUntil(() => result.IsCompleted);

                if (result.Result)
                    is_initializing = false;
                else
                {
                    Debug.Log("Retrying requesting from faucet...");
                    yield return new WaitForSeconds(60);
                }
            }
        }

        private JObject GetModule(string name)
        {
            try
            {
                // Construct the full path to the JSON file
                string filePath = Path.Combine(customResourcePath, $"{name}.json");

                // Check if the file exists
                if (!File.Exists(filePath))
                {
                    Debug.LogError("Package is missing");
                    throw new Exception("Package is missing");
                }

                // Read the file content
                string jsonContent = File.ReadAllText(filePath);

                // Parse the JSON content
                JObject jsonObject = JObject.Parse(jsonContent);

                return jsonObject;
            }
            catch (FileNotFoundException)
            {
                Debug.LogError("Package is corrupted");
                throw new Exception("Package is corrupted");
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                throw;
            }
        }
    }
}
