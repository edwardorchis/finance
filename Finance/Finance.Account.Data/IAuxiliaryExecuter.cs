using Finance.Account.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Account.Data
{
    public interface IAuxiliaryExecuter
    {
        List<Auxiliary> List();

        List<Auxiliary> List(AuxiliaryType type);

        Auxiliary Find(long id);

        void Delete(long id);

        void Save(Auxiliary auxiliary);
    }
}
