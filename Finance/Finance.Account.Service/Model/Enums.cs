using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Service
{
    public enum Operation
    {
        Invalid  =  0,
        Query,
        Add,
        Update,
        Delete,
        Check,
        UnCheck,
        Cancel,
        UnCancel,
        Post,
        UnPost
    }

    public enum TimeStampArticleEnum
    {
        Invalid,
        /// <summary>
        /// 科目
        /// </summary>
        AccountSubject,
        /// <summary>
        /// 用户列表
        /// </summary>
        UserList,
        /// <summary>
        /// 辅助资料
        /// </summary>
        Auxiliary,
    }
}
