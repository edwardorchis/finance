using Finance.Account.SDK.Response;

namespace Finance.Account.SDK.Request
{
    public class VoucherSaveRequest : IFinanceRequest<IdResponse>
    {    
        public string Method
        {
            get { return "/voucher/save"; }
        }

        public Voucher Content { set; get; }
    }

    public class VoucherDeleteRequest : IdRequest, IFinanceRequest<IdResponse>
    {
        public string Method
        {
            get { return "/voucher/delete"; }
        }
    }

    public class VoucherRequest : IdRequest,IFinanceRequest<VoucherResponse>
    {
        public string Method
        {
            get { return "/voucher/find"; }
        }

        public int Linked { set; get; }
    }

    public class VoucherListRequest :ListRequest,IFinanceRequest<VoucherListResponse>
    {
        public string Method
        {
            get { return "/voucher/list"; }
        }

    }

    public class VoucherCheckRequest :IdRequest, IFinanceRequest<FinanceResponse>
    {
        public string Method
        {
            get { return "/voucher/check"; }
        }

    }



    public class VoucherUnCheckRequest : IdRequest, IFinanceRequest<FinanceResponse>
    {
        public string Method
        {
            get { return "/voucher/uncheck"; }
        }

    }

    public class VoucherCancelRequest : IdRequest, IFinanceRequest<FinanceResponse>
    {
        public string Method
        {
            get { return "/voucher/cancel"; }
        }

    }

    public class VoucherUnCancelRequest : IdRequest, IFinanceRequest<FinanceResponse>
    {
        public string Method
        {
            get { return "/voucher/uncancel"; }
        }

    }

    public class VoucherPostRequest : IdRequest, IFinanceRequest<FinanceResponse>
    {
        public string Method => "/voucher/post";
    }

    public class VoucherUnPostRequest : IdRequest, IFinanceRequest<FinanceResponse>
    {
        public string Method => "/voucher/unpost";
    }

    public class VoucherDoTestRequest : IFinanceRequest<FinanceResponse>
    {
        public string Method => "/voucher/dotest";
    }


    public class VoucherPrintRequest : IdRequest, IFinanceRequest<VoucherPrintResponse>
    {
        public string FileName { set; get; }
        public string Method
        {
            get { return "/voucher/print"; }
        }

    }
}
