using Finance.Account.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Data
{
    public interface IInterfaceExecuter
    {       
        string ExecTask(ExecTaskType taskType, string proc, Dictionary<string, object> filter);

        TaskResult GetTaskResult(string taskId);
    }
}
