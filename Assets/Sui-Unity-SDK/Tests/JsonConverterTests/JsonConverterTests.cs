using NUnit.Framework;
using System;
using Sui.Utilities;
using UnityEngine;
using System.IO;
using static UnityEngine.Networking.UnityWebRequest;
using Sui.Rpc.Client;
using Newtonsoft.Json;
using Sui.Rpc.Models;
using Sui.Accounts;

namespace Sui.Tests.JsonConverter
{
    public class JsonConverterTests
    {
        string path = Application.dataPath + "/Sui-Unity-SDK/Tests/JsonConverterTests/Responses";

        [Test]
        public void SuiSystemSummaryConverter()
        {
            path += "/SuiSystemSummary.json";
            StreamReader reader = new StreamReader(path);
            string rawRpcResponse = reader.ReadToEnd();

            SuiSystemSummary systemSummary = JsonConvert.DeserializeObject<SuiSystemSummary>(
                    rawRpcResponse
            );

            Debug.Log(systemSummary.Epoch);
            Debug.Log(systemSummary.ValidatorReportRecords.Count);
            foreach(AccountAddress address in systemSummary.ValidatorReportRecords.Keys)
            {
                Debug.Log(address.ToHex());
            }
        }
    }
}