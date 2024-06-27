using System;
using Sui.Accounts;
using Sui.Rpc;
using Sui.Clients;
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using Sui.Rpc.Models;

namespace Sui.Tests
{
    public class TestToolbox : MonoBehaviour
    {
        int DefaultGasBudget = 10_000_000;
        int DefaultSendAmount = 1_000;
        string DefaultRecipient = "0x0c567ffdf8162cb6d51af74be0199443b92e823d4ba6ced24de5c6c463797d46";

        Account Account;
        SuiClient Client;

        public TestToolbox
        (
            Account account,
            SuiClient client = null,
            bool needs_funds = true
        )
        {
            this.Account = account;
            this.Client = client;

            if (needs_funds)
               Setup();
        }

        public TestToolbox(bool needs_funds = true)
        {
            this.Account = new Account();
            this.Client = new SuiClient(Constants.LocalnetConnection);

            if (needs_funds)
                Setup();
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
            return await this.Client.GetCoins(this.Account.SuiAddress(), "0x2::sui::SUI", 50);
        }

        public async void Setup()
        {
            await Task.Run(async () => {
                bool is_initializing = true;
                while (is_initializing)
                {
                    FaucetClient faucet = new FaucetClient(this.Client.Connection);
                    var result = await faucet.AirdropGasAsync(this.Account.SuiAddress());

                    if (result)
                        is_initializing = false;
                    else
                    {
                        Debug.Log("Retrying requesting from faucet...");
                        await new WaitForSeconds(60).Await();
                    }
                }
            });
        }
    }

    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner _instance;
        public static CoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    var obj = new GameObject("CoroutineRunner");
                    _instance = obj.AddComponent<CoroutineRunner>();
                    DontDestroyOnLoad(obj);
                }
                return _instance;
            }
        }
    }

    public static class TaskExtensions
    {
        public static Task Await(this YieldInstruction instruction)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            CoroutineRunner.Instance.StartCoroutine(AwaitCoroutine(instruction, taskCompletionSource));

            return taskCompletionSource.Task;
        }

        private static IEnumerator AwaitCoroutine(YieldInstruction instruction, TaskCompletionSource<bool> taskCompletionSource)
        {
            yield return instruction;
            taskCompletionSource.SetResult(true);
        }
    }
}
