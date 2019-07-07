using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.Account.SDK;
using Finance.Account.SDK.Request;

namespace Finance.Account.Data.Executer
{
    public class TemplateExecuter : DataExecuter, ITemplateExecuter
    {
        public void DeleteUdefTemplate(UdefTemplateItem udefTemplate)
        {
            Execute(new UdefTemplateDeleteRequest { Content = udefTemplate });
        }

        public List<ExcelTemplateItem> GetExcelTemplate(string name)
        {
            var rsp = Execute(new ExcelTemplateRequest { name = name });
            return rsp.Content;
        }

        public List<UdefTemplateItem> GetUdefTemplate(string name)
        {
            var rsp = Execute(new UdefTemplateRequest { name = name });
            return rsp.Content;
        }

        public void SaveUdefTemplate(UdefTemplateItem udefTemplate)
        {
            Execute(new UdefTemplateSaveRequest { Content = udefTemplate });
        }

        public List<CarriedForwardTemplate> ListCarriedForwardTemplate(long id)
        {
            var rsp = Execute(new CarriedForwardTemplateRequest { id = id });
            return rsp.Content;
        }

        public void SaveCarriedForwardTemplate(List<CarriedForwardTemplate> list)
        {
            Execute(new CarriedForwardTemplateSaveRequest { Content = list });
        }
    }
}
