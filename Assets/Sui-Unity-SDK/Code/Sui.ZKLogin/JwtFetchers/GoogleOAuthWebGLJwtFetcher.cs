using Sui.ZKLogin.Utils;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public class GoogleOAuthWebGLJwtFetcher : MonoBehaviour, IJwtFetcher
{
    CancellationToken _cancellationToken;
    string _googleclientId;
    string _jwt = "";
    [DllImport("__Internal")]
    private static extern void GoogleLogin(string clientId, string nonce);

    public void SetGoogleClientId(string googleclientId)
    {  
        _googleclientId = googleclientId; 
    }

    public async Task<string> FetchJwt(params string[] parameters)
    {
        _jwt = "";
        string nonce = parameters[0];
        GoogleLogin(_googleclientId, nonce);
        while(string.IsNullOrEmpty(_jwt))
        {
            await Task.Yield();
            if(_cancellationToken != null && _cancellationToken.IsCancellationRequested)
            {
                Debug.Log("Fetch JWT => Request canceled by token.");
                return null;
            }
        }
        return _jwt;
    }

    public void OnJwtReceived(string token)
    {
        Debug.Log("jwt received => " + token);
        _jwt = token;
    }

    public void Dispose()
    {
        Debug.Log("There is nothing to dispose");
    }

    public void SetCancellationToken(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
    }
}