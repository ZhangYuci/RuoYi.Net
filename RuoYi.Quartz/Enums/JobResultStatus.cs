using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuoYi.Quartz.Enums
{

    /// <summary>
    ///  执行状态（0正常 1失败）
    /// </summary>
    public enum JobResultStatus
    {
        [Description("正常")]
        NORMAL,
        [Description("失败")]
        FAILED
    }
}
