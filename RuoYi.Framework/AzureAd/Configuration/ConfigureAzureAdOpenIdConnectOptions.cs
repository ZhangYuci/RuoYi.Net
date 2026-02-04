using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace RuoYi.Framework.AzureAd.Configuration;

/// <summary>
/// Azure AD OpenID Connect 配置器
/// </summary>
public class ConfigureAzureAdOpenIdConnectOptions : IConfigureNamedOptions<OpenIdConnectOptions>
{
    private readonly IConfiguration _configuration;

    public ConfigureAzureAdOpenIdConnectOptions(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(string? name, OpenIdConnectOptions options)
    {
        if (name == OpenIdConnectDefaults.AuthenticationScheme)
        {
            Configure(options);
        }
    }

    public void Configure(OpenIdConnectOptions options)
    {
        var azureAdOptions = _configuration.GetSection("AzureAd").Get<Options.AzureAdOptions>();
        
        if (azureAdOptions?.Enabled == true)
        {
            options.Authority = $"{azureAdOptions.Instance}{azureAdOptions.TenantId}/v2.0";
            options.ClientId = azureAdOptions.ClientId;
            options.ClientSecret = azureAdOptions.ClientSecret;
            options.CallbackPath = azureAdOptions.CallbackPath;
            options.ResponseType = "code";
            options.SaveTokens = true;
        }
    }
}
