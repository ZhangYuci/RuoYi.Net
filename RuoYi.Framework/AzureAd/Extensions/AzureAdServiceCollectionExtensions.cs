using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using RuoYi.Framework.AzureAd.Options;

namespace RuoYi.Framework.AzureAd.Extensions;

/// <summary>
/// Azure AD 服务扩展
/// </summary>
public static class AzureAdServiceCollectionExtensions
{
    /// <summary>
    /// 添加 Azure AD 认证
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置</param>
    /// <returns></returns>
    public static IServiceCollection AddAzureAdAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // 配置 Azure AD 选项
        services.AddOptions<AzureAdOptions>()
            .BindConfiguration("AzureAd")
            .ValidateDataAnnotations();

        var azureAdOptions = configuration.GetSection("AzureAd").Get<AzureAdOptions>();

        // 只有在启用时才添加 AAD 认证
        if (azureAdOptions?.Enabled == true)
        {
            // 添加 Microsoft Identity Platform 认证
            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(options =>
                {
                    options.Instance = azureAdOptions.Instance;
                    options.Domain = azureAdOptions.Domain;
                    options.TenantId = azureAdOptions.TenantId;
                    options.ClientId = azureAdOptions.ClientId;
                    options.ClientSecret = azureAdOptions.ClientSecret;
                    options.CallbackPath = azureAdOptions.CallbackPath;
                    
                    // 配置响应类型为 code（授权码流程）
                    options.ResponseType = "code";
                    
                    // 保存令牌
                    options.SaveTokens = true;
                });
        }

        return services;
    }
}
