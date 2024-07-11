using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sui.Rpc.Client;
using UnityEngine;
using UnityEngine.Networking;

namespace Sui.Rpc
{
    public class UnityRpcClient
    {
        /// <summary>
        /// The RPC endpoint.
        /// </summary>
        public Uri Endpoint { get; set; }

        /// <summary>
        /// Creates an RPC Client.
        /// </summary>
        /// <param name="url"></param>
        public UnityRpcClient(string url) => Endpoint = new Uri(url);

        /// <summary>
        /// Send an asynchronous RCP call.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rpcRequest"></param>
        /// <returns></returns>
        public async Task<RpcResult<T>> SendAsync<T>(RpcRequest rpcRequest)
        {
            string requestJson = JsonConvert.SerializeObject(
                rpcRequest, new Newtonsoft.Json.Converters.StringEnumConverter()
            );

            Debug.Log("QUERY: \n" + requestJson);

            byte[] requestBytes = Encoding.UTF8.GetBytes(requestJson);

            using (UnityWebRequest request = new UnityWebRequest(Endpoint, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(requestBytes);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                request.SendWebRequest();

                while (!request.isDone)
                    await Task.Yield();

                Debug.Log("REQUEST: RESULT ::: " + request.downloadHandler.text);

                RpcResult<T> result = JsonConvert.DeserializeObject<RpcResult<T>>(request.downloadHandler.text);
                return result;
            }
        }
    }
}