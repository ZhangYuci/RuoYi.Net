namespace RuoYi.Framework.AzureAd.Options;

/// <summary>
/// Azure AD 配置选项
/// </summary>
public class AzureAdOptions
{
    /// <summary>
    /// Azure AD Instance, typically https://login.microsoftonline.com/
    /// </summary>
    public string Instance { get; set; } = "https://login.microsoftonline.com/";

    /// <summary>
    /// Azure AD Domain (e.g., contoso.onmicrosoft.com)
    /// </summary>
    public string Domain { get; set; } = string.Empty;

    /// <summary>
    /// Azure AD Tenant ID (Directory ID)
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Application (client) ID
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Client secret (Application password)
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Callback path for AAD authentication
    /// </summary>
    public string CallbackPath { get; set; } = "/signin-oidc";

    /// <summary>
    /// Whether AAD authentication is enabled
    /// </summary>
    public bool Enabled { get; set; } = false;
}
