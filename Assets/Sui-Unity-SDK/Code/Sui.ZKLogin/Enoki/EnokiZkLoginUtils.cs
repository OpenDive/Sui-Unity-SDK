using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Sui.ZKLogin.Enoki.Utils
{
    public static class EnokiZkLoginUtils
    {

        public static async Task<EnokiNonceResponse> FetchNonce(string enokiPublicKey, string network, string ephemeralPublicKey, int additionalEpochs, CancellationToken cancellationToken = default)
        {
            string url = "https://api.enoki.mystenlabs.com/v1/zklogin/nonce";
            string jsonBody = JsonConvert.SerializeObject(new EnokiNonceRequest
            {
                network = network,
                ephemeralPublicKey = ephemeralPublicKey,
                additionalEpochs = additionalEpochs
            });

            EnokiNonceResponse nonceResult = null;

            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + enokiPublicKey);

                var operation = request.SendWebRequest();

                while (!operation.isDone)
                {
                    if (cancellationToken != null && cancellationToken.IsCancellationRequested)
                    {
                        request.Abort();
                        Debug.Log("EnokiNonceResponse => Request canceled by token.");
                        return null;
                    }

                    await Task.Yield();
                }

                if (request.result == UnityWebRequest.Result.DataProcessingError || request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error while fetching nonce: " + request.error);
                }
                else
                {
                    nonceResult = JsonConvert.DeserializeObject<EnokiNonceResponse>(request.downloadHandler.text);
                }
                return nonceResult;
            }

        }

        public static async Task<EnokiZKPResponse> FetchZKP(string network, string ephemeralPublicKey, string jwt, string apiToken, int maxEpoch, string randomness, CancellationToken cancellationToken = default)
        {
            string url = "https://api.enoki.mystenlabs.com/v1/zklogin/zkp";
            string jsonBody = JsonConvert.SerializeObject(new EnokiZKPRequest
            {
                network = network,
                ephemeralPublicKey = ephemeralPublicKey,
                maxEpoch = maxEpoch,
                randomness = randomness
            });

            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + apiToken);
                request.SetRequestHeader("zklogin-jwt", jwt);

                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    if (cancellationToken != null && cancellationToken.IsCancellationRequested)
                    {
                        request.Abort();
                        Debug.Log("EnokiZKPResponse => Request canceled by token");
                        return null;
                    }

                    await Task.Yield();
                }



                if (request.result == UnityWebRequest.Result.DataProcessingError || request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error while fetching ZKP: " + request.downloadHandler.text);
                    return null;
                }
                else
                {
                    return JsonConvert.DeserializeObject<EnokiZKPResponse>(request.downloadHandler.text);
                }


            }
        }

        public static async Task<EnokiZKLoginUserResponse> FetchZKLoginUserData(string jwt, string apiToken, CancellationToken cancellationToken = default)
        {
            string url = "https://api.enoki.mystenlabs.com/v1/zklogin";
            using (UnityWebRequest request = new UnityWebRequest(url, "GET"))
            {
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + apiToken);
                request.SetRequestHeader("zklogin-jwt", jwt);


                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    if (cancellationToken != null && cancellationToken.IsCancellationRequested)
                    {
                        request.Abort();
                        Debug.Log("EnokiZKPResponse => Request canceled by token");
                        return null;
                    }

                    await Task.Yield();
                }

                if (request.result == UnityWebRequest.Result.DataProcessingError || request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error while fetching zkLoginUser: " + request.downloadHandler.text);
                }
                return JsonConvert.DeserializeObject<EnokiZKLoginUserResponse>(request.downloadHandler.text);
            }
        }


    }

    [System.Serializable]
    public class EnokiZKLoginSaveableData
    {
        public EnokiZKLoginUserResponse loginUserResponse;
        public EnokiZKPResponse zkpResponse;
        public string ephemeralPrivateKeyHex;
        public int maxEpoch;
    }

    [System.Serializable]
    public class EnokiZKPRequest
    {
        public string network;
        public string ephemeralPublicKey;
        public int maxEpoch;
        public string randomness;
    }

    [System.Serializable]
    public class EnokiNonceRequest
    {
        public string network;
        public string ephemeralPublicKey;
        public int additionalEpochs;
    }

    [System.Serializable]
    public class EnokiNonceResponseData
    {
        public string nonce { get; set; }
        public string randomness { get; set; }
        public int epoch { get; set; }
        public int maxEpoch { get; set; }
        public long estimatedExpiration { get; set; }
    }

    [System.Serializable]
    public class EnokiNonceResponse
    {
        public EnokiNonceResponseData data { get; set; }
    }

    [System.Serializable]
    public class EnokiZKLoginResponseData
    {
        public string salt { get; set; }
        public string address { get; set; }
        public string publicKey { get; set; }
    }

    [System.Serializable]
    public class EnokiZKLoginUserResponse
    {
        public EnokiZKLoginResponseData data { get; set; }
    }

    [System.Serializable]
    public class EnokiZKPResponse
    {
        public EnokiZKPResponseData data { get; set; }
    }

    [System.Serializable]
    public class EnokiZKPResponseData
    {
        public ProofPoints proofPoints { get; set; }
        public IssBase64Details issBase64Details { get; set; }
        public string headerBase64 { get; set; }
        public string addressSeed { get; set; }
    }

    [System.Serializable]
    public class IssBase64Details
    {
        public string value { get; set; }
        public int indexMod4 { get; set; }
    }

    [System.Serializable]
    public class ProofPoints
    {
        public List<string> a { get; set; }
        public List<List<string>> b { get; set; }
        public List<string> c { get; set; }
    }

}



