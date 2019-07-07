using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.Account.SDK;
using Finance.Account.SDK.Request;

namespace Finance.Account.Data.Executer
{
    public class InterfaceExecuter : DataExecuter, IInterfaceExecuter
    {
        public string ExecTask(ExecTaskType taskType,string proc, Dictionary<string, object> filter)
        {
            var rsp =  Execute(new InterfaceRequest { ProcName = proc, Filter = filter, TaskType = (int)taskType });
            return rsp.Content;
        }

        public TaskResult GetTaskResult(string taskId)
        {
            var rsp = Execute(new InterfaceGetResultRequest { TaskId=taskId });
            return rsp.Content;
        }
    }
}
