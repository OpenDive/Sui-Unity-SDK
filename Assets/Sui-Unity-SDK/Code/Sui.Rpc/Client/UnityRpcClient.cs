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

            try
            {
                byte[] requestBytes = Encoding.UTF8.GetBytes(requestJson);

                using (UnityWebRequest request = new UnityWebRequest(Endpoint, "POST"))
                {
                    request.uploadHandler = new UploadHandlerRaw(requestBytes);
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.SetRequestHeader("Content-Type", "application/json");

                    request.SendWebRequest();

                    while(!request.isDone)
                    {
                        await Task.Yield();
                    }

                    Debug.Log("REQUEST: DOWNLOADHANDLER ::: " + request.downloadHandler.ToString());

                    RpcResult<T> result = HandleResult<T>(request.downloadHandler);
                    result.RawRpcRequest = requestJson;
                    return result;
                }
            }
            catch (Exception e)
            {
                var result = new RpcResult<T>
                {
                    ErrorMessage = e.Message,
                    RawRpcRequest = requestJson
                };
                var errorMessage = $"SendAsync Caught exception: {e.Message}";
                Debug.LogError(errorMessage);

                return result;
            }
        }


        private RpcResult<T> HandleResult<T>(DownloadHandler downloadHandler)
        {
            var result = new RpcResult<T>();
            try
            {
                result.RawRpcResponse = downloadHandler.text;
                Debug.Log($"Result: {result.RawRpcResponse}");
                var res = JsonConvert.DeserializeObject<RpcValidResponse<T>>(
                    result.RawRpcResponse
                );

                if (res.Result != null)
                {
                    result.Result = res.Result;
                    result.IsSuccess = true;
                }
                else
                {
                    var errorRes = JsonConvert.DeserializeObject<RpcErrorResponse>(
                        result.RawRpcResponse
                    );

                    if (errorRes != null)
                    {
                        result.ErrorMessage = errorRes.Error.Message;
                    }
                    else
                    {
                        result.ErrorMessage = "Something wrong happened.";
                    }
                }
            }
            catch (JsonException e)
            {
                Debug.LogError($"HandleResult Caught exception: {e.Message}");
                result.IsSuccess = false;
                result.ErrorMessage = "Unable to parse json.";
            }

            return result;
        }
    }
}