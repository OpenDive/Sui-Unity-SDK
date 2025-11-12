namespace OpenDive.Utils.Jwt
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// A class to decode JWT tokens.
    /// This class only focuses on decoding necessary claims for ZKLogin.
    /// 
    /// NOTE: JWT claims fall into three categories – registered, public, or private.
    ///     > Registered claims are pre-established and publicly documented.
    ///     Using a registered claim ensures your JWTs will operate smoothly
    ///     across applications. This is because all libraries recognize these
    ///     common, standardized claims.
    ///     > Custom claims (public or private) are claims outside of the registered claims.
    ///     Public claims: Developers define their own claims and register them
    ///     with the IANA registry mentioned above. Examples include `auth_time`, `acr` and `nonce`.
    ///     Private claims: Developers define their own claims but do not publish
    ///     them. Instead, they make local agreements to ensure operability
    ///     between private parties.
    /// TODO: Add unit tests
    /// </summary>
    public class JWTDecoder
    {
        /// <summary>
        /// Decodes a JWT token and extracts the header and payload as strongly-typed classes.
        /// In its compact form, JSON Web Tokens consist of three parts separated by dots (.)
        ///     > Header - The header typically consists of two parts:
        ///         the type of the token, which is JWT, and the signing algorithm
        ///         being used, such as HMAC SHA256 or RSA.
        ///     > Payload -  The payload, which contains the claims.
        ///         Claims are statements about an entity (typically, the user)
        ///         and additional data. There are three types of claims:
        ///         registered, public, and private claims.
        ///     > Signature - To create the signature part you have to take the
        ///         encoded header, the encoded payload, a secret, the algorithm
        ///         specified in the header, and sign that.
        /// </summary>
        /// <param name="token">The JWT token string</param>
        /// <returns>A JWT object containing the header and payload</returns>
        public static JWT DecodeJWT(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                Debug.LogError("JWT token is null or empty!");
                return null;
            }

            string[] parts = token.Split('.');
            if (parts.Length != 3)
            {
                Debug.LogError("Invalid JWT token format!");
                return null;
            }

            try
            {
                // Decode header and payload
                string headerJson = Base64UrlDecode(parts[0]);
                string payloadJson = Base64UrlDecode(parts[1]);

                // Deserialize JSON into classes
                JWTHeader header = JsonConvert.DeserializeObject<JWTHeader>(headerJson);
                JWTPayload payload = JsonConvert.DeserializeObject<JWTPayload>(payloadJson);

                // Return a JWT object
                return new JWT
                {
                    Header = header,
                    Payload = payload,
                    Signature = parts[2]
                };
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error decoding JWT: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Utility class to decode non-required claims.
        /// </summary>
        /// <param name="json">The JWT token string</param>
        /// <param name="claims">Parsed claims</param>
        private static void ParseCustomClaims(string json, Dictionary<string, string> claims)
        {
            // Simple JSON parsing for claims
            // Remove the first and last curly braces and split by commas
            json = json.Trim('{', '}');
            string[] pairs = json.Split(',');

            foreach (string pair in pairs)
            {
                try
                {
                    string[] keyValue = pair.Split(':');
                    if (keyValue.Length == 2)
                    {
                        string key = keyValue[0].Trim('"', ' ');
                        string value = keyValue[1].Trim('"', ' ');

                        // Skip standard claims
                        if (!IsStandardClaim(key))
                        {
                            claims[key] = value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to parse claim pair: {pair}. Error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Utility class to check against standard claims.
        /// </summary>
        /// <param name="claimName"></param>
        /// <returns></returns>
        private static bool IsStandardClaim(string claimName)
        {
            return claimName == "iss" ||
                   claimName == "sub" ||
                   claimName == "aud" ||
                   claimName == "exp" ||
                   claimName == "nbf" ||
                   claimName == "iat" ||
                   claimName == "jti";
        }

        /// <summary>
        /// Decodes a Base64 URL-encoded string.
        /// </summary>
        /// <param name="base64Url">The Base64 URL-encoded string</param>
        /// <returns>The decoded string</returns>
        private static string Base64UrlDecode(string base64Url)
        {
            string padded = base64Url.Replace('-', '+').Replace('_', '/');
            switch (padded.Length % 4)
            {
                case 2: padded += "=="; break;
                case 3: padded += "="; break;
            }

            byte[] data = Convert.FromBase64String(padded);
            return Encoding.UTF8.GetString(data);
        }
    }

    /// <summary>
    /// Represents a decoded JWT with header, payload, and signature.
    /// </summary>
    [Preserve]
    public class JWT
    {
        [JsonConstructor]
        public JWT() { }
        /// <summary>
        /// The JOSE (JSON Object Signing and Encryption) Header is comprised
        /// of a set of Header Parameters that typically consist of a name/value pair:
        /// the hashing algorithm being used (e.g., HMAC SHA256 or RSA) and the type of the JWT.
        /// </summary>
        public JWTHeader Header { get; set; }
        /// <summary>
        /// JWS payload (set of claims): contains verifiable security statements.
        /// These are statements about an entity (typically, the user) and additional data.
        /// There are three types of claims: registered, public, and private claims.
        ///
        /// Registered claims: These are a set of predefined claims which are
        /// not mandatory but recommended, to provide a set of useful, interoperable claims.
        /// Some of them are: iss (issuer), exp (expiration time), sub (subject), aud (audience), and others.
        ///
        /// Public claims: These can be defined at will by those using JWTs.
        /// But to avoid collisions they should be defined in the IANA JSON Web Token Registry
        /// or be defined as a URI that contains a collision resistant namespace.
        ///
        /// Private claims: These are the custom claims created to share
        /// information between parties that agree on using them and are
        /// neither registered or public claims.
        /// </summary>
        public JWTPayload Payload { get; set; }
        /// <summary>
        /// The signature is used to verify that the sender of the JWT is who
        /// it says it is and to ensure that the message wasn't changed along the way.
        /// To create the signature, the Base64-encoded header and payload are taken,
        /// along with a secret, and signed with the algorithm specified in the header.
        /// 
        /// When you use a JWT, you must check its signature before storing and using it.
        ///
        /// For example, if you are creating a signature for a token using the
        /// HMAC SHA256 algorithm, you would do the following:
        /// <code>HMACSHA256(
        ///     base64UrlEncode(header) + "." +
        ///     base64UrlEncode(payload),
        ///     secret)
        /// </code>
        /// </summary>
        public string Signature { get; set; }
    }

    /// <summary>
    /// Represents the JWT header.
    /// </summary>
    [Preserve]
    public class JWTHeader
    {
        [JsonConstructor]
        public JWTHeader() { }

        [JsonProperty("alg")]
        public string alg { get; set; } // * Algorithm. Required for ZK Login.
        
        [JsonProperty("typ")]
        public string typ { get; set; } // * Token type. Required for ZK Login.
        
        [JsonProperty("kid")]
        public string kid { get; set; } // * The kid value indicates what key was used to sign the JWT. Required for ZK Login.
    }

    /// <summary>
    /// Represents the JWT payload with common claims.
    /// </summary>
    [Preserve]
    public class JWTPayload
    {
        [JsonConstructor]
        public JWTPayload() { }
        // <> Registered claims </>
        [JsonProperty("iss")]
        public string Iss { get; set; } // * Issuer of the JWT. Required for ZK Login.

        [JsonProperty("sub")]
        public string Sub { get; set; } // * Subject of the JWT (the user). Required for ZK Login.

        [JsonProperty("aud")]
        public string Aud { get; set; } // Recipient for which the JWT is intended

        [JsonProperty("azp")]
        public string Azp { get; set; } // Authorized party - the party to which the ID Token was issued

        [JsonProperty("exp")]
        public long? Exp { get; set; }  // Time after which the JWT expires

        [JsonProperty("nbf")]
        public long? Nbf { get; set; }  // Time before which the JWT must not be accepted for processing (Unix timestamp)

        [JsonProperty("iat")]
        public long? Iat { get; set; }  // Issued at .. Time at which the JWT was issued; can be used to determine age of the JWT (Unix timestamp)

        [JsonProperty("jti")]
        public string Jti { get; set; } // JWT ID (unique identifier for the token). Can be used to prevent the JWT from being replayed (allows a token to be used only once)

        // <> Public claims </>
        public long? Auth_time { get; set; }    // Authentication time (Unix timestamp)
        public string ACR { get; set; }        // The Authentication Context Class Reference (ACR) claim is a URI that identifies the authentication context class reference.
        public string Nonce { get; set; }       // * Required for ZK Login. Used instead of `iat` and `exp`.

        // <> Custom claims </>
        public string Email { get; set; }    // Email address
        public string Name { get; set; }     // Name
        // Add additional properties as needed based on your JWT structure.
    }

    //[Serializable]
    //public class JwtPayload
    //{
    //    public string Iss;    // Issuer
    //    public string Aud;    // Audience
    //    public string Sub;    // Subject
    //    public long Exp;      // Expiration Time
    //    public long Iat;      // Issued At
    //    public Dictionary<string, string> claims;

    //    public JwtPayload()
    //    {
    //        claims = new Dictionary<string, string>();
    //    }
    //}

    //public static class JwtDecoder
    //{
    //    public static JwtPayload DecodeJwt(string jwt)
    //    {
    //        try
    //        {
    //            // Split the JWT into its three parts
    //            string[] parts = jwt.Split('.');
    //            if (parts.Length != 3)
    //            {
    //                throw new ArgumentException("Invalid JWT format");
    //            }

    //            // Decode the payload (second part)
    //            string payloadJson = DecodeBase64Url(parts[1]);

    //            // Parse using Unity's JSON utility
    //            var payload = new JwtPayload();
    //            var tempPayload = JsonUtility.FromJson<JwtPayload>(payloadJson);

    //            // Copy standard claims
    //            payload.Iss = tempPayload.Iss;
    //            payload.Aud = tempPayload.Aud;
    //            payload.Sub = tempPayload.Sub;
    //            payload.Exp = tempPayload.Exp;
    //            payload.Iat = tempPayload.Iat;

    //            // Parse the JSON manually to get all claims
    //            // (because JsonUtility doesn't support Dictionary directly)
    //            ParseCustomClaims(payloadJson, payload.claims);

    //            return payload;
    //        }
    //        catch (Exception ex)
    //        {
    //            Debug.LogError($"Failed to decode JWT: {ex.Message}");
    //            throw;
    //        }
    //    }

    //    private static void ParseCustomClaims(string json, Dictionary<string, string> claims)
    //    {
    //        // Simple JSON parsing for claims
    //        // Remove the first and last curly braces and split by commas
    //        json = json.Trim('{', '}');
    //        string[] pairs = json.Split(',');

    //        foreach (string pair in pairs)
    //        {
    //            try
    //            {
    //                string[] keyValue = pair.Split(':');
    //                if (keyValue.Length == 2)
    //                {
    //                    string key = keyValue[0].Trim('"', ' ');
    //                    string value = keyValue[1].Trim('"', ' ');

    //                    // Skip standard claims
    //                    if (!IsStandardClaim(key))
    //                    {
    //                        claims[key] = value;
    //                    }
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                Debug.LogWarning($"Failed to parse claim pair: {pair}. Error: {ex.Message}");
    //            }
    //        }
    //    }

    //    private static string DecodeBase64Url(string base64Url)
    //    {
    //        string padded = base64Url.PadRight(4 * ((base64Url.Length + 3) / 4), '=');
    //        string base64 = padded.Replace('-', '+').Replace('_', '/');
    //        byte[] bytes = Convert.FromBase64String(base64);
    //        return Encoding.UTF8.GetString(bytes);
    //    }

    //    private static bool IsStandardClaim(string claimName)
    //    {
    //        return claimName == "iss" ||
    //               claimName == "sub" ||
    //               claimName == "aud" ||
    //               claimName == "exp" ||
    //               claimName == "nbf" ||
    //               claimName == "iat" ||
    //               claimName == "jti";
    //    }

    //    // Helper methods
    //    public static bool IsTokenExpired(JwtPayload payload)
    //    {
    //        var now = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
    //        return now >= payload.Exp;
    //    }

    //    public static DateTime GetTokenExpirationTime(JwtPayload payload)
    //    {
    //        return DateTime.UnixEpoch.AddSeconds(payload.Exp);
    //    }

    //    public static DateTime GetTokenIssuedTime(JwtPayload payload)
    //    {
    //        return DateTime.UnixEpoch.AddSeconds(payload.Iat);
    //    }
    //}
}