//
//  UnityRpcClient.cs
//  Sui-Unity-SDK
//
//  Copyright (c) 2024 OpenDive
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Sui.Rpc
{
    public class UnityRpcClient
    {
        /// <summary>
        /// The name parameter of the Content Type header.
        /// </summary>
        public readonly string ContentTypeName = "Content-Type";

        /// <summary>
        /// The value parameter of the Content Type header.
        /// </summary>
        public readonly string ContentTypeValue = "application/json";

        /// <summary>
        /// The POST value as a string value.
        /// </summary>
        public readonly string POSTMethod = "POST";

        /// <summary>
        /// The RPC endpoint.
        /// </summary>
        public Uri Endpoint { get; set; }

        /// <summary>
        /// Creates an RPC Client.
        /// </summary>
        /// <param name="url">The base url of the RPC endpoint.</param>
        public UnityRpcClient(string url) => Endpoint = new Uri(url);

        /// <summary>
        /// Send an asynchronous RPC call using a given method.
        /// </summary>
        /// <typeparam name="T">The result object of the RPC call.</typeparam>
        /// <param name="method">The name of the RPC method.</param>
        /// <returns>An asynchronous task that returns a wrapped `T` object.</returns>
        protected internal async Task<RpcResult<T>> SendRpcRequestAsync<T>(string method)
            => await this.SendAsync<T>(new RpcRequest(method, null));

        /// <summary>
        /// Send an asynchronous RPC call using a given method and parameters.
        /// </summary>
        /// <typeparam name="T">The result object of the RPC call.</typeparam>
        /// <param name="method">The name of the RPC method.</param>
        /// <param name="params">The input parameters of the RPC method.</param>
        /// <returns>An asynchronous task that returns a wrapped `T` object.</returns>
        protected internal async Task<RpcResult<T>> SendRpcRequestAsync<T>
        (
            string method,
            IEnumerable<object> @params
        )
            => await this.SendAsync<T>(new RpcRequest(method, @params));

        /// <summary>
        /// Send an asynchronous RCP call.
        /// </summary>
        /// <typeparam name="T">The result object of the RPC call.</typeparam>
        /// <param name="rpc_request">The formatted RPC request object.</param>
        /// <returns>An asynchronous task that returns a wrapped `T` object.</returns>
        public async Task<RpcResult<T>> SendAsync<T>(RpcRequest rpc_request)
        {
            using (UnityWebRequest request = new UnityWebRequest(this.Endpoint, this.POSTMethod))
            {
                request.uploadHandler = new UploadHandlerRaw
                (
                    Encoding.UTF8.GetBytes
                    (
                        JsonConvert.SerializeObject
                        (
                            rpc_request,
                            new Newtonsoft.Json.Converters.StringEnumConverter()
                        )
                    )
                );
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader(this.ContentTypeName, this.ContentTypeValue);
                request.SendWebRequest();

                while (!request.isDone)
                    await Task.Yield();

                return JsonConvert.DeserializeObject<RpcResult<T>>
                (
                    request.downloadHandler.text
                );
            }
        }
    }
}