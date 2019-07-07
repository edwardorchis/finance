using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Utils
{
    public interface IExportHandler
    {
        void Encode(ref DataTable data);
    }
}
