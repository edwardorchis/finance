using System.Collections.Generic;
using System.Linq;
using Finance.Account.SDK;
using Finance.Account.SDK.Request;
using Newtonsoft.Json;

namespace Finance.Account.Data.Executer
{
    public class AuxiliaryExecuter :DataExecuter ,IAuxiliaryExecuter
    {
        public void Delete(long id)
        {
            Execute(new AuxiliaryDeleteRequest { id = id });
            DataFactory.Instance.GetCacheHashtable().Remove(CacheHashkey.AuxiliaryList);
        }

        public List<Auxiliary> List()
        {
            List<Auxiliary> result = null;
            if (!DataFactory.Instance.GetCacheHashtable().ContainsKey(CacheHashkey.AuxiliaryList))
            {
                var rsp = Execute(new AuxiliaryListRequest { Type = 0 });
                result = rsp.Content;
                DataFactory.Instance.GetCacheHashtable().Set(CacheHashkey.AuxiliaryList, result);
            }
            else
            {
                result = DataFactory.Instance.GetCacheHashtable().Get(CacheHashkey.AuxiliaryList);
            }
            return result;
        }

        public List<Auxiliary> List(AuxiliaryType type)
        {
            List<Auxiliary> result = List().FindAll(aux=>aux.type == (int)type).ToList();
            return result;
        }

        public void Save(Auxiliary auxiliary)
        {
            Execute(new AuxiliarySaveRequest { Content = auxiliary });
            DataFactory.Instance.GetCacheHashtable().Remove(CacheHashkey.AuxiliaryList);
        }

        Auxiliary IAuxiliaryExecuter.Find(long id)
        {
            var lst = List();
            return lst.FirstOrDefault(a => a.id == id);
        }
    }
}
