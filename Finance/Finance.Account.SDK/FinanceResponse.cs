using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK
{
    public class FinanceResponse
    {
        /// <summary>
        /// 错误码
        /// </summary>        
        public int Result { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>        
        public string ErrMsg { get; set; }

        /// <summary>
        /// 解决方案
        /// </summary>       
        public string Solution { get; set; }

        /// <summary>
        /// 响应原始内容
        /// </summary>
        [JsonIgnore()]
        public string Body { get; set; }

        /// <summary>
        /// 响应结果是否错误
        /// </summary>
        [JsonIgnore()]
        public bool IsError
        {
            get { return Result != 0; }
        }
    }
}
