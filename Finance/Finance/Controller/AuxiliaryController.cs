using Finance.Utils;
using Finance.Account.SDK;
using System.Web.Http;
using Finance.Account.SDK.Response;
using Finance.Account.SDK.Request;
using Finance.Account.Service;
using System.Web.Http.Controllers;
using System.Collections.Generic;

namespace Finance.Controller
{
    public class AuxiliaryController : FinanceController
    {
        ILogger logger = Logger.GetLogger(typeof(AuxiliaryController));
        IDictionary<string, object> mContext = null;
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            mContext = controllerContext.Request.Properties;
            base.Initialize(controllerContext);
        }
        public FinanceResponse List(AuxiliaryListRequest request)
        {
            Auxiliary filter = new Auxiliary();
            if (request.Type != 0)
                filter.type = request.Type;
            return new AuxiliaryListResponse { Content = DataManager.GetInstance(mContext).Query(filter) };           
        }

        public FinanceResponse Find(long id)
        {
            //Auxiliary auxiliary= DataManager.GetInstance(mContext).Find<Auxiliary>(id);
            return CreateResponse(FinanceResult.SUCCESS);
        }
        [HttpPost]
        public FinanceResponse Save(AuxiliarySaveRequest json)
        {
            var aux = json.Content;

            if (aux.id != 0)
            {
                DataManager.GetInstance(mContext).Update(aux);
            }
            else
            {
                aux.id = SerialNoService.GetInstance(mContext).Get(SerialNoKey.System);
                DataManager.GetInstance(mContext).Insert(aux);
                SerialNoService.GetInstance(mContext).Update(SerialNoKey.System);
            }
            UserService.GetInstance(mContext).UpdateTimeStampArticle(TimeStampArticleEnum.Auxiliary);
            return CreateResponse(FinanceResult.SUCCESS);
        }
        [HttpPost]
        public FinanceResponse Delete(long id)
        {
            DataManager.GetInstance(mContext).Delete<Auxiliary>(id);
            UserService.GetInstance(mContext).UpdateTimeStampArticle(TimeStampArticleEnum.Auxiliary);
            return CreateResponse(FinanceResult.SUCCESS);
        }
    }
}
