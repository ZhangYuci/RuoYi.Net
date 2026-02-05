using Microsoft.Identity.Web;
using System.Security.Claims;

namespace RuoYi.Framework.AzureAd.Services;

/// <summary>
/// Azure AD 用户服务
/// </summary>
public interface IAzureAdUserService
{
    /// <summary>
    /// 从 Azure AD Claims 中提取用户信息
    /// </summary>
    /// <param name="principal">用户声明</param>
    /// <returns>用户信息</returns>
    AzureAdUserInfo ExtractUserInfo(ClaimsPrincipal principal);
}

/// <summary>
/// Azure AD 用户信息
/// </summary>
public class AzureAdUserInfo
{
    /// <summary>
    /// 用户唯一标识 (Object ID from Azure AD)
    /// </summary>
    public string ObjectId { get; set; } = string.Empty;

    /// <summary>
    /// 用户名 (UPN or preferred_username)
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 显示名称
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 租户 ID
    /// </summary>
    public string TenantId { get; set; } = string.Empty;
}

/// <summary>
/// Azure AD 用户服务实现
/// </summary>
public class AzureAdUserService : IAzureAdUserService, ITransient
{
    /// <summary>
    /// 从 Azure AD Claims 中提取用户信息
    /// </summary>
    public AzureAdUserInfo ExtractUserInfo(ClaimsPrincipal principal)
    {
        var userInfo = new AzureAdUserInfo();

        // 提取 Object ID (用户在 Azure AD 中的唯一标识)
        userInfo.ObjectId = principal.FindFirst(ClaimConstants.ObjectId)?.Value
            ?? principal.FindFirst("oid")?.Value
            ?? string.Empty;

        // 提取用户名 (优先使用 preferred_username, 其次使用 upn)
        userInfo.Username = principal.FindFirst(ClaimConstants.PreferredUserName)?.Value
            ?? principal.FindFirst(ClaimTypes.Upn)?.Value
            ?? principal.FindFirst("upn")?.Value
            ?? string.Empty;

        // 提取显示名称
        userInfo.DisplayName = principal.FindFirst(ClaimConstants.Name)?.Value
            ?? principal.FindFirst(ClaimTypes.Name)?.Value
            ?? principal.FindFirst("name")?.Value
            ?? string.Empty;

        // 提取邮箱
        userInfo.Email = principal.FindFirst(ClaimTypes.Email)?.Value
            ?? principal.FindFirst("email")?.Value
            ?? string.Empty;

        // 提取租户 ID
        userInfo.TenantId = principal.FindFirst(ClaimConstants.TenantId)?.Value
            ?? principal.FindFirst("tid")?.Value
            ?? string.Empty;

        return userInfo;
    }
}
