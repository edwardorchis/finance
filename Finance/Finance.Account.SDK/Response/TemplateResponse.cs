using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK.Response
{
    public class ExcelTemplateResponse: FinanceResponse
    {
        public List<ExcelTemplateItem> Content { set; get; }

    }

    public class UdefTemplateResponse : FinanceResponse
    {
        public List<UdefTemplateItem> Content { set; get; }

    }

    public class CarriedForwardTemplateResponse : FinanceResponse
    {
        public List<CarriedForwardTemplate> Content { set; get; }
    }

}
