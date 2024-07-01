using NUnit.Framework;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Transactions.Builder;
using Sui.Transactions.Kinds;
using UnityEngine;
using Sui.Utilities;
using Sui.Transactions.Types;
using Sui.Transactions.Types.Arguments;
using Sui.Types;
using Sui.Transactions;
using Sui.Rpc;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sui.Clients;
using UnityEngine.TestTools;

namespace Sui.Tests
{
    public class TXBuilderTest: MonoBehaviour
    {
        string suiAddressHex = "0x0000000000000000000000000000000000000000000000000000000000000002";
        string dummy_address = "0x114a63e533262fee088dcfe8c015d6786ec681611abe3c72c77abdf278a8f0f5";

        private readonly string customResourcePath = Path.Combine(Application.dataPath, "Sui-Unity-SDK/Tests/Resources");

        public JObject GetModule(string name)
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

        [Test]
        public async Task SimpleTransactionMoveCallBuildTest()
        {
            var tx = new TransactionBlock();
            AccountAddress suiAddress = AccountAddress.FromHex(suiAddressHex);
            var account = new Account();

            tx.AddMoveCallTx(
                new SuiMoveNormalizedStructType(new SuiStructTag(suiAddress, "display", "new", new ISerializableTag[0])), // TODO: THIS IS A NORMALIZED STRUCT
                new ISerializableTag[] { new StructTag(suiAddress, "capy", "Capy", new ISerializableTag[0]) },
                new SuiTransactionArgument[] { new SuiTransactionArgument(new TransactionBlockInput(0)) } // TODO: We should not use this abstract, this should be a "pure" or an "object.
            );

            tx.SetSenderIfNotSet(account.AccountAddress);
            var provider = new SuiClient(Constants.MainnetConnection);
            var build_options = new BuildOptions(provider);
            var digest = await tx.GetDigest(build_options);
            //var result = await provider
        }

        [UnityTest]
        public IEnumerator SimplePublishBuildTest()
        {
            TestToolbox toolbox = new TestToolbox();
            yield return toolbox.Setup();

            yield return new WaitForSeconds(20f);

            Task task = toolbox.PublishPackage("serializer");
            yield return new WaitUntil(() => task.IsCompleted);

            // await toolbox.PublishPackage("serializer");

            //JObject file_data = GetModule("serializer");
            //TransactionBlock tx = new TransactionBlock();

            //JArray modules_jarray = (JArray)file_data["modules"];
            //JArray dependencies_jarray = (JArray)file_data["dependencies"];

            //List<string> modules = new List<string>();
            //List<string> dependencies = new List<string>();
            //var account = new Account();

            //foreach (JToken jtoken in modules_jarray.Values())
            //    modules.Add((string)jtoken);

            //foreach (JToken jtoken in dependencies_jarray.Values())
            //    dependencies.Add((string)jtoken);

            //var cap = tx.AddPublishTx
            //(
            //    modules.ToArray(),
            //    dependencies.ToArray()
            //);

            //tx.AddTransferObjectsTx(cap.Select((cap_value) => new SuiTransactionArgument(cap_value)).ToArray(), account.SuiAddress());

            //tx.SetSenderIfNotSet(account.AccountAddress);

            //var provider = new SuiClient(Constants.MainnetConnection);
            //var build_options = new BuildOptions(provider);
            //var digest = await tx.GetDigest(build_options);
        }
    }
}
