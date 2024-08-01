using System;
using UnityEngine;
using Sui.Accounts;
using Sui.Rpc;
using Sui.Clients;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;
using Sui.Rpc.Client;
using System.Linq;
using Sui.Rpc.Models;
using System.Text.RegularExpressions;
using System.Collections;
using OpenDive.BCS;
using Sui.Utilities;
using Sui.Types;
using Sui.Transactions;

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

    public class TestToolbox
    {
        public int DefaultGasBudget = 10_000_000;
        public int DefaultSendAmount = 1_000;
        public AccountAddress DefaultRecipient = AccountAddress.FromHex("0x0c567ffdf8162cb6d51af74be0199443b92e823d4ba6ced24de5c6c463797d46");

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

        public AccountAddress Address() => this.Account.SuiAddress();

        public async Task<RpcResult<CoinPage>> GetAllCoins()
        {
            return await this.Client.GetAllCoinsAsync(this.Account, new SuiRpcFilter(limit: 50));
        }

        public async Task<RpcResult<CoinPage>> GetCoins()
        {
            return await this.Client.GetCoinsAsync(this.Account, new SuiStructTag("0x2::sui::SUI"));
        }

        public async Task<List<SuiValidatorSummary>> GetActiveValidators()
        {
            RpcResult<SuiSystemSummary> system = await this.Client.GetLatestSuiSystemStateAsync();
            return system.Result.ActiveValidators.ToList();
        }

        public IEnumerator PublishPackage(string name, Action<SuiResult<PublishedPackage>> callback)
        {
            JObject file_data = GetModule(name);
            TransactionBlock tx = new TransactionBlock();

            JArray modules_jarray = (JArray)file_data["modules"];
            JArray dependencies_jarray = (JArray)file_data["dependencies"];

            List<string> modules = new List<string>();
            List<string> dependencies = new List<string>();

            foreach (JToken jtoken in modules_jarray.Values())
                modules.Add((string)jtoken);

            foreach (JToken jtoken in dependencies_jarray.Values())
                dependencies.Add((string)jtoken);

            List<TransactionArgument> cap = tx.AddPublishTx
            (
                modules.ToArray(),
                dependencies.ToArray()
            );

            tx.AddTransferObjectsTx(cap.ToArray(), this.Account.SuiAddress());

            TransactionBlockResponseOptions options = new TransactionBlockResponseOptions(showEffects: true, showObjectChanges: true);

            Task<RpcResult<TransactionBlockResponse>> published_tx_block = this.Client.SignAndExecuteTransactionBlockAsync(tx, this.Account, options);
            yield return new WaitUntil(() => published_tx_block.IsCompleted);

            if (published_tx_block.Result.Error != null)
                callback(new SuiResult<PublishedPackage>(null, published_tx_block.Result.Error));

            if (published_tx_block.Result.Result.Effects.Status.Status == ExecutionStatus.Failure)
                callback(new SuiResult<PublishedPackage>(null, new SuiError(0, $"Transaction failed with message - {published_tx_block.Result.Result.Effects.Status.Error}", null)));

            Task<RpcResult<TransactionBlockResponse>> transaction_wait = this.Client.WaitForTransaction(published_tx_block.Result.Result.Digest, options);
            yield return new WaitUntil(() => transaction_wait.IsCompleted);

            if (transaction_wait.Result.Result.ObjectChanges == null)
                callback(new SuiResult<PublishedPackage>(null, new SuiError(0, "Objects Change was not queried.", transaction_wait.Result)));

            List<ObjectChange> object_changes = transaction_wait.Result.Result.ObjectChanges.ToList();

            List<ObjectChangePublished> package_ids = object_changes.Select((obj) =>
            {
                if (obj.Change.GetType() == typeof(ObjectChangePublished))
                    return (ObjectChangePublished)obj.Change;
                else
                    return null;
            }).Where((value) => value != null).ToList();

            if (package_ids.Count() == 0)
                callback(new SuiResult<PublishedPackage>(null, new SuiError(0, "Package ID cannot be found.", object_changes)));

            string package_id = Regex.Replace(package_ids[0].PackageID.KeyHex, "^(0x)(0+)", "0x");

            Debug.Log($"Published package {package_id} from address {this.Address()}");

            callback(new SuiResult<PublishedPackage>(new PublishedPackage(package_id, published_tx_block.Result.Result)));
        }

        public AccountAddress[] GetRandomAddresses(int amount) => Enumerable.Range(0, amount).Select(_ => new Account().SuiAddress()).ToArray();

        public async Task<RpcResult<TransactionBlockResponse>> PaySui
        (
            int num_recipients = 1,
            AccountAddress[] recipients = null,
            int[] amounts = null,
            AccountAddress coin_id = null
        )
        {
            TransactionBlock tx_block = new TransactionBlock();

            AccountAddress[] recipients_tx = recipients ?? this.GetRandomAddresses(num_recipients);
            int[] amounts_tx =
                amounts ??=
                Enumerable.Range(0, num_recipients).Select(_ => this.DefaultSendAmount).ToArray();

            if (recipients_tx.Count() != amounts_tx.Count())
                return RpcResult<TransactionBlockResponse>.GetErrorResult("Recipients and amounts do not match.");

            AccountAddress coin_id_tx = coin_id ??= (await this.Client.GetCoinsAsync(this.Account, new SuiStructTag("0x2::sui::SUI"))).Result.Data[0].CoinObjectID;

            foreach (Tuple<int, AccountAddress> recipient in recipients_tx.Select((rec, i) => new Tuple<int, AccountAddress>(i, rec)))
            {
                List<TransactionArgument> coin = tx_block.AddSplitCoinsTx
                (
                    tx_block.AddObjectInput(coin_id_tx.KeyHex),
                    new TransactionArgument[]
                    {
                        tx_block.AddPure(new U64((ulong)amounts_tx[recipient.Item1]))
                    }
                );
                tx_block.AddTransferObjectsTx(coin.ToArray(), recipient.Item2);
            }

            RpcResult<TransactionBlockResponse> published_tx_block = await this.Client.SignAndExecuteTransactionBlockAsync
            (
                tx_block,
                this.Account,
                new TransactionBlockResponseOptions(showEffects: true, showObjectChanges: true)
            );

            if (published_tx_block.Error != null || published_tx_block.Result.Effects.Status.Status == ExecutionStatus.Failure)
                return RpcResult<TransactionBlockResponse>.GetErrorResult
                (
                    $"Transaction failed with message: {published_tx_block.Error.Message ?? published_tx_block.Result.Effects.Status.Error}"
                );

            return published_tx_block;
        }

        public async Task<SuiResult<IEnumerable<TransactionBlockResponse>>> ExecutePaySuiNTimes
        (
            int n_times,
            int num_recipients_per_txn = 1,
            AccountAddress[] recipients = null,
            int[] amounts = null
        )
        {
            List<TransactionBlockResponse> txns = new List<TransactionBlockResponse>();

            for (int i = 0; i < n_times; ++i)
            {
                RpcResult<TransactionBlockResponse> tx_response = await this.PaySui(num_recipients_per_txn, recipients, amounts);

                if (tx_response.Error != null || tx_response.Result.Effects.Status.Status == ExecutionStatus.Failure)
                    return SuiResult<IEnumerable<TransactionBlockResponse>>.GetSuiErrorResult
                    (
                        $"One of the Transactions failed with message: {tx_response.Error.Message ?? tx_response.Result.Effects.Status.Error}"
                    );

                await this.Client.WaitForTransaction(tx_response.Result.Digest);
                txns.Add(tx_response.Result);
            }

            return new SuiResult<IEnumerable<TransactionBlockResponse>>(txns);
        }

        public IEnumerator Setup()
        {
            bool is_initializing = true;
            while (is_initializing)
            {
                FaucetClient faucet = new FaucetClient(this.Client.Connection);
                Task<bool> result = faucet.AirdropGasAsync(this.Account);
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
