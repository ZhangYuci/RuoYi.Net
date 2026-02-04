using RuoYi.Common.Constants;
using RuoYi.Common.Enums;
using RuoYi.Data.Models;
using RuoYi.Framework.AzureAd.Services;
using RuoYi.Framework.Exceptions;
using System.Security.Claims;

namespace RuoYi.System.Services;

/// <summary>
/// Azure AD 登录服务
/// </summary>
public class AadLoginService : ITransient
{
    private readonly ILogger<AadLoginService> _logger;
    private readonly TokenService _tokenService;
    private readonly SysUserService _sysUserService;
    private readonly SysLogininforService _sysLogininforService;
    private readonly SysPermissionService _sysPermissionService;
    private readonly IAzureAdUserService _azureAdUserService;

    public AadLoginService(
        ILogger<AadLoginService> logger,
        TokenService tokenService,
        SysUserService sysUserService,
        SysLogininforService sysLogininforService,
        SysPermissionService sysPermissionService,
        IAzureAdUserService azureAdUserService)
    {
        _logger = logger;
        _tokenService = tokenService;
        _sysUserService = sysUserService;
        _sysLogininforService = sysLogininforService;
        _sysPermissionService = sysPermissionService;
        _azureAdUserService = azureAdUserService;
    }

    /// <summary>
    /// 通过 Azure AD 登录
    /// </summary>
    /// <param name="principal">用户声明</param>
    /// <returns>JWT token</returns>
    public async Task<string> LoginWithAadAsync(ClaimsPrincipal principal)
    {
        // 提取 AAD 用户信息
        var aadUserInfo = _azureAdUserService.ExtractUserInfo(principal);

        if (string.IsNullOrEmpty(aadUserInfo.Username))
        {
            _logger.LogError("无法从 Azure AD 中提取用户名");
            await _sysLogininforService.AddAsync("AAD User", Constants.LOGIN_FAIL, "无法从 Azure AD 中提取用户名");
            throw new ServiceException("无法从 Azure AD 中提取用户名");
        }

        // 查找或创建用户
        var userDto = await GetOrCreateUserFromAadAsync(aadUserInfo);

        // 检查用户状态
        if (UserStatus.DELETED.GetValue().Equals(userDto.DelFlag))
        {
            _logger.LogInformation($"AAD 登录用户：{aadUserInfo.Username} 已被删除.");
            await _sysLogininforService.AddAsync(aadUserInfo.Username, Constants.LOGIN_FAIL, MessageConstants.User_Deleted);
            throw new ServiceException(MessageConstants.User_Deleted);
        }
        else if (UserStatus.DISABLE.GetValue().Equals(userDto.Status))
        {
            _logger.LogInformation($"AAD 登录用户：{aadUserInfo.Username} 已被停用.");
            await _sysLogininforService.AddAsync(aadUserInfo.Username, Constants.LOGIN_FAIL, MessageConstants.User_Blocked);
            throw new ServiceException(MessageConstants.User_Blocked);
        }

        // 记录登录成功
        await _sysLogininforService.AddAsync(aadUserInfo.Username, Constants.LOGIN_SUCCESS, "Azure AD 登录成功");

        var loginUser = CreateLoginUser(userDto);
        await RecordLoginInfoAsync(userDto.UserId ?? 0);

        // 生成token
        return await _tokenService.CreateToken(loginUser);
    }

    /// <summary>
    /// 从 AAD 用户信息获取或创建用户
    /// </summary>
    private async Task<SysUserDto> GetOrCreateUserFromAadAsync(AzureAdUserInfo aadUserInfo)
    {
        // 尝试通过用户名查找用户
        var userDto = await _sysUserService.GetDtoByUsernameAsync(aadUserInfo.Username);

        // 如果用户不存在，需要先在系统中创建
        if (userDto == null)
        {
            _logger.LogWarning($"AAD 用户 {aadUserInfo.Username} 在系统中不存在。需要管理员先创建此用户。");
            throw new ServiceException($"用户 {aadUserInfo.Username} 不存在，请联系管理员创建用户");
        }

        return userDto;
    }

    private LoginUser CreateLoginUser(SysUserDto user)
    {
        var permissions = _sysPermissionService.GetMenuPermission(user);
        return new LoginUser
        {
            UserId = user.UserId ?? 0,
            DeptId = user.DeptId ?? 0,
            UserName = user.UserName ?? "",
            Password = user.Password ?? "",
            User = user,
            Permissions = permissions
        };
    }

    /// <summary>
    /// 记录登录信息
    /// </summary>
    private async Task RecordLoginInfoAsync(long userId)
    {
        SysUserDto sysUser = new SysUserDto();
        sysUser.UserId = userId;
        sysUser.LoginIp = IpUtils.GetIpAddr();
        sysUser.LoginDate = DateTime.Now;
        await _sysUserService.UpdateUserLoginInfoAsync(sysUser);
    }
}
