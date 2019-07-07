using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Controls.Commons
{
    public class VoucherGridItem
    {
        public VoucherGridItem()
        {
            this.Auxiliary = new AuxiliaryObj();
            this.AccountSubject = new AccountSubjectObj();
            this.DebitsAmount = 0.00M;
            this.CreditAmount = 0.00M;
            UniqueKey = System.Guid.NewGuid().ToString("N");
        }

        public string Content { set; get; }
        public long AccountSubjectId {
            set {
                AccountSubject.id = value;
                this.AccountSubject = AccountSubjectList.Find(value);
            }
            get {
                return AccountSubject.id;
            }
        }
        internal AuxiliaryObj Auxiliary { set; get; }
        internal AccountSubjectObj AccountSubject { set; get; }   
        /// <summary>
        /// 借方
        /// </summary>
        public decimal DebitsAmount { set; get; }
        /// <summary>
        /// 贷方
        /// </summary>
        public decimal CreditAmount { set; get; }

        public string AccountSubjectNo {
            get {
                return AccountSubject.no;
            }
        }
        
        public string UniqueKey { get; set; }

    }

    

    
}
