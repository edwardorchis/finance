using Finance.Account.SDK.Response;
using System.Collections.Generic;

namespace Finance.Account.SDK.Request
{
    public class ExcelTemplateRequest : IFinanceRequest<ExcelTemplateResponse>
    {
        public string Method => "template/getexceltemplate";

        public string name { set; get; }
    }


    public class UdefTemplateRequest : IFinanceRequest<UdefTemplateResponse>
    {
        public string Method => "template/getudeftemplate";

        public string name { set; get; }
    }

    public class UdefTemplateSaveRequest : IFinanceRequest<FinanceResponse>
    {
        public string Method => "template/saveudeftemplate";

        public UdefTemplateItem Content { set; get; }
    }

    public class UdefTemplateDeleteRequest : IFinanceRequest<FinanceResponse>
    {
        public string Method => "template/deleteudeftemplate";

        public UdefTemplateItem Content { set; get; }
    }
    
    public class CarriedForwardTemplateRequest : IFinanceRequest<CarriedForwardTemplateResponse>
    {
        public string Method => "template/listcarriedforwardtempate";

        public long id { set; get; }
    }

    public class CarriedForwardTemplateSaveRequest : IFinanceRequest<FinanceResponse>
    {
        public string Method => "template/savecarriedforwardtemplate";

        public List<CarriedForwardTemplate> Content { set; get; }
    }

    public class TemplateUploadRequest : IFinanceUploadFileRequest<FinanceResponse>
    {
        public string Method => "template/upload";
        public string name { set; get; }
        public string Name => name;
    }
}
