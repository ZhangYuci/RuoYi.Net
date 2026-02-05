using RuoYi.Common.Utils;
using RuoYi.Data.Models;
using RuoYi.System.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using RuoYi.Framework.AzureAd.Options;
using Microsoft.Extensions.Options;

namespace RuoYi.Admin
{
    /// <summary>
    /// 登录验证
    /// </summary>
    [ApiDescriptionSettings("System")]
    public class SysLoginController : ControllerBase
    {
        private readonly ILogger<SysLoginController> _logger;
        private readonly TokenService _tokenService;

        private readonly SysLoginService _sysLoginService;
        private readonly AadLoginService _aadLoginService;
        private readonly SysPermissionService _sysPermissionService;
        private readonly SysMenuService _sysMenuService;
        private readonly SysLogininforService _sysLogininforService;
        private readonly AzureAdOptions _azureAdOptions;

        public SysLoginController(ILogger<SysLoginController> logger,
            TokenService tokenService,
            SysLoginService sysLoginService,
            AadLoginService aadLoginService,
            SysPermissionService sysPermissionService,
            SysMenuService sysMenuService,
            SysLogininforService sysLogininforService,
            IOptions<AzureAdOptions> azureAdOptions)
        {
            _logger = logger;
            _tokenService = tokenService;
            _sysLoginService = sysLoginService;
            _aadLoginService = aadLoginService;
            _sysPermissionService = sysPermissionService;
            _sysMenuService = sysMenuService;
            _sysLogininforService = sysLogininforService;
            _azureAdOptions = azureAdOptions.Value;
        }

        /// <summary>
        /// 登录验证
        /// </summary>
        /// <returns></returns>
        [HttpPost("/login")]
        public async Task<AjaxResult> Login([FromBody]LoginBody loginBody)
        {
            AjaxResult ajax = AjaxResult.Success();
            // 生成令牌
            string token = await _sysLoginService.LoginAsync(loginBody.Username, loginBody.Password, loginBody.Code, loginBody.Uuid);
            ajax.Add(Constants.TOKEN, token);
            return ajax;
        }

        /// <summary>
        /// 退出
        /// </summary>
        [HttpPost("/logout")]
        public async Task<AjaxResult> Logout()
        {
            LoginUser loginUser = _tokenService.GetLoginUser(App.HttpContext.Request);
            if (loginUser != null)
            {
                string userName = loginUser.UserName;
                // 删除用户缓存记录
                _tokenService.DelLoginUser(loginUser.Token);
                // 记录用户退出日志
                await _sysLogininforService.AddAsync(userName, Constants.LOGOUT, "退出成功");
            }
            return AjaxResult.Success("退出成功");
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        [HttpGet("/getInfo")]
        public async Task<AjaxResult> GetInfo()
        {
            SysUserDto user = SecurityUtils.GetLoginUser().User;
            // 角色集合
            List<string> roles = await _sysPermissionService.GetRolePermissionAsync(user);
            // 权限集合
            List<string> permissions = _sysPermissionService.GetMenuPermission(user);

            AjaxResult ajax = AjaxResult.Success();
            ajax.Add("user", user);
            ajax.Add("roles", roles);
            ajax.Add("permissions", permissions);
            return ajax;
        }

        /// <summary>
        /// 获取路由信息
        /// </summary>
        [HttpGet("/getRouters")]
        public async Task<AjaxResult> GetRouters()
        {
            long userId = SecurityUtils.GetUserId();
            List<SysMenu> menus = await _sysMenuService.SelectMenuTreeByUserId(userId);
            var treeMenus = _sysMenuService.BuildMenus(menus);
            return AjaxResult.Success(treeMenus);
        }

        /// <summary>
        /// 发起 Azure AD 登录
        /// </summary>
        [HttpGet("/login/aad")]
        public IActionResult LoginWithAad()
        {
            if (!_azureAdOptions.Enabled)
            {
                return new JsonResult(AjaxResult.Error("Azure AD 登录未启用"));
            }

            var redirectUrl = Url.Action(nameof(AadCallback), "SysLogin");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Azure AD 登录回调
        /// </summary>
        [HttpGet("/login/aad/callback")]
        public async Task<IActionResult> AadCallback()
        {
            if (!_azureAdOptions.Enabled)
            {
                return new JsonResult(AjaxResult.Error("Azure AD 登录未启用"));
            }

            try
            {
                // 获取认证结果
                var authenticateResult = await HttpContext.AuthenticateAsync(OpenIdConnectDefaults.AuthenticationScheme);
                
                if (!authenticateResult.Succeeded)
                {
                    _logger.LogError("Azure AD 认证失败");
                    return new JsonResult(AjaxResult.Error("Azure AD 认证失败"));
                }

                // 使用 AAD 信息进行登录
                string token = await _aadLoginService.LoginWithAadAsync(authenticateResult.Principal);
                
                AjaxResult ajax = AjaxResult.Success("Azure AD 登录成功");
                ajax.Add(Constants.TOKEN, token);
                return new JsonResult(ajax);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Azure AD 登录回调处理失败");
                return new JsonResult(AjaxResult.Error($"登录失败: {ex.Message}"));
            }
        }
    }
}