using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Service.Model
{
    public class OperationLog
    {
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime time { set; get; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string username { set; get; }
        /// <summary>
        /// 业务分类
        /// </summary>
        public string category { set; get; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public string operation { set; get; }
        /// <summary>
        /// 详细信息
        /// </summary>
        public string message { set; get; }
    }
}
