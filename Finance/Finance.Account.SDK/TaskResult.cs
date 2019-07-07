using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK
{
    public class TaskResult
    {
        public string taskId { set; get; }

        public string taskType { set; get; }

        public DateTime createTime { set; get; }

        public DateTime lastRefreshTime { set; get; }

        public int status { set; get; }

        public int progRate { set; get; }

        public string reserved { set; get; } 

        public string result { set; get; }
    }
}
