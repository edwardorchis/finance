using Finance.Account.SDK;
using Finance.Account.SDK.Response;
using Finance.Utils;
using System.Web.Http;

namespace Finance
{
    public class FinanceController: ApiController
    {
        public FinanceResponse CreateResponse(FinanceResult result)
        {            
            return new FinanceResponse { Result = (int)result };
        }

        public IdResponse CreateIdResponse(long id)
        {
            return new IdResponse { Result = (int)FinanceResult.SUCCESS,id=id };
        }

        public FinanceResponse CreateResponse(FinanceException ex)
        {
            FinanceResponse fex = new FinanceResponse();
            fex.Result = ex.HResult;
            fex.ErrMsg = ex.Message;
            return fex;
        }
    }
}
