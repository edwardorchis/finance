using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.SDK
{
    public class ExcelTemplateItem
    {
        public string a { set; get; }
        public string b { set; get; }
        public string c { set; get; }
        public string d { set; get; }
        public string e { set; get; }
        public string f { set; get; }
        public string g { set; get; }
        public string h { set; get; }
    }

    public class UdefTemplateItem
    {
        public string tableName { set; get; }
        public string name { set; get; }
        public string label { set; get; }
        public string dataType { set; get; }
        public string defaultVal { set; get; }
        public string reserved { set; get; }
        public int tabIndex { set; get; }
        public string tagLabel { set; get; }
        public int width { set; get; }
    }


    public class UdefReportDataSet
    {
        public List<UdefTemplateItem> header { set; get; }
        public List<Dictionary<string, object>> entries { set; get; }
    }

    public class UdefObject
    {
        public string dataType { set; get; }
        public object dataVal { set; get; }
    }

    public class CarriedForwardTemplate
    {
        public long id { set; get; }
        public int index { set; get; }
        public long src { set; get; }
        public long dst { set; get; }
    }
}
