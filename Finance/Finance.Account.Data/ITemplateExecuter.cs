using Finance.Account.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Data
{
    public interface ITemplateExecuter
    {
        List<ExcelTemplateItem> GetExcelTemplate(string name);
        List<UdefTemplateItem> GetUdefTemplate(string name);

        void SaveUdefTemplate(UdefTemplateItem udefTemplate);
        void DeleteUdefTemplate(UdefTemplateItem udefTemplate);

        List<CarriedForwardTemplate> ListCarriedForwardTemplate(long id);
        void SaveCarriedForwardTemplate(List<CarriedForwardTemplate> list);
        string UploadTemplate(string name, string fileName);
    }
}
