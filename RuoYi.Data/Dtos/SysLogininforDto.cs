using RuoYi.Data.Attributes;

namespace RuoYi.Data.Dtos
{
    /// <summary>
    ///  系统访问记录 对象 sys_logininfor
    ///  author ruoyi
    ///  date   2023-08-22 10:07:36
    /// </summary>
    public class SysLogininforDto : BaseDto
    {
        /// <summary>
        /// 访问ID
        /// </summary>
        [Excel(Name = "访问ID")]
        public long InfoId { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        [Excel(Name = "用户账号")]
        public string? UserName { get; set; }

        /// <summary>
        /// 登录IP地址
        /// </summary>
        [Excel(Name = "登录IP地址")]
        public string? Ipaddr { get; set; }

        /// <summary>
        /// 登录地点
        /// </summary>
        [Excel(Name = "登录地点")]
        public string? LoginLocation { get; set; }

        /// <summary>
        /// 浏览器类型
        /// </summary>
        [Excel(Name = "浏览器类型")]
        public string? Browser { get; set; }

        /// <summary>
        /// 操作系统
        /// </summary>
        [Excel(Name = "操作系统")]
        public string? Os { get; set; }

        /// <summary>
        /// 登录状态（0成功 1失败）
        /// </summary>
        
        public string? Status { get; set; }

        [Excel(Name = "登录状态")]
        public string? StatusDesc { get; set; }

        /// <summary>
        /// 提示消息
        /// </summary>
        [Excel(Name = "提示消息")]
        public string? Msg { get; set; }

        /// <summary>
        /// 访问时间
        /// </summary>
        [Excel(Name = "访问时间")]
        public DateTime? LoginTime { get; set; }
    }
}
