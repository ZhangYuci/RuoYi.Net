using RuoYi.Common.Enums;
using RuoYi.Common.Utils;
using RuoYi.Quartz.Services;
using RuoYi.System.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RuoYi.Quartz.Controllers
{
    /// <summary>
    /// 调度任务日志信息操作处理
    /// </summary>
    [Route("monitor/joblog")]
    [ApiDescriptionSettings("Monitor")]
    public class SysJobLogController : ControllerBase
    {
        private readonly ILogger<SysJobController> _logger;
        private readonly SysJobLogService _sysJobLogService;

        public SysJobLogController(ILogger<SysJobController> logger,
            SysJobLogService sysJobLogService)
        {
            _logger = logger;
            _sysJobLogService = sysJobLogService;
        }

        /// <summary>
        /// 查询定时任务调度日志列表
        /// </summary>
        [HttpGet("list")]
        [AppAuthorize("monitor:job:list")]
        public async Task<SqlSugarPagedList<SysJobLogDto>> GetSysJobPagedList([FromQuery] SysJobLogDto dto)
        {
            return await _sysJobLogService.GetDtoPagedListAsync(dto);
        }


        /// <summary>
        /// 获取 定时任务调度日志 详细信息
        /// </summary>
        [HttpGet("")]
        [HttpGet("{jobLogId}")]
        [AppAuthorize("monitor:job:query")]
        public async Task<AjaxResult> Get(long jobLogId)
        {
            var data = await _sysJobLogService.GetDtoAsync(jobLogId);
            return AjaxResult.Success(data);
        }

        /// <summary>
        /// 删除 定时任务调度日志
        /// </summary>
        [HttpDelete("{jobLogIds}")]
        [AppAuthorize("monitor:job:remove")]
        [RuoYi.System.Log(Title = "定时任务", BusinessType = BusinessType.DELETE)]
        public async Task<AjaxResult> Remove(string jobLogIds)
        {
            var idList = jobLogIds.SplitToList<long>();
            var data = await _sysJobLogService.DeleteAsync(idList);
            return AjaxResult.Success(data);
        }

        /// <summary>
        /// 清空 定时任务调度日志
        /// </summary>
        [HttpDelete("clean")]
        [AppAuthorize("monitor:job:clean")]
        [RuoYi.System.Log(Title = "定时任务", BusinessType = BusinessType.CLEAN)]
        public AjaxResult Clean()
        {
            _sysJobLogService.Clean();
            return AjaxResult.Success();
        }

        /// <summary>
        /// 导出 定时任务调度日志
        /// </summary>
        [HttpPost("export")]
        [AppAuthorize("monitor:job:export")]
        [RuoYi.System.Log(Title = "定时任务", BusinessType = BusinessType.EXPORT)]
        public async Task Export(SysJobLogDto dto)
        {
            var list = await _sysJobLogService.GetDtoListAsync(dto);
            await ExcelUtils.ExportAsync(App.HttpContext.Response, list);
        }
    }
}
