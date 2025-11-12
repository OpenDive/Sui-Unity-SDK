using Sui.ZKLogin.Utils;
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class GoogleOAuthDesktopJwtFetcher : IJwtFetcher
{
    private CancellationToken _cancellationToken;
    private HttpListener _httpListener;
    private string _clientId;
    private string _redirectUri;

    public GoogleOAuthDesktopJwtFetcher(string clientId, string redirectUri)
    {
        _clientId = clientId;
        _redirectUri = redirectUri;
        Application.quitting += Application_Quitting;
    }

    private void Application_Quitting()
    {
        Dispose();
    }

    public async Task<string> FetchJwt(params string[] parameters)
    {
        string nonce = parameters[0];
        // Build OAuth URL
        string authUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
            $"client_id={_clientId}" +
            $"&redirect_uri={Uri.EscapeDataString(_redirectUri)}" +
            $"&response_type=id_token" +
            $"&scope=openid" +
            $"&nonce=" + nonce;

        Debug.Log("Opening Google OAuth URL...");

        // Open browser
        Application.OpenURL(authUrl);

        // Start local server and wait for callback
        return await FetchJwtWithHttpListener(_redirectUri);
    }

    private async Task<string> FetchJwtWithHttpListener(string redirectUri)
    {
        _httpListener = new HttpListener();
        _httpListener.Prefixes.Add(redirectUri);

        try
        {
            _httpListener.Start();
            Debug.Log($"Local server started: {redirectUri}");

            var getContextTask = _httpListener.GetContextAsync();

            while (!getContextTask.IsCompleted)
            {
                await Task.Yield();
                if (_cancellationToken != null &&_cancellationToken.IsCancellationRequested)
                {
                    _httpListener.Stop();
                    Debug.Log("Fetch JWT => Login canceled!");
                    return null;
                }
            }

            // Wait for callback
            HttpListenerContext context = getContextTask.Result;
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;


            string requestHtml = @"
    <!DOCTYPE html>
    <html>
    <head>
    <title>Google Login</title>
    </head>
    <body>
    <h1>Processing login...</h1>
    <h2>Please wait while processing zkLogin...</h2>
    <script>
        // Extract token from fragment (after #)
        var fragment = window.location.hash.substring(1);
        var params = new URLSearchParams(fragment);
        var idToken = params.get('id_token');

        if (idToken) {
            // Redirect to same URL but with query string (?) instead of fragment (#)
            window.location.href = '/?id_token=' + idToken;
        } else if (error) {
            window.location.href = '/?error=' + error;
        } else {
            document.body.innerHTML = '<h1>Error!</h1><p>No token received.</p>';
        }
    </script>
    </body>
    </html>";

            string responseHtml = @"
    <!DOCTYPE html>
    <html>
    <head>
    <title>Google Login</title>
    </head>
    <body>
    <h1>You are done!</h1>
    <h2>You can close this page and return to the game.</h2>
    </body>
    </html>";
            SendResponse(response, requestHtml);

            getContextTask = _httpListener.GetContextAsync();

            while (!getContextTask.IsCompleted)
            {
                await Task.Yield();
                if (_cancellationToken != null && _cancellationToken.IsCancellationRequested)
                {
                    _httpListener.Stop();
                    Debug.Log("Fetch JWT => Login canceled!");
                    return null;
                }
            }
            request = getContextTask.Result.Request;
            response = getContextTask.Result.Response;
            SendResponse(response, responseHtml);
            // Parse URL parameters
            string token = request.QueryString.Get("id_token");

            //await StartLocalServer();
            _httpListener.Stop();
            return token;


        }
        catch (Exception ex)
        {
            Debug.LogError($"Server error: {ex.Message}");
            if (_httpListener != null && _httpListener.IsListening)
            {
                _httpListener.Stop();
            }
            return "";
        }
    }

    private static void SendResponse(HttpListenerResponse response, string html)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(html);
        response.ContentLength64 = buffer.Length;
        response.ContentType = "text/html";
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }

    public void Dispose()
    {
        if (_httpListener != null && _httpListener.IsListening)
        {
            _httpListener.Stop();
            Debug.Log("HttpListener Disposed");
        }
    }

    public void SetCancellationToken(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
    }

    // JSON response models
    [Serializable]
    private class TokenResponse
    {
        public string access_token;
        public string refresh_token;
        public int expires_in;
        public string token_type;
        public string id_token;
    }

    [Serializable]
    private class UserInfo
    {
        public string id;
        public string email;
        public bool verified_email;
        public string name;
        public string given_name;
        public string family_name;
        public string picture;
    }
}