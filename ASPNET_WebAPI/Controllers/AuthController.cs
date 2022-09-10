using GoogleOIDC_Angular_ASPNETWebAPI_Auth_Code_Flow.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoogleOIDC_Angular_ASPNETWebAPI_Auth_Code_Flow.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly OIDCService _oidcService;
    private readonly IConfiguration _config;

    public AuthController(
        OIDCService oidcService,
        IConfiguration config)
    {
        _oidcService = oidcService;
        _config = config;
    }

    [HttpGet("oidc/signin", Name = nameof(SigninOIDCAsync))]
    [HttpPost("oidc/signin", Name = nameof(SigninOIDCAsync))]
    public async Task<IActionResult> SigninOIDCAsync(string code, string state, string? error)
    {
        if (state != "12345GG")
        {
            return BadRequest();
        }

        if (!string.IsNullOrEmpty(error))
        {
            throw new Exception(error);
        }

        _oidcService.SetupRedirectUri(Request, "api/Auth/oidc/signin");
        string idToken = await _oidcService.GetIdTokenAsync(code);

        return Redirect($"{_config["FrontEndUri"]}?idToken={idToken}");
    }

}
