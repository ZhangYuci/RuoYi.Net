using Microsoft.Extensions.Logging;
using RuoYi.Common.Files;
using RuoYi.Common.Utils;
using RuoYi.Data.Models;
using RuoYi.System.Services;

namespace RuoYi.Admin
{
    /// <summary>
    /// 系统服务接口
    /// </summary>
    [ApiDescriptionSettings("Common")]
    [Route("common")]
    public class IndexController : ControllerBase
    {
        private readonly ILogger<IndexController> _logger;

        private readonly SystemService _systemService;
        public IndexController(ILogger<IndexController> logger, SystemService systemService)
        {
            _logger = logger;
            _systemService = systemService;
        }

        /// <summary>
        /// 获取系统描述
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetDescription")]
        public string GetDescription()
        {
            _logger.LogInformation("获取系统描述");
            return _systemService.GetDescription();
        }

        [HttpPost("upload")]
        public async Task<AjaxResult> upload([Required] IFormFile file)
        {
            if (file != null)
            {
                string avatar = await FileUploadUtils.UploadAsync(file, RyApp.RuoYiConfig.UploadPath, MimeTypeUtils.DEFAULT_ALLOWED_EXTENSION);
                
                AjaxResult ajax = AjaxResult.Success();
                ajax.Add("fileName", avatar);
                ajax.Add("newFileName", Path.GetFileName(avatar));
                ajax.Add("originalFilename", file.FileName);
                ajax.Add("url", $"{HttpContext.Request.Host.Value}{avatar}");
                return ajax;
            }
            return AjaxResult.Error("上传文件异常，请联系管理员");
        }
    }
}