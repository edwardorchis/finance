using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Utils
{
    /// <summary>
    /// 心跳下发的任务枚举
    /// </summary>
    public enum HeartBeatTask
    {
        InvaildTask,
        /// <summary>
        /// 刷新全局数据任务
        /// </summary>
        RefreshAccountSubjectTask,
        /// <summary>
        /// 刷新用户列表
        /// </summary>
        RefreshUserListTask,
        /// <summary>
        /// 刷新辅助资料
        /// </summary>
        RefreshAuxiliaryTask
    }
}
