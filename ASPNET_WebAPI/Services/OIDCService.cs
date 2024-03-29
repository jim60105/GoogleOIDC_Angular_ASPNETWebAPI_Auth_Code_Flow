﻿using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;

namespace GoogleOIDC_Angular_ASPNETWebAPI_Auth_Code_Flow.Services;

public class OIDCService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;

    private string RedirectUri { get; set; } = "http://localhost:7091/api/Auth/oidc/signin";
    private string ClientId { get; }
    private string ClientSecret { get; }

    public OIDCService(IHttpClientFactory httpClientFactory,
                       IConfiguration config,
                       string requestUrl)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;

        if (!string.IsNullOrEmpty(requestUrl))
            SetupRedirectUri(requestUrl);

        // Read redirectUri from configuration if it exists.
        RedirectUri =  !string.IsNullOrEmpty(_config["Authentication:Google:RedirectUri"])
            ? _config["Authentication:Google:RedirectUri"]
            : RedirectUri;
        ClientId = _config["Authentication:Google:ClientId"];
        ClientSecret = _config["Authentication:Google:ClientSecret"];
    }

    /// <summary>
    /// Request access token with authorization code.
    /// </summary>
    /// <param name="authorization_code"></param>
    /// <returns></returns>
    public async Task<string> GetIdTokenAsync(string authorization_code)
    {
        using HttpClient client = _httpClientFactory.CreateClient();
        AuthorizationCodeTokenRequest request = new()
        {
            Code = authorization_code,
            RedirectUri = RedirectUri,
            ClientId = ClientId,
            ClientSecret = ClientSecret,
            Scope = "openid profile email"
        };

        TokenResponse responce = await request.ExecuteAsync(client,
                                                            GoogleAuthConsts.OidcTokenUrl,
                                                            new(),
                                                            Google.Apis.Util.SystemClock.Default);

        return responce.IdToken;
    }

    #region Additional Feature
    /// <summary>
    /// 動態取得request URL，以產生並覆寫RedirectUri
    /// </summary>
    /// <param name="request"></param>
    /// <param name="route"></param>
    internal void SetupRedirectUri(string requestUrl, string? route = "api/Auth/oidc/signin")
    {
        Uri uri = new(requestUrl);
        string port = uri.Scheme == "https" && uri.Port == 443
                      || uri.Scheme == "http" && uri.Port == 80
                      ? ""
                      : $":{uri.Port}";
        RedirectUri = $"{uri.Scheme}://{uri.Host}{port}/{route}";
    }
    #endregion
}
