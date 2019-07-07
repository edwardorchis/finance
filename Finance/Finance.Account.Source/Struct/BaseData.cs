using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.Account.SDK;
using Finance.Account.Service.Model;

namespace Finance.Account.Source.Struct
{
    public class BaseData
    {
        public AccountSubject accountSuject { set; get; }

        public Auxiliary auxiliary { set; get; }

        public SerialNo serialNo { set; get; }

        public SystemProfile systemProfile { set; get; }

        public OperationLog operationLog { set; get; }

        public ExcelTemplate excelTemplate { set; get; }

        public TimeStampArticle timeStampArticle { set; get; }
    }
}
